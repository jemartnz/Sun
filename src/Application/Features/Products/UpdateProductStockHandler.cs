using Application.DTOs;
using Application.Interfaces;
using Domain.Commons;
using Domain.Entities;
using MediatR;

namespace Application.Features.Products;

public sealed class UpdateProductStockHandler : IRequestHandler<UpdateProductStockCommand, Result<ProductResponse>>
{
    private readonly IProductRepository _productRepository;

    public UpdateProductStockHandler(IProductRepository productRepository) =>
        _productRepository = productRepository;

    public async Task<Result<ProductResponse>> Handle(UpdateProductStockCommand request, CancellationToken ct)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId, ct);
        if (product is null)
            return Result<ProductResponse>.Failure(ProductErrors.NotFound);

        var stockResult = product.UpdateStock(request.Stock);
        if (!stockResult.IsSuccess)
            return Result<ProductResponse>.Failure(stockResult.Error);

        await _productRepository.SaveChangesAsync(ct);

        return Result<ProductResponse>.Success(new ProductResponse(
            product.Id, product.Name, product.Description,
            product.Price.Amount, product.Price.Currency, product.Stock));
    }
}
