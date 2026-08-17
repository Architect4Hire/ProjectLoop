import { defineConfig } from '@playwright/test';

export default defineConfig({
  testDir: './visual-regression/tests',
  outputDir: './visual-regression/test-results',
  fullyParallel: true,
  retries: 0,
  reporter: [['list'], ['html', { outputFolder: 'visual-regression/report', open: 'never' }]],
  expect: {
    toHaveScreenshot: {
      animations: 'disabled',
      caret: 'hide',
      scale: 'css',
    },
  },
  use: {
    baseURL: 'http://127.0.0.1:4207',
    browserName: 'chromium',
    colorScheme: 'light',
    locale: 'en-US',
    timezoneId: 'UTC',
  },
  webServer: {
    command: 'node visual-regression/serve-fixture.mjs',
    url: 'http://127.0.0.1:4207',
    reuseExistingServer: !process.env['CI'],
    timeout: 120_000,
  },
});
