import { Component } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';

import { PageHeaderComponent, PageHeaderHeadingLevel } from './page-header.component';

@Component({
  standalone: true,
  imports: [PageHeaderComponent],
  template: `
    <lsd-page-header
      title="Requirements"
      description="Review and manage requirements."
      [headingLevel]="headingLevel"
      [breadcrumbs]="breadcrumbs"
      [metadata]="metadata">
      <div lsdPageHeaderActions>
        <button type="button">Export</button>
        <button type="button">Add requirement</button>
      </div>
    </lsd-page-header>
  `,
})
class PageHeaderTestHostComponent {
  headingLevel: PageHeaderHeadingLevel = 1;
  readonly breadcrumbs = [
    { label: 'Projects', href: '/projects' },
    { label: 'Requirements', href: '/projects/current/requirements' },
  ];
  readonly metadata = [{ label: 'Updated', value: 'Today' }];
}

describe('PageHeaderComponent', () => {
  let fixture: ComponentFixture<PageHeaderTestHostComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({ imports: [PageHeaderTestHostComponent] }).compileComponents();
    fixture = TestBed.createComponent(PageHeaderTestHostComponent);
    fixture.detectChanges();
  });

  it('renders the caller-selected native heading level without duplicate headings', () => {
    expect(fixture.debugElement.queryAll(By.css('h1'))).toHaveSize(1);
    expect(fixture.debugElement.queryAll(By.css('h2, h3, h4, h5, h6'))).toHaveSize(0);

    fixture.componentInstance.headingLevel = 3;
    fixture.detectChanges();
    expect(fixture.debugElement.queryAll(By.css('h1, h2, h4, h5, h6'))).toHaveSize(0);
    expect(fixture.debugElement.query(By.css('h3')).nativeElement.textContent).toContain('Requirements');
  });

  it('keeps projected actions in a wrapping region for narrow layouts', () => {
    const actions = fixture.debugElement.query(By.css('.lsd-page-header__actions'));
    expect(actions.attributes['aria-label']).toBe('Page actions');
    expect(actions.queryAll(By.css('button')).map(({ nativeElement }) => nativeElement.textContent.trim())).toEqual([
      'Export',
      'Add requirement',
    ]);
    expect(actions.classes['lsd-page-header__actions']).toBeTrue();
  });

  it('composes breadcrumb and semantic metadata without feature-specific fields', () => {
    expect(fixture.debugElement.query(By.css('lsd-breadcrumb'))).not.toBeNull();
    expect(fixture.debugElement.query(By.css('dl dt')).nativeElement.textContent).toContain('Updated');
    expect(fixture.debugElement.query(By.css('dl dd')).nativeElement.textContent).toContain('Today');
  });
});
