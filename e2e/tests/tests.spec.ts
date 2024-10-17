import { test, expect } from '@playwright/test';

// fixes Unacceptable TLS certificate issue
test.use({
  ignoreHTTPSErrors: true,
});

test('has ABC Warehouse title', async ({ page }) => {
  await page.goto('https://www.abcwarehouse.com');

  // Expect a title "to contain" a substring.
  await expect(page).toHaveTitle(/ABC Warehouse/);
});
