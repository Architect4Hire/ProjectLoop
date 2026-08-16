# ASP.NET Core Identity

- **Status:** Accepted
- **Date:** 2026-08-16

## Context
Project Loop requires an explicit durable architecture decision for this concern.

## Decision
Use ASP.NET Core Identity for application authentication and account lifecycle. Optional external providers may be associated later without changing business authorization semantics.

## Consequences
- Implementation and review rules must follow this decision.
- Deviations require a new/superseding ADR.
