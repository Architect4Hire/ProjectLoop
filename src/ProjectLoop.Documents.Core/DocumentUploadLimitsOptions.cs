using System.ComponentModel.DataAnnotations;

namespace ProjectLoop.Documents.Core;

public sealed class DocumentUploadLimitsOptions
{
    public const string SectionName = "DocumentUploadLimits";

    [Range(1, long.MaxValue)]
    public long MaxSizeBytes { get; init; } = 104_857_600; // 100 MiB default.
}
