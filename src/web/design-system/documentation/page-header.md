# Page header

## Purpose

Use `PageHeaderComponent` to establish page context with a title, optional description, breadcrumb trail, neutral metadata, and caller-projected actions. The pattern controls hierarchy and layout only; it does not define feature fields or action behavior.

## API and states

- Required `title`.
- Optional `headingLevel` from 1 through 6; defaults to 1 and renders the corresponding native heading element.
- Optional `description`, `breadcrumbs`, `breadcrumbLabel`, and ordered `metadata` label/value pairs.
- Project caller-owned controls through an element marked `lsdPageHeaderActions`.
- Empty breadcrumb, description, and metadata inputs omit those regions rather than rendering placeholders.

```html
<lsd-page-header
  title="Requirements"
  [breadcrumbs]="location"
  [metadata]="summary">
  <div lsdPageHeaderActions><!-- caller-owned buttons --></div>
</lsd-page-header>
```

## Accessibility and heading hierarchy

The selected level produces exactly one native `h1`–`h6`; callers choose the level that follows the surrounding document outline. Breadcrumb semantics come from `BreadcrumbComponent`, metadata uses a description list, and the closing Separator is decorative. The action region is labeled while projected controls retain their own accessible names and keyboard behavior.

## Responsive behavior

At wide widths, title content and actions share a row and actions wrap within their region. Below the tablet breakpoint, actions move after the title and description, use the full available width, wrap, and remain left aligned. Breadcrumbs preserve their own horizontal overflow behavior, metadata wraps, and long caller text breaks safely.

## Do / don't

Do choose the heading level from the page's existing hierarchy, keep metadata display-safe and concise, and project design-system action controls. Don't encode feature-specific fields in this pattern, skip heading levels for visual size, duplicate the page title, or move primary actions before the heading in DOM order.

## Visual coverage

`page-header.visual.spec.ts` defines light and dark desktop cases plus a narrow mobile case with wrapped actions.

## Standalone Angular import

```ts
import { Component } from '@angular/core';
import { PageHeaderComponent } from 'src/web/design-system/public-api';

@Component({ standalone: true, imports: [PageHeaderComponent], templateUrl: './example.html' })
export class PageHeaderExampleComponent {}
```
