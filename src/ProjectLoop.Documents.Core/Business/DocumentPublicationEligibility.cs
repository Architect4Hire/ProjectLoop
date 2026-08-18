namespace ProjectLoop.Documents.Core;

/// <summary>
/// Pure domain rule deciding whether a specific DocumentVersion may be
/// published. Only the Document's current version may be published, an
/// already-published version cannot be republished, and an archived
/// Document can no longer be published to. Approval of a later version never
/// implies eligibility changes for an earlier one — each version's
/// eligibility is evaluated independently against its own state.
/// </summary>
public static class DocumentPublicationEligibility
{
    public static bool IsEligible(Document document, DocumentVersion version) =>
        version.DocumentId == document.Id &&
        document.CurrentVersionId == version.Id &&
        document.Status != DocumentStatus.Archived &&
        !version.IsPublished;
}
