using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using Netina.Stomp.Client.Utils;
using System.Linq;

namespace Netina.Stomp.Client.Messages
{
    public class StompBinaryMessageSerializer
    {
        public byte[] Serialize(StompMessage message)
        {
            var resultBuffer = new List<byte>();
            resultBuffer.AddRange(Encoding.UTF8.GetBytes($"{message.Command}\n"));

            if (message.Headers?.Count > 0)
            {
                foreach (var messageHeader in message.Headers)
                {
                    resultBuffer.AddRange(Encoding.UTF8.GetBytes($"{messageHeader.Key}:{messageHeader.Value}\n"));
                }
            }

            resultBuffer.Add((byte)'\n');
            resultBuffer.AddRange(message.BinaryBody);
            resultBuffer.Add((byte)'\0');

            return resultBuffer.ToArray();
        }

        public StompMessage Deserialize(byte[] message)
        {
            var headerBuffer = new List<byte>();
            var bodyBuffer = new List<byte>();
            byte previousByte = 0;
            var isBodyStarted = false;

            // Building header and body buffers
            foreach (var currentByte in message)
            {
                if (!isBodyStarted && currentByte == previousByte && previousByte == (byte)'\n')
                {
                    isBodyStarted = true;
                }
                else
                {
                    if (isBodyStarted)
                    {
                        bodyBuffer.Add(currentByte);
                    }
                    else
                    {
                        headerBuffer.Add(currentByte);
                    }
                }

                previousByte = currentByte;
            }

            // Doing a cleanup of a body buffer according to a frame description:
            // "The body is then followed by the NULL octet. The NULL octet can be optionally followed by multiple EOLs"
            var ignoredChars = new byte[] { (byte)'\n', (byte)'\r' };
            var messageEnd = (byte)'\0';
            for (var index = bodyBuffer.Count - 1; index >= 0; index--)
            {
                var currentByte = bodyBuffer[index];
                if (ignoredChars.Contains(currentByte))
                {
                    bodyBuffer.RemoveAt(index);
                    continue;
                }

                if (currentByte == messageEnd)
                {
                    bodyBuffer.RemoveAt(index);
                }

                break;
            }

            var command = string.Empty;
            var headers = new Dictionary<string, string>();

            // Parse headers
            if (headerBuffer.Count > 0)
            {
                var stringHeader = Encoding.UTF8.GetString(headerBuffer.ToArray());

                using (var reader = new StringReader(stringHeader))
                {
                    command = reader.ReadLine();
                    var header = reader.ReadLine();
                    while (!string.IsNullOrEmpty(header))
                    {
                        var separatorIndex = header.IndexOf(':');
                        if (separatorIndex != -1)
                        {
                            var name = header.Substring(0, separatorIndex);
                            var value = header.Substring(separatorIndex + 1);
                            headers[name] = value;
                        }

                        header = reader.ReadLine() ?? string.Empty;
                    }
                }
            }

            // Check body content length is present
            if (headers.TryGetValue(StompHeader.ContentLength, out var contentLength))
            {
                if (long.TryParse(contentLength, out var length))
                {
                    if (length != bodyBuffer.Count)
                    {
                        throw new ApplicationException(
                            "STOMP: Content length header value is different then actual length of bytes received.");
                    }
                }
            }


            return new StompMessage(command, bodyBuffer.ToArray(), headers);
        }
    }
}