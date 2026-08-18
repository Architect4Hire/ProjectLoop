using System.Security.Claims;
using ProjectLoop.Identity.Core;

namespace ProjectLoop.Identity;

/// <summary>
/// Establishes <see cref="ICurrentTenantContextAccessor.Current"/> for an
/// authenticated request by validating the requested tenant against
/// persisted membership. Denial short-circuits the pipeline; it never falls
/// back to a different tenant.
/// </summary>
public sealed class TenantContextMiddleware
{
    private const string TenantIdHeaderName = "X-Tenant-Id";

    private readonly RequestDelegate _next;

    public TenantContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        ITenantContextResolver resolver,
        ICurrentTenantContextAccessor accessor)
    {
        var userId = context.User.Identity?.IsAuthenticated == true
            ? context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            : null;

        if (string.IsNullOrEmpty(userId))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        if (context.GetEndpoint()?.Metadata?.GetMetadata<SkipTenantContextAttribute>() is not null)
        {
            await _next(context);
            return;
        }

        var tenantIdHeader = context.Request.Headers[TenantIdHeaderName].FirstOrDefault();
        if (!Guid.TryParse(tenantIdHeader, out var requestedTenantId))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }

        var resolution = await resolver.ResolveAsync(userId, requestedTenantId, context.RequestAborted);
        if (!resolution.IsAllowed)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return;
        }

        accessor.Current = resolution.Context;
        await _next(context);
    }
}
