# Project Loop design-system completion delta — atomic SCRUB microprompts

Repository reviewed: `architect4hire/projectloop`, public `main` at `dcb86c4` (`design system spike 1`), reviewed 2026-08-16.

> **This is a delta sequence, not a fresh-start or rebuild plan.** Run it against commit `dcb86c4` or a descendant. Keep every existing design-system artifact unless a prompt explicitly names a narrow composition change. Before executing each prompt, search the current branch: if the exact artifact and contract already exist, report **SKIPPED — ALREADY SATISFIED** with evidence and continue to the next numbered prompt; never recreate or replace it.

## Audit basis and guardrails

The review covered the production-facing source under `src/web/design-system/`, its public barrels, catalog, integration manifest, tests, and the documented Project Loop application requirements. No application feature source is checked in outside the design-system migration snapshot, so “application usage” below means the concrete UI called for by `docs/requirements/requirements.md`, `docs/design/*.md`, and the Angular steps in `docs/prompts/project-loop-scrub-microprompts.md`; it does not claim observation of nonexistent feature templates.

The spike already provides strong generic foundations: semantic tokens, light/dark appearance, typography, motion, interaction states, icons, form controls, surfaces, overlays, tabs, tables, notifications, steppers, state feedback, activity streams, structured editing, review/approval, and several Lake Shore Drive consulting recipes. **They are the baseline, not work items.** Do not recreate or replace them. The delta is primarily the portal shell/navigation, dashboard summaries, document/version UI, version-bound approval UI, audit composition, and the integration gates needed to consume and verify the spike.

Every prompt below is independently executable and intentionally changes one seam. Execute in numeric order against the existing spike. For every prompt, first prove the delta is still missing. For implementation prompts, read `CLAUDE.md`, `.claude/rules/angular.md`, `.claude/rules/design-system.md`, and `.claude/skills/add-design-system-component/SKILL.md`; inspect existing APIs before editing; use Angular 22 standalone, signals, `OnPush`, strict templates, built-in control flow, semantic HTML, and existing tokens; expose supported APIs only through the appropriate barrel and `src/web/design-system/public-api.ts`; do not import feature models/services; run the smallest relevant verification; report changed files and results; then **STOP**.

## Phase 0 — establish executable truth

### PLDS-001 — Record the current public inventory

- **Class:** documentation
- **SCOPE:** Add one checked-in inventory table mapping every public token, primitive, component, pattern, recipe, layout, and icon export to its implementation and guide.
- **CONSTRAINTS:** Generate the table from the barrels and `documentation/catalog.json`; mark no item missing without searching.
- **RESTRICTIONS:** Do not change exports or implementation.
- **USAGE:** `src/web/design-system/{public-api.ts,**/index.ts,documentation/catalog.json}` → likely new `src/web/design-system/documentation/public-inventory.md`.
- **BEHAVIOR:** Run `check-documentation-coverage.mjs`; report mismatches; STOP.

### PLDS-002 — Add a design-system verification workspace

- **Class:** test
- **SCOPE:** Add only the minimal Angular 22 test/build workspace needed to compile the checked-in design-system source in place.
- **CONSTRAINTS:** Respect dependency versions in `integration-manifest.json`; keep `src/web/design-system/` the source of truth.
- **RESTRICTIONS:** Do not create feature pages, demo business data, or another component library.
- **USAGE:** likely `package.json`, `angular.json`, `tsconfig*.json`, `src/web/design-system/integration-manifest.json`.
- **BEHAVIOR:** Make one design-system compilation command pass; STOP.

### PLDS-003 — Wire the public import alias

- **Class:** test
- **SCOPE:** Configure one alias that resolves application imports to `src/web/design-system/public-api.ts`.
- **CONSTRAINTS:** Use the alias documented by the integration manifest or rename it consistently once; forbid deep imports.
- **RESTRICTIONS:** Do not modify component APIs.
- **USAGE:** likely root `tsconfig.json`/app test tsconfig and `documentation/public-imports.md`.
- **BEHAVIOR:** Add one compile-only consumer proving the public alias resolves; STOP.

### PLDS-004 — Install the global design-system stylesheet entry

- **Class:** token
- **SCOPE:** Register only `foundations/tailwind.css` as the first global style in the verification/application workspace.
- **CONSTRAINTS:** Preserve the import order declared in `integration-manifest.json`.
- **RESTRICTIONS:** Do not add feature CSS or duplicate token declarations.
- **USAGE:** `src/web/design-system/foundations/tailwind.css`, likely `angular.json`.
- **BEHAVIOR:** Build and prove semantic CSS variables exist in emitted CSS; STOP.

### PLDS-005 — Enforce the feature import boundary

- **Class:** test
- **SCOPE:** Connect the existing feature-boundary checker to one package script/CI command.
- **CONSTRAINTS:** Application code may use the public entry point only.
- **RESTRICTIONS:** Do not rewrite imports that are not present.
- **USAGE:** `src/web/design-system/testing/check-feature-boundaries.mjs`, likely `package.json` and CI config.
- **BEHAVIOR:** Add one failing fixture and one passing fixture for the checker; STOP.

## Phase 1 — shared primitives required by portal work

### PLDS-006 — Add a semantic link primitive

- **Class:** primitive
- **SCOPE:** Implement one standalone link primitive with native-anchor semantics and visual variants matching Button.
- **CONSTRAINTS:** Support router-compatible `href`/projection without intercepting navigation; preserve visited/focus/disabled guidance.
- **RESTRICTIONS:** Do not add breadcrumb or navigation behavior.
- **USAGE:** likely `primitives/link/*`, `primitives/index.ts`, `documentation/link.md`, `catalog.json`.
- **BEHAVIOR:** Test anchor semantics, keyboard focus, external-link labeling, and variants; STOP.

### PLDS-007 — Add a field-message primitive

- **Class:** primitive
- **SCOPE:** Implement one reusable help/error/success message primitive for form-control descriptions.
- **CONSTRAINTS:** Provide stable IDs and appropriate live-region behavior without owning validation.
- **RESTRICTIONS:** Do not refactor existing controls in this step.
- **USAGE:** likely `primitives/field-message/*`, barrel, catalog, guide.
- **BEHAVIOR:** Test ID association and error announcement; STOP.

### PLDS-008 — Add a menu primitive

- **Class:** primitive
- **SCOPE:** Implement one accessible action-menu primitive for user and row actions.
- **CONSTRAINTS:** Use native button triggers, managed focus, Escape, outside dismissal, and focus restoration.
- **RESTRICTIONS:** Do not encode application routes, authorization, or destructive confirmation.
- **USAGE:** likely `primitives/menu/*`, barrel, catalog, guide.
- **BEHAVIOR:** Test keyboard navigation and dismissal; STOP.

### PLDS-009 — Add a progress primitive

- **Class:** primitive
- **SCOPE:** Implement one determinate/indeterminate progress indicator.
- **CONSTRAINTS:** Expose visible label/value text, reduced-motion behavior, and native ARIA progress semantics.
- **RESTRICTIONS:** Do not add project-health or upload business logic.
- **USAGE:** likely `primitives/progress/*`, existing motion/tokens, catalog, guide.
- **BEHAVIOR:** Test valid ranges, indeterminate semantics, and accessible names; STOP.

### PLDS-010 — Add a skeleton primitive

- **Class:** primitive
- **SCOPE:** Extract one reusable decorative skeleton primitive from the existing state-feedback presentation.
- **CONSTRAINTS:** Keep the parent loading region responsible for `aria-busy` and announcements.
- **RESTRICTIONS:** Do not change `StateFeedbackComponent` behavior yet.
- **USAGE:** `patterns/state-feedback/*` → likely `primitives/skeleton/*`.
- **BEHAVIOR:** Test reduced motion and decorative semantics; STOP.

### PLDS-011 — Compose skeletons in state feedback

- **Class:** pattern
- **SCOPE:** Replace only state-feedback’s private skeleton markup with the public Skeleton primitive.
- **CONSTRAINTS:** Preserve current loading, empty, recoverable, and terminal contracts.
- **RESTRICTIONS:** Do not alter other variants or visual copy.
- **USAGE:** `patterns/state-feedback/*`.
- **BEHAVIOR:** Run existing unit and visual specs; STOP.

## Phase 2 — application shell and navigation

### PLDS-012 — Add a skip-link component

- **Class:** component
- **SCOPE:** Implement one skip link targeting the application main-content element.
- **CONSTRAINTS:** It becomes visible on focus and uses the Link primitive.
- **RESTRICTIONS:** Do not create the shell.
- **USAGE:** likely `components/skip-link/*`, barrel, catalog, guide.
- **BEHAVIOR:** Test keyboard activation and missing-target behavior; STOP.

### PLDS-013 — Add a breadcrumb component

- **Class:** component
- **SCOPE:** Implement one semantic breadcrumb component from caller-supplied labels and URLs.
- **CONSTRAINTS:** Render a labeled `nav`, ordered list, and `aria-current="page"` on the terminal item.
- **RESTRICTIONS:** Do not inspect Angular Router state.
- **USAGE:** likely `components/breadcrumb/*`, Link/Icon primitives, catalog, guide.
- **BEHAVIOR:** Test one-item, nested, and long-label cases; STOP.

### PLDS-014 — Add a pagination component

- **Class:** component
- **SCOPE:** Implement one controlled pagination component that emits page-change intent.
- **CONSTRAINTS:** Caller owns totals and fetching; expose accessible current-page and disabled states.
- **RESTRICTIONS:** Do not couple it to DataTable.
- **USAGE:** likely `components/pagination/*`, Button primitive, catalog, guide.
- **BEHAVIOR:** Test boundary pages and keyboard operation; STOP.

### PLDS-015 — Add an application navigation component

- **Class:** component
- **SCOPE:** Implement one responsive navigation list from caller-authorized link models.
- **CONSTRAINTS:** Support active state, icon, label, optional count, and compact presentation; callers filter permissions.
- **RESTRICTIONS:** Do not hard-code Project Loop routes or read auth state.
- **USAGE:** likely `components/app-navigation/*`, Link/Badge/Icon APIs, catalog, guide.
- **BEHAVIOR:** Test active semantics and narrow layout; STOP.

### PLDS-016 — Add a user-menu component

- **Class:** component
- **SCOPE:** Implement one user/account menu composed from the Menu primitive.
- **CONSTRAINTS:** Accept display-safe identity text and caller-owned actions.
- **RESTRICTIONS:** Do not sign out, switch tenants, or fetch profile data.
- **USAGE:** likely `components/user-menu/*`, Menu/Avatar-or-initial fallback, catalog, guide.
- **BEHAVIOR:** Test missing avatar, long identity, keyboard, and emitted intents; STOP.

### PLDS-017 — Add a page-header pattern

- **Class:** pattern
- **SCOPE:** Implement one page header with title, description, breadcrumb, metadata, and projected actions.
- **CONSTRAINTS:** Preserve heading hierarchy and responsive action wrapping.
- **RESTRICTIONS:** Do not encode feature fields.
- **USAGE:** likely `patterns/page-header/*`, Breadcrumb/Separator APIs, catalog, guide.
- **BEHAVIOR:** Test heading levels and narrow widths; STOP.

### PLDS-018 — Add the Project Loop portal shell

- **Class:** layout
- **SCOPE:** Implement one authenticated portal-shell layout with header, navigation, main outlet slot, notification viewport slot, and skip link.
- **CONSTRAINTS:** Compose public APIs; use semantic landmarks; keep route/auth state caller-owned.
- **RESTRICTIONS:** Do not repurpose the consulting `WorkbenchShellRecipeComponent` or add feature content.
- **USAGE:** likely `layouts/portal-shell/*`, `layouts/index.ts`, catalog, guide.
- **BEHAVIOR:** Test landmark names, focus target, desktop/mobile navigation, and projection; STOP.

## Phase 3 — dashboard and engagement recipes

### PLDS-019 — Add a metric-card recipe

- **Class:** recipe
- **SCOPE:** Implement one business-neutral summary metric card with label, value, optional trend/context, and action link.
- **CONSTRAINTS:** Values remain caller-formatted; trend meaning must include text/icon, not color alone.
- **RESTRICTIONS:** Do not name invoices, hours, or retainers in the API.
- **USAGE:** likely `recipes/metric-card/*`, Surface/Badge/Icon, catalog, guide.
- **BEHAVIOR:** Test loading-safe content, long values, and narrow width; STOP.

### PLDS-020 — Add a metric-grid layout

- **Class:** layout
- **SCOPE:** Implement one responsive grid layout for Metric Card instances.
- **CONSTRAINTS:** Support one through four items without changing DOM order.
- **RESTRICTIONS:** Do not fetch metrics or add card semantics.
- **USAGE:** likely `layouts/metric-grid/*`, layout barrel, catalog, guide.
- **BEHAVIOR:** Add responsive visual coverage; STOP.

### PLDS-021 — Add a project-health recipe

- **Class:** recipe
- **SCOPE:** Implement one project-health summary with status, explanatory text, last-updated time, and optional contributing indicators.
- **CONSTRAINTS:** Health is caller-calculated; pair every color with visible text.
- **RESTRICTIONS:** Do not calculate health or import Engagement models.
- **USAGE:** likely `recipes/project-health/*`, Surface/Badge/Progress, catalog, guide.
- **BEHAVIOR:** Test healthy/attention/at-risk/unknown states; STOP.

### PLDS-022 — Add a milestone-list recipe

- **Class:** recipe
- **SCOPE:** Implement one ordered milestone summary list with status, due date, and optional navigation intent.
- **CONSTRAINTS:** Preserve chronological reading order supplied by caller; handle missing dates.
- **RESTRICTIONS:** Do not mutate milestones or infer lateness.
- **USAGE:** likely `recipes/milestone-list/*`, Badge/Link, catalog, guide.
- **BEHAVIOR:** Test empty, overdue-labelled, and long-title presentations; STOP.

### PLDS-023 — Add an upcoming-meetings recipe

- **Class:** recipe
- **SCOPE:** Implement one meeting summary list with title, localized time text, location/channel text, and action link.
- **CONSTRAINTS:** Caller owns time-zone conversion and join authorization.
- **RESTRICTIONS:** Do not integrate calendars or conferencing.
- **USAGE:** likely `recipes/upcoming-meetings/*`, Surface/Link/Icon, catalog, guide.
- **BEHAVIOR:** Test no meetings and narrow layout; STOP.

### PLDS-024 — Add a recent-decisions recipe

- **Class:** recipe
- **SCOPE:** Implement one compact recent-decisions list showing decision label, status, date, and navigation.
- **CONSTRAINTS:** Reuse Badge and Link; caller supplies authorized records.
- **RESTRICTIONS:** Do not duplicate `DecisionComparisonComponent`.
- **USAGE:** likely `recipes/recent-decisions/*`, catalog, guide.
- **BEHAVIOR:** Test empty and mixed-status lists; STOP.

### PLDS-025 — Add a dashboard composition pattern

- **Class:** pattern
- **SCOPE:** Implement one slot-based dashboard composition for health, metrics, milestones, meetings, decisions, and deliverables.
- **CONSTRAINTS:** Own responsive layout only; allow State Feedback per region.
- **RESTRICTIONS:** Do not define API models, fetch data, or merge loading state across regions.
- **USAGE:** likely `patterns/project-dashboard/*`, Metric Grid and public recipes, catalog, guide.
- **BEHAVIOR:** Test region headings and desktop/mobile ordering; STOP.

## Phase 4 — document and version UI

### PLDS-026 — Add a document-status presentation map

- **Class:** token
- **SCOPE:** Add one typed semantic presentation map for draft, published, superseded, archived, and unavailable document states.
- **CONSTRAINTS:** Map to existing Badge semantics; keep labels overrideable/localizable.
- **RESTRICTIONS:** Do not define domain transitions.
- **USAGE:** likely `tokens/document-status.ts`, tokens barrel, badges guide.
- **BEHAVIOR:** Unit-test exhaustive state mapping; STOP.

### PLDS-027 — Add a version-chip component

- **Class:** component
- **SCOPE:** Implement one compact version identifier chip with optional current/approved/published qualifier.
- **CONSTRAINTS:** Always render the exact version label as text; qualifiers cannot imply another version is approved.
- **RESTRICTIONS:** Do not resolve current version or approval state.
- **USAGE:** likely `components/version-chip/*`, Badge, catalog, guide.
- **BEHAVIOR:** Test exact-version visibility and non-color distinction; STOP.

### PLDS-028 — Add a document-card recipe

- **Class:** recipe
- **SCOPE:** Implement one document summary card with title, category, status, visibility, exact version, updated metadata, and projected actions.
- **CONSTRAINTS:** Accept display-ready authorized data; compose Version Chip.
- **RESTRICTIONS:** Do not download, publish, or approve documents.
- **USAGE:** likely `recipes/document-card/*`, Surface/Badge/Version Chip, catalog, guide.
- **BEHAVIOR:** Test long names, confidential labels, and narrow layout; STOP.

### PLDS-029 — Add a document-row recipe

- **Class:** recipe
- **SCOPE:** Implement one table/list row equivalent of Document Card.
- **CONSTRAINTS:** Preserve the same semantic fields and exact-version presentation.
- **RESTRICTIONS:** Do not add table sorting or selection.
- **USAGE:** likely `recipes/document-row/*`, Data Table/Version Chip, catalog, guide.
- **BEHAVIOR:** Test row header semantics and action labeling; STOP.

### PLDS-030 — Add a document-list pattern

- **Class:** pattern
- **SCOPE:** Implement one responsive document collection that switches between Document Row and Document Card presentations.
- **CONSTRAINTS:** Preserve identical information and DOM reading order across breakpoints.
- **RESTRICTIONS:** Do not own filtering, pagination, API calls, or selection state.
- **USAGE:** likely `patterns/document-list/*`, document recipes, catalog, guide.
- **BEHAVIOR:** Test empty, one, many, and narrow presentations; STOP.

### PLDS-031 — Add a document-filter recipe

- **Class:** recipe
- **SCOPE:** Implement one Project Loop document filter form for project, category, status, and visibility values supplied by caller.
- **CONSTRAINTS:** Compose Filter Action Bar and existing form primitives; emit one typed filter-change intent.
- **RESTRICTIONS:** Do not fetch options or apply authorization.
- **USAGE:** likely `recipes/document-filters/*`, catalog, guide.
- **BEHAVIOR:** Test reset, submit, and accessible labels; STOP.

### PLDS-032 — Add a document-version-history pattern

- **Class:** pattern
- **SCOPE:** Implement one chronological version-history view with exact version, publication/approval qualifiers, actor/time text, and action slots.
- **CONSTRAINTS:** Historical approval stays visibly bound to its version.
- **RESTRICTIONS:** Do not reuse Activity Stream if it obscures version identity; do not mutate versions.
- **USAGE:** likely `patterns/document-version-history/*`, Activity Stream/Version Chip, catalog, guide.
- **BEHAVIOR:** Test v3-approved/v4-current separation explicitly; STOP.

### PLDS-033 — Add a document-upload recipe

- **Class:** recipe
- **SCOPE:** Implement one upload form composition using File Picker plus metadata fields and progress.
- **CONSTRAINTS:** Caller supplies MIME/size policy and performs upload; component emits intent/state only.
- **RESTRICTIONS:** Do not access Blob Storage or generate URLs.
- **USAGE:** likely `recipes/document-upload/*`, File Picker/Input/Select/Progress, catalog, guide.
- **BEHAVIOR:** Test validation display, progress, cancellation intent, and failure recovery; STOP.

### PLDS-034 — Add a document-download action component

- **Class:** component
- **SCOPE:** Implement one download action presenter with ready, preparing, downloading, failed, and unavailable states.
- **CONSTRAINTS:** Caller performs authorized retrieval; never accept or expose permanent Blob URLs.
- **RESTRICTIONS:** Do not make HTTP requests.
- **USAGE:** likely `components/document-download-action/*`, Button/State Feedback, catalog, guide.
- **BEHAVIOR:** Test state announcements and retry intent; STOP.

## Phase 5 — version-bound approval UI

### PLDS-035 — Add an approval-status presentation map

- **Class:** token
- **SCOPE:** Add one typed semantic presentation map for requested, approved, rejected, cancelled, and expired approval states.
- **CONSTRAINTS:** Use existing Badge semantics and explicit labels.
- **RESTRICTIONS:** Do not encode transition rules.
- **USAGE:** likely `tokens/approval-status.ts`, tokens barrel, badges guide.
- **BEHAVIOR:** Unit-test exhaustive state mapping; STOP.

### PLDS-036 — Add an approval-request banner recipe

- **Class:** recipe
- **SCOPE:** Implement one banner summarizing an approval request, target type, exact target/version, status, requester, and requested time.
- **CONSTRAINTS:** Exact version is mandatory for document targets and visible in the primary summary.
- **RESTRICTIONS:** Do not emit approve/reject actions.
- **USAGE:** likely `recipes/approval-request-banner/*`, Alert Banner/Version Chip, catalog, guide.
- **BEHAVIOR:** Test missing document version as an invalid input and verify v3 visibility; STOP.

### PLDS-037 — Add an approval-comment field recipe

- **Class:** recipe
- **SCOPE:** Implement one optional/required decision-comment field with count, help, validation, and caller-controlled value.
- **CONSTRAINTS:** Compose Textarea and Field Message; label requirement explicitly.
- **RESTRICTIONS:** Do not decide when comments are required.
- **USAGE:** likely `recipes/approval-comment-field/*`, catalog, guide.
- **BEHAVIOR:** Test required, optional, max-length, and error states; STOP.

### PLDS-038 — Add a version-bound approval pattern

- **Class:** pattern
- **SCOPE:** Compose Approval Request Banner, exact-version context, Approval Comment Field, and existing Approval Actions into one review pattern.
- **CONSTRAINTS:** Keep target identity and version visible beside actions; caller owns authorization/persistence.
- **RESTRICTIONS:** Do not duplicate generic `ReviewApprovalComponent` comparison behavior.
- **USAGE:** likely `patterns/version-bound-approval/*`, existing approval APIs, catalog, guide.
- **BEHAVIOR:** Test approve/reject intents against v3 while v4 is marked current; STOP.

### PLDS-039 — Add an approval-history pattern

- **Class:** pattern
- **SCOPE:** Implement one append-oriented approval history using Activity Stream with decision, actor, UTC-display text, comment summary, and exact target/version.
- **CONSTRAINTS:** Preserve chronological evidence and immutable presentation.
- **RESTRICTIONS:** Do not edit, delete, or collapse version identity.
- **USAGE:** likely `patterns/approval-history/*`, Activity Stream/Version Chip, catalog, guide.
- **BEHAVIOR:** Test multiple decisions across multiple versions; STOP.

### PLDS-040 — Add a pending-approvals list recipe

- **Class:** recipe
- **SCOPE:** Implement one compact list of pending approval requests with target, exact version where applicable, requester, due text, and review link.
- **CONSTRAINTS:** Caller supplies authorized sorted items.
- **RESTRICTIONS:** Do not approve inline or infer urgency.
- **USAGE:** likely `recipes/pending-approvals-list/*`, Link/Badge/Version Chip, catalog, guide.
- **BEHAVIOR:** Test empty, overdue-labelled, and mixed-target lists; STOP.

## Phase 6 — audit and global portal patterns

### PLDS-041 — Add an audit-event recipe

- **Class:** recipe
- **SCOPE:** Implement one audit event presentation with actor, action, resource, UTC-display text, correlation identifier, and safe details slot.
- **CONSTRAINTS:** Display already-redacted data; long identifiers wrap/copy accessibly.
- **RESTRICTIONS:** Do not render document bodies, secrets, tokens, or raw logs.
- **USAGE:** likely `recipes/audit-event/*`, Surface/Badge, catalog, guide.
- **BEHAVIOR:** Test missing actor, correlation text, and narrow layout; STOP.

### PLDS-042 — Add an audit-timeline pattern

- **Class:** pattern
- **SCOPE:** Compose Activity Stream and Audit Event into one paged timeline presentation.
- **CONSTRAINTS:** Caller owns filtering, ordering, and pagination; preserve append-only semantics.
- **RESTRICTIONS:** Do not fetch or mutate audit records.
- **USAGE:** likely `patterns/audit-timeline/*`, Pagination, catalog, guide.
- **BEHAVIOR:** Test empty, many-event, and load-more intent states; STOP.

### PLDS-043 — Add a global error-summary component

- **Class:** component
- **SCOPE:** Implement one form error summary linking to caller-supplied invalid control IDs.
- **CONSTRAINTS:** Focus summary after failed submission and preserve native control errors.
- **RESTRICTIONS:** Do not inspect Angular form trees automatically.
- **USAGE:** likely `components/error-summary/*`, Alert Banner/Link, catalog, guide.
- **BEHAVIOR:** Test focus, link targets, pluralization input, and zero-error behavior; STOP.

### PLDS-044 — Add a confirmation-dialog recipe

- **Class:** recipe
- **SCOPE:** Implement one business-neutral confirmation composition for consequential actions.
- **CONSTRAINTS:** Require explicit action label, consequence text, processing state, cancel action, and caller-owned confirmation.
- **RESTRICTIONS:** Do not perform deletion/publication/cancellation or infer danger level.
- **USAGE:** likely `recipes/confirmation-dialog/*`, Dialog/Button, catalog, guide.
- **BEHAVIOR:** Test focus, Escape, processing lock, and emitted intents; STOP.

## Phase 7 — conformance, documentation, and acceptance

### PLDS-045 — Add unit coverage for every new public API

- **Class:** test
- **SCOPE:** Add only missing focused unit tests for public artifacts introduced by PLDS-006 through PLDS-044.
- **CONSTRAINTS:** One spec describes one artifact contract; test outputs and accessibility behavior rather than private implementation.
- **RESTRICTIONS:** Do not change production behavior to make weak assertions pass.
- **USAGE:** adjacent `*.component.spec.ts` files and the verification workspace.
- **BEHAVIOR:** Run the design-system unit suite; report uncovered public APIs; STOP.

### PLDS-046 — Add portal visual-regression coverage

- **Class:** test
- **SCOPE:** Add one visual matrix covering the new portal shell, dashboard, documents, approval, and audit compositions.
- **CONSTRAINTS:** Cover light/dark, desktop/mobile, long content, and meaningful states; use deterministic fixtures.
- **RESTRICTIONS:** Do not regenerate approved baselines automatically.
- **USAGE:** existing `*.visual.spec.ts`, `visual-regression/`, `playwright.config.ts` per manifest.
- **BEHAVIOR:** Run visual tests and attach only intentional diffs; STOP.

### PLDS-047 — Add portal accessibility coverage

- **Class:** test
- **SCOPE:** Add one Playwright/axe suite for shell navigation, document list, upload, version-bound approval, and audit timeline.
- **CONSTRAINTS:** Include keyboard-only flows, focus restoration, landmarks, accessible names, and live regions.
- **RESTRICTIONS:** Do not suppress axe rules without a documented exception.
- **USAGE:** manifest `test:accessibility` contract and new portal fixtures.
- **BEHAVIOR:** Run the accessibility suite at desktop and mobile widths; STOP.

### PLDS-048 — Add exact-version regression tests

- **Class:** test
- **SCOPE:** Add one cross-component regression suite proving approval of document v3 never presents v4 as approved.
- **CONSTRAINTS:** Cover Version Chip, version history, approval banner, approval pattern, pending list, and approval history.
- **RESTRICTIONS:** Do not add backend approval logic.
- **USAGE:** new focused integration spec under design-system testing/fixtures.
- **BEHAVIOR:** Assert exact visible version text and qualifiers in every surface; STOP.

### PLDS-049 — Document portal composition guidance

- **Class:** documentation
- **SCOPE:** Add one guide mapping Project Loop dashboard, documents, approvals, and audit screens to the correct design-system compositions.
- **CONSTRAINTS:** Show public imports and slot ownership; distinguish presentation from feature state/API/security responsibilities.
- **RESTRICTIONS:** Do not provide deep-import examples or duplicate individual component guides.
- **USAGE:** likely `documentation/project-loop-portal-composition.md`, `documentation/README.md`.
- **BEHAVIOR:** Run documentation coverage checks; STOP.

### PLDS-050 — Update the integration manifest for completed portal APIs

- **Class:** documentation
- **SCOPE:** Update only the integration manifest’s copy/test/public-layer declarations required by the completed additions.
- **CONSTRAINTS:** Keep runtime peers and exclusions accurate; preserve no-deep-import policy.
- **RESTRICTIONS:** Do not change dependency versions unless compilation proved the checked-in values invalid.
- **USAGE:** `integration-manifest.json`, schema only if the manifest shape truly changes.
- **BEHAVIOR:** Run `check-integration-manifest.mjs`; STOP.

### PLDS-051 — Run the design-system acceptance gate

- **Class:** test
- **SCOPE:** Run one final gate covering clean install, compile, lint, unit, documentation, boundaries, accessibility, responsive, and visual checks.
- **CONSTRAINTS:** Fix only defects exposed in already-implemented design-system work; record any feature-app dependency as a blocker.
- **RESTRICTIONS:** Do not add new components or broaden APIs during the gate.
- **USAGE:** commands declared by `integration-manifest.json` and the verification workspace.
- **BEHAVIOR:** Publish a pass/fail checklist with exact commands and residual gaps; STOP.

## Explicitly not requested by this sequence

- No React/JSX conventions, third-party component framework, or second token system.
- No feature services, API calls, route guards, authorization decisions, domain transitions, Blob access, or audit persistence inside design-system artifacts.
- No reimplementation of existing generic APIs when composition suffices.
- No silent promotion of the private `documentation/migration/` snapshot into production.
- No claim that a client-side disabled/hidden action enforces authorization.
