using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace cs_client.DTO
{
    public class LoginResponse
    {

        [JsonProperty(PropertyName = "accessToken")]
        public string AccessToken { get; set; }
    }
}
