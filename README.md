# ABCWarehouse

NOPCommerce codebase that runs both abcwarehouse.com and hawthorneonline.com

## Getting Started with Codespace

1. Create Codespace
2. Initialize setup
    
    ```bash
    . ./.devcontainer/initialize.sh
    ```
    
3. Connect to DB within Codespace, run contents of `configureDb-local.sql`

### Post-Restore App Configuration

1. After application startup, uninstall the following plugins unless required:
    1. HTML Widgets
    2. Product Ribbons
    3. CRON Tasks
    4. PowerReviewsUpDate
    5. AJAX Filters
2. Reinstall AJAX Filters