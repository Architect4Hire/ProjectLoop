namespace ProjectLoop.Engagement.Core;

public sealed class CurrentTenantContextAccessor : ICurrentTenantContextAccessor
{
    public ITenantContext? Current { get; set; }
}
