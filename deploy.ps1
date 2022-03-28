$appPoolName = $env:IISAppPoolName

dotnet clean src/NopCommerce.sln
dotnet build src/NopCommerce.sln
dotnet clean src/NopCommerce.sln -c Release
dotnet build src/NopCommerce.sln -c Release

Stop-WebAppPool -Name $appPoolName | Out-Null
Write-Host 'App Pool Stopped.'
Start-Sleep -s 15
rm -Force -Recurse C:/NopABC/Plugins
dotnet publish -c Release ./src/Presentation/Nop.Web/Nop.Web.csproj --no-restore -o C:/NopABC
Start-WebAppPool -Name $appPoolName -ErrorAction 'SilentlyContinue' | Out-Null
Write-Host 'App Pool Started.'
