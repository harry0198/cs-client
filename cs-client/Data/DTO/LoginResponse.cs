using Newtonsoft.Json;

namespace CsClient.Data.DTO
{
    /// <summary>
    /// Data object for mapping the json received Login Response.
    /// Contains fields for the access token.
    /// </summary>
    public class LoginResponse
    {
        /// <summary>
        /// Access Token for authorization in subsequent requests.
        /// </summary>
        [JsonProperty(PropertyName = "accessToken")]
        public string AccessToken { get; set; }
    }
}
