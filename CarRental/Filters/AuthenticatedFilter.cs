using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CarRental.Filters;

public class AuthenticatedFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        var userId = context.HttpContext.Items["UserId"];

        if (userId is null)
        {
            if (context.HttpContext.Request.Path.StartsWithSegments("/api"))
                context.Result = new UnauthorizedResult();
            else
                context.Result = new RedirectToActionResult("Login", "User", null);
        }
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}