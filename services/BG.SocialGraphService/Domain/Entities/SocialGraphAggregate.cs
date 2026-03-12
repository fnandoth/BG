namespace BG.SocialGraphService.Domain.Entities;

public sealed class SocialGraphAggregate
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; init; } = "SocialGraph aggregate";
}
