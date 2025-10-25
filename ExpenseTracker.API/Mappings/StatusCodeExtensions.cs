using ExpenseTracker.API.DTO;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.API.Mappings
{
    public static class StatusCodeExtensions
    {
        public static ActionResult ToActionResult<T>(this Result<T> result)
        {
            return result.StatusCode switch
            {
                StatusCode.Ok => new OkObjectResult(result.Data),
                StatusCode.NoContent => new NoContentResult(),
                StatusCode.Created => new CreatedResult((string?)null, result.Data),
                StatusCode.BadRequest => new BadRequestObjectResult(result.ErrorMessage),
                StatusCode.NotFound => new NotFoundObjectResult(result.ErrorMessage),
                StatusCode.Unauthorize when result.ErrorMessage != null => new UnauthorizedObjectResult(result.ErrorMessage),
                StatusCode.Unauthorize when result.ErrorMessage == null => new UnauthorizedResult(),
                _ => new StatusCodeResult(500)
            };
        }

        public static ActionResult ToActionResult(this Result result)
        {
            return result.StatusCode switch
            {
                StatusCode.Ok => new OkResult(),
                StatusCode.NoContent => new NoContentResult(),
                StatusCode.Created => new CreatedResult(),
                StatusCode.BadRequest => new BadRequestObjectResult(result.ErrorMessage),
                StatusCode.NotFound => new NotFoundObjectResult(result.ErrorMessage),
                StatusCode.Unauthorize => new UnauthorizedResult(),
                _ => new StatusCodeResult(500)
            };
        }

        public static ActionResult ToCreatedActionResult<T>(this Result<T> result, ControllerBase controller, string? actionName = null, object? routeValues = null)
        {
            return ToCreatedActionResult(result, controller, actionName, null, routeValues);
        }

        public static ActionResult ToCreatedActionResult<T>(this Result<T> result, ControllerBase controller, string? actionName, string? controllerName, object? routeValues)
        {
            if (!result.Success && result.StatusCode != StatusCode.Created)
            {
                return result.ToActionResult();
            }

            return controller.CreatedAtAction(actionName, controllerName, routeValues, result.Data);
        }
    }
}
