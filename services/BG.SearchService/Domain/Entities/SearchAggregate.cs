namespace BG.SearchService.Domain.Entities;

public sealed class SearchAggregate
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; init; } = "Search aggregate";
}
