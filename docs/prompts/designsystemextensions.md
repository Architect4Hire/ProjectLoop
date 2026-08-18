# Project Loop — Lake Shore Drive Design-System Extensions SCRUB Prompts

## Purpose

This sequence extends the checked-in Lake Shore Drive-derived design system so
future Project Loop Angular components can consume stable, accessible, tested
controls and compositions. It is intentionally separate from
`project-loop-scrub-microprompts.md`.

Execute these prompts in order. Each prompt changes one coherent seam, runs its
smallest relevant verification, reports changed files and results, and then
stops. Before implementing a prompt, search the current repository. If its
contract is already satisfied, report `SKIPPED — ALREADY SATISFIED` with
evidence and stop without recreating it.

## Global rules

- Read `CLAUDE.md`, `.claude/rules/angular.md`,
  `.claude/rules/design-system.md`, and
  `.claude/skills/add-design-system-component/SKILL.md` before implementation.
- Keep `src/web/design-system/` as the only production design-system source.
- Treat the Lake Shore Drive DS-000 through DS-077 specification as the shared
  baseline and Project Loop requirements as the authority for extensions.
- Use Angular 22 standalone APIs, signals, `OnPush`, strict templates, built-in
  control flow, semantic HTML, and existing semantic tokens.
- Export supported APIs through their layer barrel and
  `src/web/design-system/public-api.ts` only.
- Application and fixture consumers import `@lsd/design-system`; deep imports
  are forbidden.
- Keep routing, authorization, API access, persistence, tenant context, and
  business state outside the design system.
- Do not introduce a second token system, third-party component framework,
  feature-local control copy, or direct production dependency on the private
  migration snapshot.
- Do not update visual baselines until the implementation difference has been
  reviewed and accepted.

# Phase 0 — Establish executable baseline

## Prompt DSE-000 — Reconcile the Lake Shore Drive baseline

```text
SCOPE: Map Lake Shore Drive DS-000 through DS-077 to current Project Loop tokens, foundations, primitives, components, patterns, recipes, layouts, icons, documentation, and tests.
CONSTRAINT: Record exact implementation evidence and distinguish shared contracts from intentional Project Loop extensions.
RESTRICTION: Do not change production code or infer pixel parity from documentation-only reference material.
USAGE: Lake Shore Drive design-system SCRUB prompts; public API barrels; catalog; integration manifest; parity report.
BEHAVIOR: Publish a pass/gap matrix with exact paths and STOP.
```

## Prompt DSE-001 — Repair the deterministic verification entry points

```text
SCOPE: Make the documented clean-install, design-system build, unit, documentation, boundary, accessibility, responsive, and visual commands executable from the repository root.
CONSTRAINT: Keep commands deterministic on Windows and CI; use the checked-in lockfile and explicit project names.
RESTRICTION: Do not weaken assertions, suppress accessibility rules, or mark failing checks as passed.
USAGE: package.json; angular.json; playwright.config.ts; documentation checkers; visual fixture server.
BEHAVIOR: Run each command once, record exact pass/fail totals and residual blockers, and STOP.
```

## Prompt DSE-002 — Correct Angular 22 zoneless unit-test hosts

```text
SCOPE: Replace stale plain-property host mutations in failing design-system specs with signal-backed state, ComponentRef.setInput where appropriate, or another documented Angular 22 zoneless testing pattern.
CONSTRAINT: Preserve production behavior and test observable contracts through public inputs, outputs, DOM, focus, and accessibility semantics.
RESTRICTION: Do not add zone.js solely for tests, monkey-patch ComponentFixture, or change production APIs to satisfy stale tests.
USAGE: Failing adjacent specs and shared testing utilities only when a utility expresses an official reusable pattern.
BEHAVIOR: Run isolated repaired specs followed by the complete unit suite; require zero failures and STOP.
```

## Prompt DSE-003 — Stabilize the browser fixture server

```text
SCOPE: Make the Playwright web-server wrapper build, bind to its configured address, report startup failures, and shut down reliably on Windows and CI.
CONSTRAINT: Preserve deterministic fixture output and avoid orphaned processes or port reuse ambiguity.
RESTRICTION: Do not bypass the production fixture build or replace browser tests with source-only assertions.
USAGE: visual-regression/serve-fixture.mjs; playwright.config.ts; visual fixture build target.
BEHAVIOR: Run one accessibility spec and one screenshot spec through the configured webServer lifecycle and STOP.
```

# Phase 1 — Prove the application-facing seam

## Prompt DSE-004 — Add a strict public-API consumer fixture

```text
SCOPE: Add one compile-only standalone application consumer importing representative shell, surface, button, input, textarea, select, checkbox, radio, dialog/drawer, state-feedback, document, approval, and audit APIs from @lsd/design-system.
CONSTRAINT: Exercise required inputs, typed generic values, model bindings, outputs, projection slots, and strict template checking with small neutral fixture data.
RESTRICTION: Do not deep-import, add feature behavior, fetch data, or turn the fixture into a demo application.
USAGE: design-system testing fixtures; public alias configuration; tsconfig design-system include list.
BEHAVIOR: Build through the public entry point, prove a deliberately invalid private import is rejected by the boundary self-test, and STOP.
```

## Prompt DSE-005 — Lock the global foundation stylesheet contract

```text
SCOPE: Verify the consuming Angular build loads src/web/design-system/foundations/tailwind.css before application styles and emits the semantic light/dark variables, typography, focus, invalid, disabled, readonly, and reduced-motion rules.
CONSTRAINT: Maintain one global design-system stylesheet entry and Tailwind source discovery only within approved production layers.
RESTRICTION: Do not copy foundation declarations into application CSS or add direct palette values.
USAGE: angular.json; foundations/tailwind.css and imports; postcss.config.mjs; integration manifest.
BEHAVIOR: Build emitted CSS, assert representative semantic variables/rules, and STOP.
```

## Prompt DSE-006 — Enforce future feature consumption

```text
SCOPE: Extend the feature-boundary checker to reject design-system deep imports, duplicated long styling bundles, and feature-local components that duplicate an existing public selector or control role.
CONSTRAINT: Supply passing and failing fixtures for every enforced rule with actionable diagnostics.
RESTRICTION: Do not reject small feature layout styles or business-specific compositions that have no reusable design-system equivalent.
USAGE: check-feature-boundaries.mjs; test-fixtures/feature-boundaries; package and CI scripts.
BEHAVIOR: Run self-tests and the real feature scan; both must pass and STOP.
```

# Phase 2 — Harden shared controls

## Prompt DSE-007 — Audit and normalize Button and Link

```text
SCOPE: Verify Button and Link share supported tone, impact, size, shape, loading/disabled, icon, full-width, focus-visible, touch-target, and reduced-motion behavior.
CONSTRAINT: Preserve native button/anchor semantics and caller-owned navigation/action effects.
RESTRICTION: Do not simulate disabled anchors without documented keyboard and screen-reader behavior or introduce feature-specific variants.
USAGE: primitives/button; primitives/link; semantic tokens; component guides and tests.
BEHAVIOR: Add missing observable-state tests and light/dark desktop/mobile visual cases; run focused verification and STOP.
```

## Prompt DSE-008 — Normalize text-entry controls

```text
SCOPE: Align Input and Textarea label, description, required, placeholder, disabled, readonly, invalid, error-announcement, focus-visible, density, and model-binding contracts.
CONSTRAINT: Errors must be visibly associated and programmatically connected; readonly must remain focusable and distinguishable from disabled.
RESTRICTION: Do not own form validation, mutate caller errors, or hide labels behind placeholder-only presentation.
USAGE: primitives/input; primitives/textarea; field-message; interaction-state foundations.
BEHAVIOR: Test keyboard entry, model updates, description/error IDs, all unavailable states, and theme contrast; STOP.
```

## Prompt DSE-009 — Normalize selection controls

```text
SCOPE: Align Select, Checkbox, and Radio Group typed values, required/error association, disabled options, indeterminate checkbox state, fieldset/legend semantics, focus-visible behavior, and touch targets.
CONSTRAINT: Preserve native keyboard interaction and typed compareWith behavior for non-string values.
RESTRICTION: Do not replace native controls with custom ARIA widgets unless an approved requirement proves native behavior insufficient.
USAGE: primitives/select; primitives/checkbox; primitives/radio-group; field-message and interaction-state foundations.
BEHAVIOR: Add strict-template consumer coverage and focused unit/accessibility cases; STOP.
```

## Prompt DSE-010 — Normalize overlays and disclosure controls

```text
SCOPE: Verify Tooltip, Menu, Dialog, Drawer, Tabs, and Command Palette naming, keyboard navigation, Escape behavior, outside/backdrop dismissal, initial focus, focus trapping/restoration, disabled items, and responsive presentation.
CONSTRAINT: Use native dialog/button semantics where already established and logical start/end placement.
RESTRICTION: Do not close processing-locked workflows or infer authorization from visible actions.
USAGE: overlay/disclosure primitives and patterns; layer/elevation/motion tokens; browser keyboard tests.
BEHAVIOR: Run focused unit, accessibility, responsive, and reduced-motion checks and STOP.
```

## Prompt DSE-011 — Normalize feedback and asynchronous states

```text
SCOPE: Verify alert banners, notifications, progress, skeleton, state feedback, upload/download, and AI-generation states use visible status text, correct live-region priority, native progress semantics, cancellation/retry intent, and reduced motion.
CONSTRAINT: Loading and processing states prevent duplicate actions without claiming persistence success.
RESTRICTION: Do not place workflow execution, timers, uploads, downloads, or API retries inside presentation components.
USAGE: feedback primitives/components/patterns and their documentation/fixtures.
BEHAVIOR: Test idle, busy, success, recoverable failure, terminal failure, unavailable, and retry/cancel states; STOP.
```

# Phase 3 — Project Loop extensions

## Prompt DSE-012 — Validate portal shell and navigation composition

```text
SCOPE: Prove Portal Shell, Application Navigation, User Menu, Skip Link, Breadcrumb, Page Header, and responsive navigation compose into a valid authenticated application shell.
CONSTRAINT: Routes, authorization-filtered links, identity display values, notification counts, and active state are caller supplied.
RESTRICTION: Do not read Router, authentication, tenant, or API services inside the design system.
USAGE: public shell/navigation APIs and project-loop-portal-composition.md.
BEHAVIOR: Compile a neutral shell fixture and test landmarks, skip focus, long labels, compact navigation, and desktop/mobile layouts; STOP.
```

## Prompt DSE-013 — Validate dashboard compositions

```text
SCOPE: Prove Project Dashboard, Metric Grid/Card, Project Health, Milestones, Meetings, Decisions, Pending Approvals, and state feedback compose for empty, loading, error, partial, and populated dashboards.
CONSTRAINT: Caller owns data fetching, authorization, formatting, ordering, freshness, and business status meaning.
RESTRICTION: Do not add service calls, date inference, currency calculation, or tenant logic.
USAGE: public dashboard patterns/recipes and portal composition guide.
BEHAVIOR: Add a deterministic composition fixture with responsive and accessibility coverage; STOP.
```

## Prompt DSE-014 — Validate document and version compositions

```text
SCOPE: Prove filters, list rows/cards, upload, download, version chips/history, pagination, and state feedback compose into the planned Project Loop document screens.
CONSTRAINT: Preserve exact document/version identity, immutable approved-version meaning, native navigation, and caller-owned authorization/storage operations.
RESTRICTION: Do not expose blob URLs, infer access from visibility, mutate approved versions, or collapse current and approved qualifiers.
USAGE: public document APIs; exact-version regression fixture; document requirements.
BEHAVIOR: Test empty/one/many, long metadata, upload/download states, desktop/mobile switching, and v3-approved/v4-current separation; STOP.
```

## Prompt DSE-015 — Validate approval and audit compositions

```text
SCOPE: Prove approval banner/actions/comment/history, version-bound approval, confirmation dialog, audit event/timeline, and pagination compose for Project Loop review and audit screens.
CONSTRAINT: Every approval surface names the exact target version; audit order and identifiers are caller supplied and append-only in meaning.
RESTRICTION: Do not perform authorization, persistence, destructive action, audit writes, or automatic approval in presentation code.
USAGE: public approval/audit APIs; exact-version regression; portal composition guide.
BEHAVIOR: Test pending/processing/approved/rejected/changes-requested, confirmation lockout, long identifiers, missing actor fallback, focus restoration, and responsive layout; STOP.
```

# Phase 4 — Documentation and acceptance

## Prompt DSE-016 — Publish extension usage guidance

```text
SCOPE: Document how future Project Loop feature components select and compose public Lake Shore Drive-derived APIs, including one import example and ownership boundaries for state, APIs, routing, authorization, and persistence.
CONSTRAINT: Link existing per-component guides instead of duplicating them; include do/don't examples for deep imports and feature-local control copies.
RESTRICTION: Do not document private implementation paths as supported APIs.
USAGE: documentation README, public imports, integration, portal composition, and catalog.
BEHAVIOR: Run documentation coverage and link checks and STOP.
```

## Prompt DSE-017 — Run the extension acceptance gate

```text
SCOPE: Run clean install, design-system compile, strict public-consumer compile, unit, documentation, integration-manifest, boundary, accessibility, responsive, and visual checks.
CONSTRAINT: Record exact commands, totals, approved baselines, warnings, and residual blockers; all release-required checks must pass.
RESTRICTION: Do not update snapshots automatically, suppress failures, or describe historical results as current evidence.
USAGE: repository package scripts, integration manifest, final acceptance checklist, and Lake Shore Drive parity report.
BEHAVIOR: Publish the final pass/fail record; if any check fails, leave acceptance FAIL with the owning follow-up prompt and STOP.
```

## Completion definition

The extension sequence is complete only when:

1. Future Angular features compile using `@lsd/design-system` without deep imports.
2. Shared controls cover observable interaction and accessibility states.
3. Portal, dashboard, document, approval, and audit compositions have deterministic fixtures.
4. Unit, accessibility, responsive, visual, documentation, boundary, and build gates pass from the repository root.
5. Project Loop-specific recipes remain presentation-only and preserve tenant, authorization, document-version, approval, and audit boundaries.
