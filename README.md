# ABCWarehouse

NOPCommerce codebase that runs both abcwarehouse.com and hawthorneonline.com

## Getting Started

1. Access Azure and start downloading backup content (suggest using Azure Storage Explorer).
2. Clone this repo with `git clone`.
3. Start the development container for VSCode.
4. After backup is downloaded, locally (not in dev container), run the following commands to copy the database backup from your machine to the container:
```
docker exec nopcommerce_devcontainer-db-1 mkdir /var/opt/mssql/backup
docker cp NOPCommerce.bak nopcommerce_devcontainer-db-1:/var/opt/mssql/backup/NOPCommerce.bak
```
5. Run inside dev container (takes a minute):
```
sudo chmod u+x .devcontainer/restore-scripts/restore.sh
sudo .devcontainer/restore-scripts/restore.sh
```
6. Copy `appSettings.json`, `dataSettings.json`, `plugins.json` (this could be moved into a copy step)
