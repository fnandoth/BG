using SharedKernel.BG.SharedKernel.Events;

namespace BG.IdentityService.Application.Events;

public sealed record UserRegisteredIntegrationEvent : IntegrationEvent
{
    public Guid UserId { get; init; }
    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
}