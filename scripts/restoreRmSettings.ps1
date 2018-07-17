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

#Get the website's propeties.
if (!$websiteName) { 
        Write-Host "variable is null" 
        BREAK}

$storage = Get-AzureRmStorageAccount -StorageAccountName $storageAccountName
$storageAccountKey = (Get-AzureRmStorageKey -StorageAccountName $storageAccountName).Primary
$blobContext = New-AzureRmStorageContext -StorageAccountName $storageAccountName -StorageAccountKey $storageAccountKey

if($slotName -eq "")
{
$website = Get-AzureRmWebSite -Name $websiteName 
}
else
{
$website = Get-AzureRmWebSite -Name $websiteName -Slot $slotName
}

$siteProperties = $website.SiteProperties.Properties
Write-Host "State:" $website.State

#extract url, username and password
$url = ($siteProperties | ?{ $_.Name -eq "RepositoryURI" }).Value.ToString() + "/MsDeploy.axd"
$userName = ($siteProperties | ?{ $_.Name -eq "PublishingUsername" }).Value
$pw = ($siteProperties | ?{ $_.Name -eq "PublishingPassword" }).Value
$base64AuthInfo = [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes(("{0}:{1}" -f $userName,$pw)))

if($slotName -eq "")
{
    $apiUrl = "https://$websiteName.scm.azurewebsites.net/api/vfs/site/wwwroot"
}
else
{
    $apiUrl = "https://$websiteName-$slotName.scm.azurewebsites.net/api/vfs/site/wwwroot"
}

$unzipFolder = "$sourcedir\$websiteName"
$BlobFolderName = "$websiteName"

Write-Host $apiUrl

# Create unzip folder
New-Item -ItemType Directory -Force -Path $unzipFolder
New-Item -ItemType Directory -Force -Path $unzipFolder\app_data

Start-Sleep -s 10

try 
{ 
	# Get file from storage
	Write-Host "Getting file from storage..."
    Get-AzureStorageBlobContent -Destination "$unzipFolder\web.config" -Container $ContainerName -Blob "$BlobFolderName\web.config"  -Context $blobContext
	Get-AzureStorageBlobContent -Destination "$unzipFolder\app_data\settings.txt" -Container $ContainerName -Blob "$BlobFolderName\app_data\settings.txt"  -Context $blobContext
	Get-AzureStorageBlobContent -Destination "$unzipFolder\app_data\InstalledPlugins.txt" -Container $ContainerName -Blob "$BlobFolderName\app_data\InstalledPlugins.txt"  -Context $blobContext
}
catch {
        $Error[0]
        $_.Exception.Response.StatusCode.Value__}

try 
{ 
# Upload settings files 
Write-Host "Deleting file from appdata..."
Invoke-RestMethod -Uri "$apiUrl/web.config" -TimeoutSec 600 -Headers @{Authorization=("Basic {0}" -f $base64AuthInfo);'If-Match'="*"} -Method DELETE -ContentType "multipart/form-data"
Invoke-RestMethod -Uri "$apiUrl/app_data/settings.txt" -TimeoutSec 600 -Headers @{Authorization=("Basic {0}" -f $base64AuthInfo);'If-Match'="*"} -Method DELETE -ContentType "multipart/form-data"
Invoke-RestMethod -Uri "$apiUrl/app_data/InstalledPlugins.txt" -TimeoutSec 600 -Headers @{Authorization=("Basic {0}" -f $base64AuthInfo);'If-Match'="*"} -Method DELETE -ContentType "multipart/form-data"
	}
catch {
      $_.Exception.Response.StatusCode.Value__}

Write-Host "Uploading setting installed pluging files to appdata..."     
Invoke-RestMethod -Uri "$apiUrl/web.config" -TimeoutSec 600 -Headers @{Authorization=("Basic {0}" -f $base64AuthInfo);'If-Match'="*"} -Method PUT -InFile "$unzipFolder\web.config" -ContentType "multipart/form-data"
Invoke-RestMethod -Uri "$apiUrl/app_data/settings.txt" -TimeoutSec 600 -Headers @{Authorization=("Basic {0}" -f $base64AuthInfo);'If-Match'="*"} -Method PUT -InFile "$unzipFolder\app_data\settings.txt" -ContentType "multipart/form-data"
Invoke-RestMethod -Uri "$apiUrl/app_data/InstalledPlugins.txt" -TimeoutSec 600 -Headers @{Authorization=("Basic {0}" -f $base64AuthInfo);'If-Match'="*"} -Method PUT -InFile "$unzipFolder\app_data\InstalledPlugins.txt" -ContentType "multipart/form-data"


Restart-AzureWebsite $websiteName 