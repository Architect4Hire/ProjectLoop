import { ChangeDetectionStrategy, Component, input } from '@angular/core';

import { IconComponent } from '../../icons/icon.component';
import { LinkDirective } from '../../primitives/link/link.directive';

export interface BreadcrumbItem {
  readonly label: string;
  readonly href: string;
}

@Component({
  selector: 'lsd-breadcrumb',
  standalone: true,
  imports: [IconComponent, LinkDirective],
  templateUrl: './breadcrumb.component.html',
  styleUrl: './breadcrumb.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class BreadcrumbComponent {
  readonly items = input.required<readonly BreadcrumbItem[]>();
  readonly accessibleName = input('Breadcrumb');
}
