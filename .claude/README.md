# Project Loop Claude Toolkit

This folder converts the root `CLAUDE.md` constitution into enforceable guidance.

- `rules/` — concern-specific invariants and prohibited shortcuts.
- `skills/` — repeatable implementation recipes Claude should follow.
- `agents/` — read-only reviewers focused on a particular risk area.
- `hooks/` — deterministic guardrails and quality checks.

Use the root constitution first, then the narrowest applicable rule and skill. Agents review; they do not redesign the system silently.
