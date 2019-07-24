Steps:
1. Backup your existing database
2. Install nopCommerce 2.00 to your existing database (1.90).
	IMPORTANT NOTE: Install it without sample data!!!
3. Backup your existing database (again)
4. Execute upgrade.sql script over your database
5. Restart your application (for example, modify Web.config file)
6. If your pictures were stored on the file system, then manually copy them into \Content\Images (previously they were stored into \images directory)
7. Now manually complete the following steps: 
	7a. Configure Store settings (Admin area > Configuration > Settings)
	7b. Configure your currencies (Admin area > Configuration > Currencies). Ensure primary store currency and primary exchange currencies are the same
	7c. Configure your measure units (Admin area > Configuration > Measures)
	7d. Configure your email accounts (Admin area > Configuration > Email accounts)
	7e. Update your message templates (some tokens were changed)
	7f. Configure your payment, shipping, tax providers
	7g. Update topics (can't be automated)
	7h. Reconfigure prices by customer roles (Now we have new implementation of tier prices based on customer roles)
	7i. Reconfigure discount requirements for each discount (new implementation)
8. Ensure that everything is OK



Notes:
1. Now there're several built-in customer roles. You can delete some old ones if you don't need them (e.g. 'Global Administrators' became 'Administrators')
