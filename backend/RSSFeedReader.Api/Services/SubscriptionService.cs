using RSSFeedReader.Api.DTOs;
using RSSFeedReader.Api.Models;

namespace RSSFeedReader.Api.Services;

public class SubscriptionService : ISubscriptionService
{
    private readonly List<Subscription> _subscriptions = [];
    private readonly object _lock = new();

    public Task<SubscriptionDto> AddAsync(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("URL must not be empty.", nameof(url));

        var subscription = new Subscription(url);
        lock (_lock)
        {
            _subscriptions.Add(subscription);
        }

        return Task.FromResult(new SubscriptionDto(subscription.Url));
    }

    public Task<IReadOnlyList<SubscriptionDto>> GetAllAsync()
    {
        IReadOnlyList<SubscriptionDto> snapshot;
        lock (_lock)
        {
            snapshot = _subscriptions.Select(s => new SubscriptionDto(s.Url)).ToList();
        }

        return Task.FromResult(snapshot);
    }
}
