using Domain.Entities;

namespace Application.Interfaces;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<(List<Product> Items, int TotalCount)> GetAllAsync(int page, int pageSize, CancellationToken ct = default);
    Task AddAsync(Product product, CancellationToken ct = default);
    Task RemoveAsync(Product product, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
