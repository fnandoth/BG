using BG.InteractionService.Domain.Entities;

namespace BG.InteractionService.Domain.Repositories;

public interface IInteractionRepository
{
    IReadOnlyCollection<InteractionAggregate> GetAll();
}
