import { ChangeDetectionStrategy, Component, input } from '@angular/core';

import { VersionChipComponent } from '../../components/version-chip/version-chip.component';
import { BadgeComponent, type BadgeVariant } from '../../primitives/badge/badge.component';
import { LinkDirective } from '../../primitives/link/link.directive';
import { SurfaceComponent } from '../../primitives/surface/surface.component';
import type { ApprovalRequestTarget } from '../approval-request-banner/approval-request-banner.component';

export interface PendingApprovalDuePresentation {
  readonly label: string;
  readonly variant: BadgeVariant;
  readonly dateTime?: string;
}

export interface PendingApprovalReviewNavigation {
  readonly href: string;
  readonly accessibleLabel?: string;
}

export interface PendingApprovalItem {
  readonly id: string;
  readonly target: ApprovalRequestTarget;
  readonly requester: string;
  readonly due: PendingApprovalDuePresentation;
  readonly review: PendingApprovalReviewNavigation;
}

@Component({
  selector: 'lsd-pending-approvals-list',
  standalone: true,
  imports: [BadgeComponent, LinkDirective, SurfaceComponent, VersionChipComponent],
  templateUrl: './pending-approvals-list.component.html',
  styleUrl: './pending-approvals-list.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PendingApprovalsListComponent {
  readonly items = input.required<readonly PendingApprovalItem[]>();
  readonly label = input('Pending approval requests');
  readonly emptyMessage = input('No pending approval requests.');
  readonly reviewLabel = input('Review');
}
