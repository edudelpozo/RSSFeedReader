using RSSFeedReader.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

const string FrontendPolicy = "FrontendPolicy";
builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontendPolicy, policy =>
    {
        policy.WithOrigins("http://localhost:5213", "https://localhost:7025")
              .WithMethods("GET", "POST", "OPTIONS")
              .WithHeaders("Content-Type");
    });
});

// DI placeholder — ISubscriptionService registered after implementation
builder.Services.AddSingleton<ISubscriptionService, SubscriptionService>();

var app = builder.Build();

app.UseCors(FrontendPolicy);
app.UseHttpsRedirection();
app.MapControllers();

app.Run();

public partial class Program { }
