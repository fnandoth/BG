using BG.IdentityService.Domain.Repositories;
using BG.IdentityService.Application.Abstractions;

namespace BG.IdentityService.Application.Services;

public sealed class IdentityAppService(IIdentityRepository repository) : IIdentityAppService
{
    public object GetBlueprint() => new
    {
        boundedContext = "Identity",
        purpose = "auth/login-register",
        entities = repository.GetAll().Select(x => x.Name)
    };
}
