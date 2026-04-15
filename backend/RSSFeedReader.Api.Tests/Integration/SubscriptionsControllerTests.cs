using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using RSSFeedReader.Api.DTOs;

namespace RSSFeedReader.Api.Tests.Integration;

public class SubscriptionsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public SubscriptionsControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    // ── POST /api/subscriptions ──────────────────────────────────────────────────

    [Fact]
    public async Task Post_ValidUrl_Returns201WithSubscriptionDto()
    {
        var request = new AddSubscriptionRequest("https://devblogs.microsoft.com/dotnet/feed/");

        var response = await _client.PostAsJsonAsync("/api/subscriptions", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var dto = await response.Content.ReadFromJsonAsync<SubscriptionDto>();
        Assert.NotNull(dto);
        Assert.Equal(request.Url, dto.Url);
    }

    [Fact]
    public async Task Post_EmptyUrl_Returns400()
    {
        var request = new { url = string.Empty };

        var response = await _client.PostAsJsonAsync("/api/subscriptions", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Post_MissingUrlField_Returns400()
    {
        var request = new { };

        var response = await _client.PostAsJsonAsync("/api/subscriptions", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
