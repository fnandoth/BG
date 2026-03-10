using BG.MediaService.Application.Services;
using BG.MediaService.Domain.Repositories;
using BG.MediaService.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<MediaAppService>();
builder.Services.AddSingleton<IMediaRepository, InMemoryMediaRepository>();

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new { service = "media-service", status = "ok" }));
app.MapGet("/api/media", (MediaAppService service) => Results.Ok(service.GetBlueprint()));

app.Run();
