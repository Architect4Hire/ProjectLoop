# Project Loop Design System Bootstrap

## SCRUB-DS-001 — Establish local design-system source
**Scope:** Populate `src/web/design-system/` from the approved Angular/Tailwind design-system source.
**Constraints:** Preserve Angular 22, Tailwind, accessibility and local ownership.
**Restrictions:** Do not build feature pages. Do not fork duplicate components into feature folders.
**Usage:** Treat this folder as the UI source of truth referenced by `.claude/rules/design-system.md`.
**Behavior:** Verify build, imports, tokens, recipes and examples before feature work begins.

## SCRUB-DS-002 — Add Project Loop portal recipes
Add portal-specific recipes only after inventorying existing primitives: project health badges, document rows/cards, version chips, approval callouts/actions, audit timeline, dashboard metric cards and empty/loading/error states.
