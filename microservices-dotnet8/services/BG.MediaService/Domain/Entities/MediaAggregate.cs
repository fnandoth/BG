namespace BG.MediaService.Domain.Entities;

public sealed class MediaAggregate
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; init; } = "Media aggregate";
}
