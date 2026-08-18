import { Component, signal } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';

import { AiFailureAction, AiFailureComponent, AiFailureKind, AiFailureReportReference, CorrelationCopyState } from './ai-failure.component';

@Component({
  standalone: true,
  imports: [AiFailureComponent],
  template: `
    <lsd-ai-failure
      id="generation-failure"
      [kind]="kind()"
      message="The generated draft was not saved. Your approved section is unchanged."
      [detailsAvailable]="detailsAvailable()"
      [correlationVisible]="correlationVisible()"
      correlationId="corr-safe-123"
      [copyCorrelationAvailable]="copyAvailable()"
      [reportAvailable]="reportAvailable()"
      [processing]="processing()"
      [copyState]="copyState()"
      (retryRequested)="retry()"
      (detailsToggled)="detailsOpen = $event"
      (correlationCopyRequested)="copied = $event"
      (reportRequested)="reported = $event">
      <p lsdAiFailureDetails>Display-safe timeout category.</p>
    </lsd-ai-failure>
  `,
})
class AiFailureTestHostComponent {
  readonly kind = signal<AiFailureKind>('recoverable');
  readonly detailsAvailable = signal(false);
  readonly correlationVisible = signal(false);
  readonly copyAvailable = signal(false);
  readonly reportAvailable = signal(false);
  readonly processing = signal<AiFailureAction | null>(null);
  readonly copyState = signal<CorrelationCopyState>('idle');
  retries = 0;
  retry(): void { this.retries += 1; }
  detailsOpen = false;
  copied: string | null = null;
  reported: AiFailureReportReference | null = null;
}

describe('AiFailureComponent', () => {
  let fixture: ComponentFixture<AiFailureTestHostComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({ imports: [AiFailureTestHostComponent] }).compileComponents();
    fixture = TestBed.createComponent(AiFailureTestHostComponent);
    fixture.detectChanges();
  });

  it('presents recoverable AI failure with a polite alert, retry, and content-safety message', () => {
    const alert = fixture.debugElement.query(By.css('[role="status"]')).nativeElement as HTMLElement;
    expect(alert.getAttribute('aria-live')).toBe('polite');
    const text = fixture.nativeElement.textContent as string;
    expect(text).toContain('AI failure');
    expect(text).toContain('Recoverable');
    expect(text).toContain('No approved content was replaced');
    expect(text).toContain('Your approved section is unchanged');
    const retry = fixture.debugElement.query(By.css('.lsd-ai-failure__actions button')).nativeElement as HTMLButtonElement;
    retry.click();
    expect(fixture.componentInstance.retries).toBe(1);
  });

  it('uses an assertive terminal alert and removes retry', () => {
    fixture.componentInstance.kind.set('terminal');
    fixture.detectChanges();
    const alert = fixture.debugElement.query(By.css('[role="alert"]')).nativeElement as HTMLElement;
    expect(alert.getAttribute('aria-live')).toBe('assertive');
    expect(fixture.nativeElement.textContent).toContain('Terminal');
    expect(fixture.debugElement.query(By.css('.lsd-ai-failure__actions button'))).toBeNull();
  });

  it('does not render protected diagnostics or correlation metadata unless enabled', () => {
    expect(fixture.debugElement.query(By.css('details'))).toBeNull();
    expect(fixture.debugElement.query(By.css('.lsd-ai-failure__correlation'))).toBeNull();
    expect(fixture.nativeElement.textContent).not.toContain('Display-safe timeout category');

    fixture.componentInstance.detailsAvailable.set(true);
    fixture.componentInstance.correlationVisible.set(true);
    fixture.detectChanges();
    const details = fixture.debugElement.query(By.css('details')).nativeElement as HTMLDetailsElement;
    expect(details.open).toBeFalse();
    expect(details.textContent).toContain('Display-safe timeout category');
    expect(fixture.nativeElement.textContent).toContain('corr-safe-123');
    details.open = true;
    details.dispatchEvent(new Event('toggle'));
    expect(fixture.componentInstance.detailsOpen).toBeTrue();
  });

  it('emits copy and report intent only through authorized controls', () => {
    fixture.componentInstance.correlationVisible.set(true);
    fixture.componentInstance.copyAvailable.set(true);
    fixture.componentInstance.reportAvailable.set(true);
    fixture.detectChanges();
    const buttons = fixture.debugElement.queryAll(By.css('.lsd-ai-failure__actions button'));
    (buttons[0].nativeElement as HTMLButtonElement).click();
    (buttons[1].nativeElement as HTMLButtonElement).click();
    expect(fixture.componentInstance.copied).toBe('corr-safe-123');
    expect(fixture.componentInstance.reported).toEqual({ kind: 'recoverable', correlationId: 'corr-safe-123' });
  });

  it('omits a hidden correlation identifier from reports', () => {
    fixture.componentInstance.reportAvailable.set(true);
    fixture.detectChanges();
    const report = fixture.debugElement.queryAll(By.css('.lsd-ai-failure__actions button'))[0];
    (report.nativeElement as HTMLButtonElement).click();
    expect(fixture.componentInstance.reported).toEqual({ kind: 'recoverable' });
  });

  it('locks concurrent actions and politely announces caller-owned copy results', () => {
    fixture.componentInstance.correlationVisible.set(true);
    fixture.componentInstance.copyAvailable.set(true);
    fixture.componentInstance.reportAvailable.set(true);
    fixture.componentInstance.processing.set('report');
    fixture.componentInstance.copyState.set('copied');
    fixture.detectChanges();
    const buttons = fixture.debugElement.queryAll(By.css('.lsd-ai-failure__actions button'));
    expect(buttons.every((item) => (item.nativeElement as HTMLButtonElement).disabled)).toBeTrue();
    const statuses = fixture.debugElement.queryAll(By.css('[role="status"]'));
    expect(statuses.some((item) => item.nativeElement.textContent.includes('Correlation identifier copied'))).toBeTrue();
  });
});
