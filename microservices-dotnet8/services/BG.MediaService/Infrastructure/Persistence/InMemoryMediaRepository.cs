using BG.MediaService.Domain.Repositories;
using BG.MediaService.Domain.Entities;

namespace BG.MediaService.Infrastructure.Persistence;

public sealed class InMemoryMediaRepository : IMediaRepository
{
    private static readonly IReadOnlyCollection<MediaAggregate> Seed =
    [
        new() { Name = "Media root" }
    ];

    public IReadOnlyCollection<MediaAggregate> GetAll() => Seed;
}
