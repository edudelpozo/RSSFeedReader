using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using RSSFeedReader.UI;
using RSSFeedReader.UI.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5151/api/";
builder.Services.AddScoped(sp =>
{
    var client = new HttpClient { BaseAddress = new Uri(apiBaseUrl) };
    return client;
});
builder.Services.AddScoped<ISubscriptionApiClient, SubscriptionApiClient>();

await builder.Build().RunAsync();
