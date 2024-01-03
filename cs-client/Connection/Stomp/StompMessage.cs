using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CsClient.Connection.Stomp
{
    /// <summary>
    /// Data class containing fields for a message to be send as STOMP sub-protocol defines..
    /// </summary>
    public class StompMessage
    {
        private readonly Dictionary<string, string> _headers = new Dictionary<string, string>();
        /// <summary>
        /// Initializes an instance of the <see cref="StompMessage"/> class.
        /// </summary>
        /// <param name="command">Command.</param>
        public StompMessage(string command) : this(command, string.Empty) { }

        /// <summary>
        /// Initializes an instance of the <see cref="StompMessage"/> class.
        /// </summary>
        /// <param name="command">Command.</param>
        /// <param name="body">Body to send in request.</param>
        public StompMessage(string command, string body) : this(command, body, new Dictionary<string, string>()) { }

        /// <summary>
        /// Initializes an instance of the <see cref="StompMessage"/> class.
        /// </summary>
        /// <param name="command">Command.</param>
        /// <param name="body">Body to send in request.</param>
        /// <param name="headers">Headers to send in request.</param>
        public StompMessage(string command, string body, Dictionary<string, string> headers)
        {
            Command = command;
            Body = body;
            _headers = headers;

            if (command != null && command.Equals(StompCommand.Connect))
            {
                _headers["accept-version"] = "1.2";
            }

            if (body != null)
            {
                _headers["content-length"] = body.Length.ToString();
            }
        }

        public Dictionary<string, string> Headers
        {
            get { return _headers; }
        }

        /// <summary>
        /// Gets the body for the stomp message.
        /// </summary>
        public string Body { get; private set; }

        /// <summary>
        /// Gets the command for the stomp message.
        /// </summary>
        public string Command { get; private set; }

        /// <summary>
        /// Sets the content type to plain text.
        /// </summary>
        public void SetPlainTextContentType()
        {
            _headers["content-type"] = "text/plain";
        } 
        
        /// <summary>
        /// Sets the ID header.
        /// </summary>
        /// <param name="id">Id to set.</param>
        public void SetId(string id)
        {
            _headers["id"] = id;
        }

        /// <summary>
        /// Sets the destination header.
        /// </summary>
        /// <param name="destination">Destination to set.</param>
        public void SetDestination(string destination)
        {
            _headers["destination"] = destination;
        }
    }
}
