import { Component } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';

import { PortalShellComponent } from './portal-shell.component';

@Component({
  standalone: true,
  imports: [PortalShellComponent],
  template: `
    <lsd-portal-shell
      navigationLabel="Workspace navigation"
      mainLabel="Portal content"
      [navigationLinks]="navigationLinks">
      <div lsdPortalHeader>Account header</div>
      <section lsdPortalMain>Routed page outlet</section>
      <aside lsdPortalNotifications aria-label="Notifications">Notification viewport</aside>
    </lsd-portal-shell>
  `,
})
class PortalShellTestHostComponent {
  readonly navigationLinks = [
    { label: 'Overview', href: '/overview', icon: 'info' as const, active: true },
    { label: 'Search', href: '/search', icon: 'search' as const },
  ];
}

describe('PortalShellComponent', () => {
  let fixture: ComponentFixture<PortalShellTestHostComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({ imports: [PortalShellTestHostComponent] }).compileComponents();
    fixture = TestBed.createComponent(PortalShellTestHostComponent);
    fixture.detectChanges();
  });

  it('provides named semantic landmarks', () => {
    expect(fixture.debugElement.query(By.css('header')).nativeElement.textContent).toContain('Account header');
    expect(fixture.debugElement.query(By.css('nav')).attributes['aria-label']).toBe('Workspace navigation');
    expect(fixture.debugElement.query(By.css('main')).attributes['aria-label']).toBe('Portal content');
  });

  it('connects the public skip link to the focusable main target', () => {
    const skipLink = fixture.debugElement.query(By.css('lsd-skip-link a')).nativeElement as HTMLAnchorElement;
    const main = fixture.debugElement.query(By.css('main')).nativeElement as HTMLElement;

    expect(skipLink.getAttribute('href')).toBe('#main-content');
    expect(main.id).toBe('main-content');
    expect(main.tabIndex).toBe(-1);
  });

  it('renders the public navigation in the responsive navigation region', () => {
    const navigation = fixture.debugElement.query(By.css('.lsd-portal-shell__navigation'));
    expect(navigation.query(By.css('lsd-app-navigation'))).not.toBeNull();
    expect(navigation.queryAll(By.css('a')).map(({ nativeElement }) => nativeElement.getAttribute('href'))).toEqual([
      '/overview',
      '/search',
    ]);
  });

  it('projects main outlet and notification viewport content into separate slots', () => {
    expect(fixture.debugElement.query(By.css('.lsd-portal-shell__main')).nativeElement.textContent).toContain('Routed page outlet');
    expect(fixture.debugElement.query(By.css('.lsd-portal-shell__notifications')).nativeElement.textContent).toContain('Notification viewport');
  });
});
