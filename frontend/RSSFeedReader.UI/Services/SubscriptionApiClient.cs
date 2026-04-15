using System.Net.Http.Json;

namespace RSSFeedReader.UI.Services;

public class SubscriptionApiClient : ISubscriptionApiClient
{
    private readonly HttpClient _httpClient;

    public SubscriptionApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<SubscriptionDto?> AddSubscriptionAsync(string url)
    {
        var response = await _httpClient.PostAsJsonAsync("subscriptions", new { url });
        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<SubscriptionDto>();
    }

    public async Task<List<SubscriptionDto>> GetSubscriptionsAsync()
    {
        var result = await _httpClient.GetFromJsonAsync<List<SubscriptionDto>>("subscriptions");
        return result ?? [];
    }
}
