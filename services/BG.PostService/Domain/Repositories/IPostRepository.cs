using BG.PostService.Domain.Entities;

namespace BG.PostService.Domain.Repositories;

public interface IPostRepository
{
    IReadOnlyCollection<PostAggregate> GetAll();
}
