using Application.Common;
using Application.DTOs;
using Application.Interfaces;
using Domain.Commons;
using MediatR;

namespace Application.Features.Products;

public sealed class GetProductsHandler : IRequestHandler<GetProductsQuery, Result<PagedResponse<ProductResponse>>>
{
    private readonly IProductRepository _productRepository;

    public GetProductsHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Result<PagedResponse<ProductResponse>>> Handle(GetProductsQuery request, CancellationToken ct)
    {
        var (products, totalCount) = await _productRepository.GetAllAsync(
            request.Page, request.PageSize,
            request.Name, request.MinPrice, request.MaxPrice, ct);

        var items = products
            .Select(p => new ProductResponse(p.Id, p.Name, p.Description, p.Price.Amount, p.Price.Currency, p.Stock))
            .ToList();

        return Result<PagedResponse<ProductResponse>>.Success(
            new PagedResponse<ProductResponse>(items, totalCount, request.Page, request.PageSize));
    }
}
