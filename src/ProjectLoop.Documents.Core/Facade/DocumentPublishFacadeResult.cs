namespace ProjectLoop.Documents.Core;

public enum DocumentPublishFacadeError
{
    NoTenantContext,
    Forbidden,
    DocumentNotFound,
    VersionNotFound,
    NotEligible,
}

public sealed class DocumentPublishFacadeResult
{
    private DocumentPublishFacadeResult(PublishDocumentVersionResult? result, DocumentPublishFacadeError? error)
    {
        Result = result;
        Error = error;
    }

    public bool IsSuccess => Error is null;

    public PublishDocumentVersionResult? Result { get; }

    public DocumentPublishFacadeError? Error { get; }

    public static DocumentPublishFacadeResult Success(PublishDocumentVersionResult result) => new(result, null);

    public static DocumentPublishFacadeResult Failure(DocumentPublishFacadeError error) => new(null, error);
}
