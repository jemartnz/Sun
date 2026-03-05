using Domain.Commons;
using Domain.Enums;

namespace Domain.Entities;

public sealed class Order : BaseEntity
{
    public Guid UserId { get; private set; }
    public OrderStatus Status { get; private set; }

    private readonly List<OrderItem> _items = [];
    public IReadOnlyList<OrderItem> Items => _items.AsReadOnly();

    private Order() { }

    private Order(Guid userId)
    {
        UserId = userId;
        Status = OrderStatus.Pending;
    }

    public static Result<Order> Create(Guid userId, IEnumerable<(Product Product, int Quantity)> items)
    {
        var itemList = items.ToList();

        if (itemList.Count == 0)
            return Result<Order>.Failure(OrderErrors.NoItems);

        var order = new Order(userId);

        foreach (var (product, quantity) in itemList)
        {
            var stockResult = product.ReduceStock(quantity);
            if (stockResult.IsFailure)
                return Result<Order>.Failure(stockResult.Error);

            var itemResult = OrderItem.Create(product.Id, quantity, product.Price);
            if (itemResult.IsFailure)
                return Result<Order>.Failure(itemResult.Error);

            order._items.Add(itemResult.Value);
        }

        return Result<Order>.Success(order);
    }
}

public static class OrderErrors
{
    public static readonly Error NoItems = new("Order.NoItems", "El pedido debe tener al menos un producto.");
    public static readonly Error InvalidQuantity = new("Order.InvalidQuantity", "La cantidad debe ser mayor a cero.");
    public static readonly Error NotFound = new("Order.NotFound", "Pedido no encontrado.");
}
