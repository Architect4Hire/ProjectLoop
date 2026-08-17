import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';

import { VersionChipComponent, type VersionQualifier } from '../../components/version-chip/version-chip.component';
import { BadgeComponent, type BadgeVariant, SurfaceComponent } from '../../primitives';

export interface DocumentBadgePresentation {
  readonly label: string;
  readonly variant: BadgeVariant;
}

export interface DocumentCardVersionPresentation {
  readonly label: string;
  readonly qualifier?: VersionQualifier;
  readonly qualifierLabel?: string;
}

export interface DocumentUpdatedPresentation {
  readonly label: string;
  readonly dateTime?: string;
}

/** Display-ready document metadata that application code has already authorized. */
export interface DocumentCardViewModel {
  readonly id: string;
  readonly title: string;
  readonly category: string;
  readonly status: DocumentBadgePresentation;
  readonly visibility: DocumentBadgePresentation;
  readonly version: DocumentCardVersionPresentation;
  readonly updated: DocumentUpdatedPresentation;
}

@Component({
  selector: 'lsd-document-card',
  standalone: true,
  imports: [BadgeComponent, SurfaceComponent, VersionChipComponent],
  templateUrl: './document-card.component.html',
  styleUrl: './document-card.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class DocumentCardComponent {
  readonly document = input.required<DocumentCardViewModel>();
  readonly headingLevel = input<2 | 3 | 4>(3);
  readonly actionsLabel = input('Document actions');

  protected readonly headingId = computed(() => `document-${this.document().id}-title`);
}
