import { ChangeDetectionStrategy, Component, computed, input, output } from '@angular/core';

import { PaginationComponent } from '../../components/pagination/pagination.component';
import { ButtonComponent } from '../../primitives/button/button.component';
import { AuditEventComponent, type AuditEventResource } from '../../recipes/audit-event/audit-event.component';
import { ActivityStreamComponent, type ActivityAttribution, type ActivityStreamItem } from '../activity-stream/activity-stream.component';
import { ActivityStreamDetailsDirective } from '../activity-stream/activity-stream-details.directive';

export interface AuditTimelineEvent {
  readonly id: string;
  readonly actor?: string | null;
  readonly action: string;
  readonly resource: AuditEventResource;
  readonly occurredAt: string;
  readonly timestampLabel: string;
  readonly correlationId: string;
  readonly attribution?: ActivityAttribution;
  readonly redactedDetailsSummary?: string;
}

export type AuditTimelinePaging =
  | Readonly<{ mode: 'pages'; currentPage: number; totalPages: number }>
  | Readonly<{ mode: 'load-more'; hasMore: boolean; loading?: boolean }>;

@Component({
  selector: 'lsd-audit-timeline',
  standalone: true,
  imports: [ActivityStreamComponent, ActivityStreamDetailsDirective, AuditEventComponent, ButtonComponent, PaginationComponent],
  templateUrl: './audit-timeline.component.html',
  styleUrl: './audit-timeline.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AuditTimelineComponent {
  readonly id = input.required<string>();
  readonly events = input.required<readonly AuditTimelineEvent[]>();
  readonly paging = input.required<AuditTimelinePaging>();
  readonly accessibleName = input('Audit timeline');
  readonly emptyTitle = input('No audit events');
  readonly emptyDescription = input('Audit events will appear here.');

  readonly pageChange = output<number>();
  readonly loadMoreRequested = output<void>();
  readonly correlationCopyRequested = output<string>();

  protected readonly activityItems = computed<readonly ActivityStreamItem<string>[]>(() =>
    this.events().map((event) => ({
      identity: event.id,
      actor: event.actor?.trim() || 'Actor unavailable',
      occurredAt: event.occurredAt,
      timestampLabel: event.timestampLabel,
      action: `${event.action} · ${event.resource.typeLabel} ${event.resource.label}`,
      attribution: event.attribution ?? 'system',
      hasDetails: true,
    })),
  );

  protected event(identity: string): AuditTimelineEvent {
    const auditEvent = this.events().find((candidate) => candidate.id === identity);
    if (!auditEvent) throw new Error(`Audit timeline event ${identity} is unavailable.`);
    return auditEvent;
  }

  protected requestMore(): void {
    const paging = this.paging();
    if (paging.mode === 'load-more' && paging.hasMore && !paging.loading) this.loadMoreRequested.emit();
  }
}
