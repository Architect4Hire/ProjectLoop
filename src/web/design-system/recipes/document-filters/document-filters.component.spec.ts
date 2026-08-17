import { Component } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';

import {
  DocumentFiltersComponent,
  type DocumentFilterChangeIntent,
  type DocumentFilterOption,
} from './document-filters.component';

const options = (label: string): readonly DocumentFilterOption[] => [
  { value: `${label}-one`, label: `${label} one` },
  { value: `${label}-two`, label: `${label} two` },
];

@Component({
  standalone: true,
  imports: [DocumentFiltersComponent],
  template: `
    <lsd-document-filters
      id="document-library"
      accessibleName="Project Loop document filters"
      [projectOptions]="projectOptions"
      [categoryOptions]="categoryOptions"
      [statusOptions]="statusOptions"
      [visibilityOptions]="visibilityOptions"
      (filterChange)="intent = $event" />
  `,
})
class DocumentFiltersTestHostComponent {
  readonly projectOptions = options('project');
  readonly categoryOptions = options('category');
  readonly statusOptions = options('status');
  readonly visibilityOptions = options('visibility');
  intent: DocumentFilterChangeIntent | null = null;
}

describe('DocumentFiltersComponent', () => {
  let fixture: ComponentFixture<DocumentFiltersTestHostComponent>;
  let host: DocumentFiltersTestHostComponent;

  beforeEach(async () => {
    await TestBed.configureTestingModule({ imports: [DocumentFiltersTestHostComponent] }).compileComponents();
    fixture = TestBed.createComponent(DocumentFiltersTestHostComponent);
    host = fixture.componentInstance;
    fixture.detectChanges();
  });

  const chooseSecondOption = (id: string): void => {
    const select = fixture.debugElement.query(By.css(`#document-library-${id}`)).nativeElement as HTMLSelectElement;
    select.value = '1';
    select.dispatchEvent(new Event('change'));
    fixture.detectChanges();
  };

  it('associates accessible labels with all four native controls', () => {
    const section = fixture.debugElement.query(By.css('lsd-filter-action-bar section')).nativeElement as HTMLElement;
    expect(section.getAttribute('aria-label')).toBe('Project Loop document filters');
    for (const field of ['project', 'category', 'status', 'visibility']) {
      const select = fixture.debugElement.query(By.css(`#document-library-${field}`)).nativeElement as HTMLSelectElement;
      const label = fixture.debugElement.query(By.css(`label[for="document-library-${field}"]`)).nativeElement as HTMLLabelElement;
      expect(label.textContent?.trim()).toBe(field[0].toUpperCase() + field.slice(1));
      expect(label.htmlFor).toBe(select.id);
    }
  });

  it('emits one typed submit intent containing current selections', () => {
    chooseSecondOption('project');
    chooseSecondOption('status');
    const form = fixture.debugElement.query(By.css('form')).nativeElement as HTMLFormElement;
    form.dispatchEvent(new SubmitEvent('submit', { bubbles: true, cancelable: true }));
    fixture.detectChanges();

    expect(host.intent).toEqual({
      source: 'submit',
      filters: { project: 'project-two', category: null, status: 'status-two', visibility: null },
    });
  });

  it('clears every control and emits one reset intent', () => {
    for (const field of ['project', 'category', 'status', 'visibility']) chooseSecondOption(field);
    const reset = fixture.debugElement.queryAll(By.css('lsd-button button'))
      .find((button) => button.nativeElement.textContent.includes('Reset'))?.nativeElement as HTMLButtonElement;
    reset.click();
    fixture.detectChanges();

    expect(host.intent).toEqual({
      source: 'reset',
      filters: { project: null, category: null, status: null, visibility: null },
    });
    for (const select of fixture.debugElement.queryAll(By.css('select'))) {
      expect((select.nativeElement as HTMLSelectElement).value).toBe('');
    }
  });
});
