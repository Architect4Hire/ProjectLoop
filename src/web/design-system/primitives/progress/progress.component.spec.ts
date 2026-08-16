import { Component } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';

import { ProgressComponent } from './progress.component';

@Component({
  standalone: true,
  imports: [ProgressComponent],
  template: `
    <lsd-progress id="generation" label="Generating summary" [value]="value" [max]="maximum" />
  `,
})
class ProgressTestHostComponent {
  value: number | undefined = 25;
  maximum = 50;
}

describe('ProgressComponent', () => {
  let fixture: ComponentFixture<ProgressTestHostComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({ imports: [ProgressTestHostComponent] }).compileComponents();
    fixture = TestBed.createComponent(ProgressTestHostComponent);
    fixture.detectChanges();
  });

  const progress = (): HTMLProgressElement =>
    fixture.debugElement.query(By.css('progress')).nativeElement as HTMLProgressElement;

  it('normalizes determinate values into the native progress range', () => {
    expect(progress().max).toBe(50);
    expect(progress().value).toBe(25);

    fixture.componentInstance.value = 75;
    fixture.detectChanges();
    expect(progress().value).toBe(50);

    fixture.componentInstance.value = -10;
    fixture.detectChanges();
    expect(progress().value).toBe(0);
  });

  it('uses native indeterminate semantics by omitting value', () => {
    fixture.componentInstance.value = undefined;
    fixture.detectChanges();

    expect(progress().hasAttribute('value')).toBeFalse();
    expect(progress().hasAttribute('aria-valuenow')).toBeFalse();
    expect(progress().getAttribute('aria-valuetext')).toBe('In progress');
  });

  it('provides a visible label and value text as the accessible name and state', () => {
    const label = fixture.debugElement.query(By.css('label')).nativeElement as HTMLLabelElement;
    const value = fixture.debugElement.query(By.css('#generation-value')).nativeElement as HTMLElement;

    expect(label.htmlFor).toBe('generation-progress');
    expect(label.textContent).toContain('Generating summary');
    expect(progress().getAttribute('aria-labelledby')).toBe('generation-label');
    expect(value.textContent).toContain('50%');
    expect(progress().getAttribute('aria-valuetext')).toBe('50%');
  });
});
