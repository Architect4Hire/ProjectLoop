using System.ComponentModel.DataAnnotations;

namespace ProjectLoop.Documents.Core;

public sealed class DocumentBlobStorageOptions
{
    public const string SectionName = "DocumentBlobStorage";

    [Required]
    [MinLength(3)]
    public required string ContainerName { get; init; }
}
