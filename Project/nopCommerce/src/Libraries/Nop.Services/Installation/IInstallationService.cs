namespace Nop.Services.Installation;

/// <summary>
/// Installation service
/// </summary>
public partial interface IInstallationService
{
    /// <summary>
    /// Install
    /// </summary>
    /// <param name="installationSettings">Installation settings</param>
    Task InstallAsync(InstallationSettings installationSettings);
}