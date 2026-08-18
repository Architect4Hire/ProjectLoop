namespace ProjectLoop.Identity.Core;

public enum TenantStatus
{
    Active,
    Suspended,
}

public sealed class Tenant
{
    public required Guid Id { get; init; }

    public required string Name { get; set; }

    public required TenantStatus Status { get; set; }

    public required DateTimeOffset CreatedAtUtc { get; init; }

    public required DateTimeOffset UpdatedAtUtc { get; set; }
}
