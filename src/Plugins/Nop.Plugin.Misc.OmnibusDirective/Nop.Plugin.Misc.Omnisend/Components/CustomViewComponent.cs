using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Misc.Omnisend.Services;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Infrastructure;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Models.Catalog;

namespace Nop.Plugin.Misc.Omnisend.Components;

/// <summary>
/// Represents view component to embed tracking script on pages
/// </summary>
public class WidgetsOmnisendViewComponent : NopViewComponent
{
    #region Fields

    private readonly ICustomerService _customerService;
    private readonly IGenericAttributeService _genericAttributeService;
    private readonly INopUrlHelper _nopUrlHelper;
    private readonly IWebHelper _webHelper;
    private readonly IWorkContext _workContext;
    private readonly OmnisendService _omnisendService;
    private readonly OmnisendSettings _omnisendSettings;

    #endregion

    #region Ctor

    public WidgetsOmnisendViewComponent(ICustomerService customerService,
        IGenericAttributeService genericAttributeService,
        INopUrlHelper nopUrlHelper,
        IWebHelper webHelper,
        IWorkContext workContext,
        OmnisendService omnisendService,
        OmnisendSettings omnisendSettings)
    {
        _customerService = customerService;
        _genericAttributeService = genericAttributeService;
        _nopUrlHelper = nopUrlHelper;
        _webHelper = webHelper;
        _workContext = workContext;
        _omnisendService = omnisendService;
        _omnisendSettings = omnisendSettings;
    }

    #endregion

    #region Utilities

    private async Task<string> AddIdentifyContactScriptAsync(string script)
    {
        var customer = await _workContext.GetCurrentCustomerAsync();

        if (await _customerService.IsGuestAsync(customer))
            await GenerateGuestScriptAsync(customer);

        var identifyContactScript = await _genericAttributeService.GetAttributeAsync<string>(customer,
            OmnisendDefaults.IdentifyContactAttribute);

        if (string.IsNullOrEmpty(identifyContactScript))
            return script;

        script += $"{Environment.NewLine}{identifyContactScript}";

        await _genericAttributeService.SaveAttributeAsync<string>(customer,
            OmnisendDefaults.IdentifyContactAttribute, null);

        return script;
    }

    private async Task GenerateGuestScriptAsync(Customer customer)
    {
        var customerEmail = await _genericAttributeService.GetAttributeAsync<string>(customer,
            OmnisendDefaults.CustomerEmailAttribute);

        if (!string.IsNullOrEmpty(customerEmail))
            return;

        //try to get the ContactId from query parameters
        var omnisendContactId = _webHelper.QueryString<string>(OmnisendDefaults.ContactIdQueryParamName);
        if (string.IsNullOrEmpty(omnisendContactId))
            //try to get the ContactId from cookies
            Request.Cookies.TryGetValue($"{OmnisendDefaults.ContactIdQueryParamName}", out omnisendContactId);

        if (string.IsNullOrEmpty(omnisendContactId))
            return;

        var contact = await _omnisendService.GetContactInfoAsync(omnisendContactId);

        var email = contact?.Identifiers.FirstOrDefault(p => !string.IsNullOrEmpty(p.Id))?.Id;

        if (string.IsNullOrEmpty(email))
            return;

        await _genericAttributeService.SaveAttributeAsync(customer,
            OmnisendDefaults.CustomerEmailAttribute, email);

        if (string.IsNullOrEmpty(_omnisendSettings.IdentifyContactScript))
            return;

        var identifyScript = _omnisendSettings.IdentifyContactScript.Replace(OmnisendDefaults.Email,
            email);

        await _genericAttributeService.SaveAttributeAsync(customer,
            OmnisendDefaults.IdentifyContactAttribute, identifyScript);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Invoke view component
    /// </summary>
    /// <param name="widgetZone">Widget zone name</param>
    /// <param name="additionalData">Additional data</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the view component result
    /// </returns>
    public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
    {
        //ensure tracking is enabled
        if (!_omnisendSettings.UseTracking || string.IsNullOrEmpty(_omnisendSettings.BrandId))
            return Content(string.Empty);

        var script = string.Empty;

        //prepare tracking script
        if (!string.IsNullOrEmpty(_omnisendSettings.TrackingScript) &&
            widgetZone.Equals(PublicWidgetZones.BodyStartHtmlTagAfter, StringComparison.InvariantCultureIgnoreCase))
            script = await AddIdentifyContactScriptAsync(_omnisendSettings.TrackingScript
                .Replace(OmnisendDefaults.BrandId, _omnisendSettings.BrandId));

        //prepare product script
        if (!string.IsNullOrEmpty(_omnisendSettings.ProductScript) &&
            widgetZone.Equals(PublicWidgetZones.ProductDetailsBottom,
                StringComparison.InvariantCultureIgnoreCase) &&
            additionalData is ProductDetailsModel productDetails)
            script = _omnisendSettings.ProductScript
                .Replace(OmnisendDefaults.ProductId, $"{productDetails.Id}")
                .Replace(OmnisendDefaults.Sku, productDetails.Sku)
                .Replace(OmnisendDefaults.Currency, productDetails.ProductPrice.CurrencyCode)
                .Replace(OmnisendDefaults.Price, $"{(int)(productDetails.ProductPrice.PriceValue ?? 0 * 100)}")
                .Replace(OmnisendDefaults.Title, productDetails.Name)
                .Replace(OmnisendDefaults.ImageUrl, productDetails.DefaultPictureModel.ImageUrl)
                .Replace(OmnisendDefaults.ProductUrl,
                    await _nopUrlHelper.RouteGenericUrlAsync<Product>(new { productDetails.SeName }, _webHelper.GetCurrentRequestProtocol()));

        return string.IsNullOrEmpty(script)
            ? Content(string.Empty)
            : new HtmlContentViewComponentResult(new HtmlString(script));
    }

    #endregion
}