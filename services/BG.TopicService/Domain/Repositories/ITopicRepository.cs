using BG.TopicService.Domain.Entities;

namespace BG.TopicService.Domain.Repositories;

public interface ITopicRepository
{
    IReadOnlyCollection<TopicAggregate> GetAll();
}
