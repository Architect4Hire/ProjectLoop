namespace ProjectLoop.Documents.Core;

/// <summary>
/// One bounded relay pass: publish everything currently Pending, mark each
/// success, and leave failures Pending with bounded failure metadata for
/// the next pass to retry. A message that keeps failing never blocks the
/// rest of the batch — each publish attempt is isolated.
/// </summary>
public sealed class OutboxRelay : IOutboxRelay
{
    private const int BatchSize = 20;
    private const int MaxErrorLength = 1024;

    private readonly DocumentsDbContext _dbContext;
    private readonly IOutboxMessageRepository _outboxMessageRepository;
    private readonly IOutboxMessagePublisher _outboxMessagePublisher;
    private readonly TimeProvider _timeProvider;

    public OutboxRelay(
        DocumentsDbContext dbContext,
        IOutboxMessageRepository outboxMessageRepository,
        IOutboxMessagePublisher outboxMessagePublisher,
        TimeProvider? timeProvider = null)
    {
        _dbContext = dbContext;
        _outboxMessageRepository = outboxMessageRepository;
        _outboxMessagePublisher = outboxMessagePublisher;
        _timeProvider = timeProvider ?? TimeProvider.System;
    }

    public async Task RelayAsync(CancellationToken cancellationToken = default)
    {
        var pending = await _outboxMessageRepository.GetPendingBatchAsync(BatchSize, cancellationToken);

        foreach (var message in pending)
        {
            var attemptedAtUtc = _timeProvider.GetUtcNow();

            try
            {
                await _outboxMessagePublisher.PublishAsync(message, cancellationToken);
                _outboxMessageRepository.MarkProcessed(message, attemptedAtUtc);
            }
            catch (Exception ex)
            {
                message.AttemptCount++;
                message.LastAttemptedAtUtc = attemptedAtUtc;
                message.LastError = Truncate(ex.Message);
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private static string Truncate(string? message)
    {
        message ??= string.Empty;
        return message.Length <= MaxErrorLength ? message : message[..MaxErrorLength];
    }
}
