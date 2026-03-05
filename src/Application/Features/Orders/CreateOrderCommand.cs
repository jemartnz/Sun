using Application.DTOs;
using Domain.Commons;
using MediatR;

namespace Application.Features.Orders;

public sealed record OrderItemRequest(Guid ProductId, int Quantity);

public sealed record CreateOrderCommand(
    Guid UserId,
    IReadOnlyList<OrderItemRequest> Items) : IRequest<Result<OrderResponse>>;
