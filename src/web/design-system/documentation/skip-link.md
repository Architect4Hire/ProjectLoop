# Skip link

## Purpose

Use `SkipLinkComponent` once near the start of the application document so keyboard users can bypass repeated navigation and move directly to the application main content.

## API and states

The component has no inputs, outputs, or visual variants. It renders “Skip to main content” as a native anchor composed with `LinkDirective`, with a fixed `href` of `#main-content`. The application owns the matching `<main id="main-content">`; adding `tabindex="-1"` lets browsers place focus on that otherwise non-focusable landmark after fragment navigation.

```html
<lsd-skip-link />
<main id="main-content" tabindex="-1">
  <router-outlet />
</main>
```

## Accessibility and responsive behavior

The link is positioned off screen until it receives visible keyboard focus, then appears at the viewport's block-start edge above application content. It uses native anchor activation and does not intercept navigation. If the target is missing, the browser handles the unresolved fragment without a component error; treat that as an application integration defect. Its position and touch target are the same at narrow and wide viewport sizes.

## Do / don't

Do render one skip link before repeated shell navigation and give the application main landmark the exact stable ID `main-content`. Do keep the main landmark present across route changes. Don't use this component to create a shell, replace landmark semantics, or point to feature-specific content.

## Standalone Angular import

```ts
import { Component } from '@angular/core';
import { SkipLinkComponent } from 'src/web/design-system/public-api';

@Component({ standalone: true, imports: [SkipLinkComponent], templateUrl: './app.html' })
export class AppComponent {}
```
