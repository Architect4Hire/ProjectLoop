# Application navigation

## Purpose

Use `AppNavigationComponent` to render a responsive list of application destinations that the caller has already authorized. It presents links only and does not know Project Loop routes, inspect authentication state, or decide permissions.

## API and states

- Required `links`: ordered `AppNavigationLink` models with `label`, `href`, and `icon`, plus optional caller-controlled `active` and numeric `count` values.
- Optional `accessibleName`: the navigation label; defaults to `Application`.
- Optional `compact`: reduces links to icon-led controls while retaining labels for assistive technology.
- There are no outputs. Navigation uses native anchors and the caller owns route changes and active-state updates.

```html
<lsd-app-navigation
  accessibleName="Primary"
  [links]="authorizedLinks()"
  [compact]="navigationCompact()" />
```

## Accessibility and keyboard behavior

The component renders a labeled `nav` with an unordered list of Link primitive anchors. A caller-marked active link receives `aria-current="page"`. Icons are decorative because every link has a textual label. Counts use Badge and include a contextual accessible label. Native anchors retain browser focus, keyboard activation, and URL behavior.

## Responsive and compact behavior

The default presentation is a full-width vertical list with visible labels. Compact presentation visually hides labels but preserves them in the accessibility tree. At narrow viewport widths, the list becomes a horizontally scrollable row instead of compressing or wrapping destinations. Labels truncate safely in the expanded presentation.

## Do / don't

Do filter links against permissions before passing them in, supply stable URLs, and mark active state from caller-owned routing. Do use compact mode where available inline space is intentionally constrained. Don't hard-code feature routes in the component, read auth or Router state, or use counts as the only accessible destination label.

## Standalone Angular import

```ts
import { Component } from '@angular/core';
import { AppNavigationComponent } from 'src/web/design-system/public-api';

@Component({ standalone: true, imports: [AppNavigationComponent], templateUrl: './example.html' })
export class NavigationExampleComponent {}
```
