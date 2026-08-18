import { Routes } from '@angular/router';
import { AppShellComponent } from './shell/app-shell.component';

export const APP_ROUTES: Routes = [
  {
    path: '',
    component: AppShellComponent,
    children: [
      {
        path: 'dashboard',
        loadComponent: () =>
          import('./routes/dashboard/dashboard.component').then(m => m.DashboardComponent),
      },
      {
        path: 'projects',
        loadComponent: () =>
          import('./routes/projects/projects.component').then(m => m.ProjectsComponent),
      },
      {
        path: 'documents',
        loadComponent: () =>
          import('./routes/documents/documents.component').then(m => m.DocumentsComponent),
      },
      {
        path: 'approvals',
        loadComponent: () =>
          import('./routes/approvals/approvals.component').then(m => m.ApprovalsComponent),
      },
      {
        path: 'account',
        loadComponent: () =>
          import('./routes/account/account.component').then(m => m.AccountComponent),
      },
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
    ],
  },
];
