using System.Reflection;
using ProjectLoop.Documents.Core;
using Xunit;

namespace ProjectLoop.Documents.Core.Tests;

public class UploadDocumentResultTests
{
    [Fact]
    public void UploadDocumentResult_Can_Be_Created_With_Required_Members()
    {
        var documentId = Guid.NewGuid();
        var versionId = Guid.NewGuid();
        var now = DateTimeOffset.UtcNow;

        var result = new UploadDocumentResult
        {
            DocumentId = documentId,
            VersionId = versionId,
            VersionNumber = 1,
            Title = "Statement of Work",
            Category = "Contract",
            Status = DocumentStatus.Draft,
            Visibility = DocumentVisibility.Internal,
            SizeBytes = 1024,
            CreatedAtUtc = now,
        };

        Assert.Equal(documentId, result.DocumentId);
        Assert.Equal(versionId, result.VersionId);
        Assert.Equal(1, result.VersionNumber);
    }

    [Fact]
    public void UploadDocumentResult_Never_Exposes_A_Blob_Key_Or_Url()
    {
        var propertyNames = typeof(UploadDocumentResult)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(p => p.Name);

        Assert.DoesNotContain(propertyNames, name =>
            name.Contains("Blob", StringComparison.OrdinalIgnoreCase) ||
            name.Contains("Url", StringComparison.OrdinalIgnoreCase) ||
            name.Contains("Uri", StringComparison.OrdinalIgnoreCase));
    }
}
