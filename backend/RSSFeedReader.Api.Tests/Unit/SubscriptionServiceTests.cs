using RSSFeedReader.Api.Services;

namespace RSSFeedReader.Api.Tests.Unit;

public class SubscriptionServiceTests
{
    // ── AddAsync ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task AddAsync_ValidUrl_ReturnsSubscriptionDtoWithSameUrl()
    {
        var service = new SubscriptionService();
        const string url = "https://devblogs.microsoft.com/dotnet/feed/";

        var result = await service.AddAsync(url);

        Assert.Equal(url, result.Url);
    }

    [Fact]
    public async Task AddAsync_ValidUrl_IsStoredAndRetrievable()
    {
        var service = new SubscriptionService();
        const string url = "https://devblogs.microsoft.com/dotnet/feed/";

        await service.AddAsync(url);
        var all = await service.GetAllAsync();

        Assert.Single(all);
        Assert.Equal(url, all[0].Url);
    }

    [Fact]
    public async Task AddAsync_EmptyString_ThrowsArgumentException()
    {
        var service = new SubscriptionService();

        await Assert.ThrowsAsync<ArgumentException>(() => service.AddAsync(string.Empty));
    }

    [Fact]
    public async Task AddAsync_WhitespaceString_ThrowsArgumentException()
    {
        var service = new SubscriptionService();

        await Assert.ThrowsAsync<ArgumentException>(() => service.AddAsync("   "));
    }
}
