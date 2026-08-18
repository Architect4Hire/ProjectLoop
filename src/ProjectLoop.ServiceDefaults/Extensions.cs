namespace ProjectLoop.ServiceDefaults;

using Aspire.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Scalar.AspNetCore;

public static class Extensions
{
    public static IDistributedApplicationBuilder AddServiceDefaults(
        this IDistributedApplicationBuilder builder)
    {
        return builder;
    }

    public static WebApplicationBuilder AddServiceDefaults(
        this WebApplicationBuilder builder)
    {
        builder.Services.AddOpenApi();
        return builder;
    }

    public static WebApplication MapDefaultHealthChecks(
        this WebApplication app)
    {
        app.MapOpenApi();
        app.MapScalarApiReference();
        app.MapHealthChecks("/health");
        return app;
    }
}
