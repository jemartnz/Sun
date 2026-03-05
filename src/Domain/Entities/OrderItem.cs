using Domain.Commons;
using Domain.ValueObjects;

namespace Domain.Entities;

public sealed class OrderItem : BaseEntity
{
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public Price UnitPrice { get; private set; }

    private OrderItem()
    {
        UnitPrice = null!;
    }

    private OrderItem(Guid productId, int quantity, Price unitPrice)
    {
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    internal static Result<OrderItem> Create(Guid productId, int quantity, Price unitPrice)
    {
        if (quantity <= 0)
            return Result<OrderItem>.Failure(OrderErrors.InvalidQuantity);

        return Result<OrderItem>.Success(new OrderItem(productId, quantity, unitPrice));
    }
}
