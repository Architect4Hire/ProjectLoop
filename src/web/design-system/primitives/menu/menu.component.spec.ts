import { Component } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';

import { MenuComponent } from './menu.component';
import { MenuItemDirective } from './menu-item.directive';

@Component({
  standalone: true,
  imports: [MenuComponent, MenuItemDirective],
  template: `
    <lsd-menu id="row-actions" accessibleLabel="Row actions">
      <span lsdMenuTriggerContent aria-hidden="true">Actions</span>
      <button lsdMenuItem type="button">Edit</button>
      <button lsdMenuItem type="button" disabled>Unavailable</button>
      <a lsdMenuItem href="/projects/42">Open project</a>
    </lsd-menu>
    <button class="outside" type="button">Outside</button>
  `,
})
class MenuTestHostComponent {}

describe('MenuComponent', () => {
  let fixture: ComponentFixture<MenuTestHostComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({ imports: [MenuTestHostComponent] }).compileComponents();
    fixture = TestBed.createComponent(MenuTestHostComponent);
    fixture.detectChanges();
  });

  const trigger = (): HTMLButtonElement =>
    fixture.debugElement.query(By.css('#row-actions-trigger')).nativeElement as HTMLButtonElement;
  const menu = (): HTMLElement =>
    fixture.debugElement.query(By.css('#row-actions-menu')).nativeElement as HTMLElement;
  const items = (): HTMLElement[] =>
    fixture.debugElement.queryAll(By.css('[lsdMenuItem]')).map(({ nativeElement }) => nativeElement as HTMLElement);

  it('uses a native menu button and moves focus with keyboard navigation', async () => {
    trigger().dispatchEvent(new KeyboardEvent('keydown', { key: 'ArrowDown', bubbles: true }));
    fixture.detectChanges();
    await fixture.whenStable();

    expect(trigger().getAttribute('aria-expanded')).toBe('true');
    expect(menu().getAttribute('role')).toBe('menu');
    expect(document.activeElement).toBe(items()[0]);

    items()[0].dispatchEvent(new KeyboardEvent('keydown', { key: 'ArrowDown', bubbles: true }));
    expect(document.activeElement).toBe(items()[2]);
    items()[2].dispatchEvent(new KeyboardEvent('keydown', { key: 'Home', bubbles: true }));
    expect(document.activeElement).toBe(items()[0]);
  });

  it('dismisses on Escape and restores focus to the trigger', async () => {
    trigger().click();
    fixture.detectChanges();
    await fixture.whenStable();
    items()[0].dispatchEvent(new KeyboardEvent('keydown', { key: 'Escape', bubbles: true }));
    fixture.detectChanges();

    expect(menu().hidden).toBeTrue();
    expect(document.activeElement).toBe(trigger());
  });

  it('dismisses on an outside pointer interaction without stealing focus', async () => {
    trigger().click();
    fixture.detectChanges();
    await fixture.whenStable();
    const outside = fixture.debugElement.query(By.css('.outside')).nativeElement as HTMLButtonElement;
    outside.focus();
    outside.dispatchEvent(new PointerEvent('pointerdown', { bubbles: true }));
    fixture.detectChanges();

    expect(menu().hidden).toBeTrue();
    expect(document.activeElement).toBe(outside);
  });
});
