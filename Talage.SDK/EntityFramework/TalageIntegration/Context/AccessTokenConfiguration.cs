using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Talage.SDK.EntityFramework.TalageIntegration.Model;

namespace Talage.SDK.EntityFramework.TalageIntegration.Context
{
    public partial class AccessTokenConfiguration : IEntityTypeConfiguration<AccessToken>
    {
        public void Configure(EntityTypeBuilder<AccessToken> builder)
        {
            builder.ToTable("AccessTokens", "dbo");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName("Id").ValueGeneratedOnAdd();
            builder.Property(x => x.Token).HasColumnName("Token").IsRequired();
            builder.Property(x => x.TokenType).HasColumnName("TokenType").IsRequired().HasMaxLength(50);
            builder.Property(x => x.RefreshToken).HasColumnName("RefreshToken");
            builder.Property(x => x.ExpiresDate).HasColumnName("ExpiresDate").IsRequired();
            builder.Property(x => x.CreatedDate).HasColumnName("CreatedDate").IsRequired();
            builder.Property(x => x.CreatedBy).HasColumnName("CreatedBy").IsRequired().HasMaxLength(100);
            builder.Property(x => x.LastUpdated).HasColumnName("LastUpdated");
            builder.Property(x => x.UpdatedBy).HasColumnName("UpdatedBy").HasMaxLength(100);
            builder.Property(x => x.IsActive).HasColumnName("IsActive").IsRequired();

            InitializePartial(builder);
        }

        partial void InitializePartial(EntityTypeBuilder<AccessToken> builder);
    }
}
