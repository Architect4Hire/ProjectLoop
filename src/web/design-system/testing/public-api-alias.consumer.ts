import { ChangeDetectionStrategy, Component, signal } from '@angular/core';

import {
  approvalStatusPresentation,
  AuditTimelineComponent,
  ButtonComponent,
  CheckboxComponent,
  DialogComponent,
  DialogInitialFocusDirective,
  DocumentListComponent,
  DrawerComponent,
  DrawerInitialFocusDirective,
  InputComponent,
  PortalShellComponent,
  RadioGroupComponent,
  SelectComponent,
  StateFeedbackComponent,
  SurfaceComponent,
  TextareaComponent,
  VersionBoundApprovalComponent,
  type AppNavigationLink,
  type ApprovalRequestDocumentTarget,
  type AuditTimelineEvent,
  type DialogCloseReason,
  type DocumentCardViewModel,
  type DrawerCloseReason,
  type RadioOption,
  type SelectOption,
  type VersionBoundApprovalIntent,
} from '@lsd/design-system';

type FixtureChoice = Readonly<{ id: 'alpha' | 'beta'; label: string }>;

/** Compile-only consumer for the supported entry point; it is never bootstrapped or packaged. */
@Component({
  selector: 'lsd-public-api-consumer-fixture',
  standalone: true,
  imports: [
    AuditTimelineComponent,
    ButtonComponent,
    CheckboxComponent,
    DialogComponent,
    DialogInitialFocusDirective,
    DocumentListComponent,
    DrawerComponent,
    DrawerInitialFocusDirective,
    InputComponent,
    PortalShellComponent,
    RadioGroupComponent,
    SelectComponent,
    StateFeedbackComponent,
    SurfaceComponent,
    TextareaComponent,
    VersionBoundApprovalComponent,
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <lsd-portal-shell [navigationLinks]="navigation">
      <div lsdPortalHeader>Fixture header</div>
      <main lsdPortalMain>
        <lsd-surface [accessibility]="{ role: 'region', label: 'Compile fixture' }">
          <lsd-button (activated)="acceptActivation()">Continue</lsd-button>
          <lsd-input id="fixture-name" label="Name" [(value)]="name" />
          <lsd-textarea id="fixture-notes" label="Notes" [(value)]="notes" />
          <lsd-select id="fixture-select" label="Choice" [options]="choices" [(value)]="selectedChoice" />
          <lsd-checkbox
            id="fixture-checkbox"
            label="Confirmed"
            [(checked)]="confirmed"
            [(indeterminate)]="partiallyConfirmed" />
          <lsd-radio-group
            id="fixture-radio"
            name="fixture-radio"
            label="Preferred choice"
            [options]="radioChoices"
            [(value)]="preferredChoice" />
          <lsd-dialog
            id="fixture-dialog"
            title="Confirm selection"
            [open]="false"
            (closeRequested)="acceptDialogClose($event)">
            <button lsdDialogInitialFocus type="button">Confirm</button>
          </lsd-dialog>
          <lsd-drawer
            id="fixture-drawer"
            title="Selection details"
            [open]="false"
            (closeRequested)="acceptDrawerClose($event)">
            <button lsdDrawerInitialFocus type="button">Done</button>
          </lsd-drawer>
          <lsd-state-feedback id="fixture-state" kind="empty" title="No pending work" />
          <lsd-document-list id="fixture-documents" [documents]="documents" />
          <lsd-version-bound-approval
            id="fixture-approval"
            [target]="approvalTarget"
            [requestStatus]="approvalStatus"
            requester="Jordan Lee"
            [requestedTime]="requestedTime"
            provenance="human-authored"
            [(comment)]="approvalComment"
            (decisionIntent)="acceptDecision($event)" />
          <lsd-audit-timeline
            id="fixture-audit"
            [events]="auditEvents"
            [paging]="{ mode: 'load-more', hasMore: false }"
            (loadMoreRequested)="acceptActivation()"
            (correlationCopyRequested)="acceptCorrelation($event)" />
        </lsd-surface>
      </main>
      <aside lsdPortalNotifications aria-label="Notifications"></aside>
    </lsd-portal-shell>
  `,
})
export class PublicApiConsumerFixtureComponent {
  protected readonly name = signal('Neutral fixture');
  protected readonly notes = signal('');
  protected readonly confirmed = signal(false);
  protected readonly partiallyConfirmed = signal(false);
  protected readonly approvalComment = signal('');
  protected readonly selectedChoice = signal<FixtureChoice | null>(null);
  protected readonly preferredChoice = signal<FixtureChoice | null>(null);

  protected readonly choices: readonly SelectOption<FixtureChoice>[] = [
    { value: { id: 'alpha', label: 'Alpha' }, label: 'Alpha' },
    { value: { id: 'beta', label: 'Beta' }, label: 'Beta' },
  ];
  protected readonly radioChoices: readonly RadioOption<FixtureChoice>[] = this.choices;
  protected readonly navigation: readonly AppNavigationLink[] = [
    { label: 'Overview', href: '#fixture', icon: 'menu', active: true },
  ];
  protected readonly documents: readonly DocumentCardViewModel[] = [
    {
      id: 'fixture-document',
      title: 'Continuity plan',
      category: 'Plan',
      status: { label: 'Ready', variant: 'success' },
      visibility: { label: 'Internal', variant: 'neutral' },
      version: { label: 'v1', qualifier: 'approved', qualifierLabel: 'Approved' },
      updated: { label: 'Updated today', dateTime: '2026-08-18' },
    },
  ];
  protected readonly approvalTarget: ApprovalRequestDocumentTarget = {
    type: 'document',
    typeLabel: 'Document',
    label: 'Continuity plan',
    versionLabel: 'v1',
    versionQualifier: 'approved',
    versionQualifierLabel: 'Approved version',
  };
  protected readonly approvalStatus = approvalStatusPresentation('requested');
  protected readonly requestedTime = { label: 'Requested today', dateTime: '2026-08-18' } as const;
  protected readonly auditEvents: readonly AuditTimelineEvent[] = [
    {
      id: 'fixture-event',
      actor: 'Jordan Lee',
      action: 'Reviewed',
      resource: { typeLabel: 'Document', label: 'Continuity plan' },
      occurredAt: '2026-08-18T12:00:00Z',
      timestampLabel: 'Today at noon',
      correlationId: 'fixture-correlation',
      attribution: 'human-authored',
    },
  ];

  protected acceptActivation(): void {}
  protected acceptDialogClose(_reason: DialogCloseReason): void {}
  protected acceptDrawerClose(_reason: DrawerCloseReason): void {}
  protected acceptDecision(_intent: VersionBoundApprovalIntent): void {}
  protected acceptCorrelation(_correlationId: string): void {}
}
