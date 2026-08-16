# Approval Model

- **Status:** Accepted
- **Date:** 2026-08-16

## Context
Project Loop requires an explicit durable architecture decision for this concern.

## Decision
Use authenticated business approvals with immutable decision history, timestamps, comments and audit metadata; do not model legal e-signature semantics.

## Consequences
- Implementation and review rules must follow this decision.
- Deviations require a new/superseding ADR.
