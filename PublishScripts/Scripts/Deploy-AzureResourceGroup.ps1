#Requires -Version 3.0
#Requires -Module AzureRM.Resources
#Requires -Module Azure.Storage

Param(
  [string] [Parameter(Mandatory=$true)] $ResourceGroupLocation,
  [string] [Parameter(Mandatory=$true)] $ResourceGroupName,
  [switch] $UploadArtifacts,
  [string] $StorageAccountName,
  [string] $StorageAccountResourceGroupName, 
  [string] $StorageContainerName = $ResourceGroupName.ToLowerInvariant() + '-stageartifacts',
  [string] $TemplateFile = '..\Templates\WebApplication1.json',
  [string] $TemplateParametersFile = '..\Templates\WebApplication1.param.dev.json',
  [string] $ArtifactStagingDirectory = '..\bin\staging',
  [string] $DSCSourceFolder = '.'
)

Set-StrictMode -Version 3
Import-Module Azure -ErrorAction SilentlyContinue

try {
    [Microsoft.Azure.Common.Authentication.AzureSession]::ClientFactory.AddUserAgent("VSAzureTools-HostInCloud$($host.name)".replace(" ","_"), "2.8")
} catch { }

$OptionalParameters = New-Object -TypeName Hashtable
$TemplateFile = [System.IO.Path]::Combine($PSScriptRoot, $TemplateFile)
$TemplateParametersFile = [System.IO.Path]::Combine($PSScriptRoot, $TemplateParametersFile)

if ($UploadArtifacts)
{
    # Convert relative paths to absolute paths if needed
    $ToolsFolder = [System.IO.Path]::Combine($PSScriptRoot, '..\Tools')
    $AzCopyPath = "$ToolsFolder\AzCopy.exe"
    $ArtifactStagingDirectory = [System.IO.Path]::Combine($PSScriptRoot, $ArtifactStagingDirectory)

    # Download AzCopy.exe
    if (-not (Test-Path $AzCopyPath))
    {
        $DownloadFolder = "$env:TEMP\AzCopy_" + [System.Guid]::NewGuid()
        $ExtractionFolder = "$DownloadFolder\extracted"
        $AzCopyMsi = "$DownloadFolder\MicrosoftAzureStorageTools.msi"

        New-Item $ToolsFolder -ItemType Directory -ErrorAction SilentlyContinue | Out-Null
        New-Item $DownloadFolder -ItemType Directory -ErrorAction SilentlyContinue | Out-Null

        Invoke-WebRequest -Uri "http://aka.ms/downloadazcopy" -OutFile $AzCopyMsi
        Unblock-File $AzCopyMsi

        Start-Process msiexec.exe -Argument "/a $AzCopyMsi /qb TARGETDIR=$ExtractionFolder /quiet" -Wait
        Copy-Item "$ExtractionFolder\Microsoft SDKs\Azure\AzCopy\*" $ToolsFolder -Force
        Remove-Item $DownloadFolder -Recurse -Force
    }

    Set-Variable ArtifactsLocationName '_artifactsLocation' -Option ReadOnly
    Set-Variable ArtifactsLocationSasTokenName '_artifactsLocationSasToken' -Option ReadOnly
    Set-Variable ConfigureWebServerName 'ConfigureWebServer.ps1' -Option ReadOnly

    $OptionalParameters.Add($ArtifactsLocationName, $null)
    $OptionalParameters.Add($ArtifactsLocationSasTokenName, $null)

    # Parse the parameter file and update the values of artifacts location and artifacts location SAS token if they are present
    $JsonContent = Get-Content $TemplateParametersFile -Raw | ConvertFrom-Json
    $JsonParameters = $JsonContent | Get-Member -Type NoteProperty | Where-Object {$_.Name -eq "parameters"}

    if ($JsonParameters -eq $null)
    {
        $JsonParameters = $JsonContent
    }
    else
    {
        $JsonParameters = $JsonContent.parameters
    }

    $JsonParameters | Get-Member -Type NoteProperty | ForEach-Object {
        $ParameterValue = $JsonParameters | Select-Object -ExpandProperty $_.Name

        if ($_.Name -eq $ArtifactsLocationName -or $_.Name -eq $ArtifactsLocationSasTokenName)
        {
            $OptionalParameters[$_.Name] = $ParameterValue.value
        }
    }

    $StorageAccountKey = (Get-AzureRmStorageAccountKey -ResourceGroupName $StorageAccountResourceGroupName -Name $StorageAccountName).Key1

    $StorageAccountContext = (Get-AzureRmStorageAccount -ResourceGroupName $StorageAccountResourceGroupName -Name $StorageAccountName).Context

    # Generate the value for artifacts location if it is not provided in the parameter file
    $ArtifactsLocation = $OptionalParameters[$ArtifactsLocationName]
    if ($ArtifactsLocation -eq $null)
    {
        $ArtifactsLocation = $StorageAccountContext.BlobEndPoint + $StorageContainerName
        $OptionalParameters[$ArtifactsLocationName] = $ArtifactsLocation
    }

    # Create DSC configuration archive
    $DSCSourceFolder = [System.IO.Path]::Combine($PSScriptRoot, $DSCSourceFolder)
    $DSCConfigureScriptPath = [System.IO.Path]::Combine($DSCSourceFolder, $ConfigureWebServerName)
    if (Test-Path $DSCConfigureScriptPath)
    {
        $DSCArchiveFile = $ArtifactStagingDirectory + "\" + $ConfigureWebServerName + ".zip"
        $DSCTempDirectory = "$env:TEMP\$ResourceGroupName\DSC"
        Add-Type -Assembly System.IO.Compression.FileSystem
        Remove-Item -Path $DSCTempDirectory -Recurse -ErrorAction SilentlyContinue
        Remove-Item -Path $DSCArchiveFile -ErrorAction SilentlyContinue
        New-Item -Path $DSCTempDirectory -ItemType Directory
        New-Item -Path $ArtifactStagingDirectory -ItemType Directory -ErrorAction SilentlyContinue
        Copy-Item -Path $DSCConfigureScriptPath -Destination $DSCTempDirectory
        [System.IO.Compression.ZipFile]::CreateFromDirectory($DSCTempDirectory, $DSCArchiveFile)
        Remove-Item -Path $DSCTempDirectory -Recurse -ErrorAction SilentlyContinue
    }

    # Use AzCopy to copy files from the local storage drop path to the storage account container
    & $AzCopyPath "/Source:$ArtifactStagingDirectory", "/Dest:$ArtifactsLocation", "/DestKey:$StorageAccountKey", "/S", "/Y", "/Z:$env:LocalAppData\Microsoft\Azure\AzCopy\$ResourceGroupName"
    if ($LASTEXITCODE -ne 0) { return }

    # Generate the value for artifacts location SAS token if it is not provided in the parameter file
    $ArtifactsLocationSasToken = $OptionalParameters[$ArtifactsLocationSasTokenName]
    if ($ArtifactsLocationSasToken -eq $null)
    {
       # Create a SAS token for the storage container - this gives temporary read-only access to the container.
       $ArtifactsLocationSasToken = New-AzureStorageContainerSASToken -Container $StorageContainerName -Context $StorageAccountContext -Permission r -ExpiryTime (Get-Date).AddHours(4)
       $ArtifactsLocationSasToken = ConvertTo-SecureString $ArtifactsLocationSasToken -AsPlainText -Force
       $OptionalParameters[$ArtifactsLocationSasTokenName] = $ArtifactsLocationSasToken
    }
}

# Create or update the resource group using the specified template file and template parameters file
New-AzureRmResourceGroup -Name $ResourceGroupName -Location $ResourceGroupLocation -Verbose -Force -ErrorAction Stop 

New-AzureRmResourceGroupDeployment -Name ((Get-ChildItem $TemplateFile).BaseName + '-' + ((Get-Date).ToUniversalTime()).ToString('MMdd-HHmm')) `
                                   -ResourceGroupName $ResourceGroupName `
                                   -TemplateFile $TemplateFile `
                                   -TemplateParameterFile $TemplateParametersFile `
                                   @OptionalParameters `
                                   -Force -Verbose
