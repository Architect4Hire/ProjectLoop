import { ChangeDetectionStrategy, Component, computed, input, output } from '@angular/core';

import { BadgeComponent, ButtonComponent, SurfaceComponent } from '../../primitives';

export interface AuditEventResource {
  readonly typeLabel: string;
  readonly label: string;
}

export interface AuditEventTimePresentation {
  readonly label: string;
  readonly dateTime: string;
}

@Component({
  selector: 'lsd-audit-event',
  standalone: true,
  imports: [BadgeComponent, ButtonComponent, SurfaceComponent],
  templateUrl: './audit-event.component.html',
  styleUrl: './audit-event.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AuditEventComponent {
  readonly id = input.required<string>();
  readonly actor = input<string | null | undefined>(undefined);
  readonly action = input.required<string>();
  readonly resource = input.required<AuditEventResource>();
  readonly occurredAt = input.required<AuditEventTimePresentation>();
  readonly correlationId = input.required<string>();
  readonly hasDetails = input(false);
  readonly unavailableActorLabel = input('Actor unavailable');
  readonly detailsLabel = input('Redacted event details');

  readonly correlationCopyRequested = output<string>();

  protected readonly actorLabel = computed(() => this.actor()?.trim() || this.unavailableActorLabel());
  protected readonly headingId = computed(() => `${this.id()}-action`);
}
