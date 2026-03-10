using BG.SocialGraphService.Domain.Entities;

namespace BG.SocialGraphService.Domain.Repositories;

public interface ISocialGraphRepository
{
    IReadOnlyCollection<SocialGraphAggregate> GetAll();
}
