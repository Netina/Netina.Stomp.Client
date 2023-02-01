using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Netina.Stomp.Client.Messages
{
    public class StompMessageSerializer
    {
        public string Serialize(StompMessage message)
        {
            var buffer = new StringBuilder();

            buffer.Append(message.Command + "\n");

            if (message.Headers != null)
            {
                foreach (var header in message.Headers)
                {
                    buffer.Append(header.Key + ":" + header.Value + "\n");
                }
            }

            buffer.Append('\n');
            buffer.Append(message.Body);
            buffer.Append('\0');

            return buffer.ToString();
        }

        public StompMessage Deserialize(string message)
        {
            using (var reader = new StringReader(message))
            {
                var command = reader.ReadLine();

                var headers = new Dictionary<string, string>();

                var header = reader.ReadLine();
                while (!string.IsNullOrEmpty(header))
                {
                    var split = header.Split(':');
                    if (split.Length == 2) headers[split[0].Trim()] = split[1].Trim();
                    header = reader.ReadLine() ?? string.Empty;
                }

                var body = reader.ReadToEnd();
                body = body.TrimEnd('\r', '\n', '\0');

                return new StompMessage(command, body, headers);
            }
        }
    }
}
