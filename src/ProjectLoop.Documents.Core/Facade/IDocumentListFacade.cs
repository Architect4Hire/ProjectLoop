namespace ProjectLoop.Documents.Core;

public interface IDocumentListFacade
{
    Task<DocumentListFacadeResult> ListAsync(DocumentListQuery query, CancellationToken cancellationToken = default);
}
