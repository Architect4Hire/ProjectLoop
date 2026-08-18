namespace ProjectLoop.Documents.Core;

public interface IDocumentPublishFacade
{
    Task<DocumentPublishFacadeResult> PublishAsync(
        PublishDocumentVersionRequest request,
        CancellationToken cancellationToken = default);
}
