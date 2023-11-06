using cs_client.Connection.Stomp;
using cs_client.DTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using WebSocketSharp;

namespace cs_client.Connection
{
    public class WebSocketConnection
    {
        private readonly Utils.Environment _environment;
        private readonly AuthenticationService _authenticationService;

        public WebSocketConnection(AuthenticationService _authenticationService, Utils.Environment environment) 
        {
            this._environment = environment;
            this._authenticationService = _authenticationService;
        }

        public void Connect()
        {
            // Impossible for the client to continue without the jwt so block until we have it.
            string jwt = _authenticationService.GetValidJwt().Result;
            using (var ws = new WebSocket("ws://localhost:8080/handshake"))
            {
                ws.OnMessage += OnMessage;
                ws.OnOpen += OnOpen;
                ws.OnError += OnError;
                ws.Connect();
                Thread.Sleep(1000);

                StompMessageSerializer serializer = new StompMessageSerializer();

                var connect = new StompMessage("CONNECT");
                connect["accept-version"] = "1.2";
                connect["host"] = "";
                connect["Authorization"] = $"Bearer {jwt}";
                ws.Send(serializer.Serialize(connect));

                var clientId = RandomString(5);
                Console.WriteLine("Client Id :" + clientId);
                Thread.Sleep(1000);

                var sub = new StompMessage("SUBSCRIBE");
                sub["id"] = "m-01";
                sub["destination"] = "/channel/energy";
                sub["Authorization"] = $"Bearer {jwt}";
                ws.Send(serializer.Serialize(sub));

                Thread.Sleep(1000);
                var statistic = new EnergyStatistic();
                statistic.AppId = "hello!";
                var list = new List<EnergyStatistic>();
                list.Add(statistic);
                var broad = new StompMessage("SEND", JsonConvert.SerializeObject(list, Formatting.Indented));
                broad["content-type"] = "application/json";
                broad["destination"] = "/publish/energy";
                ws.Send(serializer.Serialize(broad));

                Console.ReadKey(true);
            }

        }

        public static void OnOpen(object sender, EventArgs e)
        {
            Console.WriteLine("------");
            Console.WriteLine(DateTime.Now.ToString() + "WSOPEN" + e.ToString());
        }

        public static void OnMessage(object sender, MessageEventArgs e)
        {
            Console.WriteLine(e.ToString());
            Console.WriteLine(DateTime.Now.ToString() + "WSMESS" + e.Data);
        }

        public static void OnError(object sender, ErrorEventArgs e)
        {
            throw e.Exception;
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
