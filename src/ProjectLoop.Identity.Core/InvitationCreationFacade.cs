using System.Net.Mail;

namespace ProjectLoop.Identity.Core;

/// <summary>
/// Use-case validation and authorization orchestration for invitation
/// creation. Tenant scope and caller identity come only from the
/// server-established <see cref="ICurrentTenantContextAccessor"/>, never
/// from request payload values.
/// </summary>
public sealed class InvitationCreationFacade : IInvitationCreationFacade
{
    private readonly ICurrentTenantContextAccessor _tenantContextAccessor;
    private readonly IInvitationCreationService _creationService;

    public InvitationCreationFacade(
        ICurrentTenantContextAccessor tenantContextAccessor,
        IInvitationCreationService creationService)
    {
        _tenantContextAccessor = tenantContextAccessor;
        _creationService = creationService;
    }

    public async Task<InvitationCreationFacadeResult> CreateInvitationAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        var tenantContext = _tenantContextAccessor.Current;
        if (tenantContext is null)
        {
            return InvitationCreationFacadeResult.Failure(InvitationCreationFacadeError.NoTenantContext);
        }

        if (tenantContext.Role != TenantMembershipRole.Admin)
        {
            return InvitationCreationFacadeResult.Failure(InvitationCreationFacadeError.NotAuthorized);
        }

        if (!IsValidEmail(email))
        {
            return InvitationCreationFacadeResult.Failure(InvitationCreationFacadeError.InvalidEmail);
        }

        var result = await _creationService.CreateAsync(
            tenantContext.TenantId,
            email,
            tenantContext.UserId,
            cancellationToken);

        return InvitationCreationFacadeResult.Success(result);
    }

    private static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        try
        {
            _ = new MailAddress(email);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
