using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.DropShipping.AliExpress.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Catalog;
using Nop.Web.Framework.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;


namespace Nop.Plugin.DropShipping.AliExpress.Controllers;

[Route("ali-express")]
public class AliExpressAuthController : BasePluginController
{
    
    private readonly ISettingService _settingService;
    private readonly IStoreContext _storeContext;
    private readonly INotificationService _notificationService;
    private readonly ILocalizationService _localizationService;
    private readonly IPermissionService _permissionService;
    private readonly IAliExpressService _aliExpressService;
    private readonly IAliExpressProductMappingService _mappingService;
    private readonly IProductService _productService;

    public AliExpressAuthController(ISettingService settingService, IStoreContext storeContext, INotificationService notificationService, ILocalizationService localizationService, IPermissionService permissionService, IAliExpressService aliExpressService, IAliExpressProductMappingService mappingService, IProductService productService)
    {
        _settingService = settingService;
        _storeContext = storeContext;
        _notificationService = notificationService;
        _localizationService = localizationService;
        _permissionService = permissionService;
        _aliExpressService = aliExpressService;
        _mappingService = mappingService;
        _productService = productService;
    }


    [Route("callback")]
    public async Task<IActionResult> CallBackAsync([FromQuery] string code)
    {
        
        return RedirectToAction( "ExchangeAuthCode", "AliExpress", new {authCode = code });
    }
 
}


// Create an action Filter attribute that checks that the code was provided from the referring domain
// api-sg.aliexpress.com

// If no use the _notificationService to display an error message and redirect to return RedirectToAction( "Configure", "AliExpress");

public class AliExpressAuthRequirementAttribute : TypeFilterAttribute
{
    public AliExpressAuthRequirementAttribute() : base(typeof(AliExpressAuthRequirementFilter))
    {
    }

    private class AliExpressAuthRequirementFilter : IAsyncActionFilter
    {
        private readonly INotificationService _notificationService;
        private readonly ILocalizationService _localizationService;

        public AliExpressAuthRequirementFilter(INotificationService notificationService, ILocalizationService localizationService)
        {
            _notificationService = notificationService;
            _localizationService = localizationService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var referer = context.HttpContext.Request.Headers["Referer"].ToString();
            if (string.IsNullOrEmpty(referer) || !referer.Contains("api-sg.aliexpress.com", StringComparison.OrdinalIgnoreCase))
            {
                var message = await _localizationService.GetResourceAsync("Plugins.DropShipping.AliExpress.Auth.InvalidReferer");
                _notificationService.ErrorNotification(message);
                context.Result = new RedirectToActionResult("Configure", "AliExpress", null);
                return;
            }

            await next();
        }
    }
}
