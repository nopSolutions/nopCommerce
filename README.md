# ABCWarehouse

NOPCommerce codebase that runs both abcwarehouse.com and hawthorneonline.com

## Getting Started

1. Clone the repo with `git clone`.
2. Start the Development container for VSCode.
3. Access Azure and start downloading backup content (suggest using Azure Storage Explorer).
4. After backup is downloaded, locally (not in dev container), run the following commands to copy the database backup from your machine to the container:
```
docker exec nopcommerce_devcontainer-db-1 mkdir /var/opt/mssql/backup
docker cp NOPCommerce.bak nopcommerce_devcontainer-db-1:/var/opt/mssql/backup/NOPCommerce.bak
```
1. Run inside dev container (takes a minute):
```
sudo chmod u+x .devcontainer/restore-scripts/restore.sh
sudo .devcontainer/restore-scripts/restore.sh
```
1. Copy `App_Data/appSettings.json`, `App_Data/plugins.json` from Prod into
`Presentation/Nop.Web/App_Data`
1. Run application in debug mode with F5.
1. Uninstall any plugins not for use:
    1. Google Analytics 4
    2. Accessibe
    3. Listrak