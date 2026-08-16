# Metric card recipe

## Purpose

`MetricCardComponent` presents one business-neutral summary value with an optional trend, context, and native action link. Formatting, calculation, comparison periods, and domain meaning remain caller-owned.

## Usage

```ts
readonly trend: MetricTrend = {
  label: '12% higher than previous period',
  direction: 'up',
};

readonly action: MetricCardAction = {
  label: 'View details',
  href: '/summary/details',
};
```

```html
<lsd-metric-card
  label="Completion"
  value="1,240"
  [trend]="trend"
  context="Compared with the previous period"
  [action]="action"
/>
```

## Inputs

- `label` names the metric and is always visible.
- `value` is required caller-formatted text. The recipe does not parse, calculate, abbreviate, or localize it.
- `trend` accepts a visible `label` and an `up`, `down`, or `neutral` direction. Direction chooses presentation only; callers supply its meaning in text.
- `context` adds optional supporting text.
- `action` accepts a label, `href`, and optional accessible label. It renders as a native anchor through the Link primitive.
- `loading` marks the card `aria-busy`, retains the metric label, and replaces unresolved content with a decorative Skeleton plus loading text.

## Accessibility

Trend meaning is never communicated by color alone: its visible caller-supplied text accompanies a decorative directional icon. Keep trend labels explicit, such as “12% higher than previous period,” rather than “12%.” The native action link retains browser and router-compatible anchor behavior.

## Responsive behavior

The card has no minimum inline width. Long caller-formatted values and context wrap at narrow widths, while value size scales down fluidly. Collections own their grid or list layout.

## Do / don't

Do provide display-ready values, explicit trend meaning, and a meaningful action destination. Use `loading` while a value is unresolved.

Don't pass raw quantities for the recipe to format, infer whether change is good or bad from direction, or use domain-specific fields in the component API.

## Visual regression

Capture light and dark appearances at desktop and mobile widths. Include default, long-value, and loading states, plus each trend direction and cards with and without context or actions.

## Public import

```ts
import { MetricCardComponent } from 'src/web/design-system/public-api';
```
