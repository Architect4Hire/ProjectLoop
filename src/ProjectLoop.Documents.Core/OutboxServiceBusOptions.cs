using System.ComponentModel.DataAnnotations;

namespace ProjectLoop.Documents.Core;

public sealed class OutboxServiceBusOptions
{
    public const string SectionName = "DocumentsOutboxServiceBus";

    [Required]
    [MinLength(1)]
    public required string EntityName { get; init; }
}
