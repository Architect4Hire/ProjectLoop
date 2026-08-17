import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';

import { VersionChipComponent } from '../../components/version-chip/version-chip.component';
import { ActivityStreamComponent, type ActivityAttribution, type ActivityStreamItem } from '../activity-stream/activity-stream.component';
import { ActivityStreamDetailsDirective } from '../activity-stream/activity-stream-details.directive';

export interface ApprovalHistoryEntry {
  readonly id: string;
  readonly decision: string;
  readonly actor: string;
  readonly occurredAt: string;
  readonly timestampLabel: string;
  readonly commentSummary: string;
  readonly targetTypeLabel: string;
  readonly targetLabel: string;
  readonly versionLabel: string;
  readonly attribution?: ActivityAttribution;
}

@Component({
  selector: 'lsd-approval-history',
  standalone: true,
  imports: [ActivityStreamComponent, ActivityStreamDetailsDirective, VersionChipComponent],
  templateUrl: './approval-history.component.html',
  styleUrl: './approval-history.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ApprovalHistoryComponent {
  readonly id = input.required<string>();
  readonly entries = input.required<readonly ApprovalHistoryEntry[]>();
  readonly accessibleName = input('Approval history');
  readonly emptyTitle = input('No approval history');
  readonly emptyDescription = input('Approval decisions will appear here.');

  protected readonly activityItems = computed<readonly ActivityStreamItem<string>[]>(() =>
    this.entries().map((entry) => ({
      identity: entry.id,
      actor: entry.actor,
      occurredAt: entry.occurredAt,
      timestampLabel: entry.timestampLabel,
      action: `${entry.decision} ${entry.targetTypeLabel} ${entry.targetLabel} ${entry.versionLabel}`,
      attribution: entry.attribution ?? 'human-authored',
      hasDetails: true,
    })),
  );

  protected entry(identity: string): ApprovalHistoryEntry {
    const evidence = this.entries().find((candidate) => candidate.id === identity);
    if (!evidence) throw new Error(`Approval history entry ${identity} is unavailable.`);
    return evidence;
  }
}
