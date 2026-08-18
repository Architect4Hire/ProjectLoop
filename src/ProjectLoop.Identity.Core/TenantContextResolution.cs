namespace ProjectLoop.Identity.Core;

public enum TenantContextDenialReason
{
    MembershipNotFound,
    MembershipNotActive,
    TenantNotFound,
    TenantNotActive,
}

public sealed class TenantContextResolution
{
    private TenantContextResolution(bool isAllowed, ITenantContext? context, TenantContextDenialReason? denialReason)
    {
        IsAllowed = isAllowed;
        Context = context;
        DenialReason = denialReason;
    }

    public bool IsAllowed { get; }

    public ITenantContext? Context { get; }

    public TenantContextDenialReason? DenialReason { get; }

    public static TenantContextResolution Allowed(ITenantContext context) => new(true, context, null);

    public static TenantContextResolution Denied(TenantContextDenialReason reason) => new(false, null, reason);
}
