namespace Application.DTOs;

public sealed record UserResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    DateTime CreatedAtUtc
    );
