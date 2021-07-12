Steps:
1. Make a backup of everything on your site, including the database. This is extremely important so that you can roll back to a running site no matter what happens during migration.
2. Execute upgrade.sql script over your database
3. Remove all files from the previous version except \App_Data\dataSettings.json (Settings.txt) and \App_Data\plugins.json. Also \App_Data\installedPlugins.json or \App_Data\InstalledPlugins.txt (if exists)
4. Upload new site files
5. Copy back \App_Data\dataSettings.json and \App_Data\plugins.json files. And \App_Data\installedPlugins.json (if exists)
6. Ensure that everything is OK

Notes:
1. If you stored your pictures on the file system, then also backup them (\wwwroot\Images\) and copy back after upgrade