namespace RSSFeedReader.UI.Services;

public record SubscriptionDto(string Url);

public interface ISubscriptionApiClient
{
    Task<SubscriptionDto?> AddSubscriptionAsync(string url);
    Task<List<SubscriptionDto>> GetSubscriptionsAsync();
}
