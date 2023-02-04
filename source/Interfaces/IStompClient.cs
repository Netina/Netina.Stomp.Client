using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Netina.Stomp.Client.Messages;
using Netina.Stomp.Client.Utils;
using Websocket.Client;
using Websocket.Client.Models;

namespace Netina.Stomp.Client.Interfaces
{
    public interface IStompClient : IDisposable
    {
       event EventHandler<StompMessage> OnConnect;
       event EventHandler<DisconnectionInfo> OnClose;
       event EventHandler<StompMessage> OnMessage;
       event EventHandler<ReconnectionInfo> OnReconnect;
       event EventHandler<StompMessage> OnError;

        StompConnectionState StompState { get; }

        Task ConnectAsync(IDictionary<string, string> headers);
        Task SendAsync(object body, string destination, IDictionary<string, string> headers);
        Task SendAsync(string body, string destination, IDictionary<string, string> headers);
        Task SubscribeAsync<T>(string topic, IDictionary<string, string> headers, EventHandler<T> handler);
        Task SubscribeAsync(string topic, IDictionary<string, string> headers, EventHandler<StompMessage> handler);
        Task AckAsync(string id, string transaction = null);
        Task NackAsync(string id, string transaction = null);
        Task DisconnectAsync();
        Task Reconnect();
    }
}
