# Architecture Rule

Preserve bounded service ownership, local transactions, HTTP for immediacy, Service Bus for temporal decoupling, and ADR-driven changes. Never create distributed transactions or cross-service database reads.

## Required checks
- Follow the root `CLAUDE.md`.
- Preserve service ownership.
- Add or update tests for changed behavior.
- If a change establishes a new architectural convention, create/update an ADR first.
