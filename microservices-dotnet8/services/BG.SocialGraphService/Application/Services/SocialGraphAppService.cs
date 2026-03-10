using BG.SocialGraphService.Domain.Repositories;
using BG.SocialGraphService.Application.Abstractions;

namespace BG.SocialGraphService.Application.Services;

public sealed class SocialGraphAppService(ISocialGraphRepository repository) : ISocialGraphAppService
{
    public object GetBlueprint() => new
    {
        boundedContext = "SocialGraph",
        purpose = "followers-following",
        entities = repository.GetAll().Select(x => x.Name)
    };
}
