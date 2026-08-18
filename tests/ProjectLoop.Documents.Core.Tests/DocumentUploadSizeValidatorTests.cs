using Microsoft.Extensions.Options;
using ProjectLoop.Documents.Core;
using Xunit;

namespace ProjectLoop.Documents.Core.Tests;

public class DocumentUploadSizeValidatorTests
{
    private static DocumentUploadSizeValidator CreateValidator(long maxSizeBytes) =>
        new(Options.Create(new DocumentUploadLimitsOptions { MaxSizeBytes = maxSizeBytes }));

    [Fact]
    public void IsWithinLimit_Returns_True_When_Size_Is_At_Or_Below_Max()
    {
        var validator = CreateValidator(1_000);

        Assert.True(validator.IsWithinLimit(1_000));
        Assert.True(validator.IsWithinLimit(1));
    }

    [Fact]
    public void IsWithinLimit_Returns_False_When_Size_Exceeds_Max()
    {
        var validator = CreateValidator(1_000);

        Assert.False(validator.IsWithinLimit(1_001));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void IsWithinLimit_Returns_False_For_Non_Positive_Size(long sizeBytes)
    {
        var validator = CreateValidator(1_000);

        Assert.False(validator.IsWithinLimit(sizeBytes));
    }
}
