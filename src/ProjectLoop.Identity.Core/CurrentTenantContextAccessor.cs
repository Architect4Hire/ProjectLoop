namespace ProjectLoop.Identity.Core;

public sealed class CurrentTenantContextAccessor : ICurrentTenantContextAccessor
{
    public ITenantContext? Current { get; set; }
}
