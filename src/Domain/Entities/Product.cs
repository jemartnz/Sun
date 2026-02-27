using Domain.Commons;
using Domain.ValueObjects;

namespace Domain.Entities;

public sealed class Product : BaseEntity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Price Price { get; private set; }
    public int Stock { get; private set; }


    private Product()
    {
        Name = null!;
        Description = null!;
        Price = null!;
    }

    private Product(string name, string description, Price price, int stock)
    {
        Name = name;
        Description = description;
        Price = price;
        Stock = stock;
    }

    public void UpdateInfo(string name, string description, Price price)
    {
        Name = name;
        Description = description;
        Price = price;
    }

    public Result UpdateStock(int stock)
    {
        if (stock < 0)
            return Result.Failure(ProductErrors.NegativeStock);
        Stock = stock;
        return Result.Success();
    }

    public static Result<Product> Create(string name, string description, Price price, int stock)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result<Product>.Failure(ProductErrors.NameRequired);

        if (stock < 0)
            return Result<Product>.Failure(ProductErrors.NegativeStock);

        return Result<Product>.Success(new Product(name.Trim(), description?.Trim() ?? "", price, stock));
    }
}

public static class ProductErrors
{
    public static readonly Error NameRequired = new("Product.NameRequired", "El nombre del producto es obligatorio.");
    public static readonly Error NegativeStock = new("Product.NegativeStock", "El stock no puede ser negativo.");
    public static readonly Error NotFound = new("Product.NotFound", "Producto no encontrado.");
}
