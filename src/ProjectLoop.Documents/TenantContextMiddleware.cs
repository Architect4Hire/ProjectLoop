using System.Security.Claims;
using ProjectLoop.Documents.Core;

namespace ProjectLoop.Documents;

/// <summary>
/// Establishes <see cref="ICurrentTenantContextAccessor.Current"/> for an
/// authenticated request from the caller's validated token claims. Documents
/// does not own tenant membership data, so it trusts the tenant/user claims
/// already validated server-side by the token issuer rather than
/// re-resolving membership against another service's database.
/// </summary>
public sealed class TenantContextMiddleware
{
    private const string TenantIdClaimType = "tenant_id";
    private const string UserTypeClaimType = "user_type";
    private const string InternalUserTypeClaimValue = "Internal";

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

        // Default-deny: only an explicit "Internal" assertion grants
        // internal (publish-capable) privilege. A missing or unrecognized
        // claim is treated as a client user. See
        // ADR-014-internal-client-user-classification.
        var isClientUser = context.User.FindFirst(UserTypeClaimType)?.Value != InternalUserTypeClaimValue;

        accessor.Current = new TenantContext { TenantId = tenantId, UserId = userId, IsClientUser = isClientUser };
        await _next(context);
    }
}
