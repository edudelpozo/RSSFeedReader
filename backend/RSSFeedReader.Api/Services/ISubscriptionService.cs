using RSSFeedReader.Api.DTOs;

namespace RSSFeedReader.Api.Services;

public interface ISubscriptionService
{
    Task<SubscriptionDto> AddAsync(string url);
    Task<IReadOnlyList<SubscriptionDto>> GetAllAsync();
}
