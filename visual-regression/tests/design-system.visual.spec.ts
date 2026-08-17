import { expect, test } from '@playwright/test';

import { portalCompositionVisualMatrix } from '../fixtures/portal-compositions.visual-matrix';

const viewports = {
  desktop: { width: 1440, height: 1000 },
  mobile: { width: 390, height: 844 },
} as const;

for (const visualCase of portalCompositionVisualMatrix) {
  test(visualCase.name, async ({ page }) => {
    await page.setViewportSize(viewports[visualCase.viewport]);
    await page.emulateMedia({ colorScheme: visualCase.appearance, reducedMotion: 'reduce' });
    await page.goto(`/?composition=${visualCase.composition}&appearance=${visualCase.appearance}&state=${visualCase.state}`);
    await expect(page.locator('visual-root')).toBeVisible();
    await expect(page).toHaveScreenshot(`${visualCase.name}.png`, { fullPage: true });
  });
}
