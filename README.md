# Netina.Stomp.Client

[![Build status](https://ci.appveyor.com/api/projects/status/166elreftg7pc62g?svg=true)](https://ci.appveyor.com/project/mrmohande3/inet-stomp-client)
[![NuGet](https://img.shields.io/nuget/v/Netina.Stomp.Client.svg)](https://www.nuget.org/packages/Netina.Stomp.Client/)
[![NuGet](https://img.shields.io/nuget/dt/Netina.Stomp.Client.svg)](https://www.nuget.org/packages/Netina.Stomp.Client/)
[![GitHub issues](https://img.shields.io/github/issues/Netina/iNet.Stomp.Client.svg)](https://github.com/Netina/iNet.Stomp.Client/issues)

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
here we create instance from StompClient and set stomp url and create Dictionary for headers like your JWT if you have no header set you dictionary empty , now your clint connected
### 2.Subscribing
```C#
await client.SubscribeAsync<object>("notic", new Dictionary<string, string>(), ((sender, dto) =>
{
}));
```
for subscribing use SubscribeAsync method . this method is generic and get subscribed url and header for this method and action for returned objects , returned objects 

### 3.Send
```C#
await client.SendAsync(body, "notic", new Dictionary<string, string>());
```
for sending use SendAsync method , this method get body object and convert this to json for sending and url method in server and header dictionary 
