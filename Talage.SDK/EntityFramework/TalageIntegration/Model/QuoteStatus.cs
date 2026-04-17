using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talage.SDK.EntityFramework.TalageIntegration.Model
{
    
        public partial class QuoteStatus
        {
            public int Id { get; set; } // Primary Key
            public string ApiResult { get; set; } // varchar(50)
            public int QuotestatusId { get; set; } // int
            public string QuotestatusDescription { get; set; } // text
            public DateTime CreatedDate { get; set; } // datetime2
            public string CreatedBy { get; set; } // nvarchar(100)
            public DateTime UpdatedDate { get; set; } // datetime2
            public string UpdatedBy { get; set; } // nvarchar(100)

            public QuoteStatus()
            {
                CreatedDate = DateTime.Now;
                UpdatedDate = DateTime.Now;
                CreatedBy = "System";
                UpdatedBy = "System";

                InitializePartial();
            }

            partial void InitializePartial();
        }
    
}
