using CsClient.Connection.Stomp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsClient.Connection
{
    public interface IWebSocketSerializer<T>
    {

        /// <summary>
        /// Serializes the specified message.
        /// </summary>
        /// <param name = "message">The message to serialize.</param>
        /// <returns>A serialized version of the given object/></returns>
        string Serialize(T message);

        /// <summary>
        /// Deserializes the specified message.
        /// </summary>
        /// <param name="message">The message to deserialize.</param>
        /// <returns>A deserialized n object instance</returns>
        T Deserialize(string message);
    }
}
