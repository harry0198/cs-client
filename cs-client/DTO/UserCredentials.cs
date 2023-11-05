

using Newtonsoft.Json;

namespace cs_client.DTO
{
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
