using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using BG.IdentityService.Application.Contracts;

namespace BG.IdentityService.Application.Validation;

public static class IdentityRequestValidator
{
    public static void Validate(RegisterRequest request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Username) || request.Username.Trim().Length < 3)
        {
            errors.Add("Username must contain at least 3 characters.");
        }

        if (!IsValidEmail(request.Email))
        {
            errors.Add("Email format is invalid.");
        }

        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 8)
        {
            errors.Add("Password must contain at least 8 characters.");
        }

        if (!request.Password.Any(char.IsUpper) || !request.Password.Any(char.IsLower) || !request.Password.Any(char.IsDigit))
        {
            errors.Add("Password must include uppercase, lowercase and numeric characters.");
        }

        ThrowIfAny(errors);
    }

    public static void Validate(LoginRequest request)
    {
        var errors = new List<string>();

        if (!IsValidEmail(request.Email))
        {
            errors.Add("Email format is invalid.");
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            errors.Add("Password is required.");
        }

        ThrowIfAny(errors);
    }

    public static void Validate(RefreshTokenRequest request)
    {
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            errors.Add("Refresh token is required.");
        }

        ThrowIfAny(errors);
    }

    public static void Validate(ChangePasswordRequest request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.CurrentPassword))
        {
            errors.Add("Current password is required.");
        }

        if (string.IsNullOrWhiteSpace(request.NewPassword) || request.NewPassword.Length < 8)
        {
            errors.Add("New password must contain at least 8 characters.");
        }

        if (!string.IsNullOrWhiteSpace(request.NewPassword) &&
            (!request.NewPassword.Any(char.IsUpper) || !request.NewPassword.Any(char.IsLower) || !request.NewPassword.Any(char.IsDigit)))
        {
            errors.Add("New password must include uppercase, lowercase and numeric characters.");
        }

        ThrowIfAny(errors);
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            _ = new MailAddress(email);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static void ThrowIfAny(List<string> errors)
    {
        if (errors.Count > 0)
        {
            throw new ValidationException(errors);
        }
    }
}