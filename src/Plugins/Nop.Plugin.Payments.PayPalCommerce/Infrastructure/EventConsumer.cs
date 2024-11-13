using Nop.Core;
using Nop.Services.Common;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Web.Framework.Events;
using Nop.Web.Framework.Menu;

namespace Nop.Plugin.Payments.PayPalCommerce.Infrastructure;

/// <summary>
/// Represents plugin event consumer
/// </summary>
public class EventConsumer: IConsumer<AdminMenuCreatedEvent>
{
    private readonly ILocalizationService _localizationService;
    private readonly IPermissionService _permissionService;
    private readonly IPluginManager<IPaymentMethod> _pluginManager;
    private readonly IWorkContext _workContext;
    private readonly PayPalCommerceSettings _payPalCommerceSettings;

    public EventConsumer(ILocalizationService localizationService,
        IPermissionService permissionService,
        IPluginManager<IPaymentMethod> pluginManager,
        IWorkContext workContext,
        PayPalCommerceSettings payPalCommerceSettings)
    {
        _localizationService = localizationService;
        _permissionService = permissionService;
        _pluginManager = pluginManager;
        _workContext = workContext;
        _payPalCommerceSettings = payPalCommerceSettings;
    }

    public async Task HandleEventAsync(AdminMenuCreatedEvent eventMessage)
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermission.Configuration.MANAGE_PAYMENT_METHODS))
            return;

        var plugin = await _pluginManager.LoadPluginBySystemNameAsync(PayPalCommerceDefaults.SystemName);

        if (plugin == null) 
            return;

        var description = plugin.PluginDescriptor;

        if (description == null) 
            return;
        
        var newItem = new AdminMenuItem
        {
            Visible = true,
            SystemName = description.SystemName,
            Title = await _localizationService.GetLocalizedFriendlyNameAsync(plugin, (await _workContext.GetWorkingLanguageAsync()).Id),
            IconClass = "far fa-dot-circle",
            ChildNodes = new List<AdminMenuItem>
            {
                new()
                {
                    Visible = true,
                    SystemName = $"{PayPalCommerceDefaults.SystemName} Configuration",
                    Title = await _localizationService.GetResourceAsync("Plugins.Payments.PayPalCommerce.Configuration"),
                    Url = eventMessage.GetMenuItemUrl("PayPalCommerce", "Configure"),
                    IconClass = "far fa-circle"
                },
                new()
                {
                    Visible = _payPalCommerceSettings.UseSandbox || _payPalCommerceSettings.ConfiguratorSupported,
                    SystemName = $"{PayPalCommerceDefaults.SystemName} Pay Later",
                    Title = await _localizationService.GetResourceAsync("Plugins.Payments.PayPalCommerce.PayLater"),
                    Url = eventMessage.GetMenuItemUrl("PayPalCommerce", "PayLater"),
                    IconClass = "far fa-circle"
                }
            }
        };

        if (!eventMessage.RootMenuItem.InsertAfter("Misc.Zettle", newItem))
            eventMessage.RootMenuItem.InsertBefore("Local plugins", newItem);
    }
}
