param($installPath, $toolsPath, $package, $project)

$invoker = @"
public class SqlCompactConnectionFactoryConfiguratorInvoker
{
    public static void Invoke(string assemblyPath, object project)
    {
        var appDomain = System.AppDomain.CreateDomain(
            "EntityFramework.PowerShell",
            null, 
            new System.AppDomainSetup { ShadowCopyFiles = "true" });

        appDomain.CreateInstanceFrom(
            assemblyPath, 
            "System.Data.Entity.ConnectionFactoryConfig.SqlCompactConnectionFactoryConfigurator",
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

Add-Type -TypeDefinition $invoker
[SqlCompactConnectionFactoryConfiguratorInvoker]::Invoke((Join-Path $toolsPath "EntityFramework.PowerShell.dll"), $project)
