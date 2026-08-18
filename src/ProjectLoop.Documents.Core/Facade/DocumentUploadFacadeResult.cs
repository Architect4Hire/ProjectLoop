namespace ProjectLoop.Documents.Core;

public enum DocumentUploadFacadeError
{
    NoTenantContext,
    InvalidMetadata,
    InvalidFileName,
    UnsupportedMediaType,
    PayloadTooLarge,
    PersistenceFailed,
}

public sealed class DocumentUploadFacadeResult
{
    private DocumentUploadFacadeResult(UploadDocumentResult? result, DocumentUploadFacadeError? error)
    {
        Result = result;
        Error = error;
    }

    public bool IsSuccess => Error is null;

    public UploadDocumentResult? Result { get; }

    public DocumentUploadFacadeError? Error { get; }

    public static DocumentUploadFacadeResult Success(UploadDocumentResult result) => new(result, null);

    public static DocumentUploadFacadeResult Failure(DocumentUploadFacadeError error) => new(null, error);
}
