using Application.DTOs;
using Domain.Commons;
using MediatR;

namespace Application.Features.Products;

public sealed record CreateProductCommand(
    string Name,
    string Description,
    decimal PriceAmount,
    string PriceCurrency,
    int Stock) : IRequest<Result<ProductResponse>>;
