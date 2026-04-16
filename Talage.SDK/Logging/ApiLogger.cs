using Talage.SDK.Internal.Interfaces;

namespace Talage.SDK.Logging;

public sealed class ApiLogger(IApplicationCreationLogService applicationCreationLogService) : IApiLogger
{
    public Task AddApplicationLogAsync(string applicationId, object response, string user, bool isUpdate) =>
        applicationCreationLogService.AddApplicationLogAsync(applicationId, response, user, isUpdate);

    public Task UpdateApplicationLogAsync(long id, object response, string user) =>
        applicationCreationLogService.UpdateApplicationLogAsync(id, response, user);
}

