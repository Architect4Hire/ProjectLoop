import AxeBuilder from '@axe-core/playwright';
import { expect, test, type Page } from '@playwright/test';

const widths = {
  desktop: { width: 1440, height: 1000 },
  mobile: { width: 390, height: 844 },
} as const;

async function openFixture(page: Page, composition: string, state = 'default', appearance = 'light'): Promise<void> {
  await page.goto(`/?composition=${composition}&appearance=${appearance}&state=${state}`);
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

    test('feedback states expose visible status, priority, progress, intent, and reduced-motion contracts', async ({ page }) => {
      await page.emulateMedia({ reducedMotion: 'reduce' });
      await openFixture(page, 'feedback');

      await expect(page.getByRole('status', { name: 'Loading authorized records' })).toHaveAttribute('aria-busy', 'true');
      await expect(page.getByRole('alert', { name: 'Records could not be refreshed' })).toHaveAttribute('aria-live', 'polite');
      await expect(page.getByRole('alert', { name: 'Records are unavailable' })).toHaveAttribute('aria-live', 'assertive');
      await expect(page.getByRole('progressbar', { name: 'Preparing evidence package' })).toHaveAttribute('aria-valuetext', '64 percent complete');
      await expect(page.getByText('Download unavailable.')).toBeVisible();
      const retry = page.getByRole('button', { name: 'Retry refresh' });
      await expect(retry).toBeEnabled();
      await expect(page.getByRole('button', { name: 'Cancel generation' })).toBeDisabled();
      const spinner = page.locator('.lsd-state-feedback__spinner');
      expect(await spinner.evaluate((element) => getComputedStyle(element).animationName)).toBe('none');
      const aiFill = page.locator('.lsd-ai-generation-progress__fill');
      expect(await aiFill.evaluate((element) => getComputedStyle(element).animationName)).toBe('none');
      await expectNoAxeViolations(page);
    });

    for (const appearance of ['light', 'dark'] as const) {
      test(`fields preserve keyboard, state, associations, and ${appearance} contrast`, async ({ page }) => {
        await openFixture(page, 'fields', 'states', appearance);

        const editable = page.getByRole('textbox', { name: /Project name/ });
        await editable.fill('Modernization program');
        await expect(editable).toHaveValue('Modernization program');

        const invalid = page.getByRole('textbox', { name: 'Contact email' });
        await expect(invalid).toHaveAttribute('aria-describedby', 'field-invalid-description field-invalid-error');
        await expect(invalid).toHaveAttribute('aria-errormessage', 'field-invalid-error');
        await expect(page.locator('#field-invalid-error')).toHaveAttribute('aria-live', 'assertive');

        const readonly = page.getByRole('textbox', { name: 'Engagement identifier' });
        await readonly.focus();
        await expect(readonly).toBeFocused();
        await expect(readonly).toHaveAttribute('readonly', '');
        await expect(page.getByRole('textbox', { name: 'Archived reference' })).toBeDisabled();
        await expectNoAxeViolations(page);
      });
    }

    test('native choice controls preserve keyboard, grouping, disabled states, focus, and touch targets', async ({ page }) => {
      await openFixture(page, 'choices', 'states');

      const select = page.getByRole('combobox', { name: /Reviewer capacity/ });
      await select.focus();
      await expect(select).toBeFocused();
      await page.keyboard.press('ArrowDown');
      await expect(select).toHaveValue('0');
      await expect(select.locator('option').nth(3)).toBeDisabled();

      const checkbox = page.getByRole('checkbox', { name: /Include supporting evidence/ });
      await checkbox.focus();
      await page.keyboard.press('Space');
      await expect(checkbox).toBeChecked();
      await expect(checkbox).not.toHaveAttribute('aria-checked', 'mixed');

      const group = page.getByRole('group', { name: /Review depth/ });
      await expect(group).toHaveAttribute('aria-describedby', 'choice-radio-description choice-radio-error');
      const focusedRadio = group.getByRole('radio', { name: 'Focused review' });
      await focusedRadio.focus();
      await page.keyboard.press('ArrowDown');
      await expect(group.getByRole('radio', { name: 'Standard review' })).toBeChecked();
      await expect(group.getByRole('radio', { name: 'Comprehensive review' })).toBeDisabled();

      await expect(page.getByRole('combobox', { name: 'Unavailable selection' })).toBeDisabled();
      await expect(page.getByRole('checkbox', { name: 'Unavailable confirmation' })).toBeDisabled();
      const disabledGroup = page.getByRole('group', { name: 'Unavailable review depth' });
      await expect(disabledGroup).toHaveAttribute('disabled', '');
      for (const radio of await disabledGroup.getByRole('radio').all()) {
        await expect(radio).toBeDisabled();
      }

      const selectTarget = await select.boundingBox();
      expect(selectTarget?.height ?? 0).toBeGreaterThanOrEqual(44);
      const checkboxTarget = await checkbox.locator('xpath=ancestor::label').boundingBox();
      expect(checkboxTarget?.height ?? 0).toBeGreaterThanOrEqual(44);
      const radioTarget = await focusedRadio.locator('xpath=ancestor::label').boundingBox();
      expect(radioTarget?.height ?? 0).toBeGreaterThanOrEqual(44);
      await expectNoAxeViolations(page);
    });

    test('overlay and disclosure controls preserve native keyboard and focus contracts', async ({ page }) => {
      await page.emulateMedia({ reducedMotion: 'reduce' });
      await openFixture(page, 'overlays', 'states');

      const tooltipTrigger = page.getByRole('button', { name: 'Tooltip trigger' });
      await tooltipTrigger.focus();
      await expect(page.getByRole('tooltip')).toBeVisible();
      await page.keyboard.press('Escape');
      await expect(page.getByRole('tooltip')).toBeHidden();
      await expect(tooltipTrigger).toBeFocused();

      const menuTrigger = page.getByRole('button', { name: 'Fixture actions' });
      await menuTrigger.press('ArrowDown');
      await expect(menuTrigger).toHaveAttribute('aria-expanded', 'true');
      await expect(page.getByRole('menuitem', { name: 'First action' })).toBeFocused();
      await page.keyboard.press('ArrowDown');
      await expect(page.getByRole('menuitem', { name: 'Last action' })).toBeFocused();
      await page.keyboard.press('Escape');
      await expect(menuTrigger).toBeFocused();

      const overviewTab = page.getByRole('tab', { name: 'Overview' });
      await overviewTab.focus();
      await page.keyboard.press('ArrowRight');
      await expect(page.getByRole('tab', { name: 'History' })).toBeFocused();
      await expect(page.getByRole('tab', { name: 'Unavailable' })).toBeDisabled();

      const dialogOpener = page.getByRole('button', { name: 'Open dialog' });
      await dialogOpener.click();
      const dialog = page.getByRole('dialog', { name: 'Review changes' });
      await expect(dialog).toBeVisible();
      await expect(page.getByRole('button', { name: 'Review details' })).toBeFocused();
      await page.keyboard.press('Shift+Tab');
      expect(await dialog.evaluate((element) => element.contains(document.activeElement))).toBe(true);
      await page.keyboard.press('Escape');
      await expect(dialog).toBeHidden();
      await expect(dialogOpener).toBeFocused();

      const drawerOpener = page.getByRole('button', { name: 'Open drawer' });
      await drawerOpener.click();
      const drawer = page.getByRole('dialog', { name: 'Source preview' });
      await expect(page.getByRole('button', { name: 'Open source' })).toBeFocused();
      await page.keyboard.press('Shift+Tab');
      expect(await drawer.evaluate((element) => element.contains(document.activeElement))).toBe(true);
      expect(await drawer.evaluate((element) => getComputedStyle(element).transitionDuration)).toBe('0s');
      const drawerBox = await drawer.boundingBox();
      expect(drawerBox?.width ?? Number.POSITIVE_INFINITY).toBeLessThanOrEqual(viewport.width);
      await page.keyboard.press('Escape');
      await expect(drawerOpener).toBeFocused();

      const paletteOpener = page.getByRole('button', { name: 'Open command palette' });
      await paletteOpener.click();
      const search = page.getByRole('combobox', { name: 'Search commands' });
      await expect(search).toBeFocused();
      await page.keyboard.press('ArrowDown');
      await expect(search).toHaveAttribute('aria-activedescendant', 'fixture-palette-command-create');
      await expect(page.getByRole('option', { name: 'Unavailable command' })).toBeDisabled();
      await page.keyboard.press('Escape');
      await expect(paletteOpener).toBeFocused();
      await expectNoAxeViolations(page);
    });
  });
}
