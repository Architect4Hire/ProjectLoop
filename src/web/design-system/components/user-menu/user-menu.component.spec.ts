import { Component } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';

import { UserMenuAction, UserMenuComponent } from './user-menu.component';

type AccountAction = 'profile' | 'sign-out';

@Component({
  standalone: true,
  imports: [UserMenuComponent],
  template: `
    <lsd-user-menu
      id="account-menu"
      [displayName]="displayName"
      [identityDetail]="identityDetail"
      [avatarUrl]="avatarUrl"
      [actions]="actions"
      (actionRequested)="requestedAction = $event" />
  `,
})
class UserMenuTestHostComponent {
  displayName = 'Ada Lovelace';
  identityDetail = 'ada@example.com';
  avatarUrl: string | null = null;
  actions: readonly UserMenuAction<AccountAction>[] = [
    { id: 'profile', label: 'Profile' },
    { id: 'sign-out', label: 'Sign out' },
  ];
  requestedAction: AccountAction | null = null;
}

describe('UserMenuComponent', () => {
  let fixture: ComponentFixture<UserMenuTestHostComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({ imports: [UserMenuTestHostComponent] }).compileComponents();
    fixture = TestBed.createComponent(UserMenuTestHostComponent);
    fixture.detectChanges();
  });

  const trigger = (): HTMLButtonElement =>
    fixture.debugElement.query(By.css('#account-menu-trigger')).nativeElement as HTMLButtonElement;
  const items = (): HTMLButtonElement[] =>
    fixture.debugElement.queryAll(By.css('button[lsdMenuItem]')).map(({ nativeElement }) => nativeElement as HTMLButtonElement);

  it('renders initials when no avatar URL is supplied', () => {
    expect(fixture.debugElement.query(By.css('.lsd-user-menu__initials')).nativeElement.textContent.trim()).toBe('AL');
    expect(fixture.debugElement.query(By.css('img'))).toBeNull();
    expect(trigger().getAttribute('aria-label')).toBe('Account menu for Ada Lovelace');
  });

  it('preserves long display-safe identity text while allowing visual truncation', () => {
    const longName = 'Alexandria Catherine Montgomery-Worthington the Third';
    const longDetail = 'alexandria.montgomery-worthington@example-enterprise.test';
    fixture.componentInstance.displayName = longName;
    fixture.componentInstance.identityDetail = longDetail;
    fixture.detectChanges();

    expect(fixture.debugElement.query(By.css('.lsd-user-menu__name')).nativeElement.textContent).toBe(longName);
    expect(fixture.debugElement.query(By.css('.lsd-user-menu__detail')).nativeElement.textContent).toBe(longDetail);
    expect(trigger().getAttribute('aria-label')).toBe(`Account menu for ${longName}`);
  });

  it('delegates keyboard opening and focus management to the Menu primitive', async () => {
    trigger().dispatchEvent(new KeyboardEvent('keydown', { key: 'ArrowDown', bubbles: true }));
    fixture.detectChanges();
    await fixture.whenStable();

    expect(trigger().getAttribute('aria-expanded')).toBe('true');
    expect(document.activeElement).toBe(items()[0]);
  });

  it('emits caller-owned action intent without performing the action', async () => {
    trigger().click();
    fixture.detectChanges();
    await fixture.whenStable();
    items()[1].click();

    expect(fixture.componentInstance.requestedAction).toBe('sign-out');
    expect(trigger().getAttribute('aria-expanded')).toBe('false');
  });
});
