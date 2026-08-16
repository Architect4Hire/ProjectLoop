import { ChangeDetectionStrategy, Component, computed, input, output } from '@angular/core';

import { MenuComponent } from '../../primitives/menu/menu.component';
import { MenuItemDirective } from '../../primitives/menu/menu-item.directive';

export interface UserMenuAction<TAction = string> {
  readonly id: TAction;
  readonly label: string;
  readonly disabled?: boolean;
}

@Component({
  selector: 'lsd-user-menu',
  standalone: true,
  imports: [MenuComponent, MenuItemDirective],
  templateUrl: './user-menu.component.html',
  styleUrl: './user-menu.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class UserMenuComponent<TAction = string> {
  readonly id = input.required<string>();
  readonly displayName = input.required<string>();
  readonly identityDetail = input<string | undefined>(undefined);
  readonly avatarUrl = input<string | null>(null);
  readonly actions = input.required<readonly UserMenuAction<TAction>[]>();

  readonly actionRequested = output<TAction>();

  protected readonly triggerLabel = computed(() => `Account menu for ${this.displayName()}`);
  protected readonly initials = computed(() => {
    const parts = this.displayName().trim().split(/\s+/).filter(Boolean);
    if (!parts.length) return '?';
    return [parts[0], parts.length > 1 ? parts.at(-1) : undefined]
      .filter((part): part is string => Boolean(part))
      .map((part) => Array.from(part)[0])
      .join('')
      .toLocaleUpperCase();
  });

  protected requestAction(action: UserMenuAction<TAction>): void {
    if (!action.disabled) this.actionRequested.emit(action.id);
  }
}
