namespace Talage.SDK.Internal.Interfaces;

public interface IQuotePollingGuard
{
    Task EnsureAllowedAsync(string applicationId, CancellationToken cancellationToken);
}

