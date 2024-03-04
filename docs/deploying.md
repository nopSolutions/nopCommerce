# Deploying to Stage or Prod

**NOTE: A deployment will take the site down for about 30 seconds, then reduce
performance for around 2-5 minutes.**

1. RDP into the appropriate server. A list of IPs:
* ABC Prod - 163.123.137.18
* ABC Stage - 163.123.137.99
* Haw Prod - 163.123.137.41
* Mickey Prod - 163.123.137.44

2. Open a Powershell command prompt as Administrator.
3. Navigate to C:\Users\xby2\nopCommerce:
![First step](docs/deploy_1.png?raw=true)

4. Run command `git pull` to pull the latest changes from `main`.
5. Run command `.\deploy.ps1` to build and deploy changes:
![Second step](docs/deploy_2.png?raw=true)

6. (Mickey Shorr only) - revert the changes made to `web.config` to allow for redirects to work:
![Final step](docs/deploy_3.png?raw=true)
