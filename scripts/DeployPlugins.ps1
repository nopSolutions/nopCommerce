param([String]$websiteName="",[String]$nopversion="")

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

$website = Get-AzureWebSite -Name $websiteName
$siteProperties = $website.SiteProperties.Properties

#extract url, username and password
$url = ($siteProperties | ?{ $_.Name -eq "RepositoryURI" }).Value.ToString() + "/MsDeploy.axd"
$userName = ($siteProperties | ?{ $_.Name -eq "PublishingUsername" }).Value
$pw = ($siteProperties | ?{ $_.Name -eq "PublishingPassword" }).Value

$base64AuthInfo = [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes(("{0}:{1}" -f $userName,$pw)))

$apiUrl = "https://$websiteName.scm.azurewebsites.net/api/zip/site/wwwroot/plugins/"
$filePath = "C:\Temp\books.zip"
	
$sourcedir  = $Env:BUILD_SOURCESDIRECTORY
echo %$sourcedir

$nopTemplatePlugins  = "$sourcedir\Plugins NopTemplate\$nopversion\plugins"
echo $nopTemplatePlugins

$zipfilename = "$sourcedir\plugins.zip"

ZipFiles $zipfilename $nopTemplatePlugins 

Invoke-RestMethod -Uri $apiUrl -TimeoutSec 600 -Headers @{Authorization=("Basic {0}" -f $base64AuthInfo)} -Method PUT -InFile $zipfilename -ContentType "multipart/form-data"

