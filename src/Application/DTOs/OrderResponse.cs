namespace Application.DTOs;

public sealed record OrderResponse(
    Guid Id,
    Guid UserId,
    string Status,
    IReadOnlyList<OrderItemResponse> Items,
    DateTime CreatedAtUtc);
