using Talage.SDK.Internal.Interfaces;
using TalageIntegration.Shared.Exceptions;

namespace Talage.SDK.Internal.Services;

public sealed class NullApplicationCreationLogService : IApplicationCreationLogService
{
    private static TalageConfigurationException MissingConnectionString() =>
        new("Application creation logging is not configured. Set ConnectionStrings:AppLog to your TalengeIntegration database.");

    public Task AddApplicationLogAsync(string applicationId, object response, string user, bool isUpdate) =>
        throw MissingConnectionString();

    public Task UpdateApplicationLogAsync(long id, object response, string user) =>
        throw MissingConnectionString();
}

