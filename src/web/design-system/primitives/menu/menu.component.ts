import {
  ChangeDetectionStrategy,
  Component,
  ElementRef,
  contentChildren,
  input,
  signal,
  viewChild,
} from '@angular/core';

import { elevationTokens } from '../../tokens/elevation';
import { globalLayers } from '../../tokens/layers';
import { MenuItemDirective } from './menu-item.directive';

@Component({
  selector: 'lsd-menu',
  standalone: true,
  host: {
    '[attr.id]': 'null',
    '(document:pointerdown)': 'handleDocumentPointerDown($event)',
  },
  templateUrl: './menu.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class MenuComponent {
  readonly id = input.required<string>();
  readonly accessibleLabel = input.required<string>();

  protected readonly open = signal(false);
  protected readonly menuShadow = elevationTokens.popover;
  protected readonly menuLayer = globalLayers.popover;
  protected readonly menuId = () => `${this.id()}-menu`;
  protected readonly triggerId = () => `${this.id()}-trigger`;

  private readonly root = viewChild.required<ElementRef<HTMLElement>>('root');
  private readonly trigger = viewChild.required<ElementRef<HTMLButtonElement>>('trigger');
  private readonly items = contentChildren(MenuItemDirective, { descendants: true });

  protected toggle(): void {
    if (this.open()) {
      this.dismiss(true);
    } else {
      this.show(0);
    }
  }

  protected handleTriggerKeydown(event: KeyboardEvent): void {
    if (event.key === 'ArrowDown') {
      event.preventDefault();
      this.show(0);
    } else if (event.key === 'ArrowUp') {
      event.preventDefault();
      this.show(-1);
    }
  }

  protected handleMenuKeydown(event: KeyboardEvent): void {
    if (event.key === 'Escape') {
      event.preventDefault();
      event.stopPropagation();
      this.dismiss(true);
      return;
    }

    if (event.key === 'Tab') {
      this.dismiss(false);
      return;
    }

    const available = this.availableItems();
    if (!available.length) return;

    const current = available.findIndex((item) => item === document.activeElement);
    let next: number | undefined;
    if (event.key === 'ArrowDown') next = (current + 1) % available.length;
    else if (event.key === 'ArrowUp') next = (current - 1 + available.length) % available.length;
    else if (event.key === 'Home') next = 0;
    else if (event.key === 'End') next = available.length - 1;

    if (next !== undefined) {
      event.preventDefault();
      available[next].focus();
    }
  }

  protected handleMenuClick(event: MouseEvent): void {
    const item = (event.target as Element).closest<HTMLElement>('[lsdMenuItem]');
    if (item && !item.matches(':disabled, [aria-disabled="true"]')) {
      this.dismiss(true);
    }
  }

  protected handleDocumentPointerDown(event: PointerEvent): void {
    if (this.open() && !this.root().nativeElement.contains(event.target as Node)) {
      this.dismiss(false);
    }
  }

  private show(focusIndex: 0 | -1): void {
    this.open.set(true);
    queueMicrotask(() => {
      const available = this.availableItems();
      available[focusIndex === -1 ? available.length - 1 : 0]?.focus();
    });
  }

  private dismiss(restoreFocus: boolean): void {
    if (!this.open()) return;
    this.open.set(false);
    if (restoreFocus && this.trigger().nativeElement.isConnected) {
      this.trigger().nativeElement.focus();
    }
  }

  private availableItems(): HTMLElement[] {
    return this.items()
      .filter((item) => item.isAvailable())
      .map((item) => item.elementRef.nativeElement);
  }
}
