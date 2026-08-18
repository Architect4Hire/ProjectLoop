namespace ProjectLoop.Documents.Core;

public enum DocumentDownloadFacadeError
{
    NoTenantContext,
    NotFound,
    Forbidden,
}

public sealed class DocumentDownloadFacadeResult : IAsyncDisposable
{
    private DocumentDownloadFacadeResult(
        DocumentDownloadDescriptor? descriptor,
        Stream? content,
        DocumentDownloadFacadeError? error)
    {
        Descriptor = descriptor;
        Content = content;
        Error = error;
    }

    public bool IsSuccess => Error is null;

    public DocumentDownloadDescriptor? Descriptor { get; }

    public Stream? Content { get; }

    public DocumentDownloadFacadeError? Error { get; }

    public static DocumentDownloadFacadeResult Success(DocumentDownloadDescriptor descriptor, Stream content) =>
        new(descriptor, content, null);

    public static DocumentDownloadFacadeResult Failure(DocumentDownloadFacadeError error) => new(null, null, error);

    public async ValueTask DisposeAsync()
    {
        if (Content is not null)
        {
            await Content.DisposeAsync();
        }
    }
}
