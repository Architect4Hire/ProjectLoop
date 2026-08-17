# Confirmation dialog

## Purpose

`ConfirmationDialogComponent` is a business-neutral confirmation composition for consequential actions. It requires explicit action copy, consequence text, processing state, cancel behavior, and caller-selected visual tone while emitting intent only.

## API and states

- Required `id`, `title`, `consequenceText`, `actionLabel`, and `actionTone` make the action and consequence explicit. The component never infers danger from wording or domain state.
- `open` is caller-controlled. The caller closes the dialog after accepting `confirmed` or `cancelled` intent.
- `processing` locks confirm, cancel, Escape, backdrop, and close-button dismissal. `processingLabel` replaces the confirmation label accessibly while work is pending.
- `cancelLabel` defaults to `Cancel`.
- `confirmed` performs no operation. `cancelled` identifies `cancel-button`, `escape`, `backdrop`, or `close-button` intent.

```html
<lsd-confirmation-dialog
  id="consequence-confirmation"
  title="Confirm action"
  consequenceText="This change affects all collaborators."
  actionLabel="Apply change"
  actionTone="warning"
  [open]="confirmationOpen()"
  [processing]="saving()"
  (confirmed)="performAuthorizedAction()"
  (cancelled)="closeConfirmation()" />
```

## Accessibility and responsive behavior

Dialog supplies native modal semantics, labeling, initial focus inside the modal, Escape handling, and trigger-focus restoration. Buttons retain native disabled/loading behavior. Consequence text remains visible and is also the dialog description. At narrow widths actions stack with the safer cancel action last in visual order while DOM order remains cancel then confirm.

## Do / don't

Do choose the action tone explicitly, describe the concrete consequence, own authorization, and keep the dialog open during processing. Don't perform deletion, publication, cancellation, persistence, or severity inference inside this recipe.

## Public import

```ts
import { ConfirmationDialogComponent } from 'src/web/design-system/public-api';
```
