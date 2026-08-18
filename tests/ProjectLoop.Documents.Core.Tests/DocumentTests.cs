using ProjectLoop.Documents.Core;
using Xunit;

namespace ProjectLoop.Documents.Core.Tests;

public class DocumentTests
{
    [Fact]
    public void Document_Can_Be_Created_With_Required_Members()
    {
        var now = DateTimeOffset.UtcNow;

        var document = new Document
        {
            Id = Guid.NewGuid(),
            TenantId = Guid.NewGuid(),
            ProjectId = Guid.NewGuid(),
            Title = "Statement of Work",
            Category = "Contract",
            Status = DocumentStatus.Draft,
            Visibility = DocumentVisibility.Internal,
            CreatedAtUtc = now,
            UpdatedAtUtc = now,
        };

        Assert.Equal("Statement of Work", document.Title);
        Assert.Equal(DocumentStatus.Draft, document.Status);
        Assert.Equal(DocumentVisibility.Internal, document.Visibility);
        Assert.Null(document.CurrentVersionId);
    }

    [Fact]
    public void Document_Status_Can_Transition_To_Published()
    {
        var now = DateTimeOffset.UtcNow;
        var document = new Document
        {
            Id = Guid.NewGuid(),
            TenantId = Guid.NewGuid(),
            ProjectId = Guid.NewGuid(),
            Title = "Statement of Work",
            Category = "Contract",
            Status = DocumentStatus.Draft,
            Visibility = DocumentVisibility.Internal,
            CreatedAtUtc = now,
            UpdatedAtUtc = now,
        };

        document.Status = DocumentStatus.Published;
        document.Visibility = DocumentVisibility.Client;

        Assert.Equal(DocumentStatus.Published, document.Status);
        Assert.Equal(DocumentVisibility.Client, document.Visibility);
    }
}
