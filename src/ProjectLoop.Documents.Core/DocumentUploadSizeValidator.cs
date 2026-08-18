using Microsoft.Extensions.Options;

namespace ProjectLoop.Documents.Core;

public interface IDocumentUploadSizeValidator
{
    bool IsWithinLimit(long sizeBytes);
}

public sealed class DocumentUploadSizeValidator : IDocumentUploadSizeValidator
{
    private readonly IOptions<DocumentUploadLimitsOptions> _options;

    public DocumentUploadSizeValidator(IOptions<DocumentUploadLimitsOptions> options)
    {
        _options = options;
    }

    public bool IsWithinLimit(long sizeBytes) =>
        sizeBytes > 0 && sizeBytes <= _options.Value.MaxSizeBytes;
}
