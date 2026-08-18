using ProjectLoop.Documents.Core;
using Xunit;

namespace ProjectLoop.Documents.Core.Tests;

public class DocumentMimeTypePolicyTests
{
    [Theory]
    [InlineData("application/pdf")]
    [InlineData("image/png")]
    [InlineData("text/plain")]
    [InlineData("APPLICATION/PDF")]
    public void IsAllowed_Returns_True_For_Allow_Listed_Types(string mimeType)
    {
        Assert.True(DocumentMimeTypePolicy.IsAllowed(mimeType));
    }

    [Theory]
    [InlineData("application/x-msdownload")]
    [InlineData("application/octet-stream")]
    [InlineData("text/html")]
    [InlineData("")]
    [InlineData(null)]
    public void IsAllowed_Returns_False_For_Types_Not_On_The_Allow_List(string? mimeType)
    {
        Assert.False(DocumentMimeTypePolicy.IsAllowed(mimeType));
    }
}
