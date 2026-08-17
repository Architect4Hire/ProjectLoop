import { ChangeDetectionStrategy, Component, input } from '@angular/core';

import { DocumentCardComponent, type DocumentCardViewModel } from '../../recipes/document-card/document-card.component';
import { DocumentRowComponent } from '../../recipes/document-row/document-row.component';
import { StateFeedbackComponent } from '../state-feedback/state-feedback.component';

@Component({
  selector: 'lsd-document-list',
  standalone: true,
  imports: [DocumentCardComponent, DocumentRowComponent, StateFeedbackComponent],
  templateUrl: './document-list.component.html',
  styleUrl: './document-list.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class DocumentListComponent {
  readonly id = input.required<string>();
  readonly accessibleName = input('Documents');
  readonly documents = input.required<readonly DocumentCardViewModel[]>();
  readonly emptyTitle = input('No documents');
  readonly emptyDescription = input<string | undefined>(undefined);
}
