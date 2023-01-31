using System.Collections.Generic;

namespace Netina.Stomp.Client.Messages
{
    public class StompMessage
    {
        public IDictionary<string, string> Headers { get; }
        public string Body { get; }
        public string Command { get; }

        public StompMessage(string command)
            : this(command, string.Empty)
        {
        }

        public StompMessage(string command, string body)
            : this(command, body, new Dictionary<string, string>())
        {
        }

        public StompMessage(string command, IDictionary<string, string> headers)
            : this(command, string.Empty, headers)
        {
        }

        public StompMessage(string command, string body, IDictionary<string, string> headers)
        {
            Command = command;
            Body = body;
            Headers = headers;
        }

        public string this[string header]
        {
            get => Headers.ContainsKey(header) ? Headers[header] : string.Empty;
            set => Headers[header] = value;
        }
    }
}
