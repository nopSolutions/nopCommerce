using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Stores;
using Nop.Web.Framework.Components;

namespace Nop.Web.Areas.Admin.Components;

public partial class AclDisabledWarningViewComponent : NopViewComponent
{
    protected readonly CatalogSettings _catalogSettings;
    protected readonly ISettingService _settingService;
    protected readonly IStoreService _storeService;

    public AclDisabledWarningViewComponent(CatalogSettings catalogSettings,
        ISettingService settingService,
        IStoreService storeService)
    {
        _catalogSettings = catalogSettings;
        _settingService = settingService;
        _storeService = storeService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        //action displaying notification (warning) to a store owner that "ACL rules" feature is ignored

        //default setting
        var enabled = _catalogSettings.IgnoreAcl;
        if (!enabled)
        {
            //overridden settings
            var stores = await _storeService.GetAllStoresAsync();
            foreach (var store in stores)
            {
                var catalogSettings = await _settingService.LoadSettingAsync<CatalogSettings>(store.Id);
                enabled = catalogSettings.IgnoreAcl;

                if (enabled)
                    break;
            }
        }

        //This setting is disabled. No warnings.
        if (!enabled)
            return Content(string.Empty);

        return View();
    }
}