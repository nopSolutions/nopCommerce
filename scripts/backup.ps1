param([String]$websiteName="")

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

echo Testing

Write-Host "Hello World from $Env:AGENT_NAME."
Write-Host "My ID is $Env:AGENT_ID."
Write-Host "AGENT_WORKFOLDER contents:"
gci $Env:AGENT_WORKFOLDER
Write-Host "AGENT_BUILDDIRECTORY contents:"
gci $Env:AGENT_BUILDDIRECTORY
Write-Host "BUILD_SOURCESDIRECTORY contents:"
gci $Env:BUILD_SOURCESDIRECTORY
Write-Host "BUILD_SOURCESDIRECTORY contents:"

Write-Host $build.stagingDirectory

Write-Host $websiteName

#Get the website's propeties.
if (!$websiteName) { 
        Write-Host "variable is null" 
        BREAK}

$sourcedir  = $Env:BUILD_SOURCESDIRECTORY
echo %$sourcedir

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
$apiUrl = "https://$websiteName.scm.azurewebsites.net/api/vfs/site/wwwroot/app_data"


$unzipFolder = "$sourcedir\$websiteName\app_data\"
                
$BlobFolderName = "$websiteName\app_data"

# Create unzip folder
New-Item -ItemType Directory -Force -Path $unzipFolder

# download settings file        
Invoke-RestMethod -Uri "$apiUrl/settings.txt" -TimeoutSec 600 -Headers @{Authorization=("Basic {0}" -f $base64AuthInfo)} -Method GET -OutFile "$unzipFolder\settings.txt" -ContentType "multipart/form-data"
Invoke-RestMethod -Uri "$apiUrl/InstalledPlugins.txt" -TimeoutSec 600 -Headers @{Authorization=("Basic {0}" -f $base64AuthInfo)} -Method GET -OutFile "$unzipFolder\InstalledPlugins.txt" -ContentType "multipart/form-data"

# Unzip files
#Unzip -File  $destination -Destination $unzipFolder

# Upload to storage
Set-AzureStorageBlobContent -File "$unzipFolder\settings.txt" -Container $ContainerName -Blob "$BlobFolderName\settings.txt"  -Context $blobContext
Set-AzureStorageBlobContent -File "$unzipFolder\InstalledPlugins.txt" -Container $ContainerName -Blob "$BlobFolderName\InstalledPlugins.txt"  -Context $blobContext
