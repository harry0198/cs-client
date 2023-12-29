using CsClient.Connection;
using CsClient.Connection.Service;
using CsClient.Credentials;
using CsClient.Data.DTO;
using CsClient.Statistic;
using System;

namespace CsClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            
            //
            // IMPORTANT
            // Unclear as to why.. but powercfg /srumutil does not work in x86 contexts. Build mode must be x64.
            //
            Utils.Environment environment = new Utils.Environment();
            ICredentialRepository repository = new DPAPICredentialRepository(environment);
            ICredentialService credentialService = new CredentialManager(repository);
            AuthenticationService authenticationService = new AuthenticationService(environment, credentialService);
            IWebSocket webSocket = new WebSocketDecorator();
            WebSocketConnection wsc = new WebSocketConnection(authenticationService, environment, webSocket);
            SystemStatistics systemStatistics = new SystemStatistics();


            bool authenticated = authenticationService.IsAuthorized();
            while (!authenticated)
            {
                authenticated = authenticationService.AuthenticateUser().Result;
                if (authenticated) break;

                Console.WriteLine("Machine not authorized.");
                Console.WriteLine("Enter machine name: ");
                string username = Console.ReadLine();
                Console.WriteLine("Enter machine passkey: ");
                string password = Console.ReadLine();

                UserCredentials credentials = new UserCredentials(username, password);
                credentialService.SaveCredentials(credentials);
            }

            string machineName = credentialService.GetCredentials().Username ?? throw new ArgumentNullException("User credentials were used to login and stored, but could not be fetched from keystore.");
            wsc.Connect(machineName);
            Console.ReadLine();
            
            /*
            SystemStatistics statistics = new SystemStatistics();
            while (true)
            {
                float cpu = statistics.GetCpuUsageAsync().Result;
                float disk = statistics.GetDiskUsageAsync().Result;
                float mem = statistics.GetMemoryUsageAsync().Result;

                Console.WriteLine("Cpu: " + cpu);
                Console.WriteLine("Mem: " + mem);
                Console.WriteLine("Disk: " + disk);

                Thread.Sleep(1000);
            }*/
        }
    }
}
