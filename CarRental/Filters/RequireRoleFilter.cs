using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CarRental.Filters;

public class RequireRoleFilter : IActionFilter
{
    private readonly string _role;

    public RequireRoleFilter(string role)
    {
        _role = role;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var roles = context.HttpContext.Items["Roles"] as List<string>;

        if (roles is null || !roles.Contains(_role))
        {
            if (context.HttpContext.Request.Path.StartsWithSegments("/api"))
                context.Result = new ObjectResult("Forbidden") { StatusCode = 403 };
            else
                context.Result = new RedirectToActionResult("Index", "Home", null);
        }
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}