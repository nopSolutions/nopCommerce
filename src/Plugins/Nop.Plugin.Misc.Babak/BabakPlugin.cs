using Nop.Core;
using Nop.Services.Localization;
using Nop.Services.Plugins;

namespace Nop.Plugin.Misc.Babak;

/// <summary>
/// Represents Babak plugin
/// </summary>
public class BabakPlugin : BasePlugin, IMiscPlugin
{
    #region Fields

    private readonly ILocalizationService _localizationService;
    private readonly IWebHelper _webHelper;

    #endregion

    #region Ctor

    public BabakPlugin(ILocalizationService localizationService,
        IWebHelper webHelper)
    {
        _localizationService = localizationService;
        _webHelper = webHelper;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets a configuration page URL
    /// </summary>
    public override string GetConfigurationPageUrl()
    {
        return $"{_webHelper.GetStoreLocation()}Admin/Babak/Configure";
    }

    /// <summary>
    /// Install plugin
    /// </summary>
    public override async Task InstallAsync()
    {
        await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
        {
            ["Nop.Plugin.Misc.Babak.Menu"] = "Babak",
            ["Nop.Plugin.Misc.Babak.Title"] = "Babak items",
            ["Nop.Plugin.Misc.Babak.Fields.Name"] = "Name",
            ["Nop.Plugin.Misc.Babak.Fields.Name.Hint"] = "Enter the item name.",
            ["Nop.Plugin.Misc.Babak.Fields.Description"] = "Description",
            ["Nop.Plugin.Misc.Babak.Fields.Description.Hint"] = "Enter a short description.",
            ["Nop.Plugin.Misc.Babak.Fields.IsActive"] = "Active",
            ["Nop.Plugin.Misc.Babak.Fields.IsActive.Hint"] = "Check to enable this item.",
            ["Nop.Plugin.Misc.Babak.Created"] = "Babak item created successfully.",
            ["Nop.Plugin.Misc.Babak.Updated"] = "Babak item updated successfully.",
            ["Nop.Plugin.Misc.Babak.Deleted"] = "Babak item deleted successfully."
        });

        await base.InstallAsync();
    }

    /// <summary>
    /// Uninstall plugin
    /// </summary>
    public override async Task UninstallAsync()
    {
        await _localizationService.DeleteLocaleResourcesAsync("Nop.Plugin.Misc.Babak");

        await base.UninstallAsync();
    }

    #endregion
}
