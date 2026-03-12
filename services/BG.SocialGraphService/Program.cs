using BG.SocialGraphService.Application.Services;
using BG.SocialGraphService.Domain.Repositories;
using BG.SocialGraphService.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<SocialGraphAppService>();
builder.Services.AddSingleton<ISocialGraphRepository, InMemorySocialGraphRepository>();

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new { service = "socialgraph-service", status = "ok" }));
app.MapGet("/api/socialgraph", (SocialGraphAppService service) => Results.Ok(service.GetBlueprint()));

app.Run();
