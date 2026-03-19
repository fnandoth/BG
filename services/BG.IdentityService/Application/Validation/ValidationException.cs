namespace BG.IdentityService.Application.Validation;

public sealed class ValidationException(IReadOnlyCollection<string> errors) : Exception("Validation error")
{
    public IReadOnlyCollection<string> Errors { get; } = errors;
}