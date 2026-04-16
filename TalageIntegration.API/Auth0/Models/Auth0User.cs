namespace Prime.ServicesV2.Auth0.Models
{
    public class Auth0User
    {
        public string Name { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Tenant { get; set; }
        public bool Blocked { get; set; }
        public string user_id { get; set; }
        public bool email_verified { get; set; }
        public List<Auth0Identity> Identities { get; set; }
}
}
