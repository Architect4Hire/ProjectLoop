# Document Storage Design

Use private Azure Blob Storage for binaries and SQL for metadata. Blob keys are opaque and stable; names/titles remain metadata. Authorization occurs in the Documents service before streaming or issuing short-lived access. Record content hash and enforce upload size/type policy. Malware scanning/quarantine is a designed state transition, not an ad-hoc callback.
