import { ChangeDetectionStrategy, Component, input } from '@angular/core';

import { IconComponent } from '../../icons/icon.component';
import type { IconName } from '../../icons/internal/icon-paths';
import { BadgeComponent } from '../../primitives/badge/badge.component';
import { LinkDirective } from '../../primitives/link/link.directive';

export interface AppNavigationLink {
  readonly label: string;
  readonly href: string;
  readonly icon: IconName;
  readonly active?: boolean;
  readonly count?: number;
}

@Component({
  selector: 'lsd-app-navigation',
  standalone: true,
  imports: [BadgeComponent, IconComponent, LinkDirective],
  templateUrl: './app-navigation.component.html',
  styleUrl: './app-navigation.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AppNavigationComponent {
  readonly links = input.required<readonly AppNavigationLink[]>();
  readonly accessibleName = input('Application');
  readonly compact = input(false);
}
