using System.Reflection;
using ProjectLoop.Documents.Core;
using Xunit;

namespace ProjectLoop.Documents.Core.Tests;

public class DocumentListResponseTests
{
    [Fact]
    public void DocumentListResponse_Can_Be_Created_With_Required_Members()
    {
        var item = new DocumentSummaryResponse
        {
            Id = Guid.NewGuid(),
            ProjectId = Guid.NewGuid(),
            Title = "Statement of Work",
            Category = "Contract",
            Status = DocumentStatus.Draft,
            Visibility = DocumentVisibility.Internal,
            HasCurrentVersion = true,
            UpdatedAtUtc = DateTimeOffset.UtcNow,
        };

        var response = new DocumentListResponse
        {
            Items = [item],
            Page = 1,
            PageSize = 20,
            TotalCount = 1,
        };

        Assert.Single(response.Items);
        Assert.Equal(1, response.TotalCount);
    }

    [Fact]
    public void DocumentSummaryResponse_Never_Exposes_A_Blob_Key()
    {
        var propertyNames = typeof(DocumentSummaryResponse)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(p => p.Name);

        Assert.DoesNotContain(propertyNames, name => name.Contains("Blob", StringComparison.OrdinalIgnoreCase));
    }
}
