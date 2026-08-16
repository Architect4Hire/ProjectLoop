# Pagination

## Purpose

Use `PaginationComponent` to request movement through caller-owned pages. The component presents current and boundary state only; application code owns totals, fetching, caching, URL synchronization, and acceptance of each request.

## API and states

- Required `currentPage`: the caller-controlled one-based current page.
- Required `totalPages`: the caller-calculated page count.
- Optional `accessibleName`: the navigation label; defaults to `Pagination`.
- Output `pageChange`: emits the requested one-based previous or next page without changing component state.
- Previous is disabled on page 1. Next is disabled on the final page. With zero pages, both controls are disabled and the supplied state remains visible.

```html
<lsd-pagination
  accessibleName="Search result pages"
  [currentPage]="page()"
  [totalPages]="pageCount()"
  (pageChange)="requestPage($event)" />
```

## Accessibility and keyboard behavior

A labeled `nav` contains Button primitive controls with explicit “Previous page” and “Next page” accessible names. The visible `Page n of total` status has `aria-current="page"`. Native disabled semantics prevent boundary requests, and native buttons retain Enter and Space keyboard activation plus visible focus treatment.

## Responsive behavior

Controls wrap as a group on narrow viewports and retain their touch targets. The current-page status remains visible and centered. Semantic text and interaction tokens work in supported appearances, and the component introduces no motion.

## Do / don't

Do keep `currentPage` and `totalPages` valid and update the controlled page only after accepting an emitted request. Do provide a contextual accessible name when multiple pagination regions exist. Don't fetch inside the component, infer totals, couple it to DataTable, or treat emission as proof that loading succeeded.

## Standalone Angular import

```ts
import { Component } from '@angular/core';
import { PaginationComponent } from 'src/web/design-system/public-api';

@Component({ standalone: true, imports: [PaginationComponent], templateUrl: './example.html' })
export class PaginationExampleComponent {}
```
