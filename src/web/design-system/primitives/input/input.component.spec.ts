import { Component, signal } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';

import { InputComponent } from './input.component';

@Component({
  standalone: true,
  imports: [InputComponent],
  template: `
    <lsd-input
      id="engagement-name"
      label="Engagement name"
      description="Use the client-facing name."
      placeholder="Example Consulting"
      [density]="density()"
      [disabled]="disabled()"
      [error]="error()"
      [readonly]="isReadonly()"
      required
      [(value)]="value">
      <span lsdInputPrefix>Prefix</span>
      <span lsdInputSuffix>Suffix</span>
    </lsd-input>
  `,
})
class InputTestHostComponent {
  readonly density = signal<'compact' | 'default' | 'comfortable'>('default');
  readonly disabled = signal(false);
  readonly error = signal<string | undefined>(undefined);
  readonly isReadonly = signal(false);
  readonly value = signal('Initial');
}

describe('InputComponent', () => {
  let fixture: ComponentFixture<InputTestHostComponent>;
  let host: InputTestHostComponent;

  beforeEach(async () => {
    await TestBed.configureTestingModule({ imports: [InputTestHostComponent] }).compileComponents();
    fixture = TestBed.createComponent(InputTestHostComponent);
    host = fixture.componentInstance;
    fixture.detectChanges();
  });

  const control = (): HTMLInputElement => fixture.debugElement.query(By.css('input')).nativeElement;

  it('associates the native input with its visible label and description', () => {
    const label = fixture.debugElement.query(By.css('label')).nativeElement as HTMLLabelElement;
    expect(label.htmlFor).toBe('engagement-name');
    expect(control().required).toBeTrue();
    expect(control().getAttribute('aria-describedby')).toBe('engagement-name-description');
  });

  it('updates model signal state from native keyboard-compatible input events', () => {
    control().value = 'Updated';
    control().dispatchEvent(new Event('input'));
    expect(host.value()).toBe('Updated');
    expect(control().hasAttribute('tabindex')).toBeFalse();
    expect(control().placeholder).toBe('Example Consulting');
  });

  it('keeps readonly focusable and distinguishable from disabled', () => {
    host.isReadonly.set(true);
    fixture.detectChanges();
    expect(control().readOnly).toBeTrue();
    control().focus();
    expect(document.activeElement).toBe(control());
    expect(control().disabled).toBeFalse();

    host.disabled.set(true);
    fixture.detectChanges();
    expect(control().disabled).toBeTrue();
    expect(control().readOnly).toBeTrue();
  });

  it('associates and announces an accessible error', () => {
    host.error.set('An engagement name is required.');
    fixture.detectChanges();
    expect(control().getAttribute('aria-invalid')).toBe('true');
    expect(control().getAttribute('aria-errormessage')).toBe('engagement-name-error');
    expect(control().getAttribute('aria-describedby')).toBe(
      'engagement-name-description engagement-name-error',
    );
    expect(fixture.debugElement.query(By.css('[role="alert"]')).nativeElement.textContent).toContain(
      'An engagement name is required.',
    );
    expect(fixture.debugElement.query(By.css('[role="alert"]')).attributes['aria-atomic']).toBe('true');
  });

  it('applies all supported densities without changing the native control', () => {
    const wrapper = control().parentElement as HTMLElement;
    expect(wrapper.classList).toContain('min-h-10');
    host.density.set('compact');
    fixture.detectChanges();
    expect(wrapper.classList).toContain('min-h-8');
    host.density.set('comfortable');
    fixture.detectChanges();
    expect(wrapper.classList).toContain('min-h-12');
  });

  it('projects decorative prefix and suffix content', () => {
    const decorations = fixture.debugElement.queryAll(By.css('[aria-hidden="true"]'));
    expect(decorations.some((item) => item.nativeElement.textContent.includes('Prefix'))).toBeTrue();
    expect(decorations.some((item) => item.nativeElement.textContent.includes('Suffix'))).toBeTrue();
  });
});
