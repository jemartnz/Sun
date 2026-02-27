using Application.DTOs;
using Application.Interfaces;
using Domain.Commons;
using Domain.Entities;
using Domain.ValueObjects;
using MediatR;

namespace Application.Features.Products;

public sealed class UpdateProductHandler : IRequestHandler<UpdateProductCommand, Result<ProductResponse>>
{
    private readonly IProductRepository _productRepository;

    public UpdateProductHandler(IProductRepository productRepository) =>
        _productRepository = productRepository;

    public async Task<Result<ProductResponse>> Handle(UpdateProductCommand request, CancellationToken ct)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId, ct);
        if (product is null)
            return Result<ProductResponse>.Failure(ProductErrors.NotFound);

        if (string.IsNullOrWhiteSpace(request.Name))
            return Result<ProductResponse>.Failure(ProductErrors.NameRequired);

        var priceResult = Price.Create(request.PriceAmount, request.PriceCurrency);
        if (!priceResult.IsSuccess)
            return Result<ProductResponse>.Failure(priceResult.Error);

        var stockResult = product.UpdateStock(request.Stock);
        if (!stockResult.IsSuccess)
            return Result<ProductResponse>.Failure(stockResult.Error);

        product.UpdateInfo(request.Name.Trim(), request.Description?.Trim() ?? "", priceResult.Value);

        await _productRepository.SaveChangesAsync(ct);

        return Result<ProductResponse>.Success(new ProductResponse(
            product.Id, product.Name, product.Description,
            product.Price.Amount, product.Price.Currency, product.Stock));
    }
}
