using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using iNet.Stomp.Client.Interfaces;
using iNet.Stomp.Client.Messages;
using iNet.Stomp.Client.Utils;
using Newtonsoft.Json;
using Websocket.Client;
using Websocket.Client.Models;

namespace iNet.Stomp.Client
{
    public class StompClient : IStompClient
    {
        public event EventHandler OnConnect;
        public event EventHandler<DisconnectionInfo> OnClose;
        public event EventHandler<string> OnMessage;
        public event EventHandler<ReconnectionInfo> OnReconnect;
        public event EventHandler<string> OnError; 

        public StompConnectionState StompState { get; private set; } = StompConnectionState.Closed;
        private readonly WebsocketClient _socket;
        private readonly StompMessageSerializer _stompSerializer = new StompMessageSerializer();
        private readonly IDictionary<string, Subscriber> _subscribers = new Dictionary<string, Subscriber>();
        private readonly IDictionary<string, string> _connectingHeaders = new Dictionary<string, string>();

        /// <summary>
        /// StompClient Ctor
        /// </summary>
        /// <param name="url">Url of stomp websocket , start with wss or ws</param>
        /// <param name="reconnectEnable">Set reconnect enable of disable</param>
        /// <param name="stompVersion">Add stomp version in header for connecting , IF DONT SET VERSION HEADER SET 1.1,1.0 AUTOMATIC</param>
        public StompClient(string url , bool reconnectEnable = true , string stompVersion = null)
        {
            _socket = new WebsocketClient(new Uri(url));
            _socket.IsReconnectionEnabled = reconnectEnable;
            _socket.MessageReceived.Subscribe(HandleMessage);
            _socket.ErrorReconnectTimeout = TimeSpan.FromSeconds(30);
            _socket.DisconnectionHappened.Subscribe(info =>
            {
                StompState = StompConnectionState.Closed;
                OnClose?.Invoke(this, info);
                _subscribers.Clear();

            });
            _socket.ReconnectionHappened.Subscribe(async info =>
            {
                if(info.Type==ReconnectionType.Initial)
                    return;
                OnReconnect?.Invoke(this, info);
                StompState = StompConnectionState.Reconnecting;
                await Reconnect();
            });
            if(string.IsNullOrEmpty(stompVersion))
                _connectingHeaders.Add("accept-version", "1.1,1.0");
            else
                _connectingHeaders.Add("accept-version", stompVersion);

        }
        
        public async Task ConnectAsync(IDictionary<string, string> headers)
        {
            try
            {
                if(!_socket.IsRunning)
                    await _socket.Start();
                if (!_socket.IsRunning)
                    throw new Exception("Connection is not open");
                if (StompState != StompConnectionState.Closed)
                    return;
                foreach (var header in headers)
                {
                    _connectingHeaders.Add(header);
                }
                var connectMessage = new StompMessage(StompCommand.Connect, _connectingHeaders);
                await _socket.SendInstant(_stompSerializer.Serialize(connectMessage));
                StompState = StompConnectionState.Open;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task Reconnect()
        {
            try
            {
                if (!_socket.IsRunning)
                    await _socket.Start();
                if (StompState == StompConnectionState.Open)
                    return;
                var connectMessage = new StompMessage(StompCommand.Connect, _connectingHeaders);
                await _socket.SendInstant(_stompSerializer.Serialize(connectMessage));
                StompState = StompConnectionState.Open;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public async Task SendAsync(object body, string destination, IDictionary<string, string> headers)
        {
            if (StompState != StompConnectionState.Open)
                await Reconnect();

            var jsonPayload = JsonConvert.SerializeObject(body);
            headers.Add("destination", destination);
            headers.Add("content-type", "application/json;charset=UTF-8");
            headers.Add("content-length", Encoding.UTF8.GetByteCount(jsonPayload).ToString());
            var connectMessage = new StompMessage(StompCommand.Send, jsonPayload, headers);
            await _socket.SendInstant(_stompSerializer.Serialize(connectMessage));
        }

        public async Task SubscribeAsync<T>(string topic, IDictionary<string, string> headers, EventHandler<T> handler)
        {
            if (StompState != StompConnectionState.Open)
                Reconnect();

            headers.Add("destination", topic);
            headers.Add("id", $"sub-{_subscribers.Count}");
            var subscribeMessage = new StompMessage(StompCommand.Subscribe, headers);
            await _socket.SendInstant(_stompSerializer.Serialize(subscribeMessage));
            var sub = new Subscriber((sender, body) => handler(this, (T)body), typeof(T));
            _subscribers.Add(topic, sub);
        }

        public async Task DisconnectAsync()
        {

            var connectMessage = new StompMessage(StompCommand.Disconnect);
            await _socket.SendInstant(_stompSerializer.Serialize(connectMessage));
            StompState = StompConnectionState.Closed;
            _socket.Dispose();
            _subscribers.Clear();

        }

        public void Dispose()
        {
            StompState = StompConnectionState.Closed;
            ((IDisposable)_socket).Dispose();
            _subscribers.Clear();
        }
        
        private void HandleMessage(ResponseMessage messageEventArgs)
        {
            OnMessage?.Invoke(this,messageEventArgs.Text);
            var message = _stompSerializer.Deserialize(messageEventArgs.Text);
            if(message.Command==StompCommand.Connected)
                OnConnect?.Invoke(this,new EventArgs());
            if(message.Command==StompCommand.Error)
                OnError?.Invoke(this,message.Body);
            if (message.Headers.ContainsKey("destination"))
            {
                var sub = _subscribers[message.Headers["destination"]];
                var body = JsonConvert.DeserializeObject(message.Body, sub.BodyType);
                sub.Handler(this, body);
            }
        }
    }
}
