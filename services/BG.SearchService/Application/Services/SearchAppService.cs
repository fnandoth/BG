using BG.SearchService.Domain.Repositories;
using BG.SearchService.Application.Abstractions;

namespace BG.SearchService.Application.Services;

public sealed class SearchAppService(ISearchRepository repository) : ISearchAppService
{
    public object GetBlueprint() => new
    {
        boundedContext = "Search",
        purpose = "search/users-posts-topics",
        entities = repository.GetAll().Select(x => x.Name)
    };
}
