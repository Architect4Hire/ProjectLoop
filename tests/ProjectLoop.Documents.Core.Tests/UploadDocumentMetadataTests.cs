using ProjectLoop.Documents.Core;
using Xunit;

namespace ProjectLoop.Documents.Core.Tests;

public class UploadDocumentMetadataTests
{
    [Fact]
    public void UploadDocumentMetadata_Can_Be_Created_With_Required_Members()
    {
        var projectId = Guid.NewGuid();

        var metadata = new UploadDocumentMetadata
        {
            ProjectId = projectId,
            Title = "Statement of Work",
            Category = "Contract",
            Visibility = DocumentVisibility.Internal,
        };

        Assert.Equal(projectId, metadata.ProjectId);
        Assert.Equal("Statement of Work", metadata.Title);
        Assert.Equal("Contract", metadata.Category);
        Assert.Equal(DocumentVisibility.Internal, metadata.Visibility);
    }
}
