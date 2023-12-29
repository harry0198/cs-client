using CsClient.Connection.Service;
using CsClient.Connection.Stomp;
using CsClient.Credentials;
using CsClient.Data;
using CsClient.Data.DTO;
using CsClient.Statistic;
using CsClient.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;

namespace CsClient.Connection
{
    public class WebSocketConnection
    {
        private readonly Utils.Environment _environment;
        private readonly AuthenticationService _authenticationService;
        private readonly IWebSocket _webSocket;

        public WebSocketConnection(AuthenticationService authenticationService, Utils.Environment environment, IWebSocket webSocket) 
        {
            this._environment = environment;
            this._authenticationService = authenticationService;
            this._webSocket = webSocket;
        }

        public async void Connect(string machineId)
        {
            // Impossible for the client to continue without the jwt so block until we have it.
            string jwt = await _authenticationService.GetValidJwt();

            StompMessageSerializer serializer = new StompMessageSerializer();

            try
            {
                await this._webSocket.ConnectAsync(_environment.GetWebSocketUrl());

                var connect = new StompMessage(StompCommand.Connect);
                connect["accept-version"] = "1.2";
                connect["host"] = "";
                connect["Authorization"] = $"Bearer {jwt}";
                await _webSocket.SendAsync(serializer.Serialize(connect));

                while (_webSocket.State == WebSocketState.Open)
                {
                    var (result, content) = await _webSocket.ReceiveAsync();
                    switch (result.MessageType)
                    {
                        case WebSocketMessageType.Text:
                            Console.WriteLine(content);
                            StompMessage message = serializer.Deserialize(content);
                            string text = message.Body;
                            if (text.Equals("REQUEST-ENERGY"))
                            {
                                EnergyStatisticTask stat = new EnergyStatisticTask();
                                string samplePath = stat.NewSample();

                                EnergyStatisticsCsvProcessor csvProcessor = new EnergyStatisticsCsvProcessor(samplePath, new WindowsSIDAccountHelper());
                                string statistics = csvProcessor.ProcessCsv();

                                var broad2 = new StompMessage(StompCommand.Send, statistics);
                                broad2.SetPlainTextContentType();
                                broad2["destination"] = "/publish/energy";
                                try
                                {
                                    await _webSocket.SendAsync(serializer.Serialize(broad2));
                                } catch (Exception e)
                                {
                                    Console.WriteLine(e.ToString());
                                }
                            }
                            break;
                        case WebSocketMessageType.Close:
                            Console.WriteLine("Close message received.");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                if (_webSocket.State == WebSocketState.Open)
                {
                    // Close
                    await _webSocket.CloseAsync();
                }
            }
        }

        public class Content
        {
            public string Subject { get; set; }
            public string Message { get; set; }
        }


        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
