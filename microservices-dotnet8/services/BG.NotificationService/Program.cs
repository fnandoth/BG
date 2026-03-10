using BG.NotificationService.Application.Services;
using BG.NotificationService.Domain.Repositories;
using BG.NotificationService.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<NotificationAppService>();
builder.Services.AddSingleton<INotificationRepository, InMemoryNotificationRepository>();

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new { service = "notification-service", status = "ok" }));
app.MapGet("/api/notification", (NotificationAppService service) => Results.Ok(service.GetBlueprint()));

app.Run();
