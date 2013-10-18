param($installPath, $toolsPath, $package, $project)

# Return a relative path with reference to root as Uri object 
# $rootPath - root path
# $relativePath - relative path
# $appendToRelativePath - Optional parameter. If provided will be appended to relative Path using Path.Combine()
Function GetRelativeUri($rootPath, $relativePath, $appendToRelativePath)
{
	if($rootPath -eq $null)
	{
		return $null
	}
	
	if($relativePath -eq $null)
	{
		return $null
	}
	
	$rootUri = new-object system.Uri($rootPath)     
	$targetPath = $relativePath
	
	# If appendToRelativePath is provided then use it
	if($appendToRelativePath -ne $null)
	{
		$targetPath = [io.path]::Combine($relativePath, $appendToRelativePath)
	}
	
	$targetUri = new-object system.Uri($targetPath)    
	$relativeUri = $rootUri.MakeRelativeUri($targetUri)       

	return $relativeUri
}

# Visual Studio execution done via NuGet Package Manager
Function VSExecution($installPath, $package, $project)
{
    #$project.DTE.ExecuteCommand("File.SaveAll", [system.string]::Empty)

    # Get the msbuild version of the project and add the import
    $msbuild = [Microsoft.Build.Evaluation.ProjectCollection]::GlobalProjectCollection.GetLoadedProjects($project.FullName) | Select-Object -First 1

    # add a property for us to be able to reference the path where the package was installed
    $relativePackageUri = GetRelativeUri $project.FullName $installPath"\lib"

    $msbuild.Xml.AddProperty("WebGreaseLibPath", $relativePackageUri.ToString().Replace([System.IO.Path]::AltDirectorySeparatorChar, [System.IO.Path]::DirectorySeparatorChar))

    # save the project
    $project.Save()
}

# Command line execution done by any external tool (For example, NuGetUpdater)
# $package - package id 
# $project - parameter value is path to Project file in this case.
Function CommandLineExecution($installPath, $package, $project)
{
    [Reflection.Assembly]::LoadWithPartialName("Microsoft.Build")
    [Reflection.Assembly]::LoadWithPartialName("System.Xml")
    [Reflection.Assembly]::LoadWithPartialName("System.Xml.Linq")

    # Get the msbuild version of the project and add the import
    $projXDoc = [System.Xml.Linq.XDocument]::Load($project)
        
    $defaultNameSpace = $projXDoc.Root.GetDefaultNamespace()

    $propertyGroup = [System.Xml.Linq.XName]::Get("PropertyGroup", $defaultNameSpace.NamespaceName)
    $webGreaseBuildLocation = [System.Xml.Linq.XName]::Get("WebGreaseLibPath", $defaultNameSpace.NamespaceName)
    
    # add a property for us to be able to reference the path where the package was installed
    $relativePackageUri = GetRelativeUri $project.FullName $installPath"\lib"
    
    $propGroupElement = $projXDoc.Root.Elements($propertyGroup) | Select-Object -First 1
    IF ($propGroupElement -ne $null)
    {
        $newElement = new-object System.Xml.Linq.XElement($webGreaseBuildLocation, $relativePackageUri.ToString().Replace([System.IO.Path]::AltDirectorySeparatorChar, [System.IO.Path]::DirectorySeparatorChar))
        $propGroupElement.Add($newElement)
    }
    
    # save the project
    $projXDoc.Save($project)
}


IF ($project -is [system.string])
{
    CommandLineExecution $installPath $package $project
}
ELSE
{
    VSExecution $installPath $package $project
}
