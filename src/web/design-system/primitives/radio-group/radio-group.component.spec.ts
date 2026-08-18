import { Component, signal } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';

import { RadioGroupComponent, type RadioOption } from './radio-group.component';

interface ReviewDepth { readonly id: number; }

@Component({
  standalone: true,
  imports: [RadioGroupComponent],
  template: `
    <lsd-radio-group
      id="review-depth"
      name="review-depth"
      label="Review depth"
      description="Choose one review level."
      [compareWith]="compareById"
      [disabled]="disabled()"
      [error]="error()"
      [options]="options"
      required
      [(value)]="value" />
  `,
})
class RadioGroupTestHostComponent {
  readonly disabled = signal(false);
  readonly error = signal<string | undefined>(undefined);
  readonly options: readonly RadioOption<ReviewDepth>[] = [
    { value: { id: 1 }, label: 'Focused' },
    { value: { id: 2 }, label: 'Standard' },
    { value: { id: 3 }, label: 'Comprehensive', disabled: true },
  ];
  readonly value = signal<ReviewDepth | null>({ id: 1 });
  readonly compareById = (left: ReviewDepth, right: ReviewDepth): boolean => left.id === right.id;
}

describe('RadioGroupComponent', () => {
  let fixture: ComponentFixture<RadioGroupTestHostComponent>;
  let host: RadioGroupTestHostComponent;

  beforeEach(async () => {
    await TestBed.configureTestingModule({ imports: [RadioGroupTestHostComponent] }).compileComponents();
    fixture = TestBed.createComponent(RadioGroupTestHostComponent);
    host = fixture.componentInstance;
    fixture.detectChanges();
  });

  const controls = (): HTMLInputElement[] =>
    fixture.debugElement.queryAll(By.css('input')).map((item) => item.nativeElement as HTMLInputElement);

  it('groups native radios with a fieldset and legend', () => {
    expect(fixture.debugElement.query(By.css('fieldset'))).toBeTruthy();
    expect(fixture.debugElement.query(By.css('legend')).nativeElement.textContent).toContain('Review depth');
    expect(controls().every((control) => control.name === 'review-depth')).toBeTrue();
  });

  it('preserves a typed value after native radio selection', () => {
    controls()[1].checked = true;
    controls()[1].dispatchEvent(new Event('change'));
    expect(host.value()).toBe(host.options[1].value);
    expect(host.value()?.id).toBe(2);
  });

  it('uses typed compareWith for equivalent non-string values', () => {
    expect(controls()[0].checked).toBeTrue();
    host.value.set({ id: 2 });
    fixture.detectChanges();
    expect(controls()[1].checked).toBeTrue();
  });

  it('retains native keyboard grouping and option disabled state', () => {
    expect(controls().every((control) => !control.hasAttribute('tabindex'))).toBeTrue();
    expect(controls()[2].disabled).toBeTrue();
    controls()[0].focus();
    expect(document.activeElement).toBe(controls()[0]);
    host.disabled.set(true);
    fixture.detectChanges();
    expect((fixture.debugElement.query(By.css('fieldset')).nativeElement as HTMLFieldSetElement).disabled).toBeTrue();
  });

  it('associates group help and an announced error', () => {
    host.error.set('Select a review depth.');
    fixture.detectChanges();
    const group = fixture.debugElement.query(By.css('fieldset')).nativeElement as HTMLFieldSetElement;
    expect(group.getAttribute('aria-invalid')).toBe('true');
    expect(group.getAttribute('aria-errormessage')).toBe('review-depth-error');
    expect(group.getAttribute('aria-describedby')).toBe('review-depth-description review-depth-error');
    expect(fixture.debugElement.query(By.css('[role="alert"]')).nativeElement.textContent).toContain(
      'Select a review depth.',
    );
    expect(fixture.debugElement.query(By.css('[role="alert"]')).attributes['aria-atomic']).toBe('true');
  });
});
