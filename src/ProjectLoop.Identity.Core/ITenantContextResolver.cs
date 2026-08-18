namespace ProjectLoop.Identity.Core;

/// <summary>
/// Resolves an allowed <see cref="ITenantContext"/> for an authenticated user
/// against persisted tenant membership. A requested tenant identifier is only
/// ever a lookup key here, never a trusted authorization claim on its own.
/// </summary>
public interface ITenantContextResolver
{
    Task<TenantContextResolution> ResolveAsync(string userId, Guid requestedTenantId, CancellationToken cancellationToken = default);
}
