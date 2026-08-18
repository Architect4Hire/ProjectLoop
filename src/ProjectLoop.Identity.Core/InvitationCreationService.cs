namespace ProjectLoop.Identity.Core;

public sealed class InvitationCreationService : IInvitationCreationService
{
    private static readonly TimeSpan DefaultExpiry = TimeSpan.FromDays(7);

    private readonly IInvitationTokenGenerator _tokenGenerator;
    private readonly IClientInvitationRepository _repository;
    private readonly TimeProvider _timeProvider;

    public InvitationCreationService(
        IInvitationTokenGenerator tokenGenerator,
        IClientInvitationRepository repository,
        TimeProvider? timeProvider = null)
    {
        _tokenGenerator = tokenGenerator;
        _repository = repository;
        _timeProvider = timeProvider ?? TimeProvider.System;
    }

    public async Task<InvitationCreationResult> CreateAsync(
        Guid tenantId,
        string email,
        string invitedByUserId,
        CancellationToken cancellationToken = default)
    {
        var token = _tokenGenerator.Generate();
        var now = _timeProvider.GetUtcNow();

        var invitation = new ClientInvitation
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Email = email,
            TokenHash = token.TokenHash,
            ExpiresAtUtc = now.Add(DefaultExpiry),
            Status = ClientInvitationStatus.Pending,
            InvitedByUserId = invitedByUserId,
            CreatedAtUtc = now,
            UpdatedAtUtc = now,
        };

        await _repository.AddAsync(invitation, cancellationToken);

        return new InvitationCreationResult(invitation, token.RawToken);
    }
}
