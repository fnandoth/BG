using BG.PostService.Domain.Repositories;
using BG.PostService.Domain.Entities;

namespace BG.PostService.Infrastructure.Persistence;

public sealed class InMemoryPostRepository : IPostRepository
{
    private static readonly IReadOnlyCollection<PostAggregate> Seed =
    [
        new() { Name = "Post root" }
    ];

    public IReadOnlyCollection<PostAggregate> GetAll() => Seed;
}
