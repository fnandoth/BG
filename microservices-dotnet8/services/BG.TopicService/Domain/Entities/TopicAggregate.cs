namespace BG.TopicService.Domain.Entities;

public sealed class TopicAggregate
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; init; } = "Topic aggregate";
}
