using BG.PostService.Application.Services;
using BG.PostService.Domain.Repositories;
using BG.PostService.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<PostAppService>();
builder.Services.AddSingleton<IPostRepository, InMemoryPostRepository>();

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new { service = "post-service", status = "ok" }));
app.MapGet("/api/post", (PostAppService service) => Results.Ok(service.GetBlueprint()));

app.Run();
