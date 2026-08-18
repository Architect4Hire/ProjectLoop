namespace ProjectLoop.Documents.Core;

/// <summary>
/// The client-facing visibility of a document. Internal users may publish a
/// document to Client visibility; a raw Blob object key is never sufficient
/// to grant access regardless of this value — authorization is still
/// evaluated server-side on every access.
/// </summary>
public enum DocumentVisibility
{
    Internal,
    Client,
}
