import { ChangeDetectionStrategy, Component, effect, ElementRef, input, viewChild, computed } from '@angular/core';

import { LinkDirective } from '../../primitives/link/link.directive';
import { AlertBannerComponent } from '../alert-banner/alert-banner.component';

export interface ErrorSummaryItem {
  readonly controlId: string;
  readonly label: string;
}

@Component({
  selector: 'lsd-error-summary',
  standalone: true,
  imports: [AlertBannerComponent, LinkDirective],
  templateUrl: './error-summary.component.html',
  styleUrl: './error-summary.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ErrorSummaryComponent {
  readonly id = input.required<string>();
  readonly errors = input.required<readonly ErrorSummaryItem[]>();
  /** Increment only after a failed submission to request summary focus. */
  readonly failedSubmissionCount = input(0);
  readonly singularTitle = input('There is 1 error');
  readonly pluralTitle = input('There are {count} errors');

  protected readonly summary = viewChild<ElementRef<HTMLElement>>('summary');
  protected readonly title = computed(() =>
    this.errors().length === 1
      ? this.singularTitle()
      : this.pluralTitle().replace('{count}', String(this.errors().length)),
  );

  private lastFocusedSubmission = 0;
  private readonly focusAfterFailedSubmission = effect(() => {
    const attempt = this.failedSubmissionCount();
    const errors = this.errors();
    const summary = this.summary();

    if (attempt <= this.lastFocusedSubmission || attempt <= 0 || errors.length === 0 || !summary) return;
    this.lastFocusedSubmission = attempt;
    queueMicrotask(() => summary.nativeElement.focus());
  });
}
