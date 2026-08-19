namespace ProjectLoop.Audit.Core;

/// <summary>
/// Service-owned persistence operations for AuditRecord. Deliberately
/// exposes no Update or Delete/Remove member: audit history is append-only,
/// so the sanctioned application access path to AuditDb has no way to
/// mutate or destroy a previously written record.
/// </summary>
public interface IAuditRecordRepository
{
    Task AddAsync(AuditRecord record, CancellationToken cancellationToken = default);

    Task<AuditRecord?> GetByIdAsync(Guid tenantId, Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AuditRecord>> GetByResourceAsync(
        Guid tenantId,
        string resourceType,
        Guid resourceId,
        CancellationToken cancellationToken = default);
}
