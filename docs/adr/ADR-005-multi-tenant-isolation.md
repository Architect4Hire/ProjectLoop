# Multi-Tenant Isolation

- **Status:** Accepted
- **Date:** 2026-08-16

## Context
Project Loop requires an explicit durable architecture decision for this concern.

## Decision
Tenant access is derived from authenticated membership and enforced server-side on every owned resource; client-supplied TenantId is never sufficient authorization.

## Consequences
- Implementation and review rules must follow this decision.
- Deviations require a new/superseding ADR.
