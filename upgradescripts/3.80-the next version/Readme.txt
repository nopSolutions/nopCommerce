Steps:
1. Backup your existing database
2. Execute upgrade.sql script over your database
3. Remove all files from the previous version except App_Data\Settings.txt and App_Data\InstalledPlugins.txt
4. Upload new site files
5. Copy back App_Data\Settings.txt and App_Data\InstalledPlugins.txt files
6. Ensure that everything is OK

Notes:
1. If you stored your pictures on the file system, then also backup them (\Content\Images\) and copy back after upgrade