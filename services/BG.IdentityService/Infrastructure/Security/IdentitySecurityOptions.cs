namespace BG.IdentityService.Infrastructure.Security;

public sealed class IdentitySecurityOptions
{
    public const string SectionName = "IdentitySecurity";

    public string Issuer { get; init; } = "BG.IdentityService";
    public string Audience { get; init; } = "BG.Clients";
    public string SigningKey { get; init; } = "key";
    public int AccessTokenMinutes { get; init; } = 30;
    public int RefreshTokenDays { get; init; } = 14;
}