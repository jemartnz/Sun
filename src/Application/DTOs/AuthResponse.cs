namespace Application.DTOs;

public sealed record AuthResponse(string Token, Guid UserId, string Email);
