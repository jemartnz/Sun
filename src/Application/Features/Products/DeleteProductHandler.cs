using Application.Interfaces;
using Domain.Commons;
using Domain.Entities;
using MediatR;

namespace Application.Features.Products;

public sealed class DeleteProductHandler : IRequestHandler<DeleteProductCommand, Result>
{
    private readonly IProductRepository _productRepository;

    public DeleteProductHandler(IProductRepository productRepository) =>
        _productRepository = productRepository;

    public async Task<Result> Handle(DeleteProductCommand request, CancellationToken ct)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId, ct);
        if (product is null)
            return Result.Failure(ProductErrors.NotFound);

        await _productRepository.RemoveAsync(product, ct);
        await _productRepository.SaveChangesAsync(ct);

        return Result.Success();
    }
}
