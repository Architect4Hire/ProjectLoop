namespace ProjectLoop.Identity.Core;

public enum ClientInvitationStatus
{
    Pending,
    Accepted,
    Revoked,
}

public sealed class ClientInvitation
{
    public required Guid Id { get; init; }

    public required Guid TenantId { get; init; }

    public required string Email { get; init; }

    public required string TokenHash { get; init; }

    public required DateTimeOffset ExpiresAtUtc { get; init; }

    public required ClientInvitationStatus Status { get; set; }

    public required string InvitedByUserId { get; init; }

    public required DateTimeOffset CreatedAtUtc { get; init; }

    public required DateTimeOffset UpdatedAtUtc { get; set; }
}
