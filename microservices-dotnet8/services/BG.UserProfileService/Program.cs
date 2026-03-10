using BG.UserProfileService.Application.Services;
using BG.UserProfileService.Domain.Repositories;
using BG.UserProfileService.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<UserProfileAppService>();
builder.Services.AddSingleton<IUserProfileRepository, InMemoryUserProfileRepository>();

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new { service = "userprofile-service", status = "ok" }));
app.MapGet("/api/userprofile", (UserProfileAppService service) => Results.Ok(service.GetBlueprint()));

app.Run();
