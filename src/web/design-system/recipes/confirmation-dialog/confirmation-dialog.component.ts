import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';

import { DialogComponent, type DialogCloseReason } from '../../primitives/dialog/dialog.component';
import { ButtonComponent, type ButtonTone } from '../../primitives/button/button.component';

export type ConfirmationCancelReason = 'cancel-button' | DialogCloseReason;

@Component({
  selector: 'lsd-confirmation-dialog',
  standalone: true,
  imports: [ButtonComponent, DialogComponent],
  templateUrl: './confirmation-dialog.component.html',
  styleUrl: './confirmation-dialog.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ConfirmationDialogComponent {
  readonly id = input.required<string>();
  readonly title = input.required<string>();
  readonly consequenceText = input.required<string>();
  readonly actionLabel = input.required<string>();
  readonly actionTone = input.required<ButtonTone>();
  readonly open = input(false);
  readonly processing = input(false);
  readonly processingLabel = input('Processing');
  readonly cancelLabel = input('Cancel');

  readonly confirmed = output<void>();
  readonly cancelled = output<ConfirmationCancelReason>();

  protected requestConfirmation(): void {
    if (!this.processing()) this.confirmed.emit();
  }

  protected requestCancellation(reason: ConfirmationCancelReason): void {
    if (!this.processing()) this.cancelled.emit(reason);
  }
}
