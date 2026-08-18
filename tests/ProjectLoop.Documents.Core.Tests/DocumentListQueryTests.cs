using ProjectLoop.Documents.Core;
using Xunit;

namespace ProjectLoop.Documents.Core.Tests;

public class DocumentListQueryTests
{
    [Fact]
    public void DocumentListQuery_Defaults_Page_And_PageSize_When_Not_Specified()
    {
        var query = new DocumentListQuery { ProjectId = Guid.NewGuid() };

        Assert.Equal(1, query.Page);
        Assert.Equal(DocumentListQuery.DefaultPageSize, query.PageSize);
        Assert.Null(query.Category);
        Assert.Null(query.Status);
        Assert.Null(query.Visibility);
    }

    [Fact]
    public void DocumentListQuery_Can_Be_Created_With_All_Filters()
    {
        var projectId = Guid.NewGuid();

        var query = new DocumentListQuery
        {
            ProjectId = projectId,
            Category = "Contract",
            Status = DocumentStatus.Published,
            Visibility = DocumentVisibility.Client,
            Page = 2,
            PageSize = 50,
        };

        Assert.Equal(projectId, query.ProjectId);
        Assert.Equal("Contract", query.Category);
        Assert.Equal(DocumentStatus.Published, query.Status);
        Assert.Equal(DocumentVisibility.Client, query.Visibility);
        Assert.Equal(2, query.Page);
        Assert.Equal(50, query.PageSize);
    }
}
