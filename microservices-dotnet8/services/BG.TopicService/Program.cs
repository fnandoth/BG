using BG.TopicService.Application.Services;
using BG.TopicService.Domain.Repositories;
using BG.TopicService.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<TopicAppService>();
builder.Services.AddSingleton<ITopicRepository, InMemoryTopicRepository>();

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new { service = "topic-service", status = "ok" }));
app.MapGet("/api/topic", (TopicAppService service) => Results.Ok(service.GetBlueprint()));

app.Run();
