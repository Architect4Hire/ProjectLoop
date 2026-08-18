import { Component, signal } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';

import { ApprovalCommentFieldComponent } from './approval-comment-field.component';

@Component({
  standalone: true,
  imports: [ApprovalCommentFieldComponent],
  template: `
    <lsd-approval-comment-field
      id="decision-comment"
      [required]="required()"
      [maxLength]="20"
      [error]="error()"
      [(value)]="value" />
  `,
})
class ApprovalCommentFieldTestHostComponent {
  readonly required = signal(false);
  readonly error = signal<string | undefined>(undefined);
  value = '';
}

describe('ApprovalCommentFieldComponent', () => {
  let fixture: ComponentFixture<ApprovalCommentFieldTestHostComponent>;
  let host: ApprovalCommentFieldTestHostComponent;

  beforeEach(async () => {
    await TestBed.configureTestingModule({ imports: [ApprovalCommentFieldTestHostComponent] }).compileComponents();
    fixture = TestBed.createComponent(ApprovalCommentFieldTestHostComponent);
    host = fixture.componentInstance;
    fixture.detectChanges();
  });

  const textarea = (): HTMLTextAreaElement =>
    fixture.debugElement.query(By.css('textarea')).nativeElement as HTMLTextAreaElement;

  it('labels optional comments explicitly', () => {
    const label = fixture.debugElement.query(By.css('label')).nativeElement as HTMLLabelElement;
    expect(label.textContent).toContain('Decision comment (optional)');
    expect(textarea().required).toBeFalse();
  });

  it('labels and exposes caller-required comments through native semantics', () => {
    host.required.set(true);
    fixture.detectChanges();
    expect(fixture.debugElement.query(By.css('label')).nativeElement.textContent).toContain(
      'Decision comment (required)',
    );
    expect(textarea().required).toBeTrue();
  });

  it('keeps the value caller-controlled and reports its max-length count', () => {
    textarea().value = 'Looks good';
    textarea().dispatchEvent(new Event('input'));
    fixture.detectChanges();
    expect(host.value).toBe('Looks good');
    expect(fixture.debugElement.query(By.css('#decision-comment-count')).nativeElement.textContent).toContain(
      '10 of 20 characters',
    );
  });

  it('associates and announces a caller-supplied error', () => {
    host.error.set('A comment is required for this decision.');
    fixture.detectChanges();
    expect(textarea().getAttribute('aria-invalid')).toBe('true');
    expect(textarea().getAttribute('aria-errormessage')).toBe('decision-comment-error');
    expect(fixture.debugElement.query(By.css('[role="alert"]')).nativeElement.textContent).toContain(
      'A comment is required for this decision.',
    );
  });
});
