using Microsoft.EntityFrameworkCore;

namespace ProjectLoop.Audit.Core;

/// <summary>
/// EF-backed implementation of the append-only AuditRecord repository. Reads
/// are always AsNoTracking so a caller can never mutate a fetched record and
/// have it silently persisted; the only write path is AddAsync.
/// </summary>
public sealed class AuditRecordRepository : IAuditRecordRepository
{
    private readonly AuditDbContext _dbContext;

    public AuditRecordRepository(AuditDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task AddAsync(AuditRecord record, CancellationToken cancellationToken = default)
    {
        // Stages the insert only. The owning Data transaction commits this
        // together with the inbox completion record in a single
        // SaveChangesAsync call so the two can never diverge.
        _dbContext.AuditRecords.Add(record);
        return Task.CompletedTask;
    }

    public Task<AuditRecord?> GetByIdAsync(Guid tenantId, Guid id, CancellationToken cancellationToken = default) =>
        _dbContext.AuditRecords
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.TenantId == tenantId && r.Id == id, cancellationToken);

    public async Task<IReadOnlyList<AuditRecord>> GetByResourceAsync(
        Guid tenantId,
        string resourceType,
        Guid resourceId,
        CancellationToken cancellationToken = default) =>
        await _dbContext.AuditRecords
            .AsNoTracking()
            .Where(r => r.TenantId == tenantId && r.ResourceType == resourceType && r.ResourceId == resourceId)
            .OrderBy(r => r.OccurredAtUtc)
            .ToListAsync(cancellationToken);
}
