namespace Prime.ServicesV2.Auth0.Models
{
    public class Auth0Identity
    {
        public string Connection { get; set; }
        public string user_id { get; set; }
        public string Provider { get; set; }
        public string IsSocial { get; set; }
    }
}
