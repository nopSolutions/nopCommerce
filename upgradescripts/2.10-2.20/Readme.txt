Steps:
1. Backup your existing database
2. Execute upgrade.sql script over your database
2. Execute upgrade.additional.sql script over your database if it supports stored procedures and functions. Note that SQL Compact doesn't support them.
3. Remove all files from the previous version except App_Data\Settings.txt and App_Data\InstalledPlugins.txt
4. Upload new site files
5. Copy back App_Data\Settings.txt and App_Data\InstalledPlugins.txt files
6. Ensure that everything is OK