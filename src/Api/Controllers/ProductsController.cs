using Api.Extensions;
using Application.Features.Products;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class ProductsController : ControllerBase
{
    private readonly ISender _sender;

    public ProductsController(ISender sender) => _sender = sender;

    [HttpPost]
    public async Task<IActionResult> Create(CreateProductCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);
        return result.ToActionResult();
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetProductsQuery query, CancellationToken ct)
    {
        var result = await _sender.Send(query, ct);
        return result.ToActionResult();
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _sender.Send(new GetProductByIdQuery(id), ct);
        return result.ToActionResult();
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateProductRequest request, CancellationToken ct)
    {
        var command = new UpdateProductCommand(id, request.Name, request.Description,
                                               request.PriceAmount, request.PriceCurrency, request.Stock);
        var result = await _sender.Send(command, ct);
        return result.ToActionResult();
    }
}

public sealed record UpdateProductRequest(
    string Name,
    string Description,
    decimal PriceAmount,
    string PriceCurrency,
    int Stock);
