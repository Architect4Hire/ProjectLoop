using System.ComponentModel.DataAnnotations;

namespace ProjectLoop.Approvals.Core;

public sealed class OutboxServiceBusOptions
{
    public const string SectionName = "ApprovalsOutboxServiceBus";

    [Required]
    [MinLength(1)]
    public required string EntityName { get; init; }
}
