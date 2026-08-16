# Document Versioning

- **Status:** Accepted
- **Date:** 2026-08-16

## Context
Project Loop requires an explicit durable architecture decision for this concern.

## Decision
Document versions are immutable once published/approved. New content creates a new version and approvals always target an exact version.

## Consequences
- Implementation and review rules must follow this decision.
- Deviations require a new/superseding ADR.
