using MassTransit;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using NotificationService.Domain.Interfaces;
using NotificationService.Infrastructure.Consumers;
using NotificationService.Infrastructure.Data;
using NotificationService.Infrastructure.Data.Repositories;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<NotificationsContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default"));
});
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("Default")
        ?? throw new InvalidOperationException("Connection string 'Default' is not configured"));

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<PostLikedConsumer>(); // PostService
    x.AddConsumer<PostMentionedConsumer>(); // PostService
    x.AddConsumer<PostRepliedConsumer>(); // PostService
    x.AddConsumer<PostRepostedConsumer>(); // PostService
    x.AddConsumer<UserFollowedConsumer>();  // UserService
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("RabbitMQ") ?? throw new InvalidOperationException("Connection string 'RabbitMQ' is not configured"));
        cfg.ConfigureEndpoints(context);
    });
});



var app = builder.Build();

// actualizar base de datos en caso de que no exista (es decir cuando se inicia el microservicio por docker por primera vez)
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<NotificationsContext>();
    dbContext.Database.Migrate();
}



if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}




app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");
app.Run();
