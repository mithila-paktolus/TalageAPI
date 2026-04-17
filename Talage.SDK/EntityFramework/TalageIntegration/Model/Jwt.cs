using System;
using System.Collections.Generic;

namespace Talage.SDK.EntityFramework.TalageIntegration.Model
{
    public partial class Jwt
    {
        public int JwtId { get; set; }
        public string SourceApplication { get; set; }
        public string Token { get; set; }
        public string Tenant { get; set; }
        public string Audience { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ExpirationDate { get; set; }

        public Jwt()
        {
            InitializePartial();
        }

        partial void InitializePartial();
    }
}
