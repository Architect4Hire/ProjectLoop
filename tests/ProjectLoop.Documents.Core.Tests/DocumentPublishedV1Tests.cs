using System.Reflection;
using System.Text.Json;
using ProjectLoop.Contracts;
using ProjectLoop.Documents.Core;
using Xunit;

namespace ProjectLoop.Documents.Core.Tests;

public class DocumentPublishedV1Tests
{
    private static DocumentPublishedV1 CreateEvent() => new(
        DocumentId: Guid.NewGuid(),
        DocumentVersionId: Guid.NewGuid(),
        VersionNumber: 2,
        ProjectId: Guid.NewGuid(),
        TenantId: Guid.NewGuid(),
        Category: "Contract",
        Visibility: DocumentVisibility.Client,
        PublishedAtUtc: DateTimeOffset.UtcNow);

    [Fact]
    public void DocumentPublishedV1_Round_Trips_Through_Envelope_Json()
    {
        var envelope = new IntegrationEventEnvelope<DocumentPublishedV1>(
            EventId: Guid.NewGuid(),
            EventType: "DocumentPublished",
            EventVersion: 1,
            OccurredAtUtc: DateTimeOffset.UtcNow,
            TenantId: Guid.NewGuid(),
            CorrelationId: "correlation-1",
            CausationId: null,
            TraceParent: null,
            Data: CreateEvent());

        var json = JsonSerializer.Serialize(envelope);
        var roundTripped = JsonSerializer.Deserialize<IntegrationEventEnvelope<DocumentPublishedV1>>(json);

        Assert.NotNull(roundTripped);
        Assert.Equal(envelope, roundTripped);
    }

    [Fact]
    public void DocumentPublishedV1_Never_Exposes_A_Blob_Key_Or_Credential()
    {
        var propertyNames = typeof(DocumentPublishedV1)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(p => p.Name);

        Assert.DoesNotContain(propertyNames, name =>
            name.Contains("Blob", StringComparison.OrdinalIgnoreCase) ||
            name.Contains("Url", StringComparison.OrdinalIgnoreCase) ||
            name.Contains("Uri", StringComparison.OrdinalIgnoreCase) ||
            name.Contains("Sas", StringComparison.OrdinalIgnoreCase));
    }
}
