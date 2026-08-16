# Breadcrumb

## Purpose

Use `BreadcrumbComponent` to show the current page's position within a caller-defined hierarchy. The component presents navigation only; it does not inspect Angular Router state or infer application structure.

## API and states

- Required `items`: ordered `BreadcrumbItem` values containing a display `label` and native-anchor `href`.
- Optional `accessibleName`: the navigation label; defaults to `Breadcrumb`.
- The final item is always the current page. There are no loading, disabled, or application-routing variants.

```html
<lsd-breadcrumb
  accessibleName="Project location"
  [items]="[
    { label: 'Projects', href: '/projects' },
    { label: 'Requirements', href: '/projects/current/requirements' }
  ]" />
```

## Accessibility and keyboard behavior

The component renders a labeled `nav` containing an ordered list. Every item uses the Link primitive's native anchor semantics, and only the terminal link receives `aria-current="page"`. Chevron icons are decorative. Links retain normal browser keyboard focus and navigation behavior.

## Responsive behavior

The ordered trail scrolls horizontally at narrow widths instead of wrapping into an ambiguous hierarchy. Long labels are visually truncated, while their complete text remains in the accessibility tree. Semantic text and focus styles work across supported appearances.

## Do / don't

Do supply items in hierarchy order, stable URLs, concise labels, and a specific navigation label when more than one breadcrumb exists. Don't derive domain hierarchy inside the design system, inspect router state, omit the current page, or replace the application page heading with breadcrumbs.

## Standalone Angular import

```ts
import { Component } from '@angular/core';
import { BreadcrumbComponent } from 'src/web/design-system/public-api';

@Component({ standalone: true, imports: [BreadcrumbComponent], templateUrl: './example.html' })
export class BreadcrumbExampleComponent {}
```
