using BG.IdentityService.Application.Abstractions;
using BG.IdentityService.Application.Contracts;
using BG.IdentityService.Application.Events;
using BG.IdentityService.Application.Validation;
using BG.IdentityService.Domain.Entities;
using BG.IdentityService.Domain.Exceptions;
using BG.IdentityService.Domain.Repositories;
using BG.IdentityService.Domain.ValueObjects;
using SharedKernel.BG.SharedKernel.Interfaces;
using SharedKernel.BG.SharedKernel.ValueObjects;

namespace BG.IdentityService.Application.Services;

public sealed class IdentityAppService(
    IIdentityRepository repository,
    IPasswordHasher passwordHasher,
    IJwtTokenService jwtTokenService,
    IEventBus eventBus) : IIdentityAppService
{
    private const int MaxFailedAttempts = 5;
    private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(15);

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, string ipAddress, CancellationToken cancellationToken = default)
    {
        IdentityRequestValidator.Validate(request);

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var normalizedUsername = request.Username.Trim();

        if (await repository.GetByEmailAsync(normalizedEmail, cancellationToken) is not null)
        {
            throw new DomainValidationException("Email is already registered.");
        }

        if (await repository.GetByUsernameAsync(normalizedUsername, cancellationToken) is not null)
        {
            throw new DomainValidationException("Username is already taken.");
        }

        var identity = IdentityAggregate.Register(normalizedUsername, normalizedEmail, passwordHasher.Hash(request.Password));
        var tokens = jwtTokenService.GenerateTokens(identity);
        identity.StoreRefreshSession(RefreshSession.Create(tokens.RefreshToken, tokens.RefreshTokenExpiresAtUtc, ipAddress));

        await repository.AddAsync(identity, cancellationToken);
        await eventBus.PublishAsync(new UserRegisteredIntegrationEvent
        {
            UserId = identity.Id.Value,
            Username = identity.Username,
            Email = identity.Email
        }, cancellationToken);

        return new AuthResponse(Map(identity), tokens);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, string ipAddress, CancellationToken cancellationToken = default)
    {
        IdentityRequestValidator.Validate(request);

        var identity = await repository.GetByEmailAsync(request.Email.Trim().ToLowerInvariant(), cancellationToken)
            ?? throw new DomainValidationException("Invalid credentials.");

        if (identity.IsLocked)
        {
            throw new DomainValidationException("User is temporarily locked due to failed login attempts.");
        }

        if (!passwordHasher.Verify(request.Password, identity.PasswordHash))
        {
            identity.RecordFailedLogin(MaxFailedAttempts, LockoutDuration);
            await repository.UpdateAsync(identity, cancellationToken);
            throw new DomainValidationException("Invalid credentials.");
        }

        identity.RecordSuccessfulLogin();
        var tokens = jwtTokenService.GenerateTokens(identity);
        identity.StoreRefreshSession(RefreshSession.Create(tokens.RefreshToken, tokens.RefreshTokenExpiresAtUtc, ipAddress));

        await repository.UpdateAsync(identity, cancellationToken);
        await eventBus.PublishAsync(new UserLoggedInIntegrationEvent
        {
            UserId = identity.Id.Value,
            Email = identity.Email
        }, cancellationToken);

        return new AuthResponse(Map(identity), tokens);
    }

    public async Task<AuthTokensResponse> RefreshTokenAsync(RefreshTokenRequest request, string ipAddress, CancellationToken cancellationToken = default)
    {
        IdentityRequestValidator.Validate(request);

        var identity = await repository.GetByRefreshTokenAsync(request.RefreshToken, cancellationToken)
            ?? throw new DomainValidationException("Refresh token is invalid.");

        var refreshSession = identity.GetActiveRefreshSession(request.RefreshToken)
            ?? throw new DomainValidationException("Refresh token is expired or revoked.");

        refreshSession.Revoke(DateTime.UtcNow);

        var tokens = jwtTokenService.GenerateTokens(identity);
        identity.StoreRefreshSession(RefreshSession.Create(tokens.RefreshToken, tokens.RefreshTokenExpiresAtUtc, ipAddress));
        await repository.UpdateAsync(identity, cancellationToken);

        return tokens;
    }

    public async Task ChangePasswordAsync(UserId userId, ChangePasswordRequest request, CancellationToken cancellationToken = default)
    {
        IdentityRequestValidator.Validate(request);

        var identity = await repository.GetByIdAsync(userId, cancellationToken)
            ?? throw new DomainValidationException("User was not found.");

        if (!passwordHasher.Verify(request.CurrentPassword, identity.PasswordHash))
        {
            throw new DomainValidationException("Current password is invalid.");
        }

        identity.ChangePassword(passwordHasher.Hash(request.NewPassword));
        await repository.UpdateAsync(identity, cancellationToken);
        await eventBus.PublishAsync(new PasswordChangedIntegrationEvent
        {
            UserId = identity.Id.Value,
            Email = identity.Email
        }, cancellationToken);
    }

    public async Task<IdentityResponse?> GetCurrentUserAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var identity = await repository.GetByIdAsync(userId, cancellationToken);
        return identity is null ? null : Map(identity);
    }

    private static IdentityResponse Map(IdentityAggregate identity)
        => new(
            identity.Id.Value,
            identity.Username,
            identity.Email,
            identity.IsActive,
            identity.CreatedAtUtc,
            identity.UpdatedAtUtc);
}