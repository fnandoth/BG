using BG.SearchService.Domain.Repositories;
using BG.SearchService.Domain.Entities;

namespace BG.SearchService.Infrastructure.Persistence;

public sealed class InMemorySearchRepository : ISearchRepository
{
    private static readonly IReadOnlyCollection<SearchAggregate> Seed =
    [
        new() { Name = "Search root" }
    ];

    public IReadOnlyCollection<SearchAggregate> GetAll() => Seed;
}
