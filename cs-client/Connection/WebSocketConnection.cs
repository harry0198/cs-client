using cs_client.Connection.Stomp;
using cs_client.Credentials;
using cs_client.DTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
            using (var websocket = new WebSocket($"ws://{_environment.Host}/handshake"))
            {
                websocket.OnMessage += OnMessage;
                websocket.OnOpen += OnOpen;
                websocket.OnError += OnError;
                websocket.Connect();
                Thread.Sleep(1000);

                StompMessageSerializer serializer = new StompMessageSerializer();

                var connect = new StompMessage(StompCommand.Connect);
                connect["accept-version"] = "1.2";
                connect["host"] = "";
                connect["Authorization"] = $"Bearer {jwt}";
                websocket.Send(serializer.Serialize(connect));

                var sub = new StompMessage("SUBSCRIBE");
                sub["id"] = "sub-01";
                sub["destination"] = "/topic/greetings";
                websocket.Send(serializer.Serialize(sub));

                var energyStatistic = new EnergyStatistic();
                var broad = new StompMessage(StompCommand.Send, JsonConvert.SerializeObject(energyStatistic));
                broad["content-type"] = "application/json";
                broad["destination"] = "/energy";
                websocket.Send(serializer.Serialize(broad));

            }
        }

        public static void OnOpen(object sender, EventArgs e)
        {
            Console.WriteLine("------");
            Console.WriteLine(DateTime.Now.ToString() + "WSOPEN" + e.ToString());
        }

        public static void OnMessage(object sender, MessageEventArgs e)
        {
            Console.WriteLine(DateTime.Now.ToString() + "WSMESS" + e.Data);
        }

        public static void OnError(object sender, ErrorEventArgs e)
        {
            Console.WriteLine(DateTime.Now.ToString() + "WSERROR" + e.Message);
        }
    }
}
