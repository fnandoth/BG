using BG.InteractionService.Domain.Repositories;
using BG.InteractionService.Application.Abstractions;

namespace BG.InteractionService.Application.Services;

public sealed class InteractionAppService(IInteractionRepository repository) : IInteractionAppService
{
    public object GetBlueprint() => new
    {
        boundedContext = "Interaction",
        purpose = "likes-replies-saves-reposts",
        entities = repository.GetAll().Select(x => x.Name)
    };
}
