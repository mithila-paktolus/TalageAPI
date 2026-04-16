using System.Collections.Generic;

namespace Prime.ServicesV2.Auth0.Models
{
    public class Auth0Tenant
    {
        public string Name { get; set; }
        public string JwtUrl { get; set; }
        public List<Auth0Application> Applications { get; set; }
    }
}
