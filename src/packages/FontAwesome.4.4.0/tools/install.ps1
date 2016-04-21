param($installPath, $toolsPath, $package, $project)

foreach ($fontFile in $project.ProjectItems.Item("fonts").ProjectItems)
{
	$fontFile.Properties.Item("BuildAction").Value = 2;
}