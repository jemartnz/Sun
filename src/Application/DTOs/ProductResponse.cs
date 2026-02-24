namespace Application.DTOs;

public sealed record ProductResponse(
    Guid Id,
    string Name,
    string Description,
    decimal PriceAmount,
    string PriceCurrency,
    int Stock
    );
