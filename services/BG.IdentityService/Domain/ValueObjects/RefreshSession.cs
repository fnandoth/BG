namespace BG.IdentityService.Domain.ValueObjects;

public sealed class RefreshSession
{
    public string Token { get; private set; }
    public DateTime ExpiresAtUtc { get; private set; }
    public DateTime? RevokedAtUtc { get; private set; }
    public string CreatedByIp { get; private set; }

    private RefreshSession(string token, DateTime expiresAtUtc, string createdByIp)
    {
        Token = token;
        ExpiresAtUtc = expiresAtUtc;
        CreatedByIp = createdByIp;
    }

    public bool IsActive => RevokedAtUtc is null && ExpiresAtUtc > DateTime.UtcNow;

    public static RefreshSession Create(string token, DateTime expiresAtUtc, string createdByIp)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new ArgumentException("Refresh token is required.", nameof(token));
        }

        return new RefreshSession(token, expiresAtUtc, string.IsNullOrWhiteSpace(createdByIp) ? "unknown" : createdByIp);
    }

    public void Revoke(DateTime revokedAtUtc)
    {
        RevokedAtUtc = revokedAtUtc;
    }
}