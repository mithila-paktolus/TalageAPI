namespace Talage.SDK.Internal.Interfaces;

public interface IApplicationCreationLogService
{
    Task AddApplicationLogAsync(string applicationId, object response, string user, bool isUpdate);

    // Kept for admin/manual corrections (updates a specific row by Id).
    Task UpdateApplicationLogAsync(long id, object response, string user);
}

