param($installPath, $toolsPath, $package, $project)

$invoker = @"
public class ConnectionFactoryConfiguratorInvoker
{
    public static void Invoke(string assemblyPath, object project)
    {
        var appDomain = System.AppDomain.CreateDomain(
            "EntityFramework.PowerShell",
            null, 
            new System.AppDomainSetup { ShadowCopyFiles = "true" });

        appDomain.CreateInstanceFrom(
            assemblyPath, 
            "System.Data.Entity.ConnectionFactoryConfig.ConnectionFactoryConfigurator",
            false,
            0,
            null,
            new object[] { project },
            null,
            null);

        System.AppDomain.Unload(appDomain);
    }
}
"@

$version = (new-object System.Runtime.Versioning.FrameworkName($project.Properties.Item("TargetFrameworkMoniker").Value)).Version

if ($version -ge (new-object System.Version(4, 5)))
{
    $dte.ItemOperations.OpenFile((Join-Path $toolsPath 'EF4.3on.NET4.5Readme.txt'))
}

Add-Type -TypeDefinition $invoker
[ConnectionFactoryConfiguratorInvoker]::Invoke((Join-Path $toolsPath "EntityFramework.PowerShell.dll"), $project)
