# Dashboard Composition

- **Status:** Accepted
- **Date:** 2026-08-16

## Context
Project Loop requires an explicit durable architecture decision for this concern.

## Decision
Initially compose dashboard data from domain read APIs. Introduce a projection/read-model service only when measured latency/fan-out justifies it.

## Consequences
- Implementation and review rules must follow this decision.
- Deviations require a new/superseding ADR.
