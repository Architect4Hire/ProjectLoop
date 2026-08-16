import { Directive, ElementRef, inject } from '@angular/core';

@Directive({
  selector: 'button[lsdMenuItem], a[lsdMenuItem]',
  standalone: true,
  host: {
    class: 'lsd-menu-item flex min-h-10 w-full items-center gap-2 rounded-md px-3 py-2 text-start text-sm text-text-primary hover:bg-surface-panel',
    role: 'menuitem',
    tabindex: '-1',
  },
})
export class MenuItemDirective {
  readonly elementRef = inject<ElementRef<HTMLElement>>(ElementRef);

  isAvailable(): boolean {
    return !this.elementRef.nativeElement.matches(':disabled, [aria-disabled="true"]');
  }
}
