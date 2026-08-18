using Microsoft.EntityFrameworkCore;
using ProjectLoop.Identity.Core;
using Xunit;

namespace ProjectLoop.Identity.Core.Tests;

public class OutboxMessageRepositoryTests
{
    private static IdentityDbContext CreateDbContext() =>
        new(new DbContextOptionsBuilder<IdentityDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private static OutboxMessage CreateMessage(OutboxMessageStatus status, DateTimeOffset createdAtUtc) => new()
    {
        Id = Guid.NewGuid(),
        EventId = Guid.NewGuid(),
        EventType = "ClientUserInvited",
        EventVersion = 1,
        Payload = "{}",
        Status = status,
        CreatedAtUtc = createdAtUtc,
    };

    [Fact]
    public async Task GetPendingBatchAsync_Returns_Only_Pending_Rows_Oldest_First()
    {
        await using var dbContext = CreateDbContext();
        var now = DateTimeOffset.UtcNow;
        var older = CreateMessage(OutboxMessageStatus.Pending, now.AddMinutes(-10));
        var newer = CreateMessage(OutboxMessageStatus.Pending, now.AddMinutes(-1));
        var processed = CreateMessage(OutboxMessageStatus.Processed, now.AddMinutes(-20));
        dbContext.OutboxMessages.AddRange(older, newer, processed);
        await dbContext.SaveChangesAsync();

        var repository = new OutboxMessageRepository(dbContext);
        var batch = await repository.GetPendingBatchAsync(batchSize: 10);

        Assert.Equal(new[] { older.Id, newer.Id }, batch.Select(m => m.Id));
    }

    [Fact]
    public async Task GetPendingBatchAsync_Bounds_The_Requested_Batch_Size()
    {
        await using var dbContext = CreateDbContext();
        var now = DateTimeOffset.UtcNow;
        for (var i = 0; i < 5; i++)
        {
            dbContext.OutboxMessages.Add(CreateMessage(OutboxMessageStatus.Pending, now.AddMinutes(-i)));
        }
        await dbContext.SaveChangesAsync();

        var repository = new OutboxMessageRepository(dbContext);
        var batch = await repository.GetPendingBatchAsync(batchSize: 2);

        Assert.Equal(2, batch.Count);
    }

    [Fact]
    public async Task GetPendingBatchAsync_Clamps_An_Oversized_Batch_Request()
    {
        await using var dbContext = CreateDbContext();

        var repository = new OutboxMessageRepository(dbContext);
        var batch = await repository.GetPendingBatchAsync(batchSize: OutboxMessageRepository.MaxBatchSize + 1000);

        Assert.Empty(batch);
    }

    [Fact]
    public async Task MarkProcessed_Transitions_A_Pending_Row_To_Processed()
    {
        await using var dbContext = CreateDbContext();
        var message = CreateMessage(OutboxMessageStatus.Pending, DateTimeOffset.UtcNow.AddMinutes(-5));
        dbContext.OutboxMessages.Add(message);
        await dbContext.SaveChangesAsync();

        var repository = new OutboxMessageRepository(dbContext);
        var processedAtUtc = DateTimeOffset.UtcNow;
        repository.MarkProcessed(message, processedAtUtc);
        await dbContext.SaveChangesAsync();

        var persisted = await dbContext.OutboxMessages.SingleAsync(m => m.Id == message.Id);
        Assert.Equal(OutboxMessageStatus.Processed, persisted.Status);
        Assert.Equal(processedAtUtc, persisted.ProcessedAtUtc);
    }

    [Fact]
    public async Task MarkProcessed_Leaves_An_Already_Processed_Row_Untouched()
    {
        await using var dbContext = CreateDbContext();
        var originalProcessedAtUtc = DateTimeOffset.UtcNow.AddMinutes(-1);
        var message = CreateMessage(OutboxMessageStatus.Pending, DateTimeOffset.UtcNow.AddMinutes(-5));
        message.Status = OutboxMessageStatus.Processed;
        message.ProcessedAtUtc = originalProcessedAtUtc;
        dbContext.OutboxMessages.Add(message);
        await dbContext.SaveChangesAsync();

        var repository = new OutboxMessageRepository(dbContext);
        repository.MarkProcessed(message, DateTimeOffset.UtcNow);
        await dbContext.SaveChangesAsync();

        var persisted = await dbContext.OutboxMessages.SingleAsync(m => m.Id == message.Id);
        Assert.Equal(originalProcessedAtUtc, persisted.ProcessedAtUtc);
    }
}
