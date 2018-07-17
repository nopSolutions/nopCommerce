param([String]$websiteName="",[string]$slotName="proda",[string]$storageAccountName="xshop",[string]$ContainerName="xshop")


function Expand-ZIPFile($file, $destination)
{
$shell = new-object -com shell.application
$zip = $shell.NameSpace($file)
foreach($item in $zip.items())
{
$shell.Namespace($destination).copyhere($item)
}
}

function ZipFiles( $zipfilename, $sourcedir )
{
   Add-Type -Assembly System.IO.Compression.FileSystem
   $compressionLevel = [System.IO.Compression.CompressionLevel]::Optimal
   [System.IO.Compression.ZipFile]::CreateFromDirectory($sourcedir,
        $zipfilename, $compressionLevel, $false)
}

Write-Host $websiteName
$sourcedir  = $Env:BUILD_SOURCESDIRECTORY

#Get the website's propeties.
if (!$websiteName) { 
        Write-Host "variable is null" 
        BREAK}

if (!(Test-AzureName -Website $websiteName))
{
  BREAK
}


$storageAccountName = "xshop"
$ContainerName  = "xshop"
$storage = Get-AzureStorageAccount -StorageAccountName $storageAccountName
$storageAccountKey = (Get-AzureStorageKey -StorageAccountName $storageAccountName).Primary
$blobContext = New-AzureStorageContext -StorageAccountName $storageAccountName -StorageAccountKey $storageAccountKey

$website = Get-AzureWebSite -Name $websiteName
$siteProperties = $website.SiteProperties.Properties

#extract url, username and password
$url = ($siteProperties | ?{ $_.Name -eq "RepositoryURI" }).Value.ToString() + "/MsDeploy.axd"
$userName = ($siteProperties | ?{ $_.Name -eq "PublishingUsername" }).Value
$pw = ($siteProperties | ?{ $_.Name -eq "PublishingPassword" }).Value

$base64AuthInfo = [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes(("{0}:{1}" -f $userName,$pw)))
$apiUrl = "https://$websiteName.scm.azurewebsites.net/api/vfs/site/wwwroot"
        
$unzipFolder = "$sourcedir\$websiteName"                
$BlobFolderName = "$websiteName"

# Create unzip folder
New-Item -ItemType Directory -Force -Path $unzipFolder
New-Item -ItemType Directory -Force -Path $unzipFolder/app_data

try
{
# download settings file        
Invoke-RestMethod -Uri "$apiUrl/web.config" -TimeoutSec 600 -Headers @{Authorization=("Basic {0}" -f $base64AuthInfo)} -Method GET -OutFile "$unzipFolder\web.config" -ContentType "multipart/form-data"
Invoke-RestMethod -Uri "$apiUrl/app_data/settings.txt" -TimeoutSec 600 -Headers @{Authorization=("Basic {0}" -f $base64AuthInfo)} -Method GET -OutFile "$unzipFolder\app_data\settings.txt" -ContentType "multipart/form-data"
Invoke-RestMethod -Uri "$apiUrl/app_data/InstalledPlugins.txt" -TimeoutSec 600 -Headers @{Authorization=("Basic {0}" -f $base64AuthInfo)} -Method GET -OutFile "$unzipFolder\app_data\InstalledPlugins.txt" -ContentType "multipart/form-data"
Invoke-RestMethod -Uri "$apiUrl/favicon.ico" -TimeoutSec 600 -Headers @{Authorization=("Basic {0}" -f $base64AuthInfo)} -Method GET -OutFile "$unzipFolder\favicon.ico" -ContentType "multipart/form-data"
} catch {
      $_.Exception.Response.StatusCode.Value__
      }

Get-ChildItem -Path $unzipFolder

if(Test-Path "$unzipFolder\app_data\settings.txt")
{
        # Upload to storage
        Write-Host "# Upload to storage"
        Set-AzureStorageBlobContent -Force -File "$unzipFolder\web.config" -Container $ContainerName -Blob "$BlobFolderName\web.config"  -Context $blobContext
        Set-AzureStorageBlobContent -Force -File "$unzipFolder\app_data\settings.txt" -Container $ContainerName -Blob "$BlobFolderName\app_data\settings.txt"  -Context $blobContext
        Set-AzureStorageBlobContent -Force -File "$unzipFolder\app_data\InstalledPlugins.txt" -Container $ContainerName -Blob "$BlobFolderName\app_data\InstalledPlugins.txt"  -Context $blobContext
		Set-AzureStorageBlobContent -Force -File "$unzipFolder\app_data\favicon.ico" -Container $ContainerName -Blob "$BlobFolderName\app_data\favicon.ico"  -Context $blobContext
}
