# Project Loop Design System

This directory contains the Project Loop design system — a collection of reusable Angular components, patterns, layouts, and design tokens used across the portal.

## Structure

- **components/** — Reusable UI components (buttons, inputs, navigation, etc.)
- **patterns/** — Complex component compositions (approval UI, document patterns, etc.)
- **layouts/** — Page structure and shell components
- **primitives/** — Low-level form and content primitives
- **recipes/** — Application-specific component combinations
- **foundations/** — Design tokens, typography, and theming
- **documentation/** — Component guides and usage documentation
- **testing/** — Testing utilities and fixtures

## Public API

All public exports are declared in `public-api.ts`. Import design-system components via the `@lsd/design-system` path alias.

```typescript
import { PortalShellComponent } from '@lsd/design-system';
```

## Design Principles

- Accessibility-first: all components meet WCAG 2.1 AA standards
- Responsive: mobile-first design across all breakpoints
- Themeable: support light and dark modes
- Type-safe: full TypeScript support
- Standalone: use Angular standalone APIs
