namespace Prime.ServicesV2.Auth0.Models
{
    public class Auth0Application
    {
        public string Name { get; set; }
        public string ApiUrl { get; set; }
        public string Audience { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}
