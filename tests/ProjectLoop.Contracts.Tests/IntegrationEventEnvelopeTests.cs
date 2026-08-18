using System.Text.Json;
using ProjectLoop.Contracts;
using Xunit;

namespace ProjectLoop.Contracts.Tests;

public class IntegrationEventEnvelopeTests
{
    private sealed record SamplePayload(string Value);

    [Fact]
    public void Envelope_Round_Trips_Through_Json()
    {
        var envelope = new IntegrationEventEnvelope<SamplePayload>(
            EventId: Guid.NewGuid(),
            EventType: "SampleEvent",
            EventVersion: 1,
            OccurredAtUtc: DateTimeOffset.UtcNow,
            TenantId: Guid.NewGuid(),
            CorrelationId: "correlation-1",
            CausationId: "causation-1",
            TraceParent: "00-4bf92f3577b34da6a3ce929d0e0e4736-00f067aa0ba902b7-01",
            Data: new SamplePayload("hello"));

        var json = JsonSerializer.Serialize(envelope);
        var roundTripped = JsonSerializer.Deserialize<IntegrationEventEnvelope<SamplePayload>>(json);

        Assert.NotNull(roundTripped);
        Assert.Equal(envelope, roundTripped);
    }

    [Fact]
    public void Envelope_Allows_Null_Correlation_And_Trace_Metadata()
    {
        var envelope = new IntegrationEventEnvelope<SamplePayload>(
            EventId: Guid.NewGuid(),
            EventType: "SampleEvent",
            EventVersion: 1,
            OccurredAtUtc: DateTimeOffset.UtcNow,
            TenantId: null,
            CorrelationId: null,
            CausationId: null,
            TraceParent: null,
            Data: new SamplePayload("hello"));

        var json = JsonSerializer.Serialize(envelope);
        var roundTripped = JsonSerializer.Deserialize<IntegrationEventEnvelope<SamplePayload>>(json);

        Assert.NotNull(roundTripped);
        Assert.Null(roundTripped!.CorrelationId);
        Assert.Null(roundTripped.TraceParent);
    }
}
