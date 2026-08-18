namespace ProjectLoop.Documents.Core;

public interface IDocumentAddVersionFacade
{
    Task<DocumentAddVersionFacadeResult> AddVersionAsync(
        AddDocumentVersionRequest request,
        Stream content,
        string originalFileName,
        string mimeType,
        CancellationToken cancellationToken = default);
}
