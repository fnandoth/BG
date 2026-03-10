using BG.IdentityService.Application.Services;
using BG.IdentityService.Domain.Repositories;
using BG.IdentityService.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IdentityAppService>();
builder.Services.AddSingleton<IIdentityRepository, InMemoryIdentityRepository>();

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new { service = "identity-service", status = "ok" }));
app.MapGet("/api/identity", (IdentityAppService service) => Results.Ok(service.GetBlueprint()));

app.Run();
