using BG.TopicService.Domain.Repositories;
using BG.TopicService.Application.Abstractions;

namespace BG.TopicService.Application.Services;

public sealed class TopicAppService(ITopicRepository repository) : ITopicAppService
{
    public object GetBlueprint() => new
    {
        boundedContext = "Topic",
        purpose = "topics/crud",
        entities = repository.GetAll().Select(x => x.Name)
    };
}
