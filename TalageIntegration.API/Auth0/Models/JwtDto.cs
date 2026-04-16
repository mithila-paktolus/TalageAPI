using System;

namespace Prime.ServicesV2.Auth0.Models
{
    public class JwtDto
    {
        public string Token { get; set; }
        public string Tenant { get; set; }
        public string Audience { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
