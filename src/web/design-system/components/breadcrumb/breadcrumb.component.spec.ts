import { Component } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';

import { BreadcrumbComponent, BreadcrumbItem } from './breadcrumb.component';

@Component({
  standalone: true,
  imports: [BreadcrumbComponent],
  template: `<lsd-breadcrumb accessibleName="Project location" [items]="items" />`,
})
class BreadcrumbTestHostComponent {
  items: readonly BreadcrumbItem[] = [{ label: 'Projects', href: '/projects' }];
}

describe('BreadcrumbComponent', () => {
  let fixture: ComponentFixture<BreadcrumbTestHostComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({ imports: [BreadcrumbTestHostComponent] }).compileComponents();
    fixture = TestBed.createComponent(BreadcrumbTestHostComponent);
    fixture.detectChanges();
  });

  const anchors = (): HTMLAnchorElement[] =>
    fixture.debugElement.queryAll(By.css('a')).map(({ nativeElement }) => nativeElement as HTMLAnchorElement);

  it('renders a one-item breadcrumb as the current page', () => {
    const nav = fixture.debugElement.query(By.css('nav')).nativeElement as HTMLElement;
    expect(nav.getAttribute('aria-label')).toBe('Project location');
    expect(fixture.debugElement.queryAll(By.css('ol > li')).length).toBe(1);
    expect(anchors()[0].getAttribute('href')).toBe('/projects');
    expect(anchors()[0].getAttribute('aria-current')).toBe('page');
  });

  it('renders nested caller-supplied labels and URLs in order', () => {
    fixture.componentInstance.items = [
      { label: 'Projects', href: '/projects' },
      { label: 'Lake Shore Drive', href: '/projects/lake-shore-drive' },
      { label: 'Requirements', href: '/projects/lake-shore-drive/requirements' },
    ];
    fixture.detectChanges();

    expect(anchors().map((anchor) => anchor.textContent?.trim())).toEqual([
      'Projects',
      'Lake Shore Drive',
      'Requirements',
    ]);
    expect(anchors().map((anchor) => anchor.getAttribute('href'))).toEqual([
      '/projects',
      '/projects/lake-shore-drive',
      '/projects/lake-shore-drive/requirements',
    ]);
    expect(anchors().slice(0, -1).every((anchor) => !anchor.hasAttribute('aria-current'))).toBeTrue();
    expect(anchors().at(-1)?.getAttribute('aria-current')).toBe('page');
    expect(fixture.debugElement.queryAll(By.css('lsd-icon')).length).toBe(2);
  });

  it('preserves a long label as accessible text while allowing visual truncation', () => {
    const label = 'A deliberately long current-page label that must remain complete for assistive technology';
    fixture.componentInstance.items = [{ label, href: '/projects/long-label' }];
    fixture.detectChanges();

    const labelElement = fixture.debugElement.query(By.css('.lsd-breadcrumb__label')).nativeElement as HTMLElement;
    expect(labelElement.textContent).toBe(label);
    expect(labelElement.classList).toContain('lsd-breadcrumb__label');
    expect(anchors()[0].getAttribute('aria-current')).toBe('page');
  });
});
