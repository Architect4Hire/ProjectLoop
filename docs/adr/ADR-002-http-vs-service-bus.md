# HTTP vs Azure Service Bus

- **Status:** Accepted
- **Date:** 2026-08-16

## Context
Project Loop requires an explicit durable architecture decision for this concern.

## Decision
Use HTTP when the caller needs the answer now; use Service Bus for facts, fan-out, retryable independent work, temporal decoupling and durable workflow progression.

## Consequences
- Implementation and review rules must follow this decision.
- Deviations require a new/superseding ADR.
