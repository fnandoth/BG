namespace BG.IdentityService.Application.Contracts;

public sealed record RegisterRequest(string Username, string Email, string Password);