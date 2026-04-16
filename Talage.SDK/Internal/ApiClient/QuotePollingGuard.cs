using System.Collections.Concurrent;
using Talage.SDK.Internal.Interfaces;
using TalageIntegration.Shared.Models;

namespace Talage.SDK.Internal.ApiClient;

public sealed class QuotePollingGuard : IQuotePollingGuard
{
    private static readonly TimeSpan MinimumInterval = TimeSpan.FromSeconds(5);
    private readonly ConcurrentDictionary<string, DateTimeOffset> _lastRequestTimes = new(StringComparer.OrdinalIgnoreCase);

    public Task EnsureAllowedAsync(string applicationId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var now = DateTimeOffset.Now;

        if (_lastRequestTimes.TryGetValue(applicationId, out var lastRequestTime) && now - lastRequestTime < MinimumInterval)
        {
            throw new TooManyRequestsException("Quote polling is limited to one request every 5 seconds per application, per Talage API guidance.");
        }

        _lastRequestTimes[applicationId] = now;
        return Task.CompletedTask;
    }
}

