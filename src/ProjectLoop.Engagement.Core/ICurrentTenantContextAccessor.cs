namespace ProjectLoop.Engagement.Core;

/// <summary>
/// Holds the <see cref="ITenantContext"/> established for the current
/// request by transport-layer plumbing after resolver validation.
/// </summary>
public interface ICurrentTenantContextAccessor
{
    ITenantContext? Current { get; set; }
}
