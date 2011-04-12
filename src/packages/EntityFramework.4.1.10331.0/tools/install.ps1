param($installPath, $toolsPath, $package, $project)
$project.Object.References.Add("System.Data.Entity") | out-null
$project.Object.References.Add("System.ComponentModel.DataAnnotations") | out-null
