namespace BG.UserProfileService.Domain.Entities;

public sealed class UserProfileAggregate
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; init; } = "UserProfile aggregate";
}
