using BG.FeedService.Domain.Entities;

namespace BG.FeedService.Domain.Repositories;

public interface IFeedRepository
{
    IReadOnlyCollection<FeedAggregate> GetAll();
}
