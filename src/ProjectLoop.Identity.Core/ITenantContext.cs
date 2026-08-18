namespace ProjectLoop.Identity.Core;

/// <summary>
/// Represents the tenant an authenticated request has been authorized to act
/// within. Established only by server-side resolution against persisted
/// membership; request payload values never establish it.
/// </summary>
public interface ITenantContext
{
    Guid TenantId { get; }

    string UserId { get; }

    TenantMembershipRole Role { get; }
}
