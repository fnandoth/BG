using BG.SocialGraphService.Domain.Repositories;
using BG.SocialGraphService.Domain.Entities;

namespace BG.SocialGraphService.Infrastructure.Persistence;

public sealed class InMemorySocialGraphRepository : ISocialGraphRepository
{
    private static readonly IReadOnlyCollection<SocialGraphAggregate> Seed =
    [
        new() { Name = "SocialGraph root" }
    ];

    public IReadOnlyCollection<SocialGraphAggregate> GetAll() => Seed;
}
