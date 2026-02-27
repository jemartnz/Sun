using Domain.Entities;

namespace Application.Interfaces;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<(List<Product> Items, int TotalCount)> GetAllAsync(
        int page,
        int pageSize,
        string? name = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        string? sortBy = null,
        string? sortOrder = null,
        CancellationToken ct = default);
    Task AddAsync(Product product, CancellationToken ct = default);
    Task RemoveAsync(Product product, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
