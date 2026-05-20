using Microsoft.AspNetCore.Mvc.Filters;

namespace CarRental.Filters;

public class AuthenticatedAttribute : Attribute, IFilterFactory
{
    public bool IsReusable => false;

    public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        => new AuthenticatedFilter();
}