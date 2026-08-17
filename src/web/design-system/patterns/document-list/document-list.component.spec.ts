import { Component } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';

import type { DocumentCardViewModel } from '../../recipes/document-card/document-card.component';
import { DocumentListComponent } from './document-list.component';

const document = (id: string, title: string): DocumentCardViewModel => ({
  id,
  title,
  category: 'Architecture',
  status: { label: 'Published', variant: 'approved' },
  visibility: { label: 'Confidential', variant: 'warning' },
  version: { label: `v-${id}`, qualifier: 'published' },
  updated: { label: `Updated ${id}`, dateTime: '2026-08-16' },
});

@Component({
  standalone: true,
  imports: [DocumentListComponent],
  template: `<lsd-document-list id="project-documents" accessibleName="Project documents" [documents]="documents" />`,
})
class DocumentListTestHostComponent {
  documents: readonly DocumentCardViewModel[] = [];
}

describe('DocumentListComponent', () => {
  let fixture: ComponentFixture<DocumentListTestHostComponent>;
  let host: DocumentListTestHostComponent;

  beforeEach(async () => {
    await TestBed.configureTestingModule({ imports: [DocumentListTestHostComponent] }).compileComponents();
    fixture = TestBed.createComponent(DocumentListTestHostComponent);
    host = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('renders an explicit empty presentation without collection markup', () => {
    expect(fixture.debugElement.query(By.css('lsd-state-feedback'))).not.toBeNull();
    expect(fixture.nativeElement.textContent).toContain('No documents');
    expect(fixture.debugElement.query(By.css('table'))).toBeNull();
    expect(fixture.debugElement.query(By.css('ol'))).toBeNull();
  });

  it('renders one document with identical row and card information', () => {
    host.documents = [document('one', 'Document one')];
    fixture.detectChanges();

    expect(fixture.debugElement.queryAll(By.css('tr[lsdDocumentRow]')).length).toBe(1);
    expect(fixture.debugElement.queryAll(By.css('lsd-document-card')).length).toBe(1);
    for (const presentation of fixture.debugElement.queryAll(By.css('[data-presentation]'))) {
      const text = presentation.nativeElement.textContent as string;
      for (const value of ['Document one', 'Architecture', 'Published', 'Confidential', 'v-one', 'Updated one']) {
        expect(text).toContain(value);
      }
    }
  });

  it('preserves caller order for many documents in both presentations', () => {
    host.documents = [document('first', 'First'), document('second', 'Second'), document('third', 'Third')];
    fixture.detectChanges();

    const rowTitles = fixture.debugElement.queryAll(By.css('tbody th[scope="row"]'))
      .map((header) => header.nativeElement.textContent.trim());
    const cardTitles = fixture.debugElement.queryAll(By.css('.lsd-document-list__cards article h3'))
      .map((header) => header.nativeElement.textContent.trim());
    expect(rowTitles).toEqual(['First', 'Second', 'Third']);
    expect(cardTitles).toEqual(rowTitles);
  });

  it('provides mutually switched row and narrow card presentations', () => {
    host.documents = [document('one', 'Document one')];
    fixture.detectChanges();

    expect(fixture.debugElement.query(By.css('[data-presentation="rows"]'))).not.toBeNull();
    expect(fixture.debugElement.query(By.css('[data-presentation="cards"]'))).not.toBeNull();
    expect(fixture.debugElement.query(By.css('.lsd-document-list__cards ol'))).toBeNull();
  });
});
