import { Component } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { AuditTimelineComponent, type AuditTimelineEvent, type AuditTimelinePaging } from './audit-timeline.component';

@Component({ standalone: true, imports: [AuditTimelineComponent], template: `<lsd-audit-timeline id="audit" [events]="events" [paging]="paging" (loadMoreRequested)="recordLoadMore()" />` })
class AuditTimelineTestHostComponent {
  events: readonly AuditTimelineEvent[] = Array.from({ length: 12 }, (_, index) => ({
    id: `event-${index + 1}`,
    actor: `Actor ${index + 1}`,
    action: index % 2 ? 'Viewed' : 'Updated',
    resource: { typeLabel: 'Record', label: `Resource ${index + 1}` },
    occurredAt: `2026-08-17T${String(index).padStart(2, '0')}:00:00Z`,
    timestampLabel: `17 August 2026 at ${String(index).padStart(2, '0')}:00 UTC`,
    correlationId: `correlation-${index + 1}`,
  }));
  paging: AuditTimelinePaging = { mode: 'load-more', hasMore: true };
  loadMoreCount = 0;
  recordLoadMore(): void { this.loadMoreCount += 1; }
}

describe('AuditTimelineComponent', () => {
  let fixture: ComponentFixture<AuditTimelineTestHostComponent>;
  let host: AuditTimelineTestHostComponent;
  beforeEach(async () => {
    await TestBed.configureTestingModule({ imports: [AuditTimelineTestHostComponent] }).compileComponents();
    fixture = TestBed.createComponent(AuditTimelineTestHostComponent);
    host = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('uses the Activity Stream empty state without paging controls', () => {
    host.events = [];
    fixture.detectChanges();
    expect(fixture.nativeElement.textContent).toContain('No audit events');
    expect(fixture.debugElement.query(By.css('.lsd-audit-timeline__load-more'))).toBeNull();
  });

  it('preserves every caller-ordered event in a many-event page', () => {
    const items = fixture.debugElement.queryAll(By.css('.lsd-activity-stream__item'));
    expect(items).toHaveSize(12);
    expect(items[0].nativeElement.textContent).toContain('Resource 1');
    expect(items[11].nativeElement.textContent).toContain('Resource 12');
  });

  it('emits load-more intent without changing caller state', () => {
    const button = fixture.debugElement.query(By.css('.lsd-audit-timeline__load-more button')).nativeElement as HTMLButtonElement;
    button.click();
    expect(host.loadMoreCount).toBe(1);
    expect(host.events).toHaveSize(12);
    expect(host.paging).toEqual({ mode: 'load-more', hasMore: true });
  });

  it('disables load more when complete and exposes caller loading state', () => {
    host.paging = { mode: 'load-more', hasMore: false };
    fixture.detectChanges();
    expect((fixture.debugElement.query(By.css('.lsd-audit-timeline__load-more button')).nativeElement as HTMLButtonElement).disabled).toBeTrue();
    expect(fixture.nativeElement.textContent).toContain('All audit events loaded');

    host.paging = { mode: 'load-more', hasMore: true, loading: true };
    fixture.detectChanges();
    expect(fixture.nativeElement.textContent).toContain('Loading more audit events');
  });
});
