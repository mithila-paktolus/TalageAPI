using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Talage.SDK.Internal.Interfaces;
using Talage.SDK.Internal.Persistence;

namespace Talage.SDK.Internal.Services;

public sealed class ApplicationCreationLogService(
    IDbContextFactory<TalageIntegrationDbContext> dbContextFactory,
    ILogger<ApplicationCreationLogService> logger) : IApplicationCreationLogService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task AddApplicationLogAsync(string applicationId, object response, string user, bool isUpdate)
    {

        var responseJson = SerializeResponse(response);
        var normalizedUser = string.IsNullOrWhiteSpace(user) ? null : user.Trim();

        try
        {
            await using var dbContext = await dbContextFactory.CreateDbContextAsync();

            var entity = new ApplicationCreationLogEntity
            {
                ApplicationId = applicationId,
                ResponseJson = responseJson,
                IsDeleted = false,
                CreatedOn = isUpdate ? null : DateTime.Now,
                CreatedBy = isUpdate ? null : normalizedUser,
                LastUpdated = isUpdate ? DateTime.Now : null,
                UpdatedBy = isUpdate ? normalizedUser : null
            };

            dbContext.ApplicationCreationLogs.Add(entity);
            await dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Failed to insert ApplicationCreationLog history row for ApplicationId {ApplicationId}. IsUpdate={IsUpdate}",
                applicationId,
                isUpdate);
            throw;
        }
    }

    public async Task UpdateApplicationLogAsync(long id, object response, string user)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "Id must be a positive integer.");
        }

        var responseJson = SerializeResponse(response);
        var updatedBy = string.IsNullOrWhiteSpace(user) ? null : user.Trim();

        try
        {
            await using var dbContext = await dbContextFactory.CreateDbContextAsync();

            var entity = await dbContext.ApplicationCreationLogs.FirstOrDefaultAsync(x => x.Id == id);
            if (entity is null)
            {
                throw new KeyNotFoundException($"ApplicationCreationLog {id} was not found.");
            }

            entity.ResponseJson = responseJson;
            entity.LastUpdated = DateTime.Now;
            entity.UpdatedBy = updatedBy;

            await dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to update ApplicationCreationLog {Id}", id);
            throw;
        }
    }

    private static string SerializeResponse(object response)
    {
        if (response is null)
        {
            return "null";
        }

        if (response is string text)
        {
            var trimmed = text.Trim();
            if (LooksLikeJson(trimmed))
            {
                return trimmed;
            }
        }

        return JsonSerializer.Serialize(response, JsonOptions);
    }

    private static bool LooksLikeJson(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        value = value.TrimStart();
        return value.StartsWith('{') || value.StartsWith('[');
    }
}

