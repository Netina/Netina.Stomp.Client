using System.Collections.Generic;

namespace Netina.Stomp.Client.Messages
{
    public class StompMessage
    {
        public IDictionary<string, string> Headers { get; }
        public string TextBody { get; }
        public byte[] BinaryBody { get; }
        public string Command { get; }

        public StompMessageBodyType BodyType { get; }

        public StompMessage(string command)
            : this(command, string.Empty)
        {
        }

        public StompMessage(string command, string textBody)
            : this(command, textBody, new Dictionary<string, string>())
        {
        }

        public StompMessage(string command, IDictionary<string, string> headers)
            : this(command, string.Empty, headers)
        {

        }

        public StompMessage(string command, string textBody, IDictionary<string, string> headers)
        {
            Command = command;
            TextBody = textBody;
            Headers = headers;
            BodyType = string.IsNullOrEmpty(textBody) ? StompMessageBodyType.Empty : StompMessageBodyType.Text;
        }

        public StompMessage(string command, byte[] binBody)
            : this(command, binBody, new Dictionary<string, string>())
        {
        }

        public StompMessage(string command, byte[] binBody, IDictionary<string, string> headers)
        {
            Command = command;
            BinaryBody = binBody;
            Headers = headers;
            BodyType = binBody == null || binBody.Length == 0 ? StompMessageBodyType.Empty : StompMessageBodyType.Binary;
        }
    }
}
