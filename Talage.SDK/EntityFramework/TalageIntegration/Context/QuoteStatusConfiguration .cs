using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

using Talage.SDK.EntityFramework.TalageIntegration.Model;

namespace Talage.SDK.EntityFramework.TalageIntegration.Context
{
    
 

        public partial class QuoteStatusConfiguration : IEntityTypeConfiguration<QuoteStatus>
        {
            public void Configure(EntityTypeBuilder<QuoteStatus> builder)
            {
                builder.ToTable("QuoteStatus", "dbo");

                builder.HasKey(x => x.Id)
                       .HasName("PK_QuoteStatus")
                       .IsClustered();

                builder.Property(x => x.Id)
                    .HasColumnName("Id")
                    .HasColumnType("int")
                    .IsRequired()
                    .ValueGeneratedOnAdd()
                    .UseIdentityColumn();

                builder.Property(x => x.ApiResult)
                    .HasColumnName("ApiResult")
                    .HasColumnType("varchar(50)")
                    .IsRequired()
                    .HasMaxLength(50);

                builder.Property(x => x.QuotestatusId)
                    .HasColumnName("QuotestatusId")
                    .HasColumnType("int")
                    .IsRequired();

                builder.Property(x => x.QuotestatusDescription)
                    .HasColumnName("QuotestatusDescription")
                    .HasColumnType("text")
                    .IsRequired(false);

                builder.Property(x => x.CreatedDate)
                    .HasColumnName("CreatedDate")
                    .HasColumnType("datetime2")
                    .IsRequired();

                builder.Property(x => x.CreatedBy)
                    .HasColumnName("CreatedBy")
                    .HasColumnType("nvarchar(100)")
                    .IsRequired(false)
                    .HasMaxLength(100);

                builder.Property(x => x.UpdatedDate)
                    .HasColumnName("UpdatedDate")
                    .HasColumnType("datetime2")
                    .IsRequired();

                builder.Property(x => x.UpdatedBy)
                    .HasColumnName("UpdatedBy")
                    .HasColumnType("nvarchar(100)")
                    .IsRequired(false)
                    .HasMaxLength(100);

                InitializePartial(builder);
            }

            partial void InitializePartial(EntityTypeBuilder<QuoteStatus> builder);
        }
    
    // </auto-generated>

}
