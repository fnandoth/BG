using BG.FeedService.Domain.Repositories;
using BG.FeedService.Domain.Entities;

namespace BG.FeedService.Infrastructure.Persistence;

public sealed class InMemoryFeedRepository : IFeedRepository
{
    private static readonly IReadOnlyCollection<FeedAggregate> Seed =
    [
        new() { Name = "Feed root" }
    ];

    public IReadOnlyCollection<FeedAggregate> GetAll() => Seed;
}
