import AxeBuilder from '@axe-core/playwright';
import { expect, test, type Page } from '@playwright/test';

const widths = {
  desktop: { width: 1440, height: 1000 },
  mobile: { width: 390, height: 844 },
} as const;

async function openFixture(page: Page, composition: string, state = 'default'): Promise<void> {
  await page.goto(`/?composition=${composition}&appearance=light&state=${state}`);
  await expect(page.locator('visual-root')).toBeVisible();
}

async function expectNoAxeViolations(page: Page): Promise<void> {
  const results = await new AxeBuilder({ page }).analyze();
  expect(results.violations, results.violations.map(({ id, help, nodes }) =>
    `${id}: ${help}\n${nodes.map((node) => `  ${node.target.join(' ')}: ${node.failureSummary}`).join('\n')}`,
  ).join('\n\n')).toEqual([]);
}

for (const [viewportName, viewport] of Object.entries(widths)) {
  test.describe(`${viewportName} accessibility`, () => {
    test.use({ viewport });

    test('shell navigation exposes landmarks and keyboard skip navigation', async ({ page }) => {
      await openFixture(page, 'shell', viewportName === 'mobile' ? 'compact' : 'long');

      await expect(page.getByRole('banner')).toBeVisible();
      await expect(page.getByRole('navigation', { name: 'Primary navigation' })).toBeVisible();
      await expect(page.getByRole('main', { name: 'Main content' })).toBeVisible();

      await page.keyboard.press('Tab');
      const skipLink = page.getByRole('link', { name: 'Skip to main content' });
      await expect(skipLink).toBeFocused();
      await page.keyboard.press('Enter');
      await expect(page.getByRole('main', { name: 'Main content' })).toBeFocused();
      await expectNoAxeViolations(page);
    });

    test('document list preserves named collection and row/card semantics', async ({ page }) => {
      await openFixture(page, 'documents', 'long');

      await expect(page.getByRole('region', { name: 'Documents' })).toBeVisible();
      await expect(page.getByText('Architecture decision record for cross-region resilience and operational continuity').filter({ visible: true })).toBeVisible();
      await expect(page.getByText('v4.12.0').filter({ visible: true })).toBeVisible();
      await expectNoAxeViolations(page);
    });

    test('upload supports keyboard validation, accessible names, and assertive errors', async ({ page }) => {
      await openFixture(page, 'upload');

      await page.keyboard.press('Tab');
      await expect(page.getByRole('button', { name: 'Browse files' })).toBeFocused();
      await page.keyboard.press('Tab');
      await expect(page.getByRole('textbox', { name: 'Document title' })).toBeFocused();
      await page.keyboard.press('Tab');
      await expect(page.getByRole('combobox', { name: 'Category' })).toBeFocused();
      await page.keyboard.press('Tab');
      await expect(page.getByRole('combobox', { name: 'Visibility' })).toBeFocused();
      await page.keyboard.press('Tab');
      await expect(page.getByRole('button', { name: 'Upload document' })).toBeFocused();
      await page.keyboard.press('Enter');

      await expect(page.getByText('Enter a document title.')).toBeVisible();
      await expect(page.locator('[role="alert"][aria-live="assertive"]')).toContainText('Select a file that meets the upload policy.');
      await expectNoAxeViolations(page);
    });

    test('version-bound approval keeps exact names and keyboard action focus', async ({ page }) => {
      await openFixture(page, 'approval', 'long');

      await expect(page.getByRole('region', { name: 'Review Cross-region continuity plan v3' })).toBeVisible();
      await expect(page.getByText('v3').first()).toBeVisible();
      await expect(page.getByText('v4').first()).toBeVisible();
      const approve = page.getByRole('button', { name: 'Approve' });
      await approve.focus();
      await page.keyboard.press('Enter');
      await expect(approve).toBeFocused();
      await expectNoAxeViolations(page);
    });

    test('audit timeline has a named region and restores focus when details collapse', async ({ page }) => {
      await openFixture(page, 'audit', 'long');

      await expect(page.getByRole('region', { name: 'Audit timeline' }).first()).toBeVisible();
      const summary = page.getByText('Show audit event').first();
      await summary.focus();
      await page.keyboard.press('Enter');
      await expect(summary.locator('xpath=..')).toHaveAttribute('open', '');
      await page.keyboard.press('Enter');
      await expect(summary.locator('xpath=..')).not.toHaveAttribute('open', '');
      await expect(summary).toBeFocused();
      await expect(page.getByRole('button', { name: 'Load more' })).toHaveAccessibleName('Load more');
      await expectNoAxeViolations(page);
    });
  });
}
