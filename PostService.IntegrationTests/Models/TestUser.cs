namespace PostService.IntegrationTests.Models;

public sealed record TestUser(
    Guid Id,
    string UserName,
    string Password,
    string DisplayName,
    string Email,
    string AvatarUrl);
