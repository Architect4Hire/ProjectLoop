# Project health recipe

## Purpose

`ProjectHealthComponent` presents a caller-calculated health state, explanatory text, last-updated time, and optional contributing indicators. It contains no health algorithm and has no dependency on Engagement or other feature models.

## Usage

```ts
readonly status: ProjectHealthStatus = 'attention';

readonly indicators: readonly ProjectHealthIndicator[] = [
  { id: 'readiness', label: 'Readiness', value: 72, valueText: '72 percent ready' },
  { id: 'confidence', label: 'Confidence', value: 4, max: 5, valueText: '4 of 5' },
];
```

```html
<lsd-project-health
  id="delivery-health"
  [status]="status"
  description="Two caller-identified indicators need attention."
  [lastUpdated]="{ label: '16 August 2026 at 9:30 AM', dateTime: '2026-08-16T14:30:00Z' }"
  [indicators]="indicators"
/>
```

## Inputs and states

- `id` is a required stable caller-owned ID used to namespace native Progress IDs.
- `status` is required and accepts `healthy`, `attention`, `at-risk`, or `unknown`. The caller calculates and selects it.
- `description` is required caller-supplied explanatory text.
- `lastUpdated` contains a visible formatted label and optional machine-readable `dateTime` value.
- `indicators` optionally supplies display-ready Progress data. Each indicator has a stable ID, label, optional value/max, and optional visible value text.
- `label` names the region and visible heading; it defaults to “Project health.”

The component does not combine indicator values, infer status, or decide whether an increase is beneficial. Changing indicators never changes the supplied health state.

## Accessibility

Every semantic color is paired with a visible status label: Healthy, Needs attention, At risk, or Health unknown. The explanation and “Last updated” prefix are visible. A supplied ISO timestamp is rendered through native `time` semantics. Indicators compose the Progress primitive, retaining visible labels/value text and native progress semantics.

## Responsive behavior

The header wraps the title and status badge at narrow widths. Explanatory text, timestamp labels, and indicator labels wrap without requiring horizontal scrolling. Indicators remain in source order in a single readable column.

## Do / don't

Do calculate health in caller-owned domain logic, provide a plain-language explanation, use a meaningful timestamp, and pass indicators only when they help explain the status.

Don't import Engagement models into the recipe, derive health from indicator values, fetch project data, or rely on badge color without its visible text.

## Visual coverage

Capture healthy, attention, at-risk, and unknown states across light/dark appearances and desktop/mobile widths. Cover multiple indicators and no indicators, long explanatory text, and indeterminate indicator data.

## Public import

```ts
import { ProjectHealthComponent } from 'src/web/design-system/public-api';
```
