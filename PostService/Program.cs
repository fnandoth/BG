using MassTransit;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using PostService.Domain.Interfaces;
using PostService.Infrastructure.Data;
using PostService.Infrastructure.Data.Repositories;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<PostContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default"));
});
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("Default")
        ?? throw new InvalidOperationException("Connection string 'Default' is not configured"));

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<ILikeRepository, LikeRepository>();

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host("rabbitmq://localhost", h => // Configuración de autenticación para RabbitMQ TODO: Añadir las credenciales correctas
        {
            h.Username("guest");
            h.Password("guest");
        });
    });
});



var app = builder.Build();

// actualizar base de datos en caso de que no exista (es decir cuando se inicia el microservicio por docker por primera vez)
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<PostContext>();
    dbContext.Database.Migrate();
}



if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}



// Global exception handling middleware, basicamente maneja las excepciones que se lancen en el codigo y devuelve un mensaje de error con el status code correspondiente,
// esto es util para evitar que el servidor se caiga por una excepcion no manejada y para dar una respuesta mas amigable al cliente
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;

        context.Response.StatusCode = exception switch
        {
            InvalidOperationException => StatusCodes.Status400BadRequest,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status500InternalServerError
        };

        await context.Response.WriteAsJsonAsync(new
        {
            error = exception?.Message
        });
    });
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");
app.Run();
