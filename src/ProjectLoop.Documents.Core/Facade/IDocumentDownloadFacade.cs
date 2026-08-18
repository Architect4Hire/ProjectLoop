namespace ProjectLoop.Documents.Core;

public interface IDocumentDownloadFacade
{
    Task<DocumentDownloadFacadeResult> DownloadAsync(
        Guid documentId,
        Guid versionId,
        CancellationToken cancellationToken = default);
}
