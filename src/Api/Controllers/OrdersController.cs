using Api.Extensions;
using Application.Features.Orders;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class OrdersController : ControllerBase
{
    private readonly ISender _sender;

    public OrdersController(ISender sender) => _sender = sender;

    [HttpPost]
    public async Task<IActionResult> Create(CreateOrderRequest request, CancellationToken ct)
    {
        var sub = User.FindFirst("sub")?.Value;
        if (!Guid.TryParse(sub, out var userId))
            return Unauthorized();

        var command = new CreateOrderCommand(userId, request.Items);
        var result = await _sender.Send(command, ct);
        return result.ToActionResult();
    }
}

public sealed record CreateOrderRequest(IReadOnlyList<OrderItemRequest> Items);
