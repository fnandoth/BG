using System.Collections.Concurrent;
using BG.IdentityService.Domain.Entities;
using BG.IdentityService.Domain.Repositories;
using SharedKernel.BG.SharedKernel.ValueObjects;

namespace BG.IdentityService.Infrastructure.Persistence;

public sealed class InMemoryIdentityRepository : IIdentityRepository
{
    private readonly ConcurrentDictionary<Guid, IdentityAggregate> _storage = new();

    public Task<IdentityAggregate?> GetByIdAsync(UserId id, CancellationToken cancellationToken = default)
    {
        _storage.TryGetValue(id.Value, out var identity);
        return Task.FromResult(identity);
    }

    public Task<IdentityAggregate?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var identity = _storage.Values.FirstOrDefault(x => x.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(identity);
    }

    public Task<IdentityAggregate?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        var identity = _storage.Values.FirstOrDefault(x => x.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(identity);
    }

    public Task AddAsync(IdentityAggregate identity, CancellationToken cancellationToken = default)
    {
        _storage[identity.Id.Value] = identity;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(IdentityAggregate identity, CancellationToken cancellationToken = default)
    {
        _storage[identity.Id.Value] = identity;
        return Task.CompletedTask;
    }

    public Task<IdentityAggregate?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var identity = _storage.Values.FirstOrDefault(x => x.RefreshSessions.Any(session => session.Token == refreshToken));
        return Task.FromResult(identity);
    }
}