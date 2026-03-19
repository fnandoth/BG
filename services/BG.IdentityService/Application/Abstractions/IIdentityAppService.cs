using BG.IdentityService.Application.Contracts;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Identity.Data;
using SharedKernel.BG.SharedKernel.ValueObjects;

namespace BG.IdentityService.Application.Abstractions;

public interface IIdentityAppService
{
    Task<AuthResponse> RegisterAsync(Application.Contracts.RegisterRequest request, string ipAddress, CancellationToken cancellationToken = default);
    Task<AuthResponse> LoginAsync(Application.Contracts.LoginRequest request, string ipAddress, CancellationToken cancellationToken = default);
    Task<AuthTokensResponse> RefreshTokenAsync(RefreshTokenRequest request, string ipAddress, CancellationToken cancellationToken = default);
    Task ChangePasswordAsync(UserId userId, ChangePasswordRequest request, CancellationToken cancellationToken = default);
    Task<IdentityResponse?> GetCurrentUserAsync(UserId userId, CancellationToken cancellationToken = default);
}