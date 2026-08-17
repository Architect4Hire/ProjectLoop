# Approval comment field

## Purpose

`ApprovalCommentFieldComponent` presents a controlled decision-comment textarea with explicit required or optional labeling, help, character count, and caller-owned validation. It does not decide whether a comment is required or valid.

## API and states

- `id` and `maxLength` are required inputs. The maximum supplies display policy for the count; validation remains caller-owned.
- `value` is model state for `[value]`/`(valueChange)` or two-way binding.
- `required` defaults to false and is always written visibly into the label as `required` or `optional`, in addition to native required semantics.
- `help`, `label`, `rows`, and `disabled` customize the presentation.
- `error` is caller-controlled. When present, Textarea supplies `aria-invalid`, `aria-errormessage`, and an assertive error message.
- The Field Message composition displays `current of maximum characters`; the recipe reports length but does not truncate or decide validity.

```html
<lsd-approval-comment-field
  id="decision-comment"
  [required]="commentRequired"
  [maxLength]="500"
  [error]="commentError"
  [(value)]="comment" />
```

## Accessibility

Textarea associates its native control with the visible label, help, and any error. Required and optional policy is explicit text rather than an asterisk alone. The stable count ID comes from the field ID, and errors use the Field Message-compatible alert behavior already provided by Textarea.

## Responsive behavior

The field fills its container and retains native vertical resizing. Help, errors, and the character count wrap at narrow widths without changing DOM or reading order.

## Do / don't

Do pass the current policy and validation result from the caller. Do localize label, help, and error text when needed. Don't infer that rejection or any other decision requires a comment, and don't use this recipe to make an approval decision.

## Public import

```ts
import { ApprovalCommentFieldComponent } from 'src/web/design-system/public-api';
```
