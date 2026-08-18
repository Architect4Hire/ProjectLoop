namespace ProjectLoop.Identity.Contracts;

public sealed class AcceptInvitationRequest
{
    public required string Token { get; init; }
}
