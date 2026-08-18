namespace ProjectLoop.Documents.Core;

/// <summary>
/// Owns the transaction boundary for persisting a publication-eligible
/// DocumentVersion's publication state together with the owning Document's
/// Status transition. Assumes the caller has already confirmed eligibility
/// via <see cref="DocumentPublicationEligibility"/> — this transaction only
/// persists, it does not decide.
/// </summary>
public interface IDocumentPublishTransaction
{
    Task ExecuteAsync(
        Document document,
        DocumentVersion version,
        DateTimeOffset publishedAtUtc,
        CancellationToken cancellationToken = default);
}
