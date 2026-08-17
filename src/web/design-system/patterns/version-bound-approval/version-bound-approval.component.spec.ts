import { Component } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';

import { VersionBoundApprovalComponent, type VersionBoundApprovalIntent } from './version-bound-approval.component';

@Component({
  standalone: true,
  imports: [VersionBoundApprovalComponent],
  template: `
    <lsd-version-bound-approval
      id="delivery-plan-review"
      [target]="{
        type: 'document',
        typeLabel: 'Document',
        label: 'Delivery plan',
        versionLabel: 'v3'
      }"
      [requestStatus]="{ label: 'Requested', variant: 'info' }"
      requester="Morgan Lee"
      [requestedTime]="{ label: '17 August 2026' }"
      provenance="human-authored"
      [currentVersion]="{ label: 'v4' }"
      [(comment)]="comment"
      (decisionIntent)="intents.push($event)" />
  `,
})
class VersionBoundApprovalTestHostComponent {
  comment = 'Reviewed against the requested version.';
  intents: VersionBoundApprovalIntent[] = [];
}

describe('VersionBoundApprovalComponent', () => {
  let fixture: ComponentFixture<VersionBoundApprovalTestHostComponent>;
  let host: VersionBoundApprovalTestHostComponent;

  beforeEach(async () => {
    await TestBed.configureTestingModule({ imports: [VersionBoundApprovalTestHostComponent] }).compileComponents();
    fixture = TestBed.createComponent(VersionBoundApprovalTestHostComponent);
    host = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('keeps v3 review identity beside actions while v4 is marked current', () => {
    const context = fixture.debugElement.query(By.css('.lsd-version-bound-approval__context')).nativeElement as HTMLElement;
    expect(context.textContent).toContain('Reviewing');
    expect(context.textContent).toContain('Delivery plan');
    expect(context.textContent).toContain('v3');
    expect(context.textContent).toContain('Current version');
    expect(context.textContent).toContain('v4');
    expect(context.textContent).toContain('Current');
  });

  it('emits an approve intent bound to v3 rather than current v4', () => {
    const buttons = fixture.debugElement.queryAll(By.css('.lsd-approval-actions__buttons button'));
    (buttons[2].nativeElement as HTMLButtonElement).click();
    expect(host.intents[0]).toEqual({
      decision: 'approve',
      target: { type: 'document', typeLabel: 'Document', label: 'Delivery plan', versionLabel: 'v3' },
      comment: 'Reviewed against the requested version.',
    });
  });

  it('emits a reject intent bound to v3 rather than current v4', () => {
    const buttons = fixture.debugElement.queryAll(By.css('.lsd-approval-actions__buttons button'));
    (buttons[1].nativeElement as HTMLButtonElement).click();
    expect(host.intents[0].decision).toBe('reject');
    expect(host.intents[0].target.versionLabel).toBe('v3');
  });
});
