using System;
using System.Collections.Generic;

namespace Talage.SDK.EntityFramework.TalageIntegration.Model
{
    public partial class AccessToken
    {
        public int Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public string TokenType { get; set; } = "Bearer";
        public string? RefreshToken { get; set; }
        public DateTime ExpiresDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; } = "System";
        public DateTime? LastUpdated { get; set; }
        public string? UpdatedBy { get; set; }
        public bool IsActive { get; set; }
    }
}
