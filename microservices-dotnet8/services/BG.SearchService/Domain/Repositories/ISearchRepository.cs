using BG.SearchService.Domain.Entities;

namespace BG.SearchService.Domain.Repositories;

public interface ISearchRepository
{
    IReadOnlyCollection<SearchAggregate> GetAll();
}
