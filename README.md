# ABCWarehouse

NOPCommerce codebase that runs both abcwarehouse.com and hawthorneonline.com

## Getting Started with Codespace

1. Create Codespace
2. Run `bash ./.devcontainer/init.sh`
3. Connect to DB within Codespace, run contents of `configureDb-local.sql`
4. After application startup, uninstall the following plugins unless required:
    1. HTML Widgets
    2. Product Ribbons
    3. CRON Tasks
    4. PowerReviews
    5. AJAX Filters

## Running E2E tests

To run Playwright E2E tests:

```
cd e2e
npx playwright test
```