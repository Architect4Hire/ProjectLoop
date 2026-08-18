using Microsoft.EntityFrameworkCore;

namespace ProjectLoop.Identity.Core;

public sealed class OutboxMessageRepository : IOutboxMessageRepository
{
    public const int MaxBatchSize = 100;

    private readonly IdentityDbContext _dbContext;

    public OutboxMessageRepository(IdentityDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<OutboxMessage>> GetPendingBatchAsync(int batchSize, CancellationToken cancellationToken = default)
    {
        var bounded = Math.Clamp(batchSize, 1, MaxBatchSize);

        return await _dbContext.OutboxMessages
            .Where(m => m.Status == OutboxMessageStatus.Pending)
            .OrderBy(m => m.CreatedAtUtc)
            .Take(bounded)
            .ToListAsync(cancellationToken);
    }

    public void MarkProcessed(OutboxMessage message, DateTimeOffset processedAtUtc)
    {
        if (_dbContext.Entry(message).State == EntityState.Detached)
        {
            _dbContext.OutboxMessages.Attach(message);
        }

        if (message.Status != OutboxMessageStatus.Pending)
        {
            return;
        }

        message.Status = OutboxMessageStatus.Processed;
        message.ProcessedAtUtc = processedAtUtc;
        message.LastAttemptedAtUtc = processedAtUtc;
    }
}
