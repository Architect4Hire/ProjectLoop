using ProjectLoop.Documents.Core;
using Xunit;

namespace ProjectLoop.Documents.Core.Tests;

public class DocumentSummaryMapperTests
{
    private static Document CreateDocument(Guid? currentVersionId = null)
    {
        var now = DateTimeOffset.UtcNow;

        return new Document
        {
            Id = Guid.NewGuid(),
            TenantId = Guid.NewGuid(),
            ProjectId = Guid.NewGuid(),
            Title = "Statement of Work",
            Category = "Contract",
            Status = DocumentStatus.Draft,
            Visibility = DocumentVisibility.Internal,
            CurrentVersionId = currentVersionId,
            CreatedAtUtc = now,
            UpdatedAtUtc = now,
        };
    }

    [Fact]
    public void ToSummaryResponse_Maps_Fields_And_Reports_HasCurrentVersion()
    {
        var document = CreateDocument(currentVersionId: Guid.NewGuid());

        var response = DocumentSummaryMapper.ToSummaryResponse(document);

        Assert.Equal(document.Id, response.Id);
        Assert.Equal(document.Title, response.Title);
        Assert.True(response.HasCurrentVersion);
    }

    [Fact]
    public void ToSummaryResponse_Reports_No_Current_Version_When_Pointer_Is_Null()
    {
        var document = CreateDocument(currentVersionId: null);

        var response = DocumentSummaryMapper.ToSummaryResponse(document);

        Assert.False(response.HasCurrentVersion);
    }

    [Fact]
    public void ToListResponse_Maps_Items_And_Carries_Paging_Metadata()
    {
        var documents = new[] { CreateDocument(), CreateDocument() };

        var response = DocumentSummaryMapper.ToListResponse(documents, totalCount: 7, page: 2, pageSize: 2);

        Assert.Equal(2, response.Items.Count);
        Assert.Equal(7, response.TotalCount);
        Assert.Equal(2, response.Page);
        Assert.Equal(2, response.PageSize);
    }
}
