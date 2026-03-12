namespace BG.FeedService.Domain.Entities;

public sealed class FeedAggregate
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; init; } = "Feed aggregate";
}
