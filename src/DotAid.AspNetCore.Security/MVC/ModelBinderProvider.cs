using DotAid.AspNetCore.Security.Configuration;
using DotAid.AspNetCore.Security.Obfuscator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace DotAid.AspNetCore.Security.MVC;

public class ModelBinderProvider : IModelBinderProvider
{
    public IModelBinder GetBinder(ModelBinderProviderContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        return GetPathModelBinder(context) ?? GetParameterModelBinder(context) ?? null!;
    }

    private static IModelBinder? GetParameterModelBinder(ModelBinderProviderContext context)
    {
        // Only accept Bindings from parameters
        if (context.BindingInfo.BindingSource != null &&
            !context.BindingInfo.BindingSource.CanAcceptDataFrom(BindingSource.Query))
        {
            return null;
        }

        // Only int and long IDs are supported
        if (context.Metadata.ModelType != typeof(HashedId<int>) && context.Metadata.ModelType != typeof(HashedId<long>))
        {
            return null!;
        }

        return new ObfuscatorModelBinder(GetObfuscatorService(context));
    }

    private static IObfuscator GetObfuscatorService(ModelBinderProviderContext context)
    {
        return context.Services.GetService(typeof(IObfuscator)) as IObfuscator ??
               throw new ArgumentNullException(nameof(context));
    }

    private static IModelBinder? GetPathModelBinder(ModelBinderProviderContext context)
    {
        // Only int and long IDs are supported
        if (context.Metadata.ModelType != typeof(int) && context.Metadata.ModelType != typeof(long))
        {
            return null!;
        }

        // Only accept Bindings from path
        if (context.BindingInfo.BindingSource != null &&
            !context.BindingInfo.BindingSource.CanAcceptDataFrom(BindingSource.Path))
        {
            return null;
        }

        // Try to get HttpContext to determine the route pattern
        if (context.Services.GetService<IHttpContextAccessor>() is not HttpContextAccessor httpContextAccessor)
        {
            return null;
        }

        // Get the called http Endpoint
        if (httpContextAccessor.HttpContext?.GetEndpoint() is not RouteEndpoint routeEndpoint)
        {
            return null;
        }

        var routeConstraintName = context.Services.GetService<IObfuscatorSettings>()?.RouteConstraintName;

        // Ensure that the route parameter has the right RouteConstraint
        if (routeEndpoint.RoutePattern.ParameterPolicies.TryGetValue(context.Metadata.ParameterName!,
                out var parameterPolicy) && parameterPolicy.Any(x => x.Content == routeConstraintName))
        {
            return new ObfuscatorModelBinder(GetObfuscatorService(context));
        }

        return null;
    }
}