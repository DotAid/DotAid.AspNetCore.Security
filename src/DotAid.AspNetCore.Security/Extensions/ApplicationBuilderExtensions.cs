using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace DotAid.AspNetCore.Security.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder EnforceHttpsAndHsts(this IApplicationBuilder applicationBuilder)
    {
        if (applicationBuilder is WebApplication webApplication && webApplication.Environment.IsDevelopment())
        {
            return applicationBuilder;
        }

        applicationBuilder.Use(async (context, next) =>
        {
            if (!context.Request.IsHttps)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("HTTPS required!");
            }
            else
            {
                await next(context);
            }
        });
        applicationBuilder.UseHsts();

        return applicationBuilder;
    }
}