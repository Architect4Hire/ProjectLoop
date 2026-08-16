# User menu

## Purpose

Use `UserMenuComponent<TAction>` to present display-safe account identity and caller-owned account actions through the Menu primitive. The component is presentation and intent only: it does not fetch profile data, switch tenants, sign out, or make authorization decisions.

## API and states

- Required stable `id`, `displayName`, and typed `actions`.
- Optional `identityDetail` for display-safe secondary text such as an email address.
- Optional `avatarUrl`; when absent, the component derives a two-character initials fallback from `displayName`.
- Each `UserMenuAction<TAction>` supplies an identity, label, and optional caller-controlled disabled state.
- `actionRequested` emits the selected action identity. Emission does not imply that the application action completed.

```html
<lsd-user-menu
  id="account-menu"
  [displayName]="safeProfile.name"
  [identityDetail]="safeProfile.email"
  [avatarUrl]="safeProfile.avatarUrl"
  [actions]="authorizedActions"
  (actionRequested)="handleAccountIntent($event)" />
```

## Accessibility and keyboard behavior

The Menu primitive supplies the named native trigger, `aria-expanded` and menu relationship, managed Arrow/Home/End focus, Escape dismissal, outside dismissal, and focus restoration. The avatar image is decorative because the adjacent identity text names the user. Initials provide the visual fallback without replacing the trigger's accessible name. Actions are native buttons and disabled actions use native disabled semantics.

## Responsive behavior

Long identity values remain complete in the accessibility tree and are visually truncated within bounded trigger and menu regions. The trigger remains usable at narrow widths, and the menu uses its existing anchored overlay behavior. Semantic surface, text, border, and focus tokens support both appearances; no motion is introduced.

## Do / don't

Do pass only display-safe identity fields and caller-authorized actions, use a stable unique ID, and handle emitted intents in application code. Don't pass secrets, fetch profile data in the component, encode tenant-switching or sign-out behavior, or assume an emitted intent succeeded.

## Standalone Angular import

```ts
import { Component } from '@angular/core';
import { UserMenuComponent } from 'src/web/design-system/public-api';

@Component({ standalone: true, imports: [UserMenuComponent], templateUrl: './example.html' })
export class UserMenuExampleComponent {}
```
