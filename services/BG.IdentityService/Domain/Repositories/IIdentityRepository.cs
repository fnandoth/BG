using BG.IdentityService.Domain.Entities;

namespace BG.IdentityService.Domain.Repositories;

public interface IIdentityRepository
{
    IReadOnlyCollection<IdentityAggregate> GetAll();
}
