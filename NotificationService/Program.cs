using System.Text;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NotificationService.Aplication.Commands;
using NotificationService.Aplication.Queries;
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
builder.Services.AddScoped<GetNotificationsHandler>();
builder.Services.AddScoped<MarkAsReadHandler>();

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Jwt:Key no está configurado.");
var jwtIssuer = builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException("Jwt:Issuer no está configurado.");
var jwtAudience = builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException("Jwt:Audience no está configurado.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

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
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");
app.Run();