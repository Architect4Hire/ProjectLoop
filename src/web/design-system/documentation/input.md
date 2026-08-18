# Input primitive

Traceability: DS-003, DS-007, DS-008.

`InputComponent` provides the design-system contract for single-line textual input. Import it from the public design-system API.

```html
<lsd-input
  id="engagement-name"
  label="Engagement name"
  description="Use the client-facing name."
  [error]="nameError()"
  required
  [(value)]="name" />
```

## Contract

- `id` and `label` are required typed inputs. The caller-supplied stable ID associates the native control, label, description, and error without hydration-sensitive generated IDs.
- `value` is model-signal state and supports `[(value)]` binding.
- `type`: `text | email | password | search | tel | url`; `inputMode` and common `autocomplete` values are typed independently.
- `density`: `compact | default | comfortable`, aligned with Textarea while preserving native text entry.
- `required`, `disabled`, and `readonly` are forwarded to native input properties.
- `description` is associated through `aria-describedby`.
- `error` sets `aria-invalid`, `aria-errormessage`, extends `aria-describedby`, and renders an atomic assertive Field Message without interpreting or mutating caller validation state.
- Prefix and suffix content use `lsdInputPrefix` and `lsdInputSuffix`. They are decorative; include any essential meaning such as units or currency in the label or description as well.

The native `<input>` owns text editing, selection, tab order, and keyboard behavior. Readonly remains focusable and selectable; disabled is removed from the tab order and suppresses editing. The shared interaction foundation supplies visibly distinct focus, disabled, readonly, invalid, and forced-color treatments. Component colors use only semantic surface, text, border, and danger tokens, which resolve in both light and dark appearances. Placeholder text supplements rather than replaces the required visible label.

The starter's page-specific search field was not retained because it did not expose reusable label, state, or error-association APIs.
