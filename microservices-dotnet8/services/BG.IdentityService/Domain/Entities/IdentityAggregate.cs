namespace BG.IdentityService.Domain.Entities;

public sealed class IdentityAggregate
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; init; } = "Identity aggregate";
}
