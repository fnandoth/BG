using BG.MediaService.Domain.Entities;

namespace BG.MediaService.Domain.Repositories;

public interface IMediaRepository
{
    IReadOnlyCollection<MediaAggregate> GetAll();
}
