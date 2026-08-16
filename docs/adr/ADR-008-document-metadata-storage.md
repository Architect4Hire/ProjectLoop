# Document Metadata Storage

- **Status:** Accepted
- **Date:** 2026-08-16

## Context
Project Loop requires an explicit durable architecture decision for this concern.

## Decision
Store document identity, tenant/project ownership, lifecycle, visibility, version links, hashes and blob references in the Documents service SQL database.

## Consequences
- Implementation and review rules must follow this decision.
- Deviations require a new/superseding ADR.
