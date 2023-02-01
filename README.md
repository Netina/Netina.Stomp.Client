# Netina.Stomp.Client

[![Build status](https://ci.appveyor.com/api/projects/status/166elreftg7pc62g?svg=true)](https://ci.appveyor.com/project/mrmohande3/inet-stomp-client)
[![NuGet](https://img.shields.io/nuget/v/Netina.Stomp.Client.svg)](https://www.nuget.org/packages/Netina.Stomp.Client/)
[![NuGet](https://img.shields.io/nuget/dt/Netina.Stomp.Client.svg)](https://www.nuget.org/packages/Netina.Stomp.Client/)
[![GitHub issues](https://img.shields.io/github/issues/Netina/Netina.Stomp.Client.svg)](https://github.com/Netina/Netina.Stomp.Client/issues)

.NET nuget package for connecting stomp server in client async

### Usage
    Install-Package Netina.Stomp.Client
    
### 1.Add stomp url and connect
```C#
IStompClient client = new StompClient("ws://xxxxx.xx");
var headers = new Dictionary<string, string>();
headers.Add("X-Authorization", "Bearer xxx");
await client.ConnectAsync(headers);
```
Here we create instance from StompClient and set STOMP url and create Dictionary for headers like your JWT. In case you have no headers set your dictionary empty. Now your client connected.
### 2.Subscribing
```C#
await client.SubscribeAsync<object>("notic", new Dictionary<string, string>(), ((sender, dto) =>
{
}));
```
Subscribe with generic SubscribeAsync method. This method get topic and headers for STOMP SUBSCRIBE command and action for returned objects. 
```C#
await client.SubscribeAsync("notic", new Dictionary<string, string>(), ((sender, stompMessage) =>
{
    await client.AckAsync(stompMessage.Headers["ack"]);
}));
```
Subscribe and get plain STOMP message with headers, command and body. Then perform ACK operation.

### 3.Send
```C#
await client.SendAsync(body, "notic", new Dictionary<string, string>());
```
Send messages with SendAsync method. This method get body object and convert it to json for sending and url method in server and header dictionary.
