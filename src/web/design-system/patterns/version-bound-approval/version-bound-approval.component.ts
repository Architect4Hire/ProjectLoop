import { ChangeDetectionStrategy, Component, input, model, output } from '@angular/core';

import { VersionChipComponent } from '../../components/version-chip/version-chip.component';
import {
  ApprovalActionsComponent,
  type ApprovalProcessingAction,
  type ApprovalReviewStatus,
} from '../../recipes/approval-actions/approval-actions.component';
import { ApprovalCommentFieldComponent } from '../../recipes/approval-comment-field/approval-comment-field.component';
import {
  ApprovalRequestBannerComponent,
  type ApprovalRequestDocumentTarget,
  type ApprovalRequestTimePresentation,
} from '../../recipes/approval-request-banner/approval-request-banner.component';
import type { ApprovalStatusPresentation } from '../../tokens/approval-status';
import type { ReviewProvenance } from '../review-approval/review-approval.component';

export type VersionBoundApprovalDecision = 'approve' | 'reject' | 'request-change';

export interface VersionBoundApprovalIntent {
  readonly decision: VersionBoundApprovalDecision;
  readonly target: ApprovalRequestDocumentTarget;
  readonly comment: string;
}

export interface VersionBoundCurrentVersion {
  readonly label: string;
  readonly qualifierLabel?: string;
}

@Component({
  selector: 'lsd-version-bound-approval',
  standalone: true,
  imports: [
    ApprovalActionsComponent,
    ApprovalCommentFieldComponent,
    ApprovalRequestBannerComponent,
    VersionChipComponent,
  ],
  templateUrl: './version-bound-approval.component.html',
  styleUrl: './version-bound-approval.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class VersionBoundApprovalComponent {
  readonly id = input.required<string>();
  readonly target = input.required<ApprovalRequestDocumentTarget>();
  readonly requestStatus = input.required<ApprovalStatusPresentation>();
  readonly requester = input.required<string>();
  readonly requestedTime = input.required<ApprovalRequestTimePresentation>();
  readonly provenance = input.required<ReviewProvenance>();
  readonly currentVersion = input<VersionBoundCurrentVersion | undefined>(undefined);

  readonly comment = model('');
  readonly commentRequired = input(false);
  readonly commentMaxLength = input(500);
  readonly commentHelp = input('Explain the reason for this decision.');
  readonly commentError = input<string | undefined>(undefined);

  readonly reviewStatus = input<ApprovalReviewStatus>('pending');
  readonly processing = input<ApprovalProcessingAction | null>(null);
  readonly warnings = input<readonly string[]>([]);
  readonly approveDisabled = input(false);
  readonly rejectDisabled = input(false);
  readonly requestChangeDisabled = input(false);

  readonly decisionIntent = output<VersionBoundApprovalIntent>();

  protected emitDecision(decision: VersionBoundApprovalDecision): void {
    this.decisionIntent.emit({ decision, target: this.target(), comment: this.comment() });
  }
}
