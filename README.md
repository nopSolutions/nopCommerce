# ABCWarehouse

NOPCommerce codebase that runs both abcwarehouse.com and hawthorneonline.com

## Getting Started

1. Create Codespace.
2. Download Dropbox CLI: `wget https://github.com/dropbox/dbxcli/releases/download/v3.0.0/dbxcli-linux-amd64`
3. `sudo chmod u+x dbxcli-linux-amd64`
4. Download BACPAC: `./dbxcli-linux-amd64 get NOP.bacpac`
5. Delete Dropbox CLI: `rm dbxcli-linux-amd64`
6. Import BACPAC: `/opt/sqlpackage/sqlpackage /Action:Import /SourceFile:"/workspace/NOP.bacpac" /TargetConnectionString:"Server=tcp:localhost,1433;Initial Catalog=NOPCommerce;User ID=sa;Password=P@ssw0rd;TrustServerCertificate=True;"`
7. Delete BACPAC: `rm NOP.bacpac`
8. Connect to DB within Codespace and run contents of `configureDb.sql`
9. Copy `appSettings.json`, `plugins.json`, `dataSettings.json` into Nop.Web/App_Data
10. After starting up, uninstall the following plugins unless required:
    1. HTML Widgets
    1. Product Ribbons
    1. CRON Tasks
    1. PowerReviews

## Installing Apache Solr

To set up Apache Solr for the seaarch plugin, run the following commands:

```
wget https://www.apache.org/dyn/closer.lua/solr/solr/9.1.1/solr-9.1.1.tgz?action=download
mv solr-9.1.1.tgz\?action\=download solr-9.1.1.tgz
tar -xzf solr-9.1.1.tgz solr-9.1.1/bin/install_solr_service.sh --strip-components=2
sudo bash ./install_solr_service.sh solr-9.1.1.tgz
rm -rf solr-9.1.1.tgz install_solr_service.sh
```

## Handling Fatal Error

To solve the fatal error that occurs on Codespaces from time to time, stop and start
the codespace.

## Installing Let's Encrypt Certificate

Before doing this, you'll need to:
* Install `certbot` and `openssl` on server.
* Open port 80.
* nopCommerce Let's Encrypt Plugin is enabled.

1. RDP into server, run:
```
# create cert
certbot certonly --webroot -w C:/NopAbc/wwwroot -d <DOMAIN>

# convert cert to pfx
2. `& 'C:/Program Files/OpenSSL-Win64/bin/openssl.exe' pkcs12 -macalg SHA1 -keypbe PBE-SHA1-3DES -certpbe PBE-SHA1-3DES -export -out cert.pfx -inkey C:/Certbot/live/DOMAIN/privkey.pem -in C:/Certbot/live/DOMAIN/cert.pem`
```
3. Import cert into IIS.
4. Update HTTPS binding.
5. Restart App Pool.
6. Run SSL Test.
7. Close firewall if needed.
