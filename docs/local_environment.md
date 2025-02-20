# Creating a local environment

## Github Codespace

1. Create Codespace.
2. Download Dropbox CLI: wget https://github.com/dropbox/dbxcli/releases/download/v3.0.0/dbxcli-linux-amd64
3. sudo chmod u+x dbxcli-linux-amd64
4. Download BACPAC: ./dbxcli-linux-amd64 get NOP.bacpac
5. Delete Dropbox CLI: rm dbxcli-linux-amd64
6. Import BACPAC: /opt/sqlpackage/sqlpackage /Action:Import /SourceFile:"/workspace/NOP.bacpac" /TargetConnectionString:"Server=tcp:localhost,1433;Initial Catalog=NOPCommerce;User ID=sa;Password=P@ssw0rd;TrustServerCertificate=True;"
7. Delete BACPAC: rm NOP.bacpac
8. Connect to DB within Codespace and run contents of configureDb.sql
9. Copy appSettings.json, plugins.json, dataSettings.json into Nop.Web/App_Data
10. After starting up, uninstall the following plugins unless required:
  1. HTML Widgets
  2. Product Ribbons
  3. CRON Tasks
  4. PowerReviews

## Windows

### Pre-reqs:

1. Download and install .NET 5.0 Runtime.
2. Create a database to be used (two options):
    1. Inside network, copy the production database to another database on the server and link to it.
    2. Outside network, create a copy of the database and host it somewhere where it can be downloaded/restored locally.
3. Copy the `plugins.json` file from the desired environment (located in the App_Data folder)

### Process

1. git clone the codebase.
2. Open in VSCode.
3. Add your dataSettings.json and plugins.json file to src/Presentation/Nop.Web/App_Data
4. Press F5 to clean, build, and debug.
5. If you see issues involving SSL, try running the application within a private tab.

### Troubleshooting

If unable to restore, try adding NuGet as a source:

```
# add source
dotnet nuget add source "http://api.nuget.org/v3/index.json" --name "NuGet.org"

# confirm
dotnet nuget list source
```
