Steps:
1. Backup your existing database
2. Install nopCommerce 2.00 to your existing database (1.90).
	NOTE: Install it without sample data!!!
3. Backup your existing database (again)
4. Execute upgrade.sql script over your database
5. Restart your application (for example, modify Web.config file)
7. If your pictures were stored on the file system, then manually copy them into \Content\Images (previously they were stored into \images directory)
6. Now manually reconfigure your: 
	6a. Store settings (Admin area > Configuration > Settings)
	6b. Configure your currencies (Admin area > Configuration > Currencies)
	6c. Configure your measure units (Admin area > Configuration > Measures)
	6d. Configure your email accounts (Admin area > Configuration > Email accounts)
	6e. Update your message templates (some tokens were changed)
	6f. Configure your payment, shipping, tax providers

Notes:
1. Now there're several built-in customer roles. You can delete some old ones if you don't need them (e.g. 'Global Administrators' became 'Administrators')
