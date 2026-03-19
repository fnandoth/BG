using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BG.IdentityService.Application.Abstractions;
using BG.IdentityService.Application.Contracts;
using BG.IdentityService.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BG.IdentityService.Infrastructure.Security;

public sealed class JwtTokenService(IOptions<IdentitySecurityOptions> options) : IJwtTokenService
{
    private readonly IdentitySecurityOptions _options = options.Value;

    public AuthTokensResponse GenerateTokens(IdentityAggregate identity)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SigningKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiresAt = DateTime.UtcNow.AddMinutes(_options.AccessTokenMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, identity.Id.Value.ToString()),
            new(JwtRegisteredClaimNames.Email, identity.Email),
            new(JwtRegisteredClaimNames.UniqueName, identity.Username),
            new(ClaimTypes.NameIdentifier, identity.Id.Value.ToString()),
            new(ClaimTypes.Name, identity.Username)
        };

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var refreshExpiresAt = DateTime.UtcNow.AddDays(_options.RefreshTokenDays);

        return new AuthTokensResponse(
            new JwtSecurityTokenHandler().WriteToken(token),
            expiresAt,
            refreshToken,
            refreshExpiresAt);
    }
}