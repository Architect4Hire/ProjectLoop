import { Component, signal } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';

import { AppNavigationComponent, AppNavigationLink } from './app-navigation.component';

@Component({
  standalone: true,
  imports: [AppNavigationComponent],
  template: `
    <lsd-app-navigation
      accessibleName="Primary"
      [links]="links"
      [compact]="compact()" />
  `,
})
class AppNavigationTestHostComponent {
  readonly compact = signal(false);
  links: readonly AppNavigationLink[] = [
    { label: 'Overview', href: '/overview', icon: 'info' },
    { label: 'Documents', href: '/documents', icon: 'search', active: true, count: 7 },
  ];
}

describe('AppNavigationComponent', () => {
  let fixture: ComponentFixture<AppNavigationTestHostComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({ imports: [AppNavigationTestHostComponent] }).compileComponents();
    fixture = TestBed.createComponent(AppNavigationTestHostComponent);
    fixture.detectChanges();
  });

  it('renders caller-authorized links and exposes only the supplied active state', () => {
    const nav = fixture.debugElement.query(By.css('nav')).nativeElement as HTMLElement;
    const anchors = fixture.debugElement.queryAll(By.css('a'));

    expect(nav.getAttribute('aria-label')).toBe('Primary');
    expect(anchors.map(({ nativeElement }) => nativeElement.getAttribute('href'))).toEqual(['/overview', '/documents']);
    expect(anchors[0].attributes['aria-current']).toBeUndefined();
    expect(anchors[1].attributes['aria-current']).toBe('page');
    expect(anchors[1].nativeElement.textContent).toContain('Documents');
    expect(anchors[1].nativeElement.textContent).toContain('7');
  });

  it('exposes compact presentation for narrow navigation while retaining labels', () => {
    fixture.componentInstance.compact.set(true);
    fixture.detectChanges();

    const nav = fixture.debugElement.query(By.css('nav')).nativeElement as HTMLElement;
    const labels = fixture.debugElement.queryAll(By.css('.lsd-app-navigation__label'));
    expect(nav.getAttribute('data-compact')).toBe('true');
    expect(labels.every((label) => label.classes['lsd-sr-only'])).toBeTrue();
    expect(labels.map(({ nativeElement }) => nativeElement.textContent.trim())).toEqual(['Overview', 'Documents']);
  });
});
