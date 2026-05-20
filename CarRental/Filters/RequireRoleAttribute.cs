using Microsoft.AspNetCore.Mvc.Filters;

namespace CarRental.Filters;

public class RequireRoleAttribute : Attribute, IFilterFactory
{
    private readonly string _role;
    public bool IsReusable => false;

    public RequireRoleAttribute(string role)
    {
        _role = role;
    }

    public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        => new RequireRoleFilter(_role);
}