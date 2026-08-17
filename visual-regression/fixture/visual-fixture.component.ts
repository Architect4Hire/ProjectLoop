import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import {
  AppearanceService,
  AuditTimelineComponent,
  DocumentListComponent,
  DocumentUploadComponent,
  PortalShellComponent,
  ProjectDashboardComponent,
  VersionBoundApprovalComponent,
  approvalStatusPresentation,
  type AppNavigationLink,
  type AuditTimelineEvent,
  type DocumentCardViewModel,
} from '@lsd/design-system';

type Composition = 'shell' | 'dashboard' | 'documents' | 'upload' | 'approval' | 'audit';

@Component({
  selector: 'visual-root',
  standalone: true,
  imports: [AuditTimelineComponent, DocumentListComponent, DocumentUploadComponent, PortalShellComponent, ProjectDashboardComponent, VersionBoundApprovalComponent],
  templateUrl: './visual-fixture.component.html',
  styleUrl: './visual-fixture.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class VisualFixtureComponent {
  protected readonly composition: Composition;
  protected readonly state: string;
  protected readonly longContent: boolean;

  protected readonly navigation: readonly AppNavigationLink[] = [
    { label: 'Overview', href: '#overview', icon: 'menu', active: true },
    { label: 'Documents', href: '#documents', icon: 'info', count: 12 },
    { label: 'Approvals', href: '#approvals', icon: 'check' },
    { label: 'Audit trail', href: '#audit', icon: 'search' },
  ];

  protected readonly documents: readonly DocumentCardViewModel[] = [
    {
      id: 'architecture',
      title: 'Architecture decision record for cross-region resilience and operational continuity',
      category: 'Technical decision',
      status: { label: 'Published', variant: 'approved' },
      visibility: { label: 'Confidential', variant: 'warning' },
      version: { label: 'v4.12.0', qualifier: 'current', qualifierLabel: 'Current' },
      updated: { label: 'Updated 15 August 2026 at 14:32 UTC', dateTime: '2026-08-15T14:32:00Z' },
    },
    {
      id: 'accessibility',
      title: 'Accessibility verification evidence',
      category: 'Quality record',
      status: { label: 'Draft', variant: 'info' },
      visibility: { label: 'Project team', variant: 'neutral' },
      version: { label: 'v3', qualifier: 'approved', qualifierLabel: 'Approved' },
      updated: { label: 'Updated 14 August 2026 at 09:05 UTC', dateTime: '2026-08-14T09:05:00Z' },
    },
  ];

  protected readonly auditEvents: readonly AuditTimelineEvent[] = [
    {
      id: 'audit-1', actor: 'Morgan Chen', action: 'Published',
      resource: { typeLabel: 'Document', label: 'Accessibility verification evidence v3' },
      occurredAt: '2026-08-15T14:32:00Z', timestampLabel: '15 August 2026 at 14:32 UTC',
      correlationId: 'corr_01J5B3ZKFV7ER6T7V8QW4Y2PHM', attribution: 'human-authored',
      redactedDetailsSummary: 'Publication completed after the required approval evidence was recorded.',
    },
    {
      id: 'audit-2', actor: null, action: 'Retained immutable evidence for',
      resource: { typeLabel: 'Approval request', label: 'Cross-region continuity plan v4.12.0' },
      occurredAt: '2026-08-15T14:35:00Z', timestampLabel: '15 August 2026 at 14:35 UTC',
      correlationId: 'corr_01J5B40Y1SM8FQZK9GAD6W03XE_really_long_deterministic_identifier', attribution: 'system',
    },
  ];

  protected readonly requested = approvalStatusPresentation('requested');
  protected readonly categoryOptions = [
    { label: 'Decision record', value: 'decision' },
    { label: 'Quality evidence', value: 'quality' },
  ] as const;
  protected readonly visibilityOptions = [
    { label: 'Project team', value: 'team' },
    { label: 'Confidential', value: 'confidential' },
  ] as const;
  protected readonly approvalTarget = {
    type: 'document' as const,
    typeLabel: 'Document',
    label: 'Cross-region continuity plan',
    versionLabel: 'v3',
    versionQualifier: 'approved' as const,
    versionQualifierLabel: 'Approved version',
  };

  constructor() {
    const params = new URLSearchParams(globalThis.location.search);
    this.composition = (params.get('composition') as Composition | null) ?? 'shell';
    this.state = params.get('state') ?? 'default';
    this.longContent = this.state.includes('long');
    inject(AppearanceService).setAppearance(params.get('appearance') === 'dark' ? 'dark' : 'light');
  }
}
