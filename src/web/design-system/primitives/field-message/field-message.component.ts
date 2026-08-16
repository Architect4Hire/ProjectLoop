import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';

export type FieldMessageKind = 'help' | 'error' | 'success';

@Component({
  selector: 'lsd-field-message',
  standalone: true,
  host: { '[attr.id]': 'null' },
  templateUrl: './field-message.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class FieldMessageComponent {
  readonly id = input.required<string>();
  readonly kind = input<FieldMessageKind>('help');

  protected readonly role = computed(() => {
    if (this.kind() === 'error') return 'alert';
    if (this.kind() === 'success') return 'status';
    return null;
  });

  protected readonly live = computed(() => {
    if (this.kind() === 'error') return 'assertive';
    if (this.kind() === 'success') return 'polite';
    return null;
  });

  protected readonly classes = computed(() =>
    [
      'lsd-field-message m-0 text-sm',
      this.kind() === 'help' ? 'text-text-muted' : 'font-medium',
      this.kind() === 'error' ? 'text-status-danger' : '',
      this.kind() === 'success' ? 'text-status-success' : '',
    ]
      .filter(Boolean)
      .join(' '),
  );
}
