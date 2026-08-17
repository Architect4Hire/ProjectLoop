import { Component } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';

import { ButtonComponent } from '../../primitives/button/button.component';
import { DocumentCardComponent, type DocumentCardViewModel } from './document-card.component';

@Component({
  standalone: true,
  imports: [ButtonComponent, DocumentCardComponent],
  template: `
    <lsd-document-card [document]="document">
      <button lsdButton lsdDocumentCardActions type="button">Open details</button>
    </lsd-document-card>
  `,
})
class DocumentCardTestHostComponent {
  readonly document: DocumentCardViewModel = {
    id: 'architecture-plan',
    title: 'Enterprise architecture transition and implementation plan for the multi-region operating model',
    category: 'Architecture document',
    status: { label: 'Published', variant: 'approved' },
    visibility: { label: 'Confidential — authorized project members only', variant: 'warning' },
    version: { label: 'v2026.08.16-exact', qualifier: 'published' },
    updated: { label: 'Updated August 16, 2026 at 2:45 PM CDT', dateTime: '2026-08-16T14:45:00-05:00' },
  };
}

describe('DocumentCardComponent', () => {
  let fixture: ComponentFixture<DocumentCardTestHostComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({ imports: [DocumentCardTestHostComponent] }).compileComponents();
    fixture = TestBed.createComponent(DocumentCardTestHostComponent);
    fixture.detectChanges();
  });

  it('renders long display-ready names and the exact version without alteration', () => {
    const text = fixture.nativeElement.textContent as string;
    expect(text).toContain(fixture.componentInstance.document.title);
    expect(text).toContain('v2026.08.16-exact');
    expect(fixture.debugElement.query(By.css('lsd-version-chip'))).not.toBeNull();
  });

  it('presents confidential visibility and status as visible Badge text', () => {
    const badges = fixture.debugElement.queryAll(By.css('.lsd-document-card__badges lsd-badge'));
    expect(badges.map((badge) => badge.nativeElement.textContent.trim())).toEqual([
      'Published',
      'Confidential — authorized project members only',
    ]);
  });

  it('preserves updated metadata and projected actions without performing them', () => {
    const time = fixture.debugElement.query(By.css('time')).nativeElement as HTMLTimeElement;
    expect(time.dateTime).toBe('2026-08-16T14:45:00-05:00');
    expect(time.textContent).toContain('Updated August 16, 2026 at 2:45 PM CDT');
    expect(fixture.debugElement.query(By.css('[lsdDocumentCardActions]')).nativeElement.textContent).toContain('Open details');
  });

  it('provides narrow-layout hooks while retaining caller-supplied DOM order', () => {
    const card = fixture.debugElement.query(By.css('.lsd-document-card')).nativeElement as HTMLElement;
    expect(card.querySelector('.lsd-document-card__header')).not.toBeNull();
    expect(card.querySelector('.lsd-document-card__badges')).not.toBeNull();
    expect(card.querySelector('.lsd-document-card__metadata')).not.toBeNull();
  });
});
