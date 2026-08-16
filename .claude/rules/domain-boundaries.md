# Domain Boundaries Rule

Treat Identity/Tenant, Engagement, Documents, Approvals, Commercial Read Model, Notification, and Audit as proposed boundaries. Do not merge or split them without ADR evidence.

## Required checks
- Follow the root `CLAUDE.md`.
- Preserve service ownership.
- Add or update tests for changed behavior.
- If a change establishes a new architectural convention, create/update an ADR first.
