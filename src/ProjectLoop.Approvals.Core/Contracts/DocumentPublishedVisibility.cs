namespace ProjectLoop.Approvals.Core;

/// <summary>
/// Approvals' own consumer-side mirror of the Documents service's
/// client-facing visibility values carried on the DocumentPublished
/// integration event. This is not a shared CLR type — Approvals does not
/// reference ProjectLoop.Documents.Core — so these ordinal values are a
/// versioned assumption of the DocumentPublished v1 wire contract rather
/// than something the compiler keeps aligned automatically.
/// </summary>
public enum DocumentPublishedVisibility
{
    Internal = 0,
    Client = 1,
}
