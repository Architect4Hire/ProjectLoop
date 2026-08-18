import { Component, signal } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';

import { ButtonComponent } from './button.component';

@Component({
  standalone: true,
  imports: [ButtonComponent],
  template: `
    <lsd-button
      [disabled]="disabled()"
      [loading]="loading()"
      [type]="type()"
      [tone]="tone()"
      [impact]="impact()"
      [size]="size()"
      [shape]="shape()"
      [fullWidth]="fullWidth()"
      accessibleLabel="Save document"
      controls="save-details"
      [expanded]="expanded"
      (activated)="recordActivation()">
      <svg lsdButtonLeadingIcon aria-label="ignored icon label"></svg>
      Save
      <svg lsdButtonTrailingIcon></svg>
    </lsd-button>
  `,
})
class ButtonTestHostComponent {
  readonly disabled = signal(false);
  readonly loading = signal(false);
  readonly type = signal<'button' | 'submit' | 'reset'>('button');
  readonly tone = signal<'primary' | 'danger'>('primary');
  readonly impact = signal<'bold' | 'light' | 'minimal'>('bold');
  readonly size = signal<'small' | 'medium' | 'large'>('medium');
  readonly shape = signal<'square' | 'rounded' | 'pill'>('rounded');
  readonly fullWidth = signal(false);
  activations = 0;
  expanded = false;

  recordActivation(): void {
    this.activations += 1;
  }
}

describe('ButtonComponent', () => {
  let fixture: ComponentFixture<ButtonTestHostComponent>;
  let host: ButtonTestHostComponent;

  beforeEach(async () => {
    await TestBed.configureTestingModule({ imports: [ButtonTestHostComponent] }).compileComponents();
    fixture = TestBed.createComponent(ButtonTestHostComponent);
    host = fixture.componentInstance;
    fixture.detectChanges();
  });

  const nativeButton = (): HTMLButtonElement =>
    fixture.debugElement.query(By.css('button')).nativeElement as HTMLButtonElement;

  it('uses native button semantics and defaults to a non-submitting type', () => {
    expect(nativeButton().tagName).toBe('BUTTON');
    expect(nativeButton().type).toBe('button');
    expect(nativeButton().getAttribute('aria-label')).toBe('Save document');
    expect(nativeButton().getAttribute('aria-controls')).toBe('save-details');
    expect(nativeButton().getAttribute('aria-expanded')).toBe('false');
  });

  it('emits activation for an enabled native click', () => {
    nativeButton().click();
    expect(host.activations).toBe(1);
  });

  it('prevents activation when disabled', () => {
    host.disabled.set(true);
    fixture.detectChanges();
    nativeButton().click();
    expect(nativeButton().disabled).toBeTrue();
    expect(host.activations).toBe(0);
  });

  it('exposes loading state and an assertive-free status announcement', () => {
    host.loading.set(true);
    fixture.detectChanges();
    expect(nativeButton().disabled).toBeTrue();
    expect(nativeButton().getAttribute('aria-busy')).toBe('true');
    expect(fixture.debugElement.query(By.css('[role="status"]')).nativeElement.textContent).toContain('Loading');
  });

  it('projects leading and trailing icons as decorative content', () => {
    const icons = fixture.debugElement.queryAll(By.css('.lsd-button__icon'));
    expect(icons).toHaveSize(2);
    expect(icons.every((icon) => icon.attributes['aria-hidden'] === 'true')).toBeTrue();
  });

  it('applies tone, impact, size, shape, and full-width states observably', () => {
    host.tone.set('danger');
    host.impact.set('minimal');
    host.size.set('large');
    host.shape.set('pill');
    host.fullWidth.set(true);
    fixture.detectChanges();

    expect(nativeButton().classList).toContain('text-status-danger');
    expect(nativeButton().classList).toContain('h-12');
    expect(nativeButton().classList).toContain('rounded-full');
    expect(nativeButton().classList).toContain('w-full');
  });
});
