using BG.PostService.Domain.Repositories;
using BG.PostService.Application.Abstractions;

namespace BG.PostService.Application.Services;

public sealed class PostAppService(IPostRepository repository) : IPostAppService
{
    public object GetBlueprint() => new
    {
        boundedContext = "Post",
        purpose = "posts/create-read",
        entities = repository.GetAll().Select(x => x.Name)
    };
}
