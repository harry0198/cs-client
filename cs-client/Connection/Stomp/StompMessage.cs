﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs_client.Connection.Stomp
{
    public class StompMessage
    {
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
        internal StompMessage(string command, string body, Dictionary<string, string> headers)
        {
            Command = command;
            Body = body;
            Headers = headers;

            // Set the content-length header.
            this["content-length"] = body.Length.ToString();
        }

        /// <summary>
        /// Gets the headers for the stomp message.
        /// </summary>
        public Dictionary<string, string> Headers { get; private set; } = new Dictionary<string, string>();

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
        /// <param name="header"></param>
        /// <returns></returns>
        public string this[string header]
        {
            get { return this.Headers.ContainsKey(header) ? this.Headers[header] : string.Empty; }
            set { Headers[header] = value; }
        }
    }
}
