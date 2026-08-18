using ProjectLoop.Documents.Core;
using Xunit;

namespace ProjectLoop.Documents.Core.Tests;

public class DocumentVersionTests
{
    [Fact]
    public void DocumentVersion_Can_Be_Created_With_Required_Members()
    {
        var now = DateTimeOffset.UtcNow;

        var version = new DocumentVersion
        {
            Id = Guid.NewGuid(),
            DocumentId = Guid.NewGuid(),
            VersionNumber = 1,
            BlobObjectKey = "d3f1b2c4-9a7e-4b8a-9c1e-1234567890ab",
            MimeType = "application/pdf",
            SizeBytes = 2048,
            ContentHash = "sha256:abc123",
            UploadedByUserId = "user-123",
            CreatedAtUtc = now,
        };

        Assert.Equal(1, version.VersionNumber);
        Assert.Equal("application/pdf", version.MimeType);
        Assert.Equal(2048, version.SizeBytes);
    }
}
