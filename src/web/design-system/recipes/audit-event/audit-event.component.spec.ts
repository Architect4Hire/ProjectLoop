import { Component } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { AuditEventComponent } from './audit-event.component';

@Component({
  standalone: true,
  imports: [AuditEventComponent],
  template: `
    <lsd-audit-event
      id="event-42"
      [actor]="actor"
      action="Downloaded metadata"
      [resource]="{ typeLabel: 'Document', label: 'Delivery plan v3' }"
      [occurredAt]="{ label: '17 August 2026 at 14:30 UTC', dateTime: '2026-08-17T14:30:00Z' }"
      correlationId="01J5N8M7QX9Z3K2P6T4R1S0VAB"
      [hasDetails]="true"
      (correlationCopyRequested)="copied = $event">
      <p lsdAuditEventDetails>Result: metadata returned</p>
    </lsd-audit-event>
  `,
})
class AuditEventTestHostComponent {
  actor: string | null = null;
  copied?: string;
}

describe('AuditEventComponent', () => {
  let fixture: ComponentFixture<AuditEventTestHostComponent>;
  let host: AuditEventTestHostComponent;
  beforeEach(async () => {
    await TestBed.configureTestingModule({ imports: [AuditEventTestHostComponent] }).compileComponents();
    fixture = TestBed.createComponent(AuditEventTestHostComponent);
    host = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('uses explicit unavailable text when the actor is missing', () => {
    expect(fixture.debugElement.query(By.css('.lsd-audit-event__actor')).nativeElement.textContent).toContain('Actor unavailable');
  });

  it('shows and emits the complete correlation identifier for accessible copying', () => {
    const identifier = '01J5N8M7QX9Z3K2P6T4R1S0VAB';
    expect(fixture.debugElement.query(By.css('code')).nativeElement.textContent).toBe(identifier);
    const button = fixture.debugElement.query(By.css('button')).nativeElement as HTMLButtonElement;
    expect(button.getAttribute('aria-label')).toBe(`Copy correlation identifier ${identifier}`);
    button.click();
    expect(host.copied).toBe(identifier);
  });

  it('keeps semantic content and safe details in narrow-layout hooks', () => {
    const event = fixture.debugElement.query(By.css('.lsd-audit-event')).nativeElement as HTMLElement;
    expect(event.textContent).toContain('Downloaded metadata');
    expect(event.textContent).toContain('Delivery plan v3');
    expect(event.textContent).toContain('17 August 2026 at 14:30 UTC');
    expect(fixture.debugElement.query(By.css('.lsd-audit-event__details')).nativeElement.textContent).toContain('Result: metadata returned');
  });
});
