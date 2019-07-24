Steps:
1. Backup your existing database
2. Execute upgrade.sql script over your database
3. Remove all files from the previous version except App_Data\dataSettings.json (Settings.txt) and App_Data\installedPlugins.json (InstalledPlugins.txt)
4. Upload new site files
5. Copy back App_Data\dataSettings.json and App_Data\installedPlugins.json files
6. Ensure that everything is OK

Version specific steps:
7. Added "Shipping by Total" functionality to "Shipping by Weight" plugin. Upgrade instructions: do not forget to manually delete "/Plugins/Shipping.FixedOrByWeight" directory (it's not used anymore) and replace the plugin «Shipping.FixedOrByWeight» by the plugin «Shipping.FixedByWeightByTotal» in the App_Data/installedPlugins.json file.


Notes:
1. If you stored your pictures on the file system, then also backup them (\wwwroot\Images\) and copy back after upgrade