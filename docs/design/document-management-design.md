# Document Management Design

A Document has TenantId, ProjectId, Type, Title, Description, Status, Visibility, CurrentVersionId, CreatedBy/At and PublishedAt. DocumentVersion has immutable identity, version number, blob key, MIME type, size, hash, uploader/time and change description. Lifecycle covers draft, quarantine/scan, available, published, superseded and retained/deleted states as policy requires.
