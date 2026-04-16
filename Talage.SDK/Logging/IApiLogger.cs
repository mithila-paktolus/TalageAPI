namespace Talage.SDK.Logging;

public interface IApiLogger
{
    Task AddApplicationLogAsync(string applicationId, object response, string user, bool isUpdate);
    Task UpdateApplicationLogAsync(long id, object response, string user);
}
