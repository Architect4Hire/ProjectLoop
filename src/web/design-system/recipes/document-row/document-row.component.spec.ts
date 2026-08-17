import { Component } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';

import { ButtonComponent } from '../../primitives/button/button.component';
import type { DocumentCardViewModel } from '../document-card/document-card.component';
import { DocumentRowComponent } from './document-row.component';

@Component({
  standalone: true,
  imports: [ButtonComponent, DocumentRowComponent],
  template: `
    <table>
      <caption>Authorized documents</caption>
      <thead><tr><th>Title</th><th>Category</th><th>Status</th><th>Visibility</th><th>Version</th><th>Updated</th><th>Actions</th></tr></thead>
      <tbody>
        <tr lsdDocumentRow [document]="document" actionsLabel="Available actions">
          <button lsdButton lsdDocumentRowActions type="button">View details</button>
        </tr>
      </tbody>
    </table>
  `,
})
class DocumentRowTestHostComponent {
  readonly document: DocumentCardViewModel = {
    id: 'security-plan',
    title: 'Security architecture and implementation plan',
    category: 'Architecture document',
    status: { label: 'Published', variant: 'approved' },
    visibility: { label: 'Confidential', variant: 'warning' },
    version: { label: 'v4.2-exact', qualifier: 'approved' },
    updated: { label: 'Updated August 16, 2026', dateTime: '2026-08-16' },
  };
}

describe('DocumentRowComponent', () => {
  let fixture: ComponentFixture<DocumentRowTestHostComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({ imports: [DocumentRowTestHostComponent] }).compileComponents();
    fixture = TestBed.createComponent(DocumentRowTestHostComponent);
    fixture.detectChanges();
  });

  it('uses the document title as the native row header', () => {
    const rowHeader = fixture.debugElement.query(By.css('tbody th')).nativeElement as HTMLTableCellElement;
    expect(rowHeader.scope).toBe('row');
    expect(rowHeader.textContent).toBe('Security architecture and implementation plan');
  });

  it('labels projected actions with their document context', () => {
    const actions = fixture.debugElement.query(By.css('.lsd-document-row__actions')).nativeElement as HTMLElement;
    expect(actions.getAttribute('role')).toBe('group');
    expect(actions.getAttribute('aria-label')).toBe('Available actions for Security architecture and implementation plan');
    expect(actions.textContent).toContain('View details');
  });

  it('preserves all card metadata and exact-version presentation', () => {
    const text = fixture.nativeElement.textContent as string;
    expect(text).toContain('Architecture document');
    expect(text).toContain('Published');
    expect(text).toContain('Confidential');
    expect(text).toContain('v4.2-exact');
    expect(text).toContain('Approved');
    expect(text).toContain('Updated August 16, 2026');
    expect(fixture.debugElement.query(By.css('lsd-version-chip'))).not.toBeNull();
  });

  it('does not add sorting or selection controls', () => {
    const row = fixture.debugElement.query(By.css('tbody tr')).nativeElement as HTMLTableRowElement;
    expect(row.querySelector('input[type="checkbox"]')).toBeNull();
    expect(row.getAttribute('aria-selected')).toBeNull();
    expect(row.querySelector('[aria-sort]')).toBeNull();
  });
});
