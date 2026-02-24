using Domain.Commons;

namespace Domain.ValueObjects;

public sealed class Password
{
    public const int MinLength = 8;
    public string Value { get; }

    private Password(string value) => Value = value;

    public static Result<Password> Create(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return Result<Password>.Failure(PasswordErrors.Empty);

        if (input.Length < MinLength)
            return Result<Password>.Failure(PasswordErrors.TooShort);

        if (!input.Any(char.IsUpper))
            return Result<Password>.Failure(PasswordErrors.MissingUppercase);

        if (!input.Any(char.IsDigit))
            return Result<Password>.Failure(PasswordErrors.MissingDigit);

        return Result<Password>.Success(new Password(input));
    }
}

public static class PasswordErrors
{
    public static readonly Error Empty = new("Password.Empty", "La contraseña no puede estar vacía.");
    public static readonly Error TooShort = new("Password.TooShort", $"La contraseña debe tener al menos {Password.MinLength} caracteres.");
    public static readonly Error MissingUppercase = new("Password.MissingUppercase", "La contraseña debe tener al menos una mayúscula.");
    public static readonly Error MissingDigit = new("Password.MissingDigit", "La contraseña debe tener al menos un dígito.");
}
