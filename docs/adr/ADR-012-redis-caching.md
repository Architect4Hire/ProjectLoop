# Redis Caching

- **Status:** Accepted
- **Date:** 2026-08-16

## Context
Project Loop requires an explicit durable architecture decision for this concern.

## Decision
Use Redis only for optimization/coordination. SQL/service state remains authoritative and correctness must survive cache eviction.

## Consequences
- Implementation and review rules must follow this decision.
- Deviations require a new/superseding ADR.
