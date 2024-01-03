using CsClient.Connection.WebSocket;
using CsClientTests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace CsClientTests.Connection
{
    [TestClass]
    public class WebSocketConnectionTest
    {
        private MockIWebSocket _webSocket;
        private WebSocketConnection _connection;

        [TestInitialize]
        public void setUp()
        {
            _webSocket = new MockIWebSocket();
            _connection = new WebSocketConnection(new CsClient.Utils.Environment(), _webSocket);
        }

        /// <summary>
        /// Tests the <see cref="WebSocketConnection.OpenConnectionAsync(string)"/> function
        /// with the correct jwt does connect.
        /// </summary>
        [TestMethod]
        public void OpenConnectionAsync_CorrectJwt_Connects()
        {
            // Arrange
            _webSocket.RejectOpenRequest = false;
            string expectedConnectMessage = "CONNECT\naccept-version:1.2\ncontent-length:0\n\n\0";

            // Act
            _connection.OpenConnectionAsync("").Wait();

            // Assert
            Assert.AreEqual(WebSocketState.Open, _webSocket.State);
            Assert.AreEqual(1, _webSocket.Message.Count);
            Assert.AreEqual(expectedConnectMessage, _webSocket.Message.Pop());
        }

        /// <summary>
        /// Tests that when the open connection is rejected and throws the WebSocketException that it is not handled
        /// here.
        /// </summary>
        /// <returns>Task</returns>
        [TestMethod]
        public async Task OpenConnectionAsync_RejectsOrServerInvalid()
        {
            // Arrange
            _webSocket.RejectOpenRequest = true;

            // Act and Assert
            await Assert.ThrowsExceptionAsync<WebSocketException>(async () =>
            {
                await _connection.OpenConnectionAsync("");
            });
        }

        /// <summary>
        /// Tests that the <see cref="WebSocketConnection.SendConnectMessageAsync"/> does send the correct
        /// message contents.
        /// </summary>
        /// <returns>Task.</returns>
        [TestMethod]
        public async Task SendConnectMessageAsync_DoesSendCorrectly()
        {
            // Arrange
            string expectedMessage = "CONNECT\naccept-version:1.2\ncontent-length:0\n\n\0";

            // Act
            await _connection.SendConnectMessageAsync();

            // Assert
            Assert.AreEqual(1, _webSocket.Message.Count);
            Assert.AreEqual(expectedMessage, _webSocket.Message.Pop());
        }

        /// <summary>
        /// Tests the <see cref="WebSocketConnection."/>
        /// </summary>
        /// <param name="machineId">Machine id to test with.</param>
        /// <param name="throwsException">If should throw exception.</param>
        /// <returns>Task.</returns>
        [DataTestMethod]
        [DataRow("valid-machine-id", false)]
        [DataRow("valid id", false)]
        [DataRow("", true)]
        [DataRow(null, true, DisplayName = "Null")]
        public async Task SubscribeToPingChannelAsync(string machineId, bool throwsException)
        {
            // Arrange
            string expectedOutcome = $"SUBSCRIBE\ncontent-length:0\nid:{machineId}\ndestination:/channel/request\n\n\0";

            // Act / assert
            if (throwsException)
            {
                await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
                {
                    await _connection.SubscribeToPingChannelAsync(machineId);
                });
                return;
            }
            else
            {
                await _connection.SubscribeToPingChannelAsync(machineId);
            }

            // Assert outcome
            Assert.AreEqual(1, _webSocket.Message.Count);
            Assert.AreEqual(expectedOutcome, _webSocket.Message.Pop());
        }

        [DataTestMethod]
        [DataRow("message", "destination", false)]
        [DataRow("anothjer", "ok", false)]
        [DataRow("test here", "long/destination/ok/here/ok/ok/ok", false)]
        [DataRow("message here hello hello \n\0", "dest", false)]
        [DataRow("", "dest", false)]
        [DataRow("", "", true)]
        [DataRow(null, "eea", true)]
        [DataRow("e", null, true)]
        public async Task SendMessage_DoesSend(string message, string destination, bool throws)
        {
            // Arrange
            string expectedMessage = $"SEND\ncontent-length:{message?.Length}\ncontent-type:text/plain\ndestination:{destination}\n\n{message}\0";

            // Act
            if (throws)
            {
                await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
                {
                    await _connection.SendMessage(message, destination);
                });
                return;
            }
            else
            {
                await _connection.SendMessage(message, destination);
            }

            // Assert
            Assert.AreEqual(1, _webSocket.Message.Count());
            Assert.AreEqual(expectedMessage , _webSocket.Message.Pop());
        }

        /// <summary>
        /// Tests that the receive async method does receive messages.
        /// </summary>
        /// <returns>Task.</returns>
        [TestMethod]
        public async Task ReceiveAsync_DoesReceive()
        {
            // Arrange
            string expected = "send";
            string sendMe = $"SEND\ncontent-length:{expected.Length}\ncontent-type:text/plain\ndestination:dest\n\n{expected}\0";
            _webSocket.Message.Push(sendMe);
            _webSocket.State = WebSocketState.Open;

            // Act
            var (result, content) = await _connection.ReceiveAsync();

            // Assert
            Assert.AreEqual(expected, content);
            Assert.AreEqual(WebSocketMessageType.Text, result.MessageType);
        }
    }
}
