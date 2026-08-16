# Notifications Rule

Notification delivery is asynchronous and independently retryable. Notification failure must not roll back approvals, document publication, or milestone state.

## Required checks
- Follow the root `CLAUDE.md`.
- Preserve service ownership.
- Add or update tests for changed behavior.
- If a change establishes a new architectural convention, create/update an ADR first.
