using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public sealed class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context) => _context = context;

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _context.Products.FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<(List<Product> Items, int TotalCount)> GetAllAsync(
        int page,
        int pageSize,
        string? name = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        CancellationToken ct = default)
    {
        var query = _context.Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(p => p.Name.Contains(name));

        if (minPrice.HasValue)
            query = query.Where(p => p.Price.Amount >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(p => p.Price.Amount <= maxPrice.Value);

        query = query.OrderBy(p => p.CreatedAtUtc);

        var totalCount = await query.CountAsync(ct);
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return (items, totalCount);
    }

    public async Task AddAsync(Product product, CancellationToken ct = default) =>
        await _context.Products.AddAsync(product, ct);

    public Task RemoveAsync(Product product, CancellationToken ct = default)
    {
        _context.Products.Remove(product);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken ct = default) =>
        await _context.SaveChangesAsync(ct);
}
