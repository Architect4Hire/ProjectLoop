import {
  booleanAttribute,
  ChangeDetectionStrategy,
  Component,
  computed,
  input,
  model,
  numberAttribute,
} from '@angular/core';

import { FieldMessageComponent } from '../../primitives/field-message/field-message.component';
import { TextareaComponent } from '../../primitives/textarea/textarea.component';

@Component({
  selector: 'lsd-approval-comment-field',
  standalone: true,
  imports: [FieldMessageComponent, TextareaComponent],
  templateUrl: './approval-comment-field.component.html',
  styleUrl: './approval-comment-field.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ApprovalCommentFieldComponent {
  readonly id = input.required<string>();
  readonly value = model('');
  readonly required = input(false, { transform: booleanAttribute });
  readonly maxLength = input.required<number, unknown>({ transform: numberAttribute });
  readonly help = input('Explain the reason for this decision.');
  readonly error = input<string | undefined>(undefined);
  readonly label = input('Decision comment');
  readonly rows = input(5, { transform: numberAttribute });
  readonly disabled = input(false, { transform: booleanAttribute });

  protected readonly explicitLabel = computed(() =>
    `${this.label()} (${this.required() ? 'required' : 'optional'})`,
  );
  protected readonly countId = computed(() => `${this.id()}-count`);
  protected readonly countText = computed(() => `${this.value().length} of ${this.maxLength()} characters`);
}
