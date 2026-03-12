namespace BG.PostService.Domain.Entities;

public sealed class PostAggregate
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; init; } = "Post aggregate";
}
