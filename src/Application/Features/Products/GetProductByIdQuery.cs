using Application.DTOs;
using Domain.Commons;
using MediatR;

namespace Application.Features.Products;

public sealed record GetProductByIdQuery(Guid ProductId) : IRequest<Result<ProductResponse>>;
