using System.Security.Claims;
using ProjectLoop.Engagement.Core;

namespace ProjectLoop.Engagement;

/// <summary>
/// Establishes <see cref="ICurrentTenantContextAccessor.Current"/> for an
/// authenticated request from the caller's validated token claims. Engagement
/// does not own tenant membership data, so it trusts the tenant/user claims
/// already validated server-side by the token issuer rather than
/// re-resolving membership against another service's database.
/// </summary>
public sealed class TenantContextMiddleware
{
    private const string TenantIdClaimType = "tenant_id";

    private readonly RequestDelegate _next;

    public TenantContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ICurrentTenantContextAccessor accessor)
    {
        var userId = context.User.Identity?.IsAuthenticated == true
            ? context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            : null;

        if (string.IsNullOrEmpty(userId))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        var tenantIdClaim = context.User.FindFirst(TenantIdClaimType)?.Value;
        if (!Guid.TryParse(tenantIdClaim, out var tenantId))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        accessor.Current = new TenantContext { TenantId = tenantId, UserId = userId };
        await _next(context);
    }
}
