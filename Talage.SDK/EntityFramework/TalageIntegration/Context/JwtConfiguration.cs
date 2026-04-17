using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Talage.SDK.EntityFramework.TalageIntegration.Model;

namespace Talage.SDK.EntityFramework.TalageIntegration.Context
{
    public partial class JwtConfiguration : IEntityTypeConfiguration<Jwt>
    {
        public void Configure(EntityTypeBuilder<Jwt> builder)
        {
            builder.ToTable("Jwt", "dbo");
            builder.HasKey(x => x.JwtId);

            builder.Property(x => x.SourceApplication).HasMaxLength(50);
            builder.Property(x => x.Tenant).HasMaxLength(50);
            builder.Property(x => x.Audience).HasMaxLength(50);

            InitializePartial(builder);
        }

        partial void InitializePartial(EntityTypeBuilder<Jwt> builder);
    }
}
