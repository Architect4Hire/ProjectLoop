namespace ProjectLoop.Documents.Core;

public interface IDocumentUploadFacade
{
    Task<DocumentUploadFacadeResult> UploadAsync(
        UploadDocumentMetadata metadata,
        Stream content,
        string originalFileName,
        string mimeType,
        CancellationToken cancellationToken = default);
}
