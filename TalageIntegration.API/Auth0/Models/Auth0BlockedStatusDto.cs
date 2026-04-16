using Newtonsoft.Json;
using System.Collections.Generic;

namespace Prime.ServicesV2.Auth0.Models
{
    public class Auth0BlockedStatusDto
    {
        [JsonProperty("blocked_for")]
        public List<Auth0BlockedInfoDto> Blocks { get; set; }
    }

    public class Auth0BlockedInfoDto
    {
        public string Identifier { get; set; }
        public string Ip { get; set; }
        public string Connection { get; set; }
    }
}
