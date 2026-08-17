import { ChangeDetectionStrategy, Component, input } from '@angular/core';

import { VersionChipComponent } from '../../components/version-chip/version-chip.component';
import { BadgeComponent } from '../../primitives/badge/badge.component';
import type { DocumentCardViewModel } from '../document-card/document-card.component';

@Component({
  selector: 'tr[lsdDocumentRow]',
  standalone: true,
  imports: [BadgeComponent, VersionChipComponent],
  templateUrl: './document-row.component.html',
  styleUrl: './document-row.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class DocumentRowComponent {
  readonly document = input.required<DocumentCardViewModel>();
  readonly actionsLabel = input('Document actions');
}
