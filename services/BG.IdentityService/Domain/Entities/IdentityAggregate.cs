using BG.IdentityService.Domain.Exceptions;
using BG.IdentityService.Domain.ValueObjects;
using SharedKernel.BG.SharedKernel.ValueObjects;

namespace BG.IdentityService.Domain.Entities;

public sealed class IdentityAggregate
{
    private readonly List<RefreshSession> _refreshSessions = [];

    private IdentityAggregate(
        UserId id,
        string username,
        string email,
        string passwordHash,
        DateTime createdAtUtc)
    {
        Id = id;
        Username = username;
        Email = email;
        PasswordHash = passwordHash;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = createdAtUtc;
    }

    private IdentityAggregate()
    {
        Id = UserId.New();
        Username = string.Empty;
        Email = string.Empty;
        PasswordHash = string.Empty;
    }

    public UserId Id { get; private set; }
    public string Username { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public bool IsActive { get; private set; } = true;
    public int FailedLoginAttempts { get; private set; }
    public DateTime? LockedUntilUtc { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }
    public IReadOnlyCollection<RefreshSession> RefreshSessions => _refreshSessions.AsReadOnly();

    public bool IsLocked => LockedUntilUtc is not null && LockedUntilUtc > DateTime.UtcNow;

    public static IdentityAggregate Register(string username, string email, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new DomainValidationException("Username is required.");
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new DomainValidationException("Email is required.");
        }

        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new DomainValidationException("Password hash is required.");
        }

        var now = DateTime.UtcNow;
        return new IdentityAggregate(UserId.New(), username.Trim(), email.Trim().ToLowerInvariant(), passwordHash, now);
    }

    public void ChangePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
        {
            throw new DomainValidationException("Password hash is required.");
        }

        PasswordHash = newPasswordHash;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void RecordSuccessfulLogin()
    {
        FailedLoginAttempts = 0;
        LockedUntilUtc = null;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void RecordFailedLogin(int maxFailedAttempts, TimeSpan lockoutDuration)
    {
        FailedLoginAttempts++;
        if (FailedLoginAttempts >= maxFailedAttempts)
        {
            LockedUntilUtc = DateTime.UtcNow.Add(lockoutDuration);
            FailedLoginAttempts = 0;
        }

        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void StoreRefreshSession(RefreshSession refreshSession)
    {
        _refreshSessions.Add(refreshSession);
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public RefreshSession? GetActiveRefreshSession(string refreshToken)
        => _refreshSessions.LastOrDefault(x => x.Token == refreshToken && x.IsActive);

    public void RevokeRefreshSession(string refreshToken)
    {
        var session = _refreshSessions.LastOrDefault(x => x.Token == refreshToken && x.RevokedAtUtc is null);
        session?.Revoke(DateTime.UtcNow);
        UpdatedAtUtc = DateTime.UtcNow;
    }
}