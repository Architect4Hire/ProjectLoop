import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';

import { AlertBannerComponent } from '../../components/alert-banner/alert-banner.component';
import { VersionChipComponent, type VersionQualifier } from '../../components/version-chip/version-chip.component';
import { BadgeComponent } from '../../primitives/badge/badge.component';
import type { ApprovalStatusPresentation } from '../../tokens/approval-status';

export interface ApprovalRequestTimePresentation {
  readonly label: string;
  readonly dateTime?: string;
}

export interface ApprovalRequestDocumentTarget {
  readonly type: 'document';
  readonly typeLabel: string;
  readonly label: string;
  readonly versionLabel: string;
  readonly versionQualifier?: VersionQualifier;
  readonly versionQualifierLabel?: string;
}

export interface ApprovalRequestOtherTarget {
  readonly type: 'other';
  readonly typeLabel: string;
  readonly label: string;
  readonly versionLabel?: never;
}

export type ApprovalRequestTarget = ApprovalRequestDocumentTarget | ApprovalRequestOtherTarget;

@Component({
  selector: 'lsd-approval-request-banner',
  standalone: true,
  imports: [AlertBannerComponent, BadgeComponent, VersionChipComponent],
  templateUrl: './approval-request-banner.component.html',
  styleUrl: './approval-request-banner.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ApprovalRequestBannerComponent {
  readonly id = input.required<string>();
  readonly target = input.required<ApprovalRequestTarget>();
  readonly status = input.required<ApprovalStatusPresentation>();
  readonly requester = input.required<string>();
  readonly requestedTime = input.required<ApprovalRequestTimePresentation>();
  readonly title = input('Approval request');

  protected readonly validTarget = computed(() => {
    const target = this.target();
    if (target.type === 'document' && !target.versionLabel?.trim()) {
      throw new Error('ApprovalRequestBannerComponent requires an exact versionLabel for document targets.');
    }
    return target;
  });
}
