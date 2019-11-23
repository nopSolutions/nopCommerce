Steps:
1. Backup your existing database
2. Execute upgrade.sql script over your database
3. Remove all files from the previous version except App_Data\Settings.txt and App_Data\InstalledPlugins.txt
4. Upload new site files
5. Copy back App_Data\Settings.txt and App_Data\InstalledPlugins.txt files
6. Ensure that everything is OK


Version specific steps:
7. Combine "Fixed Rate Shipping" and "Shipping by weight" plugins into single one. Upgrade instructions: do not forget to manually delete "/Plugins/Shipping.FixedRateShipping" and "/Plugins/Shipping.ByWeight" directories (they are not used anymore) and replace the plugin «Shipping.Fixed Rate» or «Shipping.ByWeight» by the plugin «Shipping.FixedOrByWeight» in the App_Data/InstalledPlugins.txt file.
8. Combine "Nop.Plugin.Tax.CountryStateZip" and "Nop.Plugin.Tax.FixedRate" plugins into single one. Upgrade instructions: do not forget to manually delete "/Plugins/Tax.FixedRate" and "/Plugins/Tax.CountryStateZip" directories (they are not used anymore) and replace the plugin «Tax.Fixed» or «Tax.CountryStateZip» by the plugin «Tax.FixedOrByCountryStateZip» in the App_Data/InstalledPlugins.txt file.
9. Renamed "Froogle" plugin to "Google Shopping". Upgrade instructions: manually delete /Plugins/Feed.Froogle/ directory


Notes:
1. If you stored your pictures on the file system, then also backup them (\Content\Images\) and copy back after upgrade


