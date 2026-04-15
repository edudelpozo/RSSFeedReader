using Microsoft.AspNetCore.Mvc;
using RSSFeedReader.Api.DTOs;
using RSSFeedReader.Api.Services;

namespace RSSFeedReader.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubscriptionsController : ControllerBase
{
    private readonly ISubscriptionService _subscriptionService;

    public SubscriptionsController(ISubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] AddSubscriptionRequest request)
    {
        var dto = await _subscriptionService.AddAsync(request.Url);
        return CreatedAtAction(nameof(Post), dto);
    }
}
