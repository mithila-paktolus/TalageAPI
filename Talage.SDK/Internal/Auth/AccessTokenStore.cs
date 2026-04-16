using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Talage.SDK.Internal.Persistence;

namespace Talage.SDK.Internal.Auth;

public sealed class AccessTokenStore(IDbContextFactory<TalageIntegrationDbContext> dbContextFactory, IHttpContextAccessor httpContextAccessor) : IAccessTokenStore
{
    public async Task<(string Token, string TokenType, DateTimeOffset Expires)?> GetActiveAsync(CancellationToken cancellationToken)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var active = await db.AccessTokens
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderByDescending(x => x.CreatedDate)
            .FirstOrDefaultAsync(cancellationToken);

        if (active is null)
        {
            return null;
        }

        var expires = new DateTimeOffset(DateTime.SpecifyKind(active.ExpiresDate, DateTimeKind.Local));
        return (active.Token, active.TokenType, expires);
    }

    public async Task ReplaceActiveAsync(string token, string tokenType, DateTimeOffset expires, CancellationToken cancellationToken)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        var username = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";

        var now = DateTime.Now;
        var expiresDate = expires.DateTime;

        var activeTokens = await db.AccessTokens
            .Where(x => x.IsActive)
            .OrderByDescending(x => x.CreatedDate)
            .ToListAsync(cancellationToken);

        var current = activeTokens.FirstOrDefault();

        foreach (var existing in activeTokens.Skip(1))
        {
            existing.IsActive = false;
            existing.LastUpdated = now;
            existing.UpdatedBy = username;
        }

        if (current is null)
        {
            db.AccessTokens.Add(new AccessTokenEntity
            {
                Token = token,
                TokenType = tokenType,
                RefreshToken = null,
                CreatedDate = now,
                ExpiresDate = expiresDate,
                CreatedBy = username,
                LastUpdated = now,
                UpdatedBy = username,
                IsActive = true
            });
        }
        else
        {
            current.Token = token;
            current.TokenType = tokenType;
            current.RefreshToken = null;
            current.CreatedDate = now;
            current.ExpiresDate = expiresDate;
            current.CreatedBy = string.IsNullOrWhiteSpace(current.CreatedBy) ? username : current.CreatedBy;
            current.LastUpdated = now;
            current.UpdatedBy = username;
            current.IsActive = true;
        }

        await db.SaveChangesAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);
    }

    public async Task DeactivateAllAsync(CancellationToken cancellationToken)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        var username = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";

        var now = DateTime.Now;

        var activeTokens = await db.AccessTokens
            .Where(x => x.IsActive)
            .ToListAsync(cancellationToken);

        foreach (var token in activeTokens)
        {
            token.IsActive = false;
            token.LastUpdated = now;
            token.UpdatedBy = username;
        }

        await db.SaveChangesAsync(cancellationToken);
    }
}

