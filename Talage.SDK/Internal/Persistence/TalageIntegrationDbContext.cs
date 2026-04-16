using Microsoft.EntityFrameworkCore;
using TalageIntegration.Domain.Entities;

namespace Talage.SDK.Internal.Persistence;

public sealed class TalageIntegrationDbContext(DbContextOptions<TalageIntegrationDbContext> options) : DbContext(options)
{
    public DbSet<AccessTokenEntity> AccessTokens => Set<AccessTokenEntity>();
    public DbSet<ApplicationCreationLogEntity> ApplicationCreationLogs => Set<ApplicationCreationLogEntity>();
    public DbSet<Jwt> Jwt => Set<Jwt>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AccessTokenEntity>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).ValueGeneratedOnAdd();

            entity.Property(x => x.Token).IsRequired();
            entity.Property(x => x.TokenType).IsRequired();
            entity.Property(x => x.CreatedBy).IsRequired();
            entity.Property(x => x.IsActive).IsRequired();
        });

        modelBuilder.Entity<ApplicationCreationLogEntity>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).ValueGeneratedOnAdd();

            entity.Property(x => x.ApplicationId).IsRequired();
            entity.Property(x => x.ResponseJson).IsRequired();

            entity.Property(x => x.IsDeleted).IsRequired().HasDefaultValue(false);
            entity.Property(x => x.CreatedOn).IsRequired().HasDefaultValueSql("GETDATE()");

            entity.Property(x => x.CreatedBy).HasMaxLength(100);
            entity.Property(x => x.UpdatedBy).HasMaxLength(100);

            entity.HasIndex(x => x.ApplicationId);
            entity.HasQueryFilter(x => !x.IsDeleted);
        });
    }
}

