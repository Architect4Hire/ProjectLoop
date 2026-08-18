namespace ProjectLoop.Approvals.Core;

/// <summary>
/// Represents the tenant an authenticated request has been authorized to act
/// within. Established only by server-side resolution, never from request
/// payload/route values.
/// </summary>
public interface ITenantContext
{
    Guid TenantId { get; }

    string UserId { get; }
}
