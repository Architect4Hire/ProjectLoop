# Audit Strategy

- **Status:** Accepted
- **Date:** 2026-08-16

## Context
Project Loop requires an explicit durable architecture decision for this concern.

## Decision
Maintain explicit append-oriented business/security audit records separate from diagnostic application logs.

## Consequences
- Implementation and review rules must follow this decision.
- Deviations require a new/superseding ADR.
