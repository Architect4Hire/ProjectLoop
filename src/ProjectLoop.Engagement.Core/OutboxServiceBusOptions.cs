using System.ComponentModel.DataAnnotations;

namespace ProjectLoop.Engagement.Core;

public sealed class OutboxServiceBusOptions
{
    public const string SectionName = "EngagementOutboxServiceBus";

    [Required]
    [MinLength(1)]
    public required string EntityName { get; init; }
}
