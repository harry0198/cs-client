using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs_client.Connection
{
    public class AuthenticationException : ApplicationException
    {
        public AuthenticationException(string message) : base(message) { }
    }
}
