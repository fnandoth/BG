using BG.TopicService.Domain.Repositories;
using BG.TopicService.Domain.Entities;

namespace BG.TopicService.Infrastructure.Persistence;

public sealed class InMemoryTopicRepository : ITopicRepository
{
    private static readonly IReadOnlyCollection<TopicAggregate> Seed =
    [
        new() { Name = "Topic root" }
    ];

    public IReadOnlyCollection<TopicAggregate> GetAll() => Seed;
}
