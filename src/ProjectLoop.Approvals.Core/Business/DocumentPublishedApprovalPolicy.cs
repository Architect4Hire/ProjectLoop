namespace ProjectLoop.Approvals.Core;

/// <summary>
/// Pure decision of whether a published DocumentVersion requires a client
/// approval request. Only a publication visible to the client (Client
/// visibility) is approval-gated — Internal-visibility publications never
/// raise an approval. Uses only the DocumentPublished event's own metadata;
/// it has no side effects and reads no other durable state.
/// </summary>
public static class DocumentPublishedApprovalPolicy
{
    public static bool RequiresApproval(DocumentPublishedV1 documentPublished) =>
        documentPublished.Visibility == DocumentPublishedVisibility.Client;
}
