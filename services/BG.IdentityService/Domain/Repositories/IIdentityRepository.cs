using BG.IdentityService.Domain.Entities;
using SharedKernel.BG.SharedKernel.ValueObjects;

namespace BG.IdentityService.Domain.Repositories;

public interface IIdentityRepository
{
    Task<IdentityAggregate?> GetByIdAsync(UserId id, CancellationToken cancellationToken = default);
    Task<IdentityAggregate?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IdentityAggregate?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task AddAsync(IdentityAggregate identity, CancellationToken cancellationToken = default);
    Task UpdateAsync(IdentityAggregate identity, CancellationToken cancellationToken = default);
    Task<IdentityAggregate?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
}