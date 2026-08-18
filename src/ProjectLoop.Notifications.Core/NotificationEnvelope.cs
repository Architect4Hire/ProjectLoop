namespace ProjectLoop.Notifications.Core;

/// <summary>
/// Provider-neutral, already-rendered notification ready for delivery.
/// RecipientUserId is a stable identity reference, not a resolved email
/// address — resolving it to a concrete address is a sender-implementation
/// concern, kept out of this contract so swapping providers never touches
/// mapping or consumer code.
/// </summary>
public sealed record NotificationEnvelope(
    Guid TenantId,
    string RecipientUserId,
    string Subject,
    string Body);
