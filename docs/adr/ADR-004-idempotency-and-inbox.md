# Idempotency and Inbox

- **Status:** Accepted
- **Date:** 2026-08-16

## Context
Project Loop requires an explicit durable architecture decision for this concern.

## Decision
Every asynchronous consumer is idempotent. Use a transactional inbox when durable side effects need atomic duplicate protection.

## Consequences
- Implementation and review rules must follow this decision.
- Deviations require a new/superseding ADR.
