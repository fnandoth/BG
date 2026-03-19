using SharedKernel.BG.SharedKernel.Events;

namespace BG.IdentityService.Application.Events;

public sealed record PasswordChangedIntegrationEvent : IntegrationEvent
{
    public Guid UserId { get; init; }
    public string Email { get; init; } = string.Empty;
}