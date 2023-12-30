using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace CsClient.Connection
{
    public delegate void WebSocketMessageHandler(WebSocketReceiveResult result, string content);
}
