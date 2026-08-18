using Microsoft.EntityFrameworkCore;
using ProjectLoop.Engagement.Core;
using Xunit;

namespace ProjectLoop.Engagement.Core.Tests;

public class OutboxRelayTests
{
    private sealed class FakeOutboxMessagePublisher : IOutboxMessagePublisher
    {
        private readonly HashSet<Guid> _failFor;

        public FakeOutboxMessagePublisher(IEnumerable<Guid>? failFor = null)
        {
            _failFor = failFor is null ? new HashSet<Guid>() : new HashSet<Guid>(failFor);
        }

        public List<Guid> PublishedEventIds { get; } = new();

        public Task PublishAsync(OutboxMessage message, CancellationToken cancellationToken = default)
        {
            if (_failFor.Contains(message.EventId))
            {
                throw new InvalidOperationException("Simulated broker failure.");
            }

            PublishedEventIds.Add(message.EventId);
            return Task.CompletedTask;
        }
    }

    private static EngagementDbContext CreateDbContext() =>
        new(new DbContextOptionsBuilder<EngagementDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private static OutboxMessage CreateMessage(DateTimeOffset createdAtUtc) => new()
    {
        Id = Guid.NewGuid(),
        EventId = Guid.NewGuid(),
        EventType = "MilestoneCompleted",
        EventVersion = 1,
        Payload = "{}",
        Status = OutboxMessageStatus.Pending,
        CreatedAtUtc = createdAtUtc,
    };

    [Fact]
    public async Task RelayAsync_Marks_A_Successfully_Published_Message_Processed()
    {
        await using var dbContext = CreateDbContext();
        var message = CreateMessage(DateTimeOffset.UtcNow.AddMinutes(-1));
        dbContext.OutboxMessages.Add(message);
        await dbContext.SaveChangesAsync();

        var publisher = new FakeOutboxMessagePublisher();
        var relay = new OutboxRelay(dbContext, new OutboxMessageRepository(dbContext), publisher);

        await relay.RelayAsync();

        var persisted = await dbContext.OutboxMessages.SingleAsync(m => m.Id == message.Id);
        Assert.Equal(OutboxMessageStatus.Processed, persisted.Status);
        Assert.NotNull(persisted.ProcessedAtUtc);
        Assert.Contains(message.EventId, publisher.PublishedEventIds);
    }

    [Fact]
    public async Task RelayAsync_Isolates_A_Failed_Publish_From_The_Rest_Of_The_Batch()
    {
        await using var dbContext = CreateDbContext();
        var willFail = CreateMessage(DateTimeOffset.UtcNow.AddMinutes(-2));
        var willSucceed = CreateMessage(DateTimeOffset.UtcNow.AddMinutes(-1));
        dbContext.OutboxMessages.AddRange(willFail, willSucceed);
        await dbContext.SaveChangesAsync();

        var publisher = new FakeOutboxMessagePublisher(failFor: new[] { willFail.EventId });
        var relay = new OutboxRelay(dbContext, new OutboxMessageRepository(dbContext), publisher);

        await relay.RelayAsync();

        var failedRow = await dbContext.OutboxMessages.SingleAsync(m => m.Id == willFail.Id);
        var succeededRow = await dbContext.OutboxMessages.SingleAsync(m => m.Id == willSucceed.Id);

        Assert.Equal(OutboxMessageStatus.Pending, failedRow.Status);
        Assert.Equal(1, failedRow.AttemptCount);
        Assert.False(string.IsNullOrEmpty(failedRow.LastError));
        Assert.Equal(OutboxMessageStatus.Processed, succeededRow.Status);
    }

    [Fact]
    public async Task RelayAsync_Is_Safe_To_Rerun_And_Only_Retries_What_Is_Still_Pending()
    {
        await using var dbContext = CreateDbContext();
        var willFailOnce = CreateMessage(DateTimeOffset.UtcNow.AddMinutes(-2));
        var willSucceed = CreateMessage(DateTimeOffset.UtcNow.AddMinutes(-1));
        dbContext.OutboxMessages.AddRange(willFailOnce, willSucceed);
        await dbContext.SaveChangesAsync();

        var firstPassPublisher = new FakeOutboxMessagePublisher(failFor: new[] { willFailOnce.EventId });
        var relay = new OutboxRelay(dbContext, new OutboxMessageRepository(dbContext), firstPassPublisher);
        await relay.RelayAsync();

        var secondPassPublisher = new FakeOutboxMessagePublisher();
        var relayAgain = new OutboxRelay(dbContext, new OutboxMessageRepository(dbContext), secondPassPublisher);
        await relayAgain.RelayAsync();

        var retriedRow = await dbContext.OutboxMessages.SingleAsync(m => m.Id == willFailOnce.Id);
        Assert.Equal(OutboxMessageStatus.Processed, retriedRow.Status);
        Assert.Equal(1, retriedRow.AttemptCount);
        Assert.Single(secondPassPublisher.PublishedEventIds);
        Assert.Contains(willFailOnce.EventId, secondPassPublisher.PublishedEventIds);
    }
}
