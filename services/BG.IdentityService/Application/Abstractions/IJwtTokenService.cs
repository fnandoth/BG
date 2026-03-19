using BG.IdentityService.Application.Contracts;
using BG.IdentityService.Domain.Entities;
using Microsoft.AspNetCore.Authentication.OAuth;

namespace BG.IdentityService.Application.Abstractions;

public interface IJwtTokenService
{
    AuthTokensResponse GenerateTokens(IdentityAggregate identity);
}