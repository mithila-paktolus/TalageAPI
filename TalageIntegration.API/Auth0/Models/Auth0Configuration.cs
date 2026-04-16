using System.Collections.Generic;

namespace Prime.ServicesV2.Auth0.Models
{
    public class Auth0Configuration
    {
        public const string SectionName = "Auth0";
        public string WSExternalV5Audience { get; set; }
        public List<Auth0Tenant> Tenants { get; set; }
        public string HealthCheckEmail { get; set; }
    }
}
