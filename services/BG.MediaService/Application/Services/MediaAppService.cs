using BG.MediaService.Domain.Repositories;
using BG.MediaService.Application.Abstractions;

namespace BG.MediaService.Application.Services;

public sealed class MediaAppService(IMediaRepository repository) : IMediaAppService
{
    public object GetBlueprint() => new
    {
        boundedContext = "Media",
        purpose = "media/upload-assets",
        entities = repository.GetAll().Select(x => x.Name)
    };
}
