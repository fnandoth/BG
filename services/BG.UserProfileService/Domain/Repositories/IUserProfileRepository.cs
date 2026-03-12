using BG.UserProfileService.Domain.Entities;

namespace BG.UserProfileService.Domain.Repositories;

public interface IUserProfileRepository
{
    IReadOnlyCollection<UserProfileAggregate> GetAll();
}
