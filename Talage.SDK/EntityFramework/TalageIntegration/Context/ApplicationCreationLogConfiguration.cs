using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Talage.SDK.EntityFramework.TalageIntegration.Model;

namespace Talage.SDK.EntityFramework.TalageIntegration.Context
{
    public partial class ApplicationCreationLogConfiguration : IEntityTypeConfiguration<ApplicationCreationLog>
    {
        public void Configure(EntityTypeBuilder<ApplicationCreationLog> builder)
        {
            builder.ToTable("ApplicationCreationLogs", "dbo");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName("Id").ValueGeneratedOnAdd();
            builder.Property(x => x.ApplicationId).HasColumnName("ApplicationId").IsRequired().HasMaxLength(50);
            builder.Property(x => x.ResponseJson).HasColumnName("ResponseJson").IsRequired();
            builder.Property(x => x.IsDeleted).HasColumnName("IsDeleted").IsRequired().HasDefaultValue(false);
            builder.Property(x => x.CreatedOn).HasColumnName("CreatedOn").IsRequired().HasDefaultValueSql("GETDATE()");
            builder.Property(x => x.CreatedBy).HasColumnName("CreatedBy").HasMaxLength(100);
            builder.Property(x => x.LastUpdated).HasColumnName("LastUpdated");
            builder.Property(x => x.UpdatedBy).HasColumnName("UpdatedBy").HasMaxLength(100);

            builder.HasIndex(x => x.ApplicationId);

            InitializePartial(builder);
        }

        partial void InitializePartial(EntityTypeBuilder<ApplicationCreationLog> builder);
    }
}
