import { NgTemplateOutlet } from '@angular/common';
import { ChangeDetectionStrategy, Component, contentChildren, input } from '@angular/core';

import { VersionChipComponent, type VersionQualifier } from '../../components/version-chip/version-chip.component';
import { SurfaceComponent } from '../../primitives/surface/surface.component';
import {
  DocumentVersionHistoryActionsDirective,
  type DocumentVersionHistoryActionContext,
} from './document-version-history-actions.directive';

export interface DocumentHistoryVersion {
  readonly id: string;
  /** Exact caller-formatted identifier rendered by Version Chip. */
  readonly versionLabel: string;
  readonly qualifier?: VersionQualifier;
  readonly qualifierLabel?: string;
  readonly actor: string;
  readonly occurredAt: string;
  readonly timestampLabel: string;
}

@Component({
  selector: 'lsd-document-version-history',
  standalone: true,
  imports: [NgTemplateOutlet, SurfaceComponent, VersionChipComponent],
  templateUrl: './document-version-history.component.html',
  styleUrl: './document-version-history.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class DocumentVersionHistoryComponent {
  readonly id = input.required<string>();
  readonly accessibleName = input('Document version history');
  /** Versions are rendered unchanged in the caller-supplied chronological order. */
  readonly versions = input.required<readonly DocumentHistoryVersion[]>();
  readonly actionsLabel = input('Version actions');

  private readonly actionTemplates = contentChildren(DocumentVersionHistoryActionsDirective);

  protected actionsFor(versionId: string): DocumentVersionHistoryActionsDirective | undefined {
    return this.actionTemplates().find((actions) => actions.versionId() === versionId);
  }

  protected actionContext(
    version: DocumentHistoryVersion,
    index: number,
  ): DocumentVersionHistoryActionContext {
    return { $implicit: version, index, count: this.versions().length };
  }
}
