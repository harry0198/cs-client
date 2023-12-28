using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CsClient.Connection.Stomp
{
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

            this["content-length"] = body.Length.ToString();
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
        /// Gets or sets the header attributes.
        /// </summary>
        /// <param name="header">Header to get/set</param>
        public string this[string header]
        {
            get { return this._headers.ContainsKey(header) ? this._headers[header] : string.Empty; }
            set { _headers[header] = value; }
        }

        /// <summary>
        /// Sets the content type to plain text.
        /// </summary>
        public void SetPlainTextContentType()
        {
            this["content-type"] = "text/plain";
        } 
    }
}
