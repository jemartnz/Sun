using Microsoft.AspNetCore.Mvc;
using Domain.Commons;

namespace Api.Extensions;

public static class ResultExtensions
{
    public static IActionResult ToActionResult(this Result result)
    {
        if (result.IsSuccess)
            return new OkResult();

        return ToProblemResult(result.Error);
    }

    public static IActionResult ToNoContentResult(this Result result)
    {
        if (result.IsSuccess)
            return new NoContentResult();

        return ToProblemResult(result.Error);
    }

    public static IActionResult ToActionResult<T>(this Result<T> result)
    {
        if (result.IsSuccess)
            return new OkObjectResult(result.Value);

        return ToProblemResult(result.Error);
    }

    public static IActionResult ToCreatedResult<T>(this Result<T> result, string routeName, Func<T, object> routeValues)
    {
        if (result.IsSuccess)
            return new CreatedAtRouteResult(routeName, routeValues(result.Value), result.Value);

        return ToProblemResult(result.Error);
    }

    private static IActionResult ToProblemResult(Error error)
    {
        // Mapear errores de dominio a códigos HTTP apropiados
        var statusCode = error.Code switch
        {
            _ when error.Code.Contains("NotFound") => StatusCodes.Status404NotFound,
            _ when error.Code.Contains("AlreadyExists") => StatusCodes.Status409Conflict,
            _ when error.Code.Contains("InvalidCredentials") => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status400BadRequest
        };

        return new ObjectResult(new ProblemDetails
        {
            Status = statusCode,
            Title = error.Code,
            Detail = error.Message
        })
        {
            StatusCode = statusCode
        };
    }
}
