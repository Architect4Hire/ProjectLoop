# Document Versioning Design

Versions are append-only. Published or approved versions cannot be replaced. Updating content creates a new DocumentVersion and moves CurrentVersionId. Historical approvals remain attached to the exact previous version.
