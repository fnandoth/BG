namespace BG.IdentityService.Application.Contracts;

public sealed record IdentityResponse(Guid Id, string Username, string Email, bool IsActive, DateTime CreatedAtUtc, DateTime UpdatedAtUtc);