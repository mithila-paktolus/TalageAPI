using System;
using System.Collections.Generic;

namespace Talage.SDK.EntityFramework.TalageIntegration.Model
{
    public partial class ApplicationCreationLog
    {
        public long Id { get; set; }
        public string ApplicationId { get; set; }
        public string ResponseJson { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? LastUpdated { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
