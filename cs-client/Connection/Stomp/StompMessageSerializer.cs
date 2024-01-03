using System.Collections.Generic;
using System.IO;
using System.Text;
using CsClient.Connection.WebSocket;

namespace CsClient.Connection.Stomp
{
    public class StompMessageSerializer: IWebSocketSerializer<StompMessage>
    {
        /// <summary>
        /// Serializes the specified message.
        /// </summary>
        /// <param name = "message">The message.</param>
        /// <returns>A serialized version of the given <see cref="StompMessage"/></returns>
        public string Serialize(StompMessage message)
        {
            StringBuilder sb = new StringBuilder(message.Command + "\n");

            if (message.Headers != null)
            {
                foreach (var header in message.Headers)
                {
                    // Stomp Header format is <key>:<value>
                    sb.Append($"{header.Key}:{header.Value}\n");
                }
            }

            // Stomp defines a new line after headers and then body, then the end of message char.
            sb.Append("\n");
            sb.Append(message.Body);
            sb.Append('\0'); // end of message char.

            return sb.ToString();
        }

        /// <summary>
        /// Deserializes the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>A <see cref="StompMessage"/> instance</returns>
        public StompMessage Deserialize(string message)
        {
            // Initialize
            var reader = new StringReader(message);
            var command = reader.ReadLine();
            var headers = new Dictionary<string, string>();
            var header = reader.ReadLine();

            // Check if string is
            while (!string.IsNullOrEmpty(header))
            {
                // Split headers to <key>:<value>
                var split = header.Split(':');
                if (split.Length == 2)
                {
                    headers[split[0].Trim()] = split[1].Trim();
                }
                header = reader.ReadLine() ?? string.Empty;
            }

            var body = reader.ReadToEnd() ?? string.Empty;
            body = body.TrimEnd('\r', '\n', '\0');

            return new StompMessage(command, body, headers);
        }
    }
}
