using System.ComponentModel.DataAnnotations;

namespace RSSFeedReader.Api.DTOs;

public record AddSubscriptionRequest([Required] string Url);
