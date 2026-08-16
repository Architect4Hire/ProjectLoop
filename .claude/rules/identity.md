# Identity Rule

ASP.NET Core Identity owns authentication primitives. Tenant membership and project access are application concerns. Business domains reference stable user IDs only.

## Required checks
- Follow the root `CLAUDE.md`.
- Preserve service ownership.
- Add or update tests for changed behavior.
- If a change establishes a new architectural convention, create/update an ADR first.
