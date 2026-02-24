using Application.DTOs;
using Application.Interfaces;
using Domain.Commons;
using Domain.Entities;
using Domain.ValueObjects;
using MediatR;

namespace Application.Features.Products;

public sealed class CreateProductHandler : IRequestHandler<CreateProductCommand, Result<ProductResponse>>
{
    private readonly IProductRepository _productRepository;

    public CreateProductHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Result<ProductResponse>> Handle(CreateProductCommand request, CancellationToken ct)
    {
        var priceResult = Price.Create(request.PriceAmount, request.PriceCurrency);
        if (priceResult.IsFailure)
            return Result<ProductResponse>.Failure(priceResult.Error);

        var productResult = Product.Create(request.Name, request.Description, priceResult.Value, request.Stock);
        if (productResult.IsFailure)
            return Result<ProductResponse>.Failure(productResult.Error);

        var product = productResult.Value;
        await _productRepository.AddAsync(product, ct);
        await _productRepository.SaveChangesAsync(ct);

        return Result<ProductResponse>.Success(new ProductResponse(
            product.Id, product.Name, product.Description,
            product.Price.Amount, product.Price.Currency, product.Stock));
    }
}
