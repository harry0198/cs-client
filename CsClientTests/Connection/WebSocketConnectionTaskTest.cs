using CsClient.Connection.WebSocket;
using CsClientTests.Mocks;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Microsoft.VisualStudio.TestPlatform.PlatformAbstractions.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace CsClientTests.Connection
{
    [TestClass]
    public class WebSocketConnectionTaskTest
    {
        private Mock<WebSocketConnection> _mockWebSocketConnection;
        private WebSocketConnectionTask _webSocketConnectionTask;

        [TestInitialize]
        public void Setup()
        {
            // Arrange: Create a mock of WebSocketConnection
            MockEnvironment mockEnvironment = new MockEnvironment();
            MockIWebSocket mockIWebSocket = new MockIWebSocket();
            _mockWebSocketConnection = new Mock<WebSocketConnection>(mockEnvironment, mockIWebSocket);
            _webSocketConnectionTask = new WebSocketConnectionTask(_mockWebSocketConnection.Object);
        }

        /// <summary>
        /// Tests the complete lifecycle if all methods proceed as expected.
        /// Checks that the ReceiveAsync does close when close message is received.
        /// </summary>
        /// <returns>Task.</returns>
        [TestMethod]
        public async Task Run_Lifecycle_Complete()
        {
            // Arrange: Set up mock behavior
            string jwt = "sampleJwt";
            string machineId = "machineId";
            _mockWebSocketConnection.Setup(c => c.OpenConnectionAsync(It.IsAny<string>())).Returns(Task.CompletedTask);
            _mockWebSocketConnection.Setup(c => c.SubscribeToPingChannelAsync(It.IsAny<string>())).Returns(Task.CompletedTask);
            _mockWebSocketConnection.Setup(c => c.State).Returns(WebSocketState.Open);

            // Receives the close command.
            _mockWebSocketConnection.Setup(c => c.ReceiveAsync()).ReturnsAsync((new WebSocketReceiveResult(0, WebSocketMessageType.Close, true), ""));

            // Act: Call the Run method
            await _webSocketConnectionTask.Run(jwt, machineId);

            // Assert: Check that the methods were called
            _mockWebSocketConnection.Verify(c => c.OpenConnectionAsync(jwt), Times.Once);
            _mockWebSocketConnection.Verify(c => c.SubscribeToPingChannelAsync(machineId), Times.Once);
            _mockWebSocketConnection.Verify(c => c.ReceiveAsync(), Times.Once);
            _mockWebSocketConnection.Verify(c => c.CloseConnectionAsync(), Times.Once);
        }

        /// <summary>
        /// Runs the task but fail when attempting to open the connection and assert is handled correctly.
        /// </summary>
        /// <returns>Task.</returns>
        [TestMethod]
        public async Task Run_OpenConnection_WebSocketException_Handled()
        {
            // Arrange: Set up mock behavior
            string jwt = "sampleJwt";
            string machineId = "machineId";
            _mockWebSocketConnection.Setup(c => c.OpenConnectionAsync(It.IsAny<string>())).ThrowsAsync(new WebSocketException());

            // Act: Call the Run method
            await _webSocketConnectionTask.Run(jwt, machineId);

            // Assert: Check that the methods were called
            _mockWebSocketConnection.Verify(c => c.OpenConnectionAsync(jwt), Times.Once);
            _mockWebSocketConnection.Verify(c => c.CloseConnectionAsync(), Times.Never); // Should never run this as the connection was never opened.
        }

        /// <summary>
        /// Tests that the run function does keep running until the close message is received from ReceiveAsync.
        /// </summary>
        /// <returns>Task.</returns>
        [TestMethod]
        public async Task Run_ReceiveAsyncConnection_DoesLoop()
        {
            // Arrange: Set up mock behavior
            string jwt = "sampleJwt";
            string machineId = "machineId";
            string websocketReceive = "mess";
            int bytes = Encoding.UTF8.GetByteCount(websocketReceive);
            _mockWebSocketConnection.Setup(c => c.OpenConnectionAsync(It.IsAny<string>())).Returns(Task.CompletedTask);
            _mockWebSocketConnection.SetupSequence(c => c.ReceiveAsync())
                .Returns(Task.FromResult((new WebSocketReceiveResult(bytes, WebSocketMessageType.Text, true), websocketReceive)))
                .Returns(Task.FromResult((new WebSocketReceiveResult(0, WebSocketMessageType.Close, true), "")));
            _mockWebSocketConnection.Setup(c => c.State).Returns(WebSocketState.Open);

            // Act: Call the Run method
            await _webSocketConnectionTask.Run(jwt, machineId);

            // Assert: Check that the methods were called
            _mockWebSocketConnection.Verify(c => c.OpenConnectionAsync(jwt), Times.Once);
            _mockWebSocketConnection.Verify(c => c.ReceiveAsync(), Times.Exactly(2));
            _mockWebSocketConnection.Verify(c => c.CloseConnectionAsync(), Times.Once);
        }

        /// <summary>
        /// Tests that a WebSocketException is handled when failing to receive asynchronously
        /// and the connection is closed gracefully.
        /// </summary>
        /// <returns>Task.</returns>
        [TestMethod]
        public async Task Run_ReceiveAsyncConnection_WebSocketException_Handled()
        {
            // Arrange: Set up mock behavior
            string jwt = "sampleJwt";
            string machineId = "machineId";
            string websocketReceive = "mess";
            int bytes = Encoding.UTF8.GetByteCount(websocketReceive);
            _mockWebSocketConnection.Setup(c => c.OpenConnectionAsync(It.IsAny<string>())).Returns(Task.CompletedTask);
            _mockWebSocketConnection.Setup(c => c.ReceiveAsync())
                .ThrowsAsync(new WebSocketException());
            _mockWebSocketConnection.Setup(c => c.State).Returns(WebSocketState.Open);

            // Act: Call the Run method
            await _webSocketConnectionTask.Run(jwt, machineId);

            // Assert: Check that the methods were called
            _mockWebSocketConnection.Verify(c => c.OpenConnectionAsync(jwt), Times.Once);
            _mockWebSocketConnection.Verify(c => c.ReceiveAsync(), Times.Once);
            _mockWebSocketConnection.Verify(c => c.CloseConnectionAsync(), Times.Once);
        }
    }
}
