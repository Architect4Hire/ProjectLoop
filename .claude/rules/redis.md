# Redis Rule

Redis is cache/coordination only. Namespace keys by domain/tenant/version, define TTL/invalidation, and preserve correctness after eviction.

## Required checks
- Follow the root `CLAUDE.md`.
- Preserve service ownership.
- Add or update tests for changed behavior.
- If a change establishes a new architectural convention, create/update an ADR first.
