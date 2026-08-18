# Root verification entry points

Recorded 2026-08-18 for DSE-001. Commands were run once from the repository
root against the checked-in `package-lock.json`. A failure below remains a
failure; no assertions, axe rules, or snapshots were changed.

| Gate | Root command | Result | Exact result |
| --- | --- | --- | --- |
| Clean install | `npm ci` | PASS | 384 packages installed; exit 0. npm reported 5 dependency install scripts awaiting allow-list review. |
| Design-system compile | `npm run build:design-system` | PASS | Exit 0; 0 errors and 9 Angular NG8113 unused-import warnings. |
| Unit | `npm run test:unit` | PASS | 40 files passed; 138 tests passed; 0 failed. Signal-backed test hosts now schedule zoneless input updates deterministically. |
| Documentation | `npm run test:documentation` | PASS | 2 of 2 checkers passed: 110 public modules across 114 catalog entries; 42 public component/recipe modules covered by 41 guides. |
| Boundaries | `npm run test:boundaries` | PASS | 4 of 4 invocations passed: feature self-test, real feature scan, design-system self-test, and real design-system scan. |
| Integration manifest | `npm run test:integration-manifest` | FAIL | 0 passed, 1 blocker: no approved visual baselines found. |
| Responsive documentation | `npm run test:responsive` | PASS | All 12 of 12 critical recipes document narrow-screen behavior. This is the static responsive contract gate, not browser layout acceptance. |
| Accessibility | `npm run test:accessibility` | FAIL | The DSE-003 targeted lifecycle run built, bound, executed Chromium, and shut down in 11.5 seconds. Its one selected spec failed one unsuppressed axe `region` violation for `.lsd-portal-shell__notifications`. |
| Visual | `npm run test:visual` | FAIL | The DSE-003 targeted lifecycle run built, bound, executed Chromium, and shut down in 11.3 seconds. Its one selected screenshot spec failed because the approved Windows baseline is absent; the generated candidate was removed. |

## Entry-point contract

`package.json` owns explicit root scripts for the Angular `design-system`
project, documentation checks, responsive documentation, integration-manifest
validation, both boundary self-tests and real scans, accessibility, and visual
regression. `playwright.config.ts` owns the `design-system-visual` fixture
lifecycle through `visual-regression/serve-fixture.mjs`.

## Residual blockers

1. The portal-shell notification fixture must be contained by an appropriate
   landmark without suppressing the axe `region` rule.
2. Approved visual baselines must be reviewed and committed through the visual
   acceptance workflow; they must not be generated automatically by this gate.

Current root acceptance is **FAIL** because 3 of 9 root gates fail. The
commands themselves are present and executable; downstream correctness remains
assigned to DSE-003 and visual-baseline acceptance.
