# Document Binary Storage

- **Status:** Accepted
- **Date:** 2026-08-16

## Context
Project Loop requires an explicit durable architecture decision for this concern.

## Decision
Store document binaries in private Azure Blob Storage; never use permanent public blob URLs as access control.

## Consequences
- Implementation and review rules must follow this decision.
- Deviations require a new/superseding ADR.
