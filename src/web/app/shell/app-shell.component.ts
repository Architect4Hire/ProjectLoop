import { Component, computed } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { PortalShellComponent, type AppNavigationLink } from '@lsd/design-system';

@Component({
  selector: 'app-shell',
  standalone: true,
  imports: [PortalShellComponent, RouterOutlet],
  templateUrl: './app-shell.component.html',
  styleUrl: './app-shell.component.css',
})
export class AppShellComponent {
  navigationLinks = computed(() => {
    const links: readonly AppNavigationLink[] = [
      { label: 'Dashboard', href: '/dashboard', icon: 'menu' },
      { label: 'Projects', href: '/projects', icon: 'menu' },
      { label: 'Documents', href: '/documents', icon: 'menu' },
      { label: 'Approvals', href: '/approvals', icon: 'check' },
    ];
    return links;
  });
}
