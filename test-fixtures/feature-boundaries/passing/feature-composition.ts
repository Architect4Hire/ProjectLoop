import { Component } from '@angular/core';
import { ButtonComponent, SurfaceComponent } from '@lsd/design-system';

@Component({
  selector: 'feature-approval-summary',
  standalone: true,
  imports: [ButtonComponent, SurfaceComponent],
  template: `
    <lsd-surface>
      <p class="grid gap-2">Approval-specific summary</p>
      <lsd-button>Review</lsd-button>
    </lsd-surface>
  `,
})
export class FeatureApprovalSummaryComponent {}

export const uniqueBusinessLayout =
  'grid grid-cols-[minmax(0,1fr)_auto] items-start gap-6 rounded-lg border border-border-default p-5';
