using System.Globalization;
using DotAid.AspNetCore.Security.Obfuscator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace DotAid.AspNetCore.Security.MVC;

public class ObfuscatedRouteConstraint : IRouteConstraint
{
    private IObfuscator Obfuscator { get; }

    public ObfuscatedRouteConstraint(IObfuscator obfuscator)
    {
        Obfuscator = obfuscator ?? throw new ArgumentNullException(nameof(obfuscator));
    }

    public bool Match(HttpContext? httpContext, IRouter? route, string routeKey, RouteValueDictionary values,
        RouteDirection routeDirection)
    {
        if (!values.TryGetValue(routeKey, out var value)) return false;
        var obfuscatedId = Convert.ToString(value, CultureInfo.InvariantCulture);
        return Obfuscator.IsValidInput(obfuscatedId);
    }
}