import { Component } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { ApprovalHistoryComponent, type ApprovalHistoryEntry } from './approval-history.component';

@Component({ standalone: true, imports: [ApprovalHistoryComponent], template: `<lsd-approval-history id="approval-history" [entries]="entries" />` })
class ApprovalHistoryTestHostComponent {
  readonly entries: readonly ApprovalHistoryEntry[] = [
    { id: 'decision-1', decision: 'Approved', actor: 'Morgan Lee', occurredAt: '2026-08-15T14:00:00Z', timestampLabel: '15 August 2026 at 14:00 UTC', commentSummary: 'Approved for publication.', targetTypeLabel: 'Document', targetLabel: 'Delivery plan', versionLabel: 'v2' },
    { id: 'decision-2', decision: 'Rejected', actor: 'Alex Rivera', occurredAt: '2026-08-16T09:30:00Z', timestampLabel: '16 August 2026 at 09:30 UTC', commentSummary: 'Revise the delivery dates.', targetTypeLabel: 'Document', targetLabel: 'Delivery plan', versionLabel: 'v3' },
    { id: 'decision-3', decision: 'Approved', actor: 'Sam Patel', occurredAt: '2026-08-17T11:15:00Z', timestampLabel: '17 August 2026 at 11:15 UTC', commentSummary: 'Dates confirmed.', targetTypeLabel: 'Document', targetLabel: 'Delivery plan', versionLabel: 'v4' },
  ];
}

describe('ApprovalHistoryComponent', () => {
  let fixture: ComponentFixture<ApprovalHistoryTestHostComponent>;
  beforeEach(async () => {
    await TestBed.configureTestingModule({ imports: [ApprovalHistoryTestHostComponent] }).compileComponents();
    fixture = TestBed.createComponent(ApprovalHistoryTestHostComponent);
    fixture.detectChanges();
  });

  it('preserves caller chronology across multiple decisions and versions', () => {
    const events = fixture.debugElement.queryAll(By.css('.lsd-activity-stream__item'));
    expect(events).toHaveSize(3);
    expect(events[0].nativeElement.textContent).toContain('Approved Document Delivery plan v2');
    expect(events[1].nativeElement.textContent).toContain('Rejected Document Delivery plan v3');
    expect(events[2].nativeElement.textContent).toContain('Approved Document Delivery plan v4');
  });

  it('retains machine-readable UTC instants and caller display text', () => {
    const times = fixture.debugElement.queryAll(By.css('time'));
    expect((times[1].nativeElement as HTMLTimeElement).dateTime).toBe('2026-08-16T09:30:00Z');
    expect(times[1].nativeElement.textContent).toContain('16 August 2026 at 09:30 UTC');
  });

  it('keeps each exact version visible before optional evidence is expanded', () => {
    expect(fixture.nativeElement.textContent).toContain('v2');
    expect(fixture.nativeElement.textContent).toContain('v3');
    expect(fixture.nativeElement.textContent).toContain('v4');
  });

  it('shows immutable comment evidence with the exact Version Chip', () => {
    const details = fixture.debugElement.queryAll(By.css('details'));
    (details[1].nativeElement.querySelector('summary') as HTMLElement).click();
    fixture.detectChanges();
    expect(details[1].nativeElement.textContent).toContain('Revise the delivery dates.');
    expect(details[1].nativeElement.textContent).toContain('v3');
    expect(fixture.debugElement.queryAll(By.css('button'))).toHaveSize(0);
  });
});
