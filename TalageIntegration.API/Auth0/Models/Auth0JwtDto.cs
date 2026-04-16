using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.ServicesV2.Auth0.Models
{
    public class Auth0JwtDto
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string token_type { get; set; }
        public string scope { get; set; }
    }
}
