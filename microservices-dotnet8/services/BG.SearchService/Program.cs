using BG.SearchService.Application.Services;
using BG.SearchService.Domain.Repositories;
using BG.SearchService.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<SearchAppService>();
builder.Services.AddSingleton<ISearchRepository, InMemorySearchRepository>();

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new { service = "search-service", status = "ok" }));
app.MapGet("/api/search", (SearchAppService service) => Results.Ok(service.GetBlueprint()));

app.Run();
