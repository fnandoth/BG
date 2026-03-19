namespace BG.IdentityService.Application.Contracts;

public sealed record AuthTokensResponse(string AccessToken, DateTime AccessTokenExpiresAtUtc, string RefreshToken, DateTime RefreshTokenExpiresAtUtc);