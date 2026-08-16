# Recent decisions recipe

## Purpose

`RecentDecisionsComponent` presents a compact list of caller-authorized decision summaries with a label, visible status, date, and native navigation link. It is an index-style presentation and does not duplicate the side-by-side version panes or comparison behavior of `DecisionComparisonComponent`.

## Usage

```ts
readonly decisions: readonly RecentDecisionRecord[] = [
  {
    id: 'public-entry-point',
    label: 'Use the shared public entry point',
    status: { label: 'Accepted', variant: 'success' },
    date: { label: 'August 16, 2026', dateTime: '2026-08-16' },
    navigation: {
      href: '/decisions/public-entry-point',
      accessibleLabel: 'Open decision: Use the shared public entry point',
    },
  },
];
```

```html
<lsd-recent-decisions [decisions]="decisions" />
```

## Inputs and states

- `decisions` is a required immutable list of already-authorized summary records with stable IDs.
- `label` is the decision label rendered as the native navigation link text.
- `status` contains caller-owned visible text and a Badge variant.
- `date` contains visible formatted text and an optional machine-readable `dateTime`.
- `navigation` supplies the authorized destination and optional record-specific accessible label.
- The component-level `label` names the section; `emptyMessage` customizes the explicit empty state.

## Boundaries

The recipe does not fetch, filter, authorize, sort, compare, approve, or mutate decisions. Callers supply only records the current user may see and destinations they may navigate to. Use `DecisionComparisonComponent` when users need side-by-side versions; this recipe intentionally exposes no pane, version, diff, or comparison API.

## Accessibility

Native list semantics identify the collection. Every status uses visible Badge text, dates use native `time` when a machine-readable value is supplied, and navigation retains native anchor behavior through Link.

## Responsive behavior

Desktop rows keep label, status, and date compactly aligned. At narrow widths, the date moves below the label/status while DOM order remains label, status, date. Long decision labels wrap without horizontal overflow.

## Do / don't

Do provide display-safe authorized records, meaningful status text, formatted dates, and stable navigation destinations. Don't use this recipe to compare versions, infer status, perform authorization, or load decision detail.

## Visual coverage

Capture empty and mixed-status lists in light/dark appearances at desktop/mobile widths. Include long labels and multiple badge variants.

## Public import

```ts
import { RecentDecisionsComponent } from 'src/web/design-system/public-api';
```
