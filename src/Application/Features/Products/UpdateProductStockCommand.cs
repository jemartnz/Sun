using Application.DTOs;
using Domain.Commons;
using MediatR;

namespace Application.Features.Products;

public sealed record UpdateProductStockCommand(
    Guid ProductId,
    int Stock) : IRequest<Result<ProductResponse>>;
