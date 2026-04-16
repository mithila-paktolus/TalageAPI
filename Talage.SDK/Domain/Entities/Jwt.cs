namespace TalageIntegration.Domain.Entities
{
    public partial class Jwt
    {
        public int JwtId { get; set; } // JwtId (Primary key)
        public string SourceApplication { get; set; } // SourceApplication (length: 50)
        public string Token { get; set; } // Token
        public string Tenant { get; set; } // Tenant (length: 50)
        public string Audience { get; set; } // Audience (length: 50)
        public DateTime CreatedDate { get; set; } // CreatedDate
        public DateTime ExpirationDate { get; set; } // ExpirationDate

        public Jwt()
        {
            InitializePartial();
        }

        partial void InitializePartial();
    }
}

