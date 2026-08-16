# Transactional Outbox

- **Status:** Accepted
- **Date:** 2026-08-16

## Context
Project Loop requires an explicit durable architecture decision for this concern.

## Decision
When durable state and an integration event must succeed atomically, store the event in the same SQL transaction and publish asynchronously from an outbox relay.

## Consequences
- Implementation and review rules must follow this decision.
- Deviations require a new/superseding ADR.
