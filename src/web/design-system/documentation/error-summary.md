# Error summary

## Purpose

`ErrorSummaryComponent` presents caller-supplied validation errors as links to native control IDs and moves focus to the summary after a caller-signaled failed submission. It does not inspect Angular form trees, validate controls, or replace native inline errors.

## API and states

- Required `id` names the Alert Banner and required readonly `errors` contains `controlId`/visible `label` pairs.
- Increment `failedSubmissionCount` after each failed submission. A new positive count focuses the rendered summary only when errors exist.
- `singularTitle` and `pluralTitle` are caller-localizable pluralization inputs. The plural string uses a `{count}` placeholder.
- Zero errors render no summary and cause no focus change.
- The component has no form, submission, validation, or error-discovery output.

```html
<lsd-error-summary
  id="profile-errors"
  [errors]="invalidControls"
  [failedSubmissionCount]="failedSubmissions"
  singularTitle="Resolve this error"
  pluralTitle="Resolve these {count} errors" />
```

## Accessibility and responsive behavior

The assertive Alert Banner announces the failed submission summary. Its focusable wrapper receives focus without entering normal tab order, and every Link retains native fragment navigation to a caller-owned control ID. Long labels wrap at narrow widths. Controls keep their existing `aria-invalid`, `aria-errormessage`, descriptions, and native validation behavior.

## Do / don't

Do preserve inline errors beside controls, supply stable unique IDs, and increment the attempt count only for failed submissions. Don't scan form controls automatically, move focus when validation succeeds, or treat the summary as the source of validation truth.

## Public import

```ts
import { ErrorSummaryComponent, type ErrorSummaryItem } from 'src/web/design-system/public-api';
```
