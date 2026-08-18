import { APP_ROUTES } from './app.routes';
import { AppShellComponent } from './shell/app-shell.component';

describe('App Routes', () => {
  it('should have a root route with AppShellComponent', () => {
    const rootRoute = APP_ROUTES.find(route => route.path === '');
    expect(rootRoute).toBeDefined();
    expect(rootRoute?.component).toBe(AppShellComponent);
  });

  it('should have dashboard route', () => {
    const rootRoute = APP_ROUTES.find(route => route.path === '');
    const children = (rootRoute?.children || []) as any[];
    const dashboardRoute = children.find(route => route.path === 'dashboard');
    expect(dashboardRoute).toBeDefined();
  });

  it('should have projects route', () => {
    const rootRoute = APP_ROUTES.find(route => route.path === '');
    const children = (rootRoute?.children || []) as any[];
    const projectsRoute = children.find(route => route.path === 'projects');
    expect(projectsRoute).toBeDefined();
  });

  it('should have documents route', () => {
    const rootRoute = APP_ROUTES.find(route => route.path === '');
    const children = (rootRoute?.children || []) as any[];
    const documentsRoute = children.find(route => route.path === 'documents');
    expect(documentsRoute).toBeDefined();
  });

  it('should have approvals route', () => {
    const rootRoute = APP_ROUTES.find(route => route.path === '');
    const children = (rootRoute?.children || []) as any[];
    const approvalsRoute = children.find(route => route.path === 'approvals');
    expect(approvalsRoute).toBeDefined();
  });

  it('should have account route', () => {
    const rootRoute = APP_ROUTES.find(route => route.path === '');
    const children = (rootRoute?.children || []) as any[];
    const accountRoute = children.find(route => route.path === 'account');
    expect(accountRoute).toBeDefined();
  });

  it('should redirect empty child path to dashboard', () => {
    const rootRoute = APP_ROUTES.find(route => route.path === '');
    const children = (rootRoute?.children || []) as any[];
    const redirectRoute = children.find(route => route.path === '');
    expect(redirectRoute?.redirectTo).toBe('dashboard');
  });
});
