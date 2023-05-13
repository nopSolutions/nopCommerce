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

## Creating a Plugin

1. Copy a plugin from the `/src/Plugins` folder.
2. Update the following:
   1. Folder name
   2. `.csproj`
   3. `Plugin.cs`
   4. `logo.png`
   5. `plugin.json` (only include the above files)
3. Add to project with: `dotnet sln src/NopCommerce.sln add src/Plugins/PLUGIN_FOLDER/PLUGIN_CSPROJ

## Tooltip

```
<div class="tooltip">
  <!-- area to hover over, usually an icon -->
  <span class="tooltiptext">
    <!-- Tooltip text, use a locale -->
  </span>
</div> 
```