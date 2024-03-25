# Creating New Plugin

First, select a plugin type based on desired functionality (Such as Widget, Payment, etc.):
https://docs.nopcommerce.com/en/developer/plugins/how-to-write-plugin-4.40.html

Name: `AbcWarehouse.Plugin.TYPE.NAME`

1. Copy a plugin from the `/src/Plugins` folder (match the type you're trying to make)
2. Update the following:
   1. Folder name
   2. `.csproj`
   3. `Plugin.cs`
   4. `logo.png`
   5. `plugin.json` (only include the above files)
2. Add to solution: `dotnet sln src/NopCommerce.sln add <PLUGIN_CSPROJ_PATH>`
3. Build, verify plugin is available in backend list.