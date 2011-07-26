Steps:
1. Backup your existing database
2. Install nopCommerce 2.00 to your existing database (1.90).
	NOTE: Install it without sample data!!!
3. Backup your existing database (again)
4. Execute upgrade.sql script over your database
5. Restart your application (for example, modify Web.config file)
6. Now manually reconfigure your store settings (Admin area > Configuration > Settings)

Notes:
1. Now there're several built-in customer roles. You can delete some ones you don't need them (e.g. 'Global Administrators' became 'Administrators')
