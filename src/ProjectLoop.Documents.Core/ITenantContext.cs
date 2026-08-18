namespace ProjectLoop.Documents.Core;

/// <summary>
/// Represents the tenant an authenticated request has been authorized to act
/// within. Established only by server-side resolution, never from request
/// payload/route values.
/// </summary>
public interface ITenantContext
{
    Guid TenantId { get; }

    string UserId { get; }

    /// <summary>
    /// True unless the caller's token asserts internal (consulting-firm)
    /// status. Internal-only actions such as publishing a document version
    /// to client visibility require this to be false. See
    /// ADR-014-internal-client-user-classification.
    /// </summary>
    bool IsClientUser { get; }
}
