namespace ProjectLoop.Identity.Core;

public enum TenantMembershipRole
{
    Member,
    Admin,
}

public enum TenantMembershipStatus
{
    Active,
    Revoked,
}

public sealed class TenantMembership
{
    public required Guid Id { get; init; }

    public required string UserId { get; init; }

    public required Guid TenantId { get; init; }

    public required TenantMembershipRole Role { get; set; }

    public required TenantMembershipStatus Status { get; set; }

    public required DateTimeOffset CreatedAtUtc { get; init; }

    public required DateTimeOffset UpdatedAtUtc { get; set; }
}
