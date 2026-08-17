import { Component } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { PendingApprovalsListComponent, type PendingApprovalItem } from './pending-approvals-list.component';

@Component({ standalone: true, imports: [PendingApprovalsListComponent], template: `<lsd-pending-approvals-list [items]="items" />` })
class PendingApprovalsListTestHostComponent {
  items: readonly PendingApprovalItem[] = [
    { id: 'document-v3', target: { type: 'document', typeLabel: 'Document', label: 'Delivery plan', versionLabel: 'v3' }, requester: 'Morgan Lee', due: { label: 'Overdue by 2 days', variant: 'danger', dateTime: '2026-08-15' }, review: { href: '/approvals/document-v3' } },
    { id: 'milestone', target: { type: 'other', typeLabel: 'Milestone', label: 'Architecture sign-off' }, requester: 'Alex Rivera', due: { label: 'Due August 20', variant: 'neutral', dateTime: '2026-08-20' }, review: { href: '/approvals/milestone' } },
  ];
}

describe('PendingApprovalsListComponent', () => {
  let fixture: ComponentFixture<PendingApprovalsListTestHostComponent>;
  let host: PendingApprovalsListTestHostComponent;
  beforeEach(async () => {
    await TestBed.configureTestingModule({ imports: [PendingApprovalsListTestHostComponent] }).compileComponents();
    fixture = TestBed.createComponent(PendingApprovalsListTestHostComponent);
    host = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('renders an explicit empty state', () => {
    host.items = [];
    fixture.detectChanges();
    expect(fixture.debugElement.query(By.css('ul'))).toBeNull();
    expect(fixture.nativeElement.textContent).toContain('No pending approval requests.');
  });

  it('presents caller-supplied overdue text without inferring urgency', () => {
    const first = fixture.debugElement.queryAll(By.css('li'))[0].nativeElement as HTMLElement;
    expect(first.textContent).toContain('Overdue by 2 days');
    expect((first.querySelector('time') as HTMLTimeElement).dateTime).toBe('2026-08-15');
  });

  it('preserves mixed-target order and shows versions only where applicable', () => {
    const items = fixture.debugElement.queryAll(By.css('li'));
    expect(items[0].nativeElement.textContent).toContain('Document');
    expect(items[0].nativeElement.textContent).toContain('v3');
    expect(items[1].nativeElement.textContent).toContain('Milestone');
    expect(items[1].query(By.css('lsd-version-chip'))).toBeNull();
  });

  it('uses caller-authorized native review links with version-specific labels', () => {
    const links = fixture.debugElement.queryAll(By.css('a[lsdLink]'));
    expect((links[0].nativeElement as HTMLAnchorElement).getAttribute('href')).toBe('/approvals/document-v3');
    expect(links[0].nativeElement.getAttribute('aria-label')).toBe('Review Delivery plan v3');
    expect(fixture.debugElement.queryAll(By.css('button'))).toHaveSize(0);
  });
});
