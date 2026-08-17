import { Component } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';

import {
  ApprovalHistoryComponent,
  ApprovalRequestBannerComponent,
  DocumentVersionHistoryComponent,
  PendingApprovalsListComponent,
  VersionBoundApprovalComponent,
  VersionChipComponent,
  approvalStatusPresentation,
  type ApprovalHistoryEntry,
  type DocumentHistoryVersion,
  type PendingApprovalItem,
} from '@lsd/design-system';

@Component({
  standalone: true,
  imports: [
    ApprovalHistoryComponent,
    ApprovalRequestBannerComponent,
    DocumentVersionHistoryComponent,
    PendingApprovalsListComponent,
    VersionBoundApprovalComponent,
    VersionChipComponent,
  ],
  template: `
    <section data-surface="version-chip">
      <lsd-version-chip versionLabel="v3" qualifier="approved" />
      <lsd-version-chip versionLabel="v4" qualifier="current" />
    </section>

    <section data-surface="version-history">
      <lsd-document-version-history id="version-history" [versions]="versions" />
    </section>

    <section data-surface="approval-banner">
      <lsd-approval-request-banner
        id="approval-banner"
        [target]="approvedV3Target"
        [status]="approvedStatus"
        requester="Morgan Lee"
        [requestedTime]="requestedTime" />
    </section>

    <section data-surface="approval-pattern">
      <lsd-version-bound-approval
        id="approval-pattern"
        [target]="approvedV3Target"
        [requestStatus]="approvedStatus"
        requester="Morgan Lee"
        [requestedTime]="requestedTime"
        provenance="human-approved"
        reviewStatus="approved"
        [currentVersion]="{ label: 'v4', qualifierLabel: 'Current' }" />
    </section>

    <section data-surface="pending-list">
      <lsd-pending-approvals-list [items]="pending" />
    </section>

    <section data-surface="approval-history">
      <lsd-approval-history id="approval-history" [entries]="approvalHistory" />
    </section>
  `,
})
class DocumentVersionApprovalRegressionHostComponent {
  readonly approvedStatus = approvalStatusPresentation('approved');
  readonly requestedTime = {
    label: '15 August 2026 at 14:00 UTC',
    dateTime: '2026-08-15T14:00:00Z',
  };
  readonly approvedV3Target = {
    type: 'document' as const,
    typeLabel: 'Document',
    label: 'Delivery plan',
    versionLabel: 'v3',
    versionQualifier: 'approved' as const,
    versionQualifierLabel: 'Approved',
  };
  readonly versions: readonly DocumentHistoryVersion[] = [
    {
      id: 'v3', versionLabel: 'v3', qualifier: 'approved', actor: 'Morgan Lee',
      occurredAt: '2026-08-15T14:00:00Z', timestampLabel: '15 August 2026 at 14:00 UTC',
    },
    {
      id: 'v4', versionLabel: 'v4', qualifier: 'current', actor: 'Alex Rivera',
      occurredAt: '2026-08-16T09:00:00Z', timestampLabel: '16 August 2026 at 09:00 UTC',
    },
  ];
  readonly pending: readonly PendingApprovalItem[] = [{
    id: 'delivery-plan-v3',
    target: { type: 'document', typeLabel: 'Document', label: 'Delivery plan', versionLabel: 'v3' },
    requester: 'Morgan Lee',
    due: { label: 'Due 18 August', variant: 'neutral', dateTime: '2026-08-18' },
    review: { href: '/approvals/delivery-plan-v3' },
  }];
  readonly approvalHistory: readonly ApprovalHistoryEntry[] = [
    {
      id: 'approved-v3', decision: 'Approved', actor: 'Morgan Lee',
      occurredAt: '2026-08-15T14:00:00Z', timestampLabel: '15 August 2026 at 14:00 UTC',
      commentSummary: 'Approved for publication.', targetTypeLabel: 'Document',
      targetLabel: 'Delivery plan', versionLabel: 'v3',
    },
    {
      id: 'created-v4', decision: 'Created current version', actor: 'Alex Rivera',
      occurredAt: '2026-08-16T09:00:00Z', timestampLabel: '16 August 2026 at 09:00 UTC',
      commentSummary: 'New edits remain unapproved.', targetTypeLabel: 'Document',
      targetLabel: 'Delivery plan', versionLabel: 'v4',
    },
  ];
}

describe('document approval remains bound to its exact version across public surfaces', () => {
  let fixture: ComponentFixture<DocumentVersionApprovalRegressionHostComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({ imports: [DocumentVersionApprovalRegressionHostComponent] }).compileComponents();
    fixture = TestBed.createComponent(DocumentVersionApprovalRegressionHostComponent);
    fixture.detectChanges();
  });

  function surface(name: string): HTMLElement {
    return fixture.debugElement.query(By.css(`[data-surface="${name}"]`)).nativeElement as HTMLElement;
  }

  function chipTexts(container: HTMLElement): string[] {
    return Array.from(container.querySelectorAll('lsd-version-chip'), (chip) => {
      const version = chip.querySelector('.lsd-version-chip__version')?.textContent?.trim() ?? '';
      const qualifier = chip.querySelector('.lsd-version-chip__qualifier')?.textContent?.trim();
      return qualifier ? `${version} · ${qualifier}` : version;
    });
  }

  it('binds standalone Version Chip qualifiers to v3 and v4 independently', () => {
    expect(chipTexts(surface('version-chip'))).toEqual(['v3 · Approved', 'v4 · Current']);
  });

  it('keeps the approved and current qualifiers on separate version-history entries', () => {
    expect(chipTexts(surface('version-history'))).toEqual(['v3 · Approved', 'v4 · Current']);
  });

  it('shows only approved v3 in the approval banner', () => {
    expect(chipTexts(surface('approval-banner'))).toEqual(['v3 · Approved']);
    expect(surface('approval-banner').textContent).not.toContain('v4');
  });

  it('shows approved v3 beside current v4 without qualifying v4 as approved in the approval pattern', () => {
    const chips = chipTexts(surface('approval-pattern'));
    expect(chips).toContain('v3 · Approved');
    expect(chips).toContain('v3');
    expect(chips).toContain('v4 · Current');
    expect(chips).not.toContain('v4 · Approved');
  });

  it('keeps the pending review link and unqualified Version Chip bound to v3', () => {
    expect(chipTexts(surface('pending-list'))).toEqual(['v3']);
    const link = surface('pending-list').querySelector('a');
    expect(link?.getAttribute('aria-label')).toBe('Review Delivery plan v3');
    expect(surface('pending-list').textContent).not.toContain('v4');
  });

  it('records approval against v3 while v4 remains visibly separate in approval history', () => {
    const events = surface('approval-history').querySelectorAll('.lsd-activity-stream__item');
    expect(events[0].textContent).toContain('Approved Document Delivery plan v3');
    expect(events[0].textContent).not.toContain('v4');
    expect(events[1].textContent).toContain('Created current version Document Delivery plan v4');
    expect(events[1].textContent).not.toContain('Approved Document Delivery plan v4');

    (events[0].querySelector('summary') as HTMLElement).click();
    (events[1].querySelector('summary') as HTMLElement).click();
    fixture.detectChanges();
    expect(chipTexts(surface('approval-history'))).toEqual(['v3', 'v4']);
  });
});
