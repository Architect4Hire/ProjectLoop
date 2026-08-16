# Database Rule

One SQL database per bounded service. EF models are internal. No cross-database joins. Transactions are local. Schema changes use migrations and preserve tenant indexes.

## Required checks
- Follow the root `CLAUDE.md`.
- Preserve service ownership.
- Add or update tests for changed behavior.
- If a change establishes a new architectural convention, create/update an ADR first.
