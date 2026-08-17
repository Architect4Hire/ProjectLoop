import { Directive, input, TemplateRef } from '@angular/core';

import type { DocumentHistoryVersion } from './document-version-history.component';

export interface DocumentVersionHistoryActionContext {
  readonly $implicit: DocumentHistoryVersion;
  readonly index: number;
  readonly count: number;
}

@Directive({
  selector: 'ng-template[lsdDocumentVersionActions]',
  standalone: true,
})
export class DocumentVersionHistoryActionsDirective {
  readonly versionId = input.required<string>({ alias: 'lsdDocumentVersionActions' });

  constructor(readonly template: TemplateRef<DocumentVersionHistoryActionContext>) {}

  static ngTemplateContextGuard(
    _directive: DocumentVersionHistoryActionsDirective,
    context: unknown,
  ): context is DocumentVersionHistoryActionContext {
    return true;
  }
}
