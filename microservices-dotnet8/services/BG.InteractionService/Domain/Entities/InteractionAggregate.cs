namespace BG.InteractionService.Domain.Entities;

public sealed class InteractionAggregate
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; init; } = "Interaction aggregate";
}
