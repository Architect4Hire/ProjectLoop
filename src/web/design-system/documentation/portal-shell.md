# Portal shell layout

## Purpose

Use `PortalShellComponent` as the semantic frame for an authenticated portal. It composes the public Skip Link and Application Navigation APIs and provides slots for caller-owned header, routed main content, and notification viewport content. It is distinct from the consulting-oriented `WorkbenchShellRecipeComponent` and contains no feature content.

## API and states

- Required `navigationLinks`: caller-authorized `AppNavigationLink` models, including caller-owned active state.
- Optional `navigationLabel`, `navigationCompact`, `mainLabel`, and `notificationsLabel`.
- Project header content with `lsdPortalHeader`, the application's router outlet or page content with `lsdPortalMain`, and a public `NotificationViewportComponent` with `lsdPortalNotifications`.
- The layout has no outputs and does not read Router, authentication, profile, tenant, or permission state.

```html
<lsd-portal-shell [navigationLinks]="authorizedLinks()">
  <app-portal-header lsdPortalHeader />
  <router-outlet lsdPortalMain />
  <lsd-notification-viewport lsdPortalNotifications />
</lsd-portal-shell>
```

## Accessibility and landmarks

The projected header sits in the page `header` banner. `AppNavigationComponent` supplies the labeled `nav`. The projected outlet sits in a labeled `main` with the stable ID `main-content` and `tabindex="-1"`; the composed Skip Link targets that element. The shell owns a labeled complementary landmark around caller-projected notification content; callers should not add a second `aside` around that content. Only one portal shell should exist in a document.

## Responsive behavior

Desktop layouts use a persistent 16rem navigation rail beside main content. Below the tablet breakpoint, navigation moves above main content; Application Navigation supplies its horizontally scrollable narrow presentation. Main padding becomes compact. The header and projected controls own their internal responsive behavior.

## Do / don't

Do pass already-authorized links, derive active state from caller-owned routing, project only global header controls, and place feature content through the main slot. Don't inspect auth or route state in the layout, add feature content, project unfiltered permissions, or substitute this layout for `WorkbenchShellRecipeComponent`.

## Visual coverage

`portal-shell.visual.spec.ts` defines light and dark desktop rail cases and light and dark mobile horizontal-navigation cases. Component specs cover named landmarks, the focus target, public navigation composition, and slot projection.

## Standalone Angular import

```ts
import { Component } from '@angular/core';
import { PortalShellComponent } from 'src/web/design-system/public-api';

@Component({ standalone: true, imports: [PortalShellComponent], templateUrl: './app.html' })
export class PortalAppComponent {}
```
