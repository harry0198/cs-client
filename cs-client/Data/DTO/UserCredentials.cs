

using Newtonsoft.Json;

namespace CsClient.Data.DTO
{
    /// <summary>
    /// Data Transfer Object for machine authorization.
    /// Contains fields for the username and password.
    /// </summary>
    public class UserCredentials
    {
        public UserCredentials(string userName, string password)
        {
            Username = userName;
            Password = password;
        }

        [JsonProperty(PropertyName = "machineName")]
        public string Username { get;}

        [JsonProperty(PropertyName = "password")]
        public string Password { get;}
    }
}
