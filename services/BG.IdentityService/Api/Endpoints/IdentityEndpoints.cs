using System.ComponentModel.DataAnnotations;
using BG.IdentityService.Application.Abstractions;
using BG.IdentityService.Application.Contracts;
using BG.IdentityService.Application.Validation;
using BG.IdentityService.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using SharedKernel.BG.SharedKernel.Interfaces;
using SharedKernel.BG.SharedKernel.ValueObjects;

namespace BG.IdentityService.Api.Endpoints;

public static class IdentityEndpoints
{
    public static IEndpointRouteBuilder MapIdentityEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/identity").WithTags("Identity");

        group.MapPost("/register", async (Application.Contracts.RegisterRequest request, HttpContext context, IIdentityAppService service, CancellationToken ct) =>
            await ExecuteAsync(() => service.RegisterAsync(request, context.Connection.RemoteIpAddress?.ToString() ?? "unknown", ct)));

        group.MapPost("/login", async (Application.Contracts.LoginRequest request, HttpContext context, IIdentityAppService service, CancellationToken ct) =>
            await ExecuteAsync(() => service.LoginAsync(request, context.Connection.RemoteIpAddress?.ToString() ?? "unknown", ct)));

        group.MapPost("/refresh", async (RefreshTokenRequest request, HttpContext context, IIdentityAppService service, CancellationToken ct) =>
            await ExecuteAsync(() => service.RefreshTokenAsync(request, context.Connection.RemoteIpAddress?.ToString() ?? "unknown", ct)));

        group.MapGet("/me", [Authorize] async (IIdentityAppService service, ICurrentUser currentUser, CancellationToken ct) =>
        {
            if (!currentUser.IsAuthenticated || currentUser.UserId == Guid.Empty)
            {
                return Results.Unauthorized();
            }

            var user = await service.GetCurrentUserAsync(UserId.From(currentUser.UserId), ct);
            return user is null ? Results.NotFound() : Results.Ok(user);
        });

        group.MapPost("/change-password", [Authorize] async (ChangePasswordRequest request, IIdentityAppService service, ICurrentUser currentUser, CancellationToken ct) =>
        {
            if (!currentUser.IsAuthenticated || currentUser.UserId == Guid.Empty)
            {
                return Results.Unauthorized();
            }

            return await ExecuteAsync(async () =>
            {
                await service.ChangePasswordAsync(UserId.From(currentUser.UserId), request, ct);
                return Results.NoContent();
            });
        });

        return app;
    }

    private static async Task<IResult> ExecuteAsync<T>(Func<Task<T>> handler)
    {
        try
        {
            return Results.Ok(await handler());
        }
        catch (Application.Validation.ValidationException ex)
        {
            return Results.ValidationProblem(ex.Errors.ToDictionary(x => x, x => new[] { x }));
        }
        catch (DomainValidationException ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }

    private static async Task<IResult> ExecuteAsync(Func<Task<IResult>> handler)
    {
        try
        {
            return await handler();
        }
        catch (Application.Validation.ValidationException ex)
        {
            return Results.ValidationProblem(ex.Errors.ToDictionary(x => x, x => new[] { x }));
        }
        catch (DomainValidationException ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }
}