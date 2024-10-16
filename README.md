# ABCWarehouse

NOPCommerce codebase that runs both abcwarehouse.com and hawthorneonline.com

## Getting Started with Codespace

1. Create Codespace
2. Connect to DB within Codespace, run contents of `configureDb-local.sql`

### Post-Restore App Configuration

1. After application startup, uninstall the following plugins unless required:
    1. HTML Widgets
    2. Product Ribbons
    3. CRON Tasks
    4. PowerReviewsUpDate
    5. AJAX Filters
2. Reinstall AJAX Filters

## Running E2E tests

To run Playwright E2E tests:

```
cd e2e
npx playwright test
```