using Application.DTOs;
using Application.Interfaces;
using Domain.Commons;
using Domain.Entities;
using MediatR;

namespace Application.Features.Orders;

public sealed class CreateOrderHandler : IRequestHandler<CreateOrderCommand, Result<OrderResponse>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;

    public CreateOrderHandler(IOrderRepository orderRepository, IProductRepository productRepository)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
    }

    public async Task<Result<OrderResponse>> Handle(CreateOrderCommand request, CancellationToken ct)
    {
        var productPairs = new List<(Product Product, int Quantity)>();

        foreach (var item in request.Items)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId, ct);
            if (product is null)
                return Result<OrderResponse>.Failure(ProductErrors.NotFound);

            productPairs.Add((product, item.Quantity));
        }

        var orderResult = Order.Create(request.UserId, productPairs);
        if (orderResult.IsFailure)
            return Result<OrderResponse>.Failure(orderResult.Error);

        var order = orderResult.Value;

        await _orderRepository.AddAsync(order, ct);
        await _orderRepository.SaveChangesAsync(ct);

        var response = new OrderResponse(
            order.Id,
            order.UserId,
            order.Status.ToString(),
            order.Items.Select(i => new OrderItemResponse(
                i.Id,
                i.ProductId,
                i.Quantity,
                i.UnitPrice.Amount,
                i.UnitPrice.Currency)).ToList(),
            order.CreatedAtUtc);

        return Result<OrderResponse>.Success(response);
    }
}
