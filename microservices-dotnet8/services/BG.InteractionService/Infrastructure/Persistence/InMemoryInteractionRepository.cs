using BG.InteractionService.Domain.Repositories;
using BG.InteractionService.Domain.Entities;

namespace BG.InteractionService.Infrastructure.Persistence;

public sealed class InMemoryInteractionRepository : IInteractionRepository
{
    private static readonly IReadOnlyCollection<InteractionAggregate> Seed =
    [
        new() { Name = "Interaction root" }
    ];

    public IReadOnlyCollection<InteractionAggregate> GetAll() => Seed;
}
