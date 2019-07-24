Steps:
1. Disable full-text in admin area
2. Backup your existing database
3. Execute upgrade.sql script over your database
4. Remove all files from the previous version except App_Data\Settings.txt and App_Data\InstalledPlugins.txt
5. Upload new site files
6. Copy back App_Data\Settings.txt and App_Data\InstalledPlugins.txt files
7. Enable full-text in admin area
8. Ensure that everything is OK

9. Now manually complete the following steps: 
	9a. Update "Required product IDs" product property (visible when "Require other products are added to the cart" enable)
	9b. Re-configure "Customer has one of these products in the cart" and "Customer has all of these products in the cart" discount requirements if were used
	9c. Re-configure "Google product search" (froogle) plugin

Notes:
1. If you stored your pictures on the file system, then also backup them (\Content\Images\) and copy back after upgrade

