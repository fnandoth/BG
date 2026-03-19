using System.Text;
using BG.IdentityService.Api.Endpoints;
using BG.IdentityService.Application.Abstractions;
using BG.IdentityService.Application.Services;
using BG.IdentityService.Domain.Repositories;
using BG.IdentityService.Infrastructure.Events;
using BG.IdentityService.Infrastructure.Persistence;
using BG.IdentityService.Infrastructure.Security;
using BG.IdentityService.Infrastructure.Users;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SharedKernel.BG.SharedKernel.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<IdentitySecurityOptions>(builder.Configuration.GetSection(IdentitySecurityOptions.SectionName));
var securityOptions = builder.Configuration.GetSection(IdentitySecurityOptions.SectionName).Get<IdentitySecurityOptions>() ?? new IdentitySecurityOptions();

builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = securityOptions.Issuer,
            ValidAudience = securityOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityOptions.SigningKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddSingleton<IIdentityAppService, IdentityAppService>();
builder.Services.AddSingleton<IIdentityRepository, InMemoryIdentityRepository>();
builder.Services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();
builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();
builder.Services.AddSingleton<IEventBus, InMemoryEventBus>();
builder.Services.AddScoped<ICurrentUser, HttpContextCurrentUser>();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", () => Results.Ok(new { service = "identity-service", status = "ok" }));
app.MapIdentityEndpoints();

app.Run();