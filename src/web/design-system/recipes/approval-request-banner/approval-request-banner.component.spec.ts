import { Component } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';

import { ApprovalRequestBannerComponent, type ApprovalRequestTarget } from './approval-request-banner.component';

@Component({
  standalone: true,
  imports: [ApprovalRequestBannerComponent],
  template: `
    <lsd-approval-request-banner
      id="approval-request"
      [target]="target"
      [status]="{ label: 'Requested', variant: 'info' }"
      requester="Morgan Lee"
      [requestedTime]="{ label: '17 August 2026 at 10:30 AM', dateTime: '2026-08-17T15:30:00Z' }" />
  `,
})
class ApprovalRequestBannerTestHostComponent {
  target: ApprovalRequestTarget = {
    type: 'document',
    typeLabel: 'Document',
    label: 'Delivery plan',
    versionLabel: 'v3',
  };
}

describe('ApprovalRequestBannerComponent', () => {
  let fixture: ComponentFixture<ApprovalRequestBannerTestHostComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({ imports: [ApprovalRequestBannerTestHostComponent] }).compileComponents();
    fixture = TestBed.createComponent(ApprovalRequestBannerTestHostComponent);
  });

  it('keeps the exact document version visible in the primary summary', () => {
    fixture.detectChanges();
    const summary = fixture.debugElement.query(By.css('.lsd-approval-request-banner__summary')).nativeElement as HTMLElement;
    expect(summary.textContent).toContain('Document');
    expect(summary.textContent).toContain('Delivery plan');
    expect(summary.textContent).toContain('v3');
    expect(summary.textContent).toContain('Requested');
  });

  it('renders requester and machine-readable requested time', () => {
    fixture.detectChanges();
    const time = fixture.debugElement.query(By.css('time')).nativeElement as HTMLTimeElement;
    expect(fixture.nativeElement.textContent).toContain('Morgan Lee');
    expect(time.dateTime).toBe('2026-08-17T15:30:00Z');
  });

  it('rejects a document target without an exact version', () => {
    fixture.componentInstance.target = {
      type: 'document',
      typeLabel: 'Document',
      label: 'Delivery plan',
      versionLabel: '',
    };

    expect(() => fixture.detectChanges()).toThrowError(/requires an exact versionLabel/);
  });

  it('does not expose approval or rejection actions', () => {
    fixture.detectChanges();
    expect(fixture.debugElement.queryAll(By.css('button'))).toHaveSize(0);
  });
});
