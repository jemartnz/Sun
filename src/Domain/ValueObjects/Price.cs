using Domain.Commons;

namespace Domain.ValueObjects;

public sealed class Price
{
    public decimal Amount { get; }
    public string Currency { get; }

    private Price(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public static Result<Price> Create(decimal amount, string currency = "USD")
    {
        if (amount < 0)
            return Result<Price>.Failure(PriceErrors.Negative);

        if (string.IsNullOrWhiteSpace(currency))
            return Result<Price>.Failure(PriceErrors.InvalidCurrency);

        return Result<Price>.Success(new Price(amount, currency.ToUpperInvariant()));
    }
}

public static class PriceErrors
{
    public static readonly Error Negative = new("Price.Negative", "El precio no puede ser negativo.");
    public static readonly Error InvalidCurrency = new("Price.InvalidCurrency", "La moneda es requerida.");
}
