namespace ProjectLoop.Identity.Core;

public interface IInvitationTokenGenerator
{
    /// <summary>
    /// Generates a new invitation token. <see cref="InvitationToken.RawToken"/>
    /// is returned to the caller once and never persisted;
    /// <see cref="InvitationToken.TokenHash"/> is the value stored on
    /// <see cref="ClientInvitation.TokenHash"/> for later lookup.
    /// </summary>
    InvitationToken Generate();

    /// <summary>
    /// Hashes a raw token presented by a caller so it can be compared
    /// against a stored <see cref="ClientInvitation.TokenHash"/>.
    /// </summary>
    string Hash(string rawToken);
}
