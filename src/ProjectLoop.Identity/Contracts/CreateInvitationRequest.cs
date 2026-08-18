namespace ProjectLoop.Identity.Contracts;

public sealed class CreateInvitationRequest
{
    public required string Email { get; init; }
}
