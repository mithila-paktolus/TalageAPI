using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talage.SDK.EntityFramework.TalageIntegration.Model;

namespace Talage.SDK.EntityFramework.TalageIntegration.Context
{
    
    public partial class TalageIntegrationContext : DbContext, ITalageIntegrationContext
    {
        public TalageIntegrationContext()
        {
            InitializePartial();
        }

        public TalageIntegrationContext(DbContextOptions<TalageIntegrationContext> options)
            : base(options)
        {
            InitializePartial();
        }

        public DbSet<QuoteStatus> QuoteStatus { get; set; }
        public DbSet<AccessToken> AccessTokens { get; set; }
        public DbSet<ApplicationCreationLog> ApplicationCreationLogs { get; set; }
        public DbSet<Jwt> Jwt { get; set; }


        public bool IsSqlParameterNull(SqlParameter param)
        {
            var sqlValue = param.SqlValue;
            var nullableValue = sqlValue as INullable;
            if (nullableValue != null)
                return nullableValue.IsNull;
            return (sqlValue == null || sqlValue == DBNull.Value);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new QuoteStatusConfiguration());
            modelBuilder.ApplyConfiguration(new AccessTokenConfiguration());
            modelBuilder.ApplyConfiguration(new ApplicationCreationLogConfiguration());
            modelBuilder.ApplyConfiguration(new JwtConfiguration());
         


          //  modelBuilder.Entity<AuditLog>().ToTable(tb => tb.HasTrigger("tr_dbo_AuditLog"));
           
       

            OnModelCreatingPartial(modelBuilder);
        }


        partial void InitializePartial();
        partial void DisposePartial(bool disposing);
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
        static partial void OnCreateModelPartial(ModelBuilder modelBuilder, string schema);



    }
}
