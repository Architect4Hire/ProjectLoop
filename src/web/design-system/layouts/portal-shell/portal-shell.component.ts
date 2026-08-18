import { ChangeDetectionStrategy, Component, input } from '@angular/core';

import {
  AppNavigationComponent,
  type AppNavigationLink,
} from '../../components/app-navigation/app-navigation.component';
import { SkipLinkComponent } from '../../components/skip-link/skip-link.component';

@Component({
  selector: 'lsd-portal-shell',
  standalone: true,
  imports: [AppNavigationComponent, SkipLinkComponent],
  templateUrl: './portal-shell.component.html',
  styleUrl: './portal-shell.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PortalShellComponent {
  readonly navigationLinks = input.required<readonly AppNavigationLink[]>();
  readonly navigationLabel = input('Primary navigation');
  readonly navigationCompact = input(false);
  readonly mainLabel = input('Main content');
  readonly notificationsLabel = input('Notifications');
}
