# API Design Rule

Design resource-oriented APIs, explicit contracts, ProblemDetails errors, pagination for collections, idempotency where retried commands matter, and authorization at the owning service.

## Required checks
- Follow the root `CLAUDE.md`.
- Preserve service ownership.
- Add or update tests for changed behavior.
- If a change establishes a new architectural convention, create/update an ADR first.
