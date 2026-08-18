# Root verification entry points

Recorded 2026-08-18 for DSE-001. Commands were run once from the repository
root against the checked-in `package-lock.json`. A failure below remains a
failure; no assertions, axe rules, or snapshots were changed.

| Gate | Root command | Result | Exact result |
| --- | --- | --- | --- |
| Clean install | `npm ci` | PASS | 384 packages installed; exit 0. npm reported 5 dependency install scripts awaiting allow-list review. |
| Design-system compile | `npm run build:design-system` | PASS | Exit 0; 0 errors and 9 Angular NG8113 unused-import warnings. |
| Strict public-consumer compile | `npm run build:design-system` | PASS | The strict Angular compilation includes `testing/public-api-alias.consumer.ts` through `tsconfig.design-system.json`; exit 0. |
| Unit | `npm run test:unit` | PASS | 58 files passed; 221 tests passed; 0 failed. Test compilation reported the 9 production NG8113 warnings plus 3 unused test-host imports. |
| Documentation | `npm run test:documentation` | PASS | 3 of 3 checkers passed: 110 public modules across 115 catalog entries; 42 public component/recipe modules in 41 guides; 104 Markdown files passed local-link validation. |
| Integration manifest | `npm run test:integration-manifest` | FAIL | Exit 1; 1 blocker: no approved visual baselines found. |
| Boundaries | `npm run test:boundaries` | NOT RUN | Stopped after the integration-manifest failure as required by DSE-017. |
| Accessibility | `npm run test:accessibility` | NOT RUN | Stopped after the integration-manifest failure as required by DSE-017. |
| Responsive documentation | `npm run test:responsive` | NOT RUN | Stopped after the integration-manifest failure as required by DSE-017. |
| Visual | `npm run test:visual` | NOT RUN | No snapshots were updated; stopped after the integration-manifest failure. |

## Entry-point contract

`package.json` owns explicit root scripts for the Angular `design-system`
project, documentation checks, responsive documentation, integration-manifest
validation, both boundary self-tests and real scans, accessibility, and visual
regression. `playwright.config.ts` owns the `design-system-visual` fixture
lifecycle through `visual-regression/serve-fixture.mjs`.

## Residual blockers

1. Approved visual baselines must be reviewed and committed through a dedicated
   visual-baseline acceptance follow-up; they must not be generated automatically
   by DSE-017.
2. Boundary, accessibility, responsive, and visual gates require a new DSE-017
   run after the manifest blocker is resolved.

Current root acceptance is **FAIL** because the integration-manifest gate fails
and four downstream gates were not run. Ownership is the visual-baseline
acceptance follow-up, followed by a complete rerun of DSE-017.
