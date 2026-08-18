namespace ProjectLoop.Documents.Core;

/// <summary>
/// Pure domain rule deciding whether a resolved (already tenant-scoped)
/// Document/DocumentVersion pair may be downloaded by the current caller.
/// Internal users may download any version belonging to their tenant,
/// including unpublished/internal-only drafts they are actively working on.
/// Client users may only download a version that has been explicitly
/// published on a Client-visibility document — an exact version Id alone
/// never bypasses this.
/// </summary>
public static class DocumentDownloadAuthorization
{
    public static bool IsAuthorized(Document document, DocumentVersion version, bool isClientUser)
    {
        if (version.DocumentId != document.Id)
        {
            return false;
        }

        if (!isClientUser)
        {
            return true;
        }

        return document.Visibility == DocumentVisibility.Client && version.IsPublished;
    }
}
