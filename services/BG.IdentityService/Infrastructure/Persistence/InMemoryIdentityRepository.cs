using BG.IdentityService.Domain.Repositories;
using BG.IdentityService.Domain.Entities;

namespace BG.IdentityService.Infrastructure.Persistence;

public sealed class InMemoryIdentityRepository : IIdentityRepository
{
    private static readonly IReadOnlyCollection<IdentityAggregate> Seed =
    [
        new() { Name = "Identity root" }
    ];

    public IReadOnlyCollection<IdentityAggregate> GetAll() => Seed;
}
