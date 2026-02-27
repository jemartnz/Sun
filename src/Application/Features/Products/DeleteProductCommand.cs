using Domain.Commons;
using MediatR;

namespace Application.Features.Products;

public sealed record DeleteProductCommand(Guid ProductId) : IRequest<Result>;
