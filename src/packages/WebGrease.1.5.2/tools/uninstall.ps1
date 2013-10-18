param($installPath, $toolsPath, $package, $project)

# Visual Studio execution done via NuGet Package Manager
Function VSExecution($toolsPath, $project)
{
	

	$project.DTE.ExecuteCommand("File.SaveAll", [system.string]::Empty)

	# Get the msbuild version of the project and add the import
	$msbuild = [Microsoft.Build.Evaluation.ProjectCollection]::GlobalProjectCollection.GetLoadedProjects($project.FullName) | Select-Object -First 1

	# now remove our property that points to this package path, "WebGreaseLibPath"
	foreach ($property in $msbuild.Properties)
	{
		if ($property.Name -eq "WebGreaseLibPath")
		{
			$propertyToRemove = $property
		}
	}

	if ($propertyToRemove -ne $null)
	{
		$propertyToRemove.Project.RemoveProperty($propertyToRemove)
		$project.Save()
	}

	$project.DTE.ExecuteCommand("File.SaveAll", [system.string]::Empty)
}

# Command line execution done by any external tool (For example, NuGetUpdater)
# $project - parameter value is path to Project file in this case.
Function CommandLineExecution($toolsPath, $project)
{
	[Reflection.Assembly]::LoadWithPartialName("System.Xml")
	[Reflection.Assembly]::LoadWithPartialName("System.Xml.Linq")


	
	$projXDoc = [System.Xml.Linq.XDocument]::Load($project)
	$defaultNameSpace = $projXDoc.Root.GetDefaultNamespace()
	$xmlReader = $projXDoc.CreateReader()
	$namespaceManager = new-object System.Xml.XmlNamespaceManager($xmlReader.NameTable)
	$namespaceManager.AddNamespace("my", $defaultNameSpace.NamespaceName)

	$msnRfPackageElement = [System.Xml.XPath.Extensions]::XPathSelectElement($projXDoc.Root, "//my:WebGreaseLibPath", $namespaceManager)
	if($msnRfPackageElement -ne $null)
	{
		$msnRfPackageElement.Remove()
	}
	
	# save the project
	$projXDoc.Save($project)
}

IF ($project -is [system.string])
{
    CommandLineExecution $toolsPath $project
}
ELSE
{
    VSExecution $toolsPath $project
}
