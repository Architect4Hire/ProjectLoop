namespace ProjectLoop.Contracts;

/// <summary>
/// The outer shape every integration event is published in. TraceParent
/// carries W3C trace-context (traceparent header format) so a consumer can
/// resume the same distributed trace the producing operation started.
/// CorrelationId is the business-level id used to relate a chain of
/// requests/events across services independent of the tracing backend.
/// </summary>
public sealed record IntegrationEventEnvelope<TData>(
    Guid EventId,
    string EventType,
    int EventVersion,
    DateTimeOffset OccurredAtUtc,
    Guid? TenantId,
    string? CorrelationId,
    string? CausationId,
    string? TraceParent,
    TData Data)
    where TData : notnull;
