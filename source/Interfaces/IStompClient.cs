using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Netina.Stomp.Client.Utils;
using Websocket.Client;
using Websocket.Client.Models;

namespace Netina.Stomp.Client.Interfaces
{
    public interface IStompClient : IDisposable
    {
        event EventHandler OnConnect;
        event EventHandler<DisconnectionInfo> OnClose;
        event EventHandler<string> OnMessage;
        event EventHandler<ReconnectionInfo> OnReconnect;

        StompConnectionState StompState { get; }

        Task ConnectAsync(IDictionary<string, string> headers);
        Task SendAsync(object body, string destination, IDictionary<string, string> headers);
        Task SubscribeAsync<T>(string topic, IDictionary<string, string> headers, EventHandler<T> handler);
        Task DisconnectAsync();
        Task Reconnect();
    }
}
