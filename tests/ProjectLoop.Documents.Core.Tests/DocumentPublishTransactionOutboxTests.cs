using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using ProjectLoop.Contracts;
using ProjectLoop.Documents.Core;
using Xunit;

namespace ProjectLoop.Documents.Core.Tests;

public class DocumentPublishTransactionOutboxTests
{
    private static DocumentsDbContext CreateDbContext() =>
        new(new DbContextOptionsBuilder<DocumentsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private static (Document Document, DocumentVersion Version) CreateEligibleDraft()
    {
        var now = DateTimeOffset.UtcNow;
        var document = new Document
        {
            Id = Guid.NewGuid(),
            TenantId = Guid.NewGuid(),
            ProjectId = Guid.NewGuid(),
            Title = "Statement of Work",
            Category = "Contract",
            Status = DocumentStatus.Draft,
            Visibility = DocumentVisibility.Internal,
            CreatedAtUtc = now,
            UpdatedAtUtc = now,
        };
        var version = new DocumentVersion
        {
            Id = Guid.NewGuid(),
            DocumentId = document.Id,
            VersionNumber = 1,
            BlobObjectKey = Guid.NewGuid().ToString("n"),
            MimeType = "application/pdf",
            SizeBytes = 4096,
            ContentHash = new string('a', 64),
            UploadedByUserId = "user-1",
            CreatedAtUtc = now,
        };

        return (document, version);
    }

    [Fact]
    public async Task ExecuteAsync_Commits_Publication_State_And_Outbox_Row_In_The_Same_SaveChanges()
    {
        await using var dbContext = CreateDbContext();
        var (document, version) = CreateEligibleDraft();
        dbContext.Documents.Add(document);
        dbContext.DocumentVersions.Add(version);
        await dbContext.SaveChangesAsync();

        var transaction = new DocumentPublishTransaction(
            dbContext,
            new DocumentRepository(dbContext),
            new DocumentVersionRepository(dbContext));

        var publishedAtUtc = DateTimeOffset.UtcNow;
        await transaction.ExecuteAsync(document, version, publishedAtUtc);

        var persistedDocument = await dbContext.Documents.SingleAsync(d => d.Id == document.Id);
        var outboxRow = await dbContext.OutboxMessages.SingleAsync(m => m.EventType == "DocumentPublished");

        Assert.Equal(DocumentStatus.Published, persistedDocument.Status);
        Assert.Equal(OutboxMessageStatus.Pending, outboxRow.Status);
        Assert.Equal(1, outboxRow.EventVersion);

        var envelope = JsonSerializer.Deserialize<IntegrationEventEnvelope<DocumentPublishedV1>>(outboxRow.Payload);
        Assert.NotNull(envelope);
        Assert.Equal(document.Id, envelope!.Data.DocumentId);
        Assert.Equal(version.Id, envelope.Data.DocumentVersionId);
        Assert.Equal(document.TenantId, envelope.TenantId);
    }
}
