using Newtonsoft.Json;

namespace Prime.ServicesV2.Auth0.Models
{
    public class Auth0ErrorDto
    {
        [JsonProperty("error")]
        public string ErrorType { get; set; }

        [JsonProperty("error_description")]
        public string ErrorDescription { get; set; }
    }
}
