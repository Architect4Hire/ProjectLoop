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
      await expect(page.getByRole('navigation', { name: 'Breadcrumb' })).toBeVisible();
      await expect(page.getByRole('heading', { name: 'Delivery confidence and evidence', level: 1 })).toBeVisible();
      await expect(page.getByRole('button', { name: 'Account menu for Alexandria Catherine Montgomery-Worthington' })).toBeVisible();
      await expect(page.getByRole('link', { name: /Engagement overview with delivery confidence and evidence/ })).toHaveAttribute('aria-current', 'page');
      await expect(page.getByLabel('Documents: 12')).toBeVisible();
      await expect(page.locator('.lsd-app-navigation')).toHaveAttribute('data-compact', viewportName === 'mobile' ? 'true' : 'false');

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

    test('approval states preserve exact versions, confirmation lockout, history, and focus restoration', async ({ page }) => {
      for (const state of ['pending', 'processing', 'approved', 'rejected', 'changes-requested'] as const) {
        await openFixture(page, 'approval', state);
        const review = page.getByRole('region', { name: 'Review Cross-region continuity plan v3' });
        await expect(review).toContainText('v3');
        await expect(review).toContainText('v4');
        await expect(review).not.toContainText('v4 · Approved');
        await expect(page.getByRole('region', { name: 'Approval history' })).toContainText('Approved Document Cross-region continuity plan v3');
        if (state === 'pending' || state === 'processing') {
          const opener = page.getByRole('button', { name: 'Open approval confirmation' });
          await opener.focus();
          await page.keyboard.press('Enter');
          const dialog = page.getByRole('dialog', { name: 'Approve Cross-region continuity plan v3' });
          if (state === 'processing') {
            await expect(page.getByRole('button', { name: 'Approve v3' })).toBeDisabled();
            await page.keyboard.press('Escape');
            await expect(dialog).toBeVisible();
          } else {
            await page.keyboard.press('Escape');
            await expect(dialog).toBeHidden();
          }
        }
        await expectNoAxeViolations(page);
      }
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

    test('audit preserves caller order, long identifiers, missing actors, and pagination', async ({ page }) => {
      await openFixture(page, 'audit', 'pages');
      const events = page.locator('.lsd-activity-stream__item');
      await expect(events).toHaveCount(2);
      await expect(events.nth(0)).toContainText('Morgan Chen');
      await expect(events.nth(1)).toContainText('Actor unavailable');
      await events.nth(1).getByText('Show audit event').click();
      await expect(events.nth(1)).toContainText('corr_01J5B40Y1SM8FQZK9GAD6W03XE_really_long_deterministic_identifier');
      await expect(page.getByRole('navigation', { name: 'Audit timeline pages' })).toContainText('Page 2 of 5');
      await expectNoAxeViolations(page);
    });

    test('document collection composes filters, empty/one/many feedback, pagination, and responsive presentations', async ({ page }) => {
      for (const state of ['empty', 'one', 'many', 'loading', 'error'] as const) {
        await openFixture(page, 'documents', state);
        await expect(page.getByRole('region', { name: 'Document filters' })).toBeVisible();
        if (state === 'empty') {
          await expect(page.getByRole('status', { name: 'No documents' })).toBeVisible();
        } else if (state === 'loading') {
          await expect(page.getByRole('status', { name: 'Loading authorized documents' })).toHaveAttribute('aria-busy', 'true');
        } else if (state === 'error') {
          await expect(page.getByRole('alert', { name: 'Documents could not be loaded' })).toHaveAttribute('aria-live', 'polite');
          await expect(page.getByRole('button', { name: 'Retry documents' })).toBeEnabled();
        } else {
          const expected = state === 'one' ? 1 : 2;
          await expect(page.locator('[data-presentation="rows"] tbody tr')).toHaveCount(expected);
          await expect(page.locator('[data-presentation="cards"] > li')).toHaveCount(expected);
          await expect(page.getByRole('navigation', { name: 'Document pages' })).toContainText(state === 'many' ? 'Page 2 of 4' : 'Page 1 of 1');
          const rowsVisible = await page.locator('[data-presentation="rows"]').isVisible();
          const cardsVisible = await page.locator('[data-presentation="cards"]').isVisible();
          expect(rowsVisible).toBe(viewportName === 'desktop');
          expect(cardsVisible).toBe(viewportName === 'mobile');
        }
        await expectNoAxeViolations(page);
      }
    });

    test('document detail preserves v3 approval, v4 current identity, and caller-owned transfer states', async ({ page }) => {
      for (const state of ['ready', 'download-preparing', 'download-downloading', 'download-failed', 'download-unavailable', 'uploading', 'upload-failed', 'upload-completed'] as const) {
        await openFixture(page, 'document-detail', state);
        const qualifierText = await page.locator('.fixture-version-pair lsd-version-chip').allTextContents();
        expect(qualifierText.join(' ')).toContain('v3');
        expect(qualifierText.join(' ')).toContain('Approved');
        expect(qualifierText.join(' ')).toContain('v4');
        expect(qualifierText.join(' ')).toContain('Current');
        expect(qualifierText.join(' ')).not.toContain('v4 Approved');
        await expect(page.getByRole('region', { name: 'Continuity plan version history' })).toContainText('v3');
        await expect(page.getByRole('region', { name: 'Continuity plan version history' })).toContainText('v4');
        expect(await page.locator('a[href^="blob:"]').count()).toBe(0);
        if (state === 'download-downloading') await expect(page.getByRole('progressbar', { name: 'Downloading Continuity plan v3 approved' })).toHaveAttribute('aria-valuetext', '55%');
        if (state === 'download-failed') await expect(page.getByRole('button', { name: 'Retry download of Continuity plan v3 approved' })).toBeEnabled();
        if (state === 'uploading') await expect(page.getByRole('button', { name: 'Cancel upload' })).toBeEnabled();
        if (state === 'upload-failed') await expect(page.getByRole('button', { name: 'Retry upload' })).toBeEnabled();
        if (state === 'upload-completed') await expect(page.locator('#detail-upload-completed')).toContainText('approval remains bound to v3');
        await expectNoAxeViolations(page);
      }
    });

    test('dashboard composes deterministic empty, loading, error, partial, and populated regions', async ({ page }) => {
      for (const state of ['empty', 'loading', 'error', 'partial', 'populated'] as const) {
        await openFixture(page, 'dashboard', state);
        for (const heading of ['Project health', 'Summary metrics', 'Milestones', 'Upcoming meetings', 'Recent decisions', 'Pending approvals']) {
          await expect(page.getByRole('heading', { name: heading, level: 2, exact: true })).toBeVisible();
        }

        if (state === 'empty') {
          await expect(page.getByText('No milestones to show.')).toBeVisible();
          await expect(page.getByText('No upcoming meetings.')).toBeVisible();
          await expect(page.getByText('No pending approval requests.')).toBeVisible();
        } else if (state === 'loading') {
          await expect(page.getByRole('status', { name: 'Loading project health' })).toHaveAttribute('aria-busy', 'true');
          expect(await page.locator('[aria-busy="true"]').count()).toBeGreaterThanOrEqual(6);
        } else if (state === 'error') {
          await expect(page.getByRole('alert', { name: 'Project health unavailable' })).toHaveAttribute('aria-live', 'assertive');
          await expect(page.getByRole('button', { name: 'Retry milestones' })).toBeEnabled();
        } else if (state === 'partial') {
          await expect(page.getByText('Operational readiness review with stakeholders')).toBeVisible();
          await expect(page.getByRole('alert', { name: 'Meetings unavailable' })).toHaveAttribute('aria-live', 'polite');
          await expect(page.getByText('No pending approval requests.')).toBeVisible();
        } else {
          await expect(page.getByRole('region', { name: 'Project health detail' })).toContainText('Healthy');
          await expect(page.getByRole('link', { name: 'Review Continuity plan v4' })).toBeVisible();
        }

        const dashboard = page.locator('lsd-project-dashboard');
        const box = await dashboard.boundingBox();
        expect(box?.width ?? Number.POSITIVE_INFINITY).toBeLessThanOrEqual(viewport.width);
        await expectNoAxeViolations(page);
      }
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
