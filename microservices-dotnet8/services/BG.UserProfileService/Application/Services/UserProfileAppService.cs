using BG.UserProfileService.Domain.Repositories;
using BG.UserProfileService.Application.Abstractions;

namespace BG.UserProfileService.Application.Services;

public sealed class UserProfileAppService(IUserProfileRepository repository) : IUserProfileAppService
{
    public object GetBlueprint() => new
    {
        boundedContext = "UserProfile",
        purpose = "users/profile-settings",
        entities = repository.GetAll().Select(x => x.Name)
    };
}
