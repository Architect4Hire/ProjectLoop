using System.ComponentModel.DataAnnotations;

namespace ProjectLoop.Identity.Core;

public sealed class OutboxServiceBusOptions
{
    public const string SectionName = "IdentityOutboxServiceBus";

    [Required]
    [MinLength(1)]
    public required string EntityName { get; init; }
}
