import { Component } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { ErrorSummaryComponent, type ErrorSummaryItem } from './error-summary.component';

@Component({
  standalone: true,
  imports: [ErrorSummaryComponent],
  template: `
    <lsd-error-summary
      id="form-errors"
      [errors]="errors"
      [failedSubmissionCount]="failedSubmissionCount"
      pluralTitle="Resolve {count} validation issues" />
    <label for="project-name">Project name</label>
    <input id="project-name" aria-invalid="true" aria-describedby="project-name-error" />
    <p id="project-name-error">Project name is required.</p>
    <textarea id="decision-comment"></textarea>
  `,
})
class ErrorSummaryTestHostComponent {
  errors: readonly ErrorSummaryItem[] = [
    { controlId: 'project-name', label: 'Enter a project name' },
    { controlId: 'decision-comment', label: 'Explain the decision' },
  ];
  failedSubmissionCount = 0;
}

describe('ErrorSummaryComponent', () => {
  let fixture: ComponentFixture<ErrorSummaryTestHostComponent>;
  let host: ErrorSummaryTestHostComponent;
  beforeEach(async () => {
    await TestBed.configureTestingModule({ imports: [ErrorSummaryTestHostComponent] }).compileComponents();
    fixture = TestBed.createComponent(ErrorSummaryTestHostComponent);
    host = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('focuses the summary after a new failed submission and preserves native control errors', async () => {
    host.failedSubmissionCount = 1;
    fixture.detectChanges();
    await fixture.whenStable();
    const summary = fixture.debugElement.query(By.css('.lsd-error-summary')).nativeElement as HTMLElement;
    expect(document.activeElement).toBe(summary);
    const input = fixture.debugElement.query(By.css('#project-name')).nativeElement as HTMLInputElement;
    expect(input.getAttribute('aria-invalid')).toBe('true');
    expect(input.getAttribute('aria-describedby')).toBe('project-name-error');
  });

  it('links directly to caller-supplied invalid control IDs', () => {
    const links = fixture.debugElement.queryAll(By.css('a[lsdLink]'));
    expect(links.map((link) => link.nativeElement.getAttribute('href'))).toEqual([
      '#project-name',
      '#decision-comment',
    ]);
  });

  it('uses caller-supplied singular and plural title inputs', () => {
    expect(fixture.nativeElement.textContent).toContain('Resolve 2 validation issues');
    host.errors = [{ controlId: 'project-name', label: 'Enter a project name' }];
    fixture.detectChanges();
    expect(fixture.nativeElement.textContent).toContain('There is 1 error');
  });

  it('renders and focuses nothing when there are zero errors', async () => {
    host.errors = [];
    host.failedSubmissionCount = 1;
    fixture.detectChanges();
    await fixture.whenStable();
    expect(fixture.debugElement.query(By.css('lsd-alert-banner'))).toBeNull();
    expect(fixture.debugElement.query(By.css('.lsd-error-summary'))).toBeNull();
  });
});
