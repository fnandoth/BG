using BG.FeedService.Domain.Repositories;
using BG.FeedService.Application.Abstractions;

namespace BG.FeedService.Application.Services;

public sealed class FeedAppService(IFeedRepository repository) : IFeedAppService
{
    public object GetBlueprint() => new
    {
        boundedContext = "Feed",
        purpose = "timeline/home-feed",
        entities = repository.GetAll().Select(x => x.Name)
    };
}
