# Multi-Tenancy Rule

Tenant isolation is mandatory on reads, writes, caches, blobs, events and audit. Never trust route/body TenantId as proof of access.

## Required checks
- Follow the root `CLAUDE.md`.
- Preserve service ownership.
- Add or update tests for changed behavior.
- If a change establishes a new architectural convention, create/update an ADR first.
