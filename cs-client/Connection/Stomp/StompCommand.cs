using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs_client.Connection.Stomp
{
    public static class StompCommand
    {
        public static readonly string Connect = "CONNECT";
        public static readonly string Subscribe = "SUBSCRIBE";
        public static readonly string Send = "SEND";
    }
}
