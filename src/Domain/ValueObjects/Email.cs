using Domain.Commons;
using System.Text.RegularExpressions;

namespace Domain.ValueObjects;

public sealed partial class Email
{
    public string Value { get; }

    private Email(string value) => Value = value;

    public static Result<Email> Create(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return Result<Email>.Failure(EmailErrors.Empty);

        input = input.Trim().ToLowerInvariant();

        if (!EmailRegex().IsMatch(input))
            return Result<Email>.Failure(EmailErrors.InvalidFormat);

        return Result<Email>.Success(new Email(input));
    }

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex EmailRegex();

    // Value Object equality: dos emails con el mismo valor son iguales
    public override bool Equals(object? obj) =>
        obj is Email other && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value;
}

public static class EmailErrors
{
    public static readonly Error Empty = new("Email.Empty", "El email no puede estar vacío.");
    public static readonly Error InvalidFormat = new("Email.InvalidFormat", "El formato del email es inválido.");
}