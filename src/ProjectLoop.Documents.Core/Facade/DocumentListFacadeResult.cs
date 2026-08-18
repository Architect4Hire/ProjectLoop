namespace ProjectLoop.Documents.Core;

public enum DocumentListFacadeError
{
    NoTenantContext,
}

public sealed class DocumentListFacadeResult
{
    private DocumentListFacadeResult(DocumentListResponse? response, DocumentListFacadeError? error)
    {
        Response = response;
        Error = error;
    }

    public bool IsSuccess => Error is null;

    public DocumentListResponse? Response { get; }

    public DocumentListFacadeError? Error { get; }

    public static DocumentListFacadeResult Success(DocumentListResponse response) => new(response, null);

    public static DocumentListFacadeResult Failure(DocumentListFacadeError error) => new(null, error);
}
