using BG.FeedService.Application.Services;
using BG.FeedService.Domain.Repositories;
using BG.FeedService.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<FeedAppService>();
builder.Services.AddSingleton<IFeedRepository, InMemoryFeedRepository>();

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new { service = "feed-service", status = "ok" }));
app.MapGet("/api/feed", (FeedAppService service) => Results.Ok(service.GetBlueprint()));

app.Run();
