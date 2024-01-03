using CsClient.Connection.Service;
using CsClient.Connection.WebSocket;
using CsClient.Credentials;
using CsClient.Data.DTO;
using NLog;
using System;
using System.Net.Http;
using System.Net.WebSockets;
using System.Threading;

namespace CsClient
{
    /// <summary>
    /// Entry point for the application. Handles the service process.
    /// </summary>
    public class Program
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly Utils.Environment environment = new Utils.Environment();
        private readonly ICredentialRepository repository;
        private readonly ICredentialService credentialService;
        private readonly AuthenticationService authenticationService;


        public Program()
        {
            environment = new Utils.Environment();
            repository = new DPAPICredentialRepository(environment);
            credentialService = new CredentialManager(repository);
            authenticationService = new AuthenticationService(environment, credentialService);
            
        }

        /// <summary>
        /// Entry point
        /// </summary>
        /// <param name="args">Arguments.</param>
        public static void Main(string[] args)
        {
            //
            // IMPORTANT
            // Unclear as to why.. but powercfg /srumutil does not work in x86 contexts. Build mode must be x64.
            //
            WriteAsciiNameToWindow();
            Program program = new Program();
            program.Start();
        }

        /// <summary>
        /// Starts the recursive service.
        /// </summary>
        /// <exception cref="ArgumentNullException">If failed to fetch credentials from keystore.</exception>
        public void Start()
        {
            logger.Info("Starting Service.");

            // Create WebSocket. This is essential because in the event of a disconnect. It's good practice to dispose of old WebSocket clients.
            IWebSocket webSocket = new WebSocketDecorator();
            WebSocketConnection wsc = new WebSocketConnection(environment, webSocket);

            bool authenticated = authenticationService.IsAuthorized();
            while (!authenticated)
            {
                try
                {
                    logger.Info("Authenticating Machine...");
                    authenticated = authenticationService.AuthenticateUser().Result;
                }
                catch (AggregateException ae)
                {
                    foreach (var ex in ae.InnerExceptions)
                    {
                        switch (ex)
                        {
                            case HttpRequestException:
                            case WebSocketException:
                                logger.Info("Unable to make request to authentication endpoint");
                                logger.Error(ex.Message);
                                logger.ForExceptionEvent(ex);
                                continue;
                            case UriFormatException:
                                // Cannot recover from a bad uri format. Log the error and exit program.
                                logger.Fatal("Authentication endpoint url is malformed. Cannot connect to authentication server.");
                                logger.ForExceptionEvent(ex);
                                Environment.Exit(1); // Exit with error code 1 to define an unrecoverable error has thrown.
                                return;
                            default:
                                // Rethrow unhandled exceptions.
                                throw ex;
                        }
                    }
                    logger.Info("Retrying in 10s...");
                    Thread.Sleep(10000);
                    continue;
                }
              
                if (authenticated) break;

                Console.WriteLine("Machine not authorized.");
                Console.WriteLine("Enter machine name: ");
                string username = Console.ReadLine();
                Console.WriteLine("Enter machine passkey: ");
                string password = Console.ReadLine();

                UserCredentials credentials = new UserCredentials(username, password);
                credentialService.SaveCredentials(credentials);
            }

            logger.Info("Machine authenticated.");

            string machineName = credentialService.GetCredentials().Username ?? throw new ArgumentNullException("User credentials were used to login and stored, but could not be fetched from keystore.");
            string jwt = authenticationService.GetValidJwt().Result;

            WebSocketConnectionTask webSocketTask = new WebSocketConnectionTask(wsc);

            try
            {
                webSocketTask.Run(jwt, machineName).Wait();
            }
            catch (Exception ex)
            {
                logger.Info("UnExpected error thrown while running WebSocket service.");
                logger.Error(ex.Message);
                logger.ForExceptionEvent(ex);
            }
            finally
            {
                // If socket is closed or errors, re-establish service and restart the program.
                logger.Info("Service Finished. Attempting Restart...");
                Thread.Sleep(10000);
                Start();
            }
        }

        private static void WriteAsciiNameToWindow()
        {
            string[] ascii =
            {
                "=======================================================================",
                "## ##    ## ##    ## ##   ####       ####   ### ###  ###  ##  #### ##", 
                "##   ##  ##   ##  ##   ##   ##         ##     ##  ##    ## ##  # ## ##",
                "##       ####     ##        ##         ##     ##       # ## #    ##",
                "##        #####   ##        ##         ##     ## ##    ## ##     ##",
                "##           ###  ##        ##         ##     ##       ##  ##    ##",
                "##   ##  ##   ##  ##   ##   ##  ##     ##     ##  ##   ##  ##    ##",
                "## ##    ## ##    ## ##   ### ###    ####   ### ###  ###  ##   ####",
                "=======================================================================",
                "",
            };

            foreach (var item in ascii)
            {
                Console.WriteLine(item);
            }
        }
    }
}
