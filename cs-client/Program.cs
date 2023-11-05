using cs_client.Credentials;
using cs_client.Utils;
using cs_client.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cs_client.Connection;

namespace cs_client
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Utils.Environment environment = new Utils.Environment();
            ICredentialRepository repository = new DPAPICredentialRepository(environment);
            ICredentialService credentialService = new CredentialManager(repository);
            AuthenticationService authenticationService = new AuthenticationService(environment, credentialService);
            WebSocketConnection wsc = new WebSocketConnection(authenticationService, environment);

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



            wsc.Connect();

/*

            ICredentialRepository credentialRepository = new DPAPICredentialRepository(environment);
            ICredentialService credentialService = new JWTCredentialManager(credentialRepository);

            UserCredentials userCreds = credentialService.GetCredentials();

            string machineId = System.Environment.MachineName;
            Console.WriteLine($"Your machine id is {machineId}");

            while (userCreds == null)
            {
                Console.Write("Enter the machine passkey: ");
                string passKey = Console.ReadLine();

                if (passKey != null)
                {
                    // authenticate
                }
            }*/
        }
    }
}
