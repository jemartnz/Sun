using Application.Common;
using Application.DTOs;
using Domain.Commons;
using MediatR;

namespace Application.Features.Products;

public sealed record GetProductsQuery(int Page = 1, int PageSize = 10) : IRequest<Result<PagedResponse<ProductResponse>>>;
