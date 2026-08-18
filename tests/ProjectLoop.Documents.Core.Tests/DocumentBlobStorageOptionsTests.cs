using System.ComponentModel.DataAnnotations;
using ProjectLoop.Documents.Core;
using Xunit;

namespace ProjectLoop.Documents.Core.Tests;

public class DocumentBlobStorageOptionsTests
{
    [Fact]
    public void Valid_ContainerName_Passes_Validation()
    {
        var options = new DocumentBlobStorageOptions { ContainerName = "documents" };

        var results = Validate(options);

        Assert.Empty(results);
    }

    [Fact]
    public void Empty_ContainerName_Fails_Validation()
    {
        var options = new DocumentBlobStorageOptions { ContainerName = "" };

        var results = Validate(options);

        Assert.NotEmpty(results);
    }

    private static List<ValidationResult> Validate(DocumentBlobStorageOptions options)
    {
        var context = new ValidationContext(options);
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(options, context, results, validateAllProperties: true);
        return results;
    }
}
