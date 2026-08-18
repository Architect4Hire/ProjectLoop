namespace ProjectLoop.Identity.Core;

public sealed record InvitationCreationResult(ClientInvitation Invitation, string RawToken);
