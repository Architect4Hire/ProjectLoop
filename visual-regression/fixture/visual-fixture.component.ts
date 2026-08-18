import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import {
  AppearanceService,
  AiGenerationProgressComponent,
  AlertBannerComponent,
  ApprovalHistoryComponent,
  AuditTimelineComponent,
  ButtonComponent,
  CheckboxComponent,
  CommandPaletteComponent,
  ConfirmationDialogComponent,
  DialogComponent,
  DialogInitialFocusDirective,
  DocumentListComponent,
  DocumentDownloadActionComponent,
  DocumentFiltersComponent,
  DocumentUploadComponent,
  DocumentVersionHistoryComponent,
  DrawerComponent,
  DrawerInitialFocusDirective,
  InputComponent,
  PortalShellComponent,
  ProjectDashboardComponent,
  LinkDirective,
  MenuComponent,
  MenuItemDirective,
  MetricCardComponent,
  MetricGridComponent,
  MilestoneListComponent,
  PageHeaderComponent,
  PaginationComponent,
  PendingApprovalsListComponent,
  RadioGroupComponent,
  ProgressComponent,
  ProjectHealthComponent,
  RecentDecisionsComponent,
  SelectComponent,
  StateFeedbackComponent,
  TabPanelDirective,
  TabsComponent,
  TextareaComponent,
  TooltipComponent,
  TooltipTriggerDirective,
  UpcomingMeetingsComponent,
  UserMenuComponent,
  VersionBoundApprovalComponent,
  VersionChipComponent,
  approvalStatusPresentation,
  type AppNavigationLink,
  type ApprovalHistoryEntry,
  type AuditTimelineEvent,
  type DocumentCardViewModel,
  type DocumentHistoryVersion,
  type CommandPaletteGroup,
  type MeetingSummary,
  type MilestoneSummary,
  type PendingApprovalItem,
  type RecentDecisionRecord,
  type RadioOption,
  type SelectOption,
  type TabItem,
  type UserMenuAction,
} from '@lsd/design-system';

type Composition = 'shell' | 'dashboard' | 'documents' | 'document-detail' | 'upload' | 'approval' | 'audit' | 'controls' | 'fields' | 'choices' | 'overlays' | 'feedback';

@Component({
  selector: 'visual-root',
  standalone: true,
  imports: [AiGenerationProgressComponent, AlertBannerComponent, ApprovalHistoryComponent, AuditTimelineComponent, ButtonComponent, CheckboxComponent, CommandPaletteComponent, ConfirmationDialogComponent, DialogComponent, DialogInitialFocusDirective, DocumentDownloadActionComponent, DocumentFiltersComponent, DocumentListComponent, DocumentUploadComponent, DocumentVersionHistoryComponent, DrawerComponent, DrawerInitialFocusDirective, InputComponent, LinkDirective, MenuComponent, MenuItemDirective, MetricCardComponent, MetricGridComponent, MilestoneListComponent, PageHeaderComponent, PaginationComponent, PendingApprovalsListComponent, PortalShellComponent, ProgressComponent, ProjectDashboardComponent, ProjectHealthComponent, RadioGroupComponent, RecentDecisionsComponent, SelectComponent, StateFeedbackComponent, TabPanelDirective, TabsComponent, TextareaComponent, TooltipComponent, TooltipTriggerDirective, UpcomingMeetingsComponent, UserMenuComponent, VersionBoundApprovalComponent, VersionChipComponent],
  templateUrl: './visual-fixture.component.html',
  styleUrl: './visual-fixture.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class VisualFixtureComponent {
  protected readonly composition: Composition;
  protected readonly state: string;
  protected readonly longContent: boolean;
  protected readonly dialogOpen = signal(false);
  protected readonly drawerOpen = signal(false);
  protected readonly paletteOpen = signal(false);
  protected readonly confirmationOpen = signal(false);
  protected readonly selectedOverlayTab = signal<'overview' | 'disabled' | 'history'>('overview');

  protected readonly navigation: readonly AppNavigationLink[] = [
    { label: 'Engagement overview with delivery confidence and evidence', href: '#overview', icon: 'menu', active: true },
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
  protected readonly choiceOptions: readonly SelectOption<number>[] = [
    { value: 10, label: 'Ten reviewers' },
    { value: 20, label: 'Twenty reviewers' },
    { value: 50, label: 'Fifty reviewers', disabled: true },
  ];
  protected readonly documentVersions: readonly DocumentHistoryVersion[] = [
    { id: 'v3', versionLabel: 'v3', qualifier: 'approved', qualifierLabel: 'Approved', actor: 'Morgan Lee', occurredAt: '2026-08-15T14:00:00Z', timestampLabel: '15 August 2026 at 14:00 UTC' },
    { id: 'v4', versionLabel: 'v4', qualifier: 'current', qualifierLabel: 'Current', actor: 'Alex Rivera', occurredAt: '2026-08-16T09:00:00Z', timestampLabel: '16 August 2026 at 09:00 UTC' },
  ];
  protected readonly projectFilterOptions = [{ label: 'Transformation program', value: 'transformation' }] as const;
  protected readonly statusFilterOptions = [{ label: 'Published', value: 'published' }, { label: 'Draft', value: 'draft' }] as const;
  protected readonly shellBreadcrumbs = [
    { label: 'Client workspaces', href: '#workspaces' },
    { label: 'Transformation program with an intentionally long engagement name', href: '#overview' },
  ] as const;
  protected readonly accountActions: readonly UserMenuAction<'profile' | 'preferences' | 'sign-out'>[] = [
    { id: 'profile', label: 'Profile' },
    { id: 'preferences', label: 'Preferences' },
    { id: 'sign-out', label: 'Sign out' },
  ];
  protected readonly dashboardMilestones: readonly MilestoneSummary[] = [
    { id: 'readiness', title: 'Operational readiness review with stakeholders', status: { label: 'On track', variant: 'success' }, dueDate: { label: '21 August 2026', dateTime: '2026-08-21' }, navigation: { href: '#readiness' } },
    { id: 'evidence', title: 'Accessibility evidence package', status: { label: 'Needs attention', variant: 'warning' }, dueDate: { label: '24 August 2026', dateTime: '2026-08-24' } },
  ];
  protected readonly dashboardMeetings: readonly MeetingSummary[] = [
    { id: 'design-review', title: 'Design-system review', time: { label: '20 August 2026 at 16:00 UTC', dateTime: '2026-08-20T16:00:00Z' }, location: 'Microsoft Teams', action: { label: 'Open details', href: '#meeting' } },
  ];
  protected readonly dashboardDecisions: readonly RecentDecisionRecord[] = [
    { id: 'failover', label: 'Regional failover policy', status: { label: 'Approved', variant: 'success' }, date: { label: '18 August 2026', dateTime: '2026-08-18' }, navigation: { href: '#decision' } },
  ];
  protected readonly dashboardApprovals: readonly PendingApprovalItem[] = [
    { id: 'approval-1', target: { type: 'document', typeLabel: 'Document', label: 'Continuity plan', versionLabel: 'v4' }, requester: 'Jordan Lee', due: { label: 'Due 22 August 2026', variant: 'warning', dateTime: '2026-08-22' }, review: { href: '#approval' } },
  ];
  protected readonly radioOptions: readonly RadioOption<number>[] = [
    { value: 1, label: 'Focused review' },
    { value: 2, label: 'Standard review' },
    { value: 3, label: 'Comprehensive review', disabled: true },
  ];
  protected readonly overlayTabs: readonly TabItem<'overview' | 'disabled' | 'history'>[] = [
    { identity: 'overview', label: 'Overview' },
    { identity: 'disabled', label: 'Unavailable', disabled: true },
    { identity: 'history', label: 'History' },
  ];
  protected readonly commandGroups: readonly CommandPaletteGroup<string>[] = [{
    id: 'fixture', label: 'Fixture commands', commands: [
      { id: 'open', identity: 'open', label: 'Open project' },
      { id: 'disabled', identity: 'disabled', label: 'Unavailable command', disabled: true },
      { id: 'create', identity: 'create', label: 'Create project' },
    ],
  }];
  protected readonly approvalTarget = {
    type: 'document' as const,
    typeLabel: 'Document',
    label: 'Cross-region continuity plan',
    versionLabel: 'v3',
    versionQualifier: 'approved' as const,
    versionQualifierLabel: 'Approved version',
  };
  protected readonly approvalHistory: readonly ApprovalHistoryEntry[] = [
    { id: 'approved-v3', decision: 'Approved', actor: 'Morgan Lee', occurredAt: '2026-08-15T14:00:00Z', timestampLabel: '15 August 2026 at 14:00 UTC', commentSummary: 'Approved for publication.', targetTypeLabel: 'Document', targetLabel: 'Cross-region continuity plan', versionLabel: 'v3' },
    { id: 'created-v4', decision: 'Created current version', actor: 'Alex Rivera', occurredAt: '2026-08-16T09:00:00Z', timestampLabel: '16 August 2026 at 09:00 UTC', commentSummary: 'New edits remain unapproved.', targetTypeLabel: 'Document', targetLabel: 'Cross-region continuity plan', versionLabel: 'v4' },
  ];

  constructor() {
    const params = new URLSearchParams(globalThis.location.search);
    this.composition = (params.get('composition') as Composition | null) ?? 'shell';
    this.state = params.get('state') ?? 'default';
    this.longContent = this.state.includes('long');
    inject(AppearanceService).setAppearance(params.get('appearance') === 'dark' ? 'dark' : 'light');
  }
}
