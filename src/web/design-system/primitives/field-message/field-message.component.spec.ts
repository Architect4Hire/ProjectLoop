import { Component } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';

import { FieldMessageComponent } from './field-message.component';

@Component({
  standalone: true,
  imports: [FieldMessageComponent],
  template: `
    <input aria-describedby="project-name-help project-name-error" aria-errormessage="project-name-error" />
    <lsd-field-message id="project-name-help">Use the client-facing name.</lsd-field-message>
    <lsd-field-message id="project-name-error" kind="error">A project name is required.</lsd-field-message>
    <lsd-field-message id="project-name-success" kind="success">Project name is available.</lsd-field-message>
  `,
})
class FieldMessageTestHostComponent {}

describe('FieldMessageComponent', () => {
  let fixture: ComponentFixture<FieldMessageTestHostComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({ imports: [FieldMessageTestHostComponent] }).compileComponents();
    fixture = TestBed.createComponent(FieldMessageTestHostComponent);
    fixture.detectChanges();
  });

  it('renders caller-stable IDs for control description association', () => {
    const control = fixture.debugElement.query(By.css('input')).nativeElement as HTMLInputElement;
    const help = fixture.debugElement.query(By.css('#project-name-help')).nativeElement as HTMLElement;
    const error = fixture.debugElement.query(By.css('#project-name-error')).nativeElement as HTMLElement;

    expect(control.getAttribute('aria-describedby')).toBe('project-name-help project-name-error');
    expect(control.getAttribute('aria-errormessage')).toBe('project-name-error');
    expect(help.id).toBe('project-name-help');
    expect(error.id).toBe('project-name-error');
    expect(help.hasAttribute('aria-live')).toBeFalse();
  });

  it('announces an error atomically and assertively', () => {
    const error = fixture.debugElement.query(By.css('#project-name-error')).nativeElement as HTMLElement;
    expect(error.getAttribute('role')).toBe('alert');
    expect(error.getAttribute('aria-live')).toBe('assertive');
    expect(error.getAttribute('aria-atomic')).toBe('true');
    expect(error.textContent).toContain('A project name is required.');
  });

  it('announces success politely without treating it as an error', () => {
    const success = fixture.debugElement.query(By.css('#project-name-success')).nativeElement as HTMLElement;
    expect(success.getAttribute('role')).toBe('status');
    expect(success.getAttribute('aria-live')).toBe('polite');
    expect(success.classList).toContain('text-status-success');
  });
});
