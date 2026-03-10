using BG.UserProfileService.Domain.Repositories;
using BG.UserProfileService.Domain.Entities;

namespace BG.UserProfileService.Infrastructure.Persistence;

public sealed class InMemoryUserProfileRepository : IUserProfileRepository
{
    private static readonly IReadOnlyCollection<UserProfileAggregate> Seed =
    [
        new() { Name = "UserProfile root" }
    ];

    public IReadOnlyCollection<UserProfileAggregate> GetAll() => Seed;
}
