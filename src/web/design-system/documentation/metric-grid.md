# Metric grid layout

## Purpose

`MetricGridComponent` provides responsive placement for one through four caller-projected `MetricCardComponent` instances. It owns only layout: metric data, rendering, card semantics, and loading remain with callers and the cards.

## Usage

```html
<lsd-metric-grid>
  @for (metric of metrics(); track metric.id) {
    <lsd-metric-card
      [label]="metric.label"
      [value]="metric.formattedValue"
      [trend]="metric.trend"
    />
  }
</lsd-metric-grid>
```

## Projection and ordering

Project one through four Metric Card instances as direct content. The layout uses a single content slot and CSS Grid, so it neither clones nor reorders projected nodes. Screen-reader, keyboard, and DOM order remain the caller's source order.

The layout intentionally adds no list, region, article, or card semantics. Add a caller-owned heading or grouping element around it when the collection needs an accessible name.

## Responsive behavior

Available inline space determines the number of columns. Wide containers accommodate up to four cards, intermediate widths collapse naturally to two or three columns, and narrow containers use one column. Every track can shrink to the container width, preventing overflow from the layout itself.

## Do / don't

Do project already prepared Metric Card instances and keep the most important metric first in source order. Use multiple grids when more than four metrics require meaningful grouping.

Don't fetch or format metric data, apply card roles to the grid, reorder cards visually, or place unrelated feature content in this layout.

## Visual coverage

`metric-grid.visual.spec.ts` covers one, two, three, and four items; light and dark appearances; and desktop, tablet, and mobile widths. The mobile cases verify a single-column composition without changing DOM order.

## Public import

```ts
import { MetricGridComponent } from 'src/web/design-system/public-api';
```
