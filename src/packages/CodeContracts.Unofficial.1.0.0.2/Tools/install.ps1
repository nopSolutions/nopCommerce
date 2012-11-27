param($installPath, $toolsPath, $package, $project)
    $project.Object.References | Where-Object { $_.Name -eq 'DELETE_ME' } | ForEach-Object { $_.Remove() }