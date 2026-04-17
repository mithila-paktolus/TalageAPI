using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Talage.SDK.Internal.Interfaces;
using Talage.SDK.EntityFramework.Repository;
using Talage.SDK.EntityFramework.TalageIntegration.Model;

namespace Talage.SDK.Internal.Services;

public sealed class ApplicationCreationLogService(
    ITalageIntegrationRepository repository,
    ILogger<ApplicationCreationLogService> logger) : IApplicationCreationLogService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task AddApplicationLogAsync(string applicationId, object response, string user, bool isUpdate)
    {
        var responseJson = SerializeResponse(response);
        var normalizedUser = string.IsNullOrWhiteSpace(user) ? null : user.Trim();

        try
        {
            var entity = new ApplicationCreationLog
            {
                ApplicationId = applicationId,
                ResponseJson = responseJson,
                IsDeleted = false,
                CreatedOn = isUpdate ? null : DateTime.Now,
                CreatedBy = isUpdate ? null : normalizedUser,
                LastUpdated = isUpdate ? DateTime.Now : null,
                UpdatedBy = isUpdate ? normalizedUser : null
            };

            repository.Add(entity);
            await repository.SaveAsync();
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
            var entity = await repository.GetAll<ApplicationCreationLog>().FirstOrDefaultAsync(x => x.Id == id);
            if (entity is null)
            {
                throw new KeyNotFoundException($"ApplicationCreationLog {id} was not found.");
            }

            entity.ResponseJson = responseJson;
            entity.LastUpdated = DateTime.Now;
            entity.UpdatedBy = updatedBy;

            await repository.SaveAsync();
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

