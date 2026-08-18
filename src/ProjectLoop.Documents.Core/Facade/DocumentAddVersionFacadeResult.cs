namespace ProjectLoop.Documents.Core;

public enum DocumentAddVersionFacadeError
{
    NoTenantContext,
    DocumentNotFound,
    InvalidFileName,
    UnsupportedMediaType,
    PayloadTooLarge,
    PersistenceFailed,
}

public sealed class DocumentAddVersionFacadeResult
{
    private DocumentAddVersionFacadeResult(AddDocumentVersionResult? result, DocumentAddVersionFacadeError? error)
    {
        Result = result;
        Error = error;
    }

    public bool IsSuccess => Error is null;

    public AddDocumentVersionResult? Result { get; }

    public DocumentAddVersionFacadeError? Error { get; }

    public static DocumentAddVersionFacadeResult Success(AddDocumentVersionResult result) => new(result, null);

    public static DocumentAddVersionFacadeResult Failure(DocumentAddVersionFacadeError error) => new(null, error);
}
