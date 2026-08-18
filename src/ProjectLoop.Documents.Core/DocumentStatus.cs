namespace ProjectLoop.Documents.Core;

/// <summary>
/// The lifecycle/publication state of a Document's current version. Once a
/// document reaches Published or Approved, its current DocumentVersion is
/// immutable: replacing content creates a new version rather than mutating
/// the existing one.
/// </summary>
public enum DocumentStatus
{
    Draft,
    Published,
    Approved,
    Archived,
}
