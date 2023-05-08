using DotAid.AspNetCore.Security.Configuration;
using DotAid.AspNetCore.Security.Json;
using DotAid.AspNetCore.Security.MVC;
using DotAid.AspNetCore.Security.Obfuscator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace DotAid.AspNetCore.Security.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIdObfuscator(this IServiceCollection services, IObfuscatorSettings setup)
    {
        var obfuscator = (IObfuscator)Activator.CreateInstance(setup.GetObfuscatorType, setup)! ??
                         throw new InvalidOperationException();
        services.AddSingleton(setup);
        services.AddHttpContextAccessor();
        services.AddSingleton(obfuscator);

        services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new ObfuscatedLongIdJsonSerializer(obfuscator));
            options.JsonSerializerOptions.Converters.Add(new ObfuscatedIntIdJsonSerializer(obfuscator));
        });

        services.AddMvc(
            config => config.ModelBinderProviders.Insert(0, new ModelBinderProvider())
        );

        services.PostConfigure<RouteOptions>(routeOptions =>
        {
            routeOptions.ConstraintMap.Add(setup.RouteConstraintName, typeof(ObfuscatedRouteConstraint));
        });
        return services;
    }
}