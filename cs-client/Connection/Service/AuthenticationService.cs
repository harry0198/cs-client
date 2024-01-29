using CsClient.Credentials;
using CsClient.Data.DTO;
using Newtonsoft.Json;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace CsClient.Connection.Service
{
    public class AuthenticationService
    {
        private readonly string _authUrl;
        private readonly ICredentialService _credentialService;
        private readonly Utils.Environment _environment;
        private string _jwt;
        public AuthenticationService(Utils.Environment environment, ICredentialService credentialService)
        {
            this._credentialService = credentialService;
            this._environment = environment;
            this._authUrl = $"http://{_environment.GetHost()}/auth/login";
        }

        /// <summary>
        /// Pings the authentication endpoint and returns if a response was recevied.
        /// </summary>
        /// <returns>If received a result back from the endpoint.</returns>
        public async Task<bool> PingAsync()
        {
            Ping ping = new Ping();

            PingReply result = await ping.SendPingAsync(_authUrl);
            return result.Status == IPStatus.Success;
        }

        /// <summary>
        /// Checks to see if the user is authorized within a 1 minute window. If the token
        /// is set to expire within 1 minute or less, user is marked as unauthorized.
        /// </summary>
        /// <returns>True if jwt is validated. False if not. If token expires in <1 minutes, token is still marked as unauthorized.</returns>
        public bool IsAuthorized()
        {
            if (_jwt == null) return false;

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken token = handler.ReadJwtToken(_jwt);
            DateTime validilityWindow = DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(1));

            return validilityWindow <= token.ValidTo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="HttpRequestException">If posting a request to the endpoint fails.</exception>
        /// <exception cref="UriFormatException">If the given url is incorrect format.</exception>
        public async Task<bool> AuthenticateUser()
        {
            UserCredentials creds = _credentialService.GetCredentials();
            if (creds == null) return false;

            using (HttpClient client = new HttpClient())
            {
                string body = JsonConvert.SerializeObject(creds);
                StringContent content = new StringContent(body, System.Text.Encoding.UTF8, "application/json");

                // Critical, throws http/uri
                Task<HttpResponseMessage> postTask = client.PostAsync(_authUrl, content);
                var response = await postTask;
                

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    LoginResponse loginResponse = JsonConvert.DeserializeObject<LoginResponse>(responseContent);
                    this._jwt = loginResponse.AccessToken;

                    return true;
                }
            }

            return false;
        }

        public async Task<string> GetValidJwt()
        {
            if (IsAuthorized())
            {
                return _jwt;
            }

            bool result = await AuthenticateUser();
            if (result)
            {
                throw new AuthenticationException("User could not be authenticated.");
            }

            return _jwt;
        }
    }
}
