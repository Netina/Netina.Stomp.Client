namespace Netina.Stomp.Client.Utils
{
    public static class StompCommand
    {
        //Client Command
        public const string Connect = "CONNECT";
        public const string Disconnect = "DISCONNECT";
        public const string Subscribe = "SUBSCRIBE";
        public const string Unsubscribe = "UNSUBSCRIBE";
        public const string Send = "SEND";
        public const string Ack = "ACK";
        public const string Nack = "NACK";

        //Server Response
        public const string Connected = "CONNECTED";
        public const string Message = "MESSAGE";
        public const string Error = "ERROR";

        //Fictional
        public const string HeartBeat = "HEARTBEAT";
    }
}
