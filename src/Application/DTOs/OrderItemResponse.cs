namespace Application.DTOs;

public sealed record OrderItemResponse(
    Guid Id,
    Guid ProductId,
    int Quantity,
    decimal UnitPriceAmount,
    string UnitPriceCurrency);
