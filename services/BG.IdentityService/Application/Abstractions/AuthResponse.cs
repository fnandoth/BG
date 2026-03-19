using Microsoft.AspNetCore.Authentication.OAuth;

namespace BG.IdentityService.Application.Contracts;

public sealed record AuthResponse(IdentityResponse User, AuthTokensResponse Tokens);