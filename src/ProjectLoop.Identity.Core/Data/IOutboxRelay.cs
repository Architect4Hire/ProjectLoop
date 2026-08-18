namespace ProjectLoop.Identity.Core;

/// <summary>
/// Runs one bounded relay pass over this service's pending outbox rows.
/// Safe to invoke repeatedly — every pass is a fresh at-least-once attempt.
/// </summary>
public interface IOutboxRelay
{
    Task RelayAsync(CancellationToken cancellationToken = default);
}
