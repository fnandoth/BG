using BG.InteractionService.Application.Services;
using BG.InteractionService.Domain.Repositories;
using BG.InteractionService.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<InteractionAppService>();
builder.Services.AddSingleton<IInteractionRepository, InMemoryInteractionRepository>();

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new { service = "interaction-service", status = "ok" }));
app.MapGet("/api/interaction", (InteractionAppService service) => Results.Ok(service.GetBlueprint()));

app.Run();
