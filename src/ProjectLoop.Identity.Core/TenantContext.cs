namespace ProjectLoop.Identity.Core;

public sealed class TenantContext : ITenantContext
{
    public required Guid TenantId { get; init; }

    public required string UserId { get; init; }

    public required TenantMembershipRole Role { get; init; }
}
