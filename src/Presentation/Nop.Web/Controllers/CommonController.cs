using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Core.Http;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Html;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Vendors;
using Nop.Web.Factories;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Themes;
using Nop.Web.Models.Common;
using Nop.Web.Models.Sitemap;

namespace Nop.Web.Controllers;

[AutoValidateAntiforgeryToken]
public partial class CommonController : BasePublicController
{
    #region Fields

    protected readonly CaptchaSettings _captchaSettings;
    protected readonly CommonSettings _commonSettings;
    protected readonly ICommonModelFactory _commonModelFactory;
    protected readonly ICurrencyService _currencyService;
    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly IHtmlFormatter _htmlFormatter;
    protected readonly ILanguageService _languageService;
    protected readonly ILocalizationService _localizationService;
    protected readonly ISitemapModelFactory _sitemapModelFactory;
    protected readonly IStoreContext _storeContext;
    protected readonly IThemeContext _themeContext;
    protected readonly IVendorService _vendorService;
    protected readonly IWorkContext _workContext;
    protected readonly IWorkflowMessageService _workflowMessageService;
    protected readonly LocalizationSettings _localizationSettings;
    protected readonly SitemapSettings _sitemapSettings;
    protected readonly SitemapXmlSettings _sitemapXmlSettings;
    protected readonly StoreInformationSettings _storeInformationSettings;
    protected readonly VendorSettings _vendorSettings;

    #endregion

    #region Ctor

    public CommonController(CaptchaSettings captchaSettings,
        CommonSettings commonSettings,
        ICommonModelFactory commonModelFactory,
        ICurrencyService currencyService,
        ICustomerActivityService customerActivityService,
        IGenericAttributeService genericAttributeService,
        IHtmlFormatter htmlFormatter,
        ILanguageService languageService,
        ILocalizationService localizationService,
        ISitemapModelFactory sitemapModelFactory,
        IStoreContext storeContext,
        IThemeContext themeContext,
        IVendorService vendorService,
        IWorkContext workContext,
        IWorkflowMessageService workflowMessageService,
        LocalizationSettings localizationSettings,
        SitemapSettings sitemapSettings,
        SitemapXmlSettings sitemapXmlSettings,
        StoreInformationSettings storeInformationSettings,
        VendorSettings vendorSettings)
    {
        _captchaSettings = captchaSettings;
        _commonSettings = commonSettings;
        _commonModelFactory = commonModelFactory;
        _currencyService = currencyService;
        _customerActivityService = customerActivityService;
        _genericAttributeService = genericAttributeService;
        _htmlFormatter = htmlFormatter;
        _languageService = languageService;
        _localizationService = localizationService;
        _sitemapModelFactory = sitemapModelFactory;
        _storeContext = storeContext;
        _themeContext = themeContext;
        _vendorService = vendorService;
        _workContext = workContext;
        _workflowMessageService = workflowMessageService;
        _localizationSettings = localizationSettings;
        _sitemapSettings = sitemapSettings;
        _sitemapXmlSettings = sitemapXmlSettings;
        _storeInformationSettings = storeInformationSettings;
        _vendorSettings = vendorSettings;
    }

    #endregion

    #region Methods

    //page not found
    public virtual IActionResult PageNotFound()
    {
        Response.StatusCode = 404;
        Response.ContentType = "text/html";

        return View();
    }

    //available even when a store is closed
    [CheckAccessClosedStore(ignore: true)]
    //available even when navigation is not allowed
    [CheckAccessPublicStore(ignore: true)]
    public virtual async Task<IActionResult> SetLanguage(int langid, string returnUrl = "")
    {
        var language = await _languageService.GetLanguageByIdAsync(langid);
        if (!language?.Published ?? false)
            language = await _workContext.GetWorkingLanguageAsync();

        //home page
        if (string.IsNullOrEmpty(returnUrl))
            returnUrl = Url.RouteUrl("Homepage");

        //language part in URL
        if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
        {
            //remove current language code if it's already localized URL
            if ((await returnUrl.IsLocalizedUrlAsync(Request.PathBase, true)).IsLocalized)
                returnUrl = returnUrl.RemoveLanguageSeoCodeFromUrl(Request.PathBase, true);

            //and add code of passed language
            returnUrl = returnUrl.AddLanguageSeoCodeToUrl(Request.PathBase, true, language);
        }

        await _workContext.SetWorkingLanguageAsync(language);

        //prevent open redirection attack
        if (!Url.IsLocalUrl(returnUrl))
            returnUrl = Url.RouteUrl("Homepage");

        return Redirect(returnUrl);
    }

    //available even when navigation is not allowed
    [CheckAccessPublicStore(ignore: true)]
    public virtual async Task<IActionResult> SetCurrency(int customerCurrency, string returnUrl = "")
    {
        var currency = await _currencyService.GetCurrencyByIdAsync(customerCurrency);
        if (currency != null)
            await _workContext.SetWorkingCurrencyAsync(currency);

        //home page
        if (string.IsNullOrEmpty(returnUrl))
            returnUrl = Url.RouteUrl("Homepage");

        //prevent open redirection attack
        if (!Url.IsLocalUrl(returnUrl))
            returnUrl = Url.RouteUrl("Homepage");

        return Redirect(returnUrl);
    }

    //available even when navigation is not allowed
    [CheckAccessPublicStore(ignore: true)]
    public virtual async Task<IActionResult> SetTaxType(int customerTaxType, string returnUrl = "")
    {
        var taxDisplayType = (TaxDisplayType)Enum.ToObject(typeof(TaxDisplayType), customerTaxType);
        await _workContext.SetTaxDisplayTypeAsync(taxDisplayType);

        //home page
        if (string.IsNullOrEmpty(returnUrl))
            returnUrl = Url.RouteUrl("Homepage");

        //prevent open redirection attack
        if (!Url.IsLocalUrl(returnUrl))
            returnUrl = Url.RouteUrl("Homepage");

        return Redirect(returnUrl);
    }

    //contact us page
    //available even when a store is closed
    [CheckAccessClosedStore(ignore: true)]
    public virtual async Task<IActionResult> ContactUs()
    {
        var model = new ContactUsModel();
        model = await _commonModelFactory.PrepareContactUsModelAsync(model, false);

        return View(model);
    }

    [HttpPost, ActionName("ContactUs")]
    [ValidateCaptcha]
    //available even when a store is closed
    [CheckAccessClosedStore(ignore: true)]
    public virtual async Task<IActionResult> ContactUsSend(ContactUsModel model, bool captchaValid)
    {
        //validate CAPTCHA
        if (_captchaSettings.Enabled && _captchaSettings.ShowOnContactUsPage && !captchaValid)
        {
            ModelState.AddModelError("", await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));
        }

        model = await _commonModelFactory.PrepareContactUsModelAsync(model, true);

        if (ModelState.IsValid)
        {
            var subject = _commonSettings.SubjectFieldOnContactUsForm ? model.Subject : null;
            var body = _htmlFormatter.FormatText(model.Enquiry, false, true, false, false, false, false);

            await _workflowMessageService.SendContactUsMessageAsync((await _workContext.GetWorkingLanguageAsync()).Id,
                model.Email, model.FullName, subject, body);

            model.SuccessfullySent = true;
            model.Result = await _localizationService.GetResourceAsync("ContactUs.YourEnquiryHasBeenSent");

            //activity log
            await _customerActivityService.InsertActivityAsync("PublicStore.ContactUs",
                await _localizationService.GetResourceAsync("ActivityLog.PublicStore.ContactUs"));

            return View(model);
        }

        return View(model);
    }

    //contact vendor page
    public virtual async Task<IActionResult> ContactVendor(int vendorId)
    {
        if (!_vendorSettings.AllowCustomersToContactVendors)
            return RedirectToRoute("Homepage");

        var vendor = await _vendorService.GetVendorByIdAsync(vendorId);
        if (vendor == null || !vendor.Active || vendor.Deleted)
            return RedirectToRoute("Homepage");

        var model = new ContactVendorModel();
        model = await _commonModelFactory.PrepareContactVendorModelAsync(model, vendor, false);

        return View(model);
    }

    [HttpPost, ActionName("ContactVendor")]
    [ValidateCaptcha]
    public virtual async Task<IActionResult> ContactVendorSend(ContactVendorModel model, bool captchaValid)
    {
        if (!_vendorSettings.AllowCustomersToContactVendors)
            return RedirectToRoute("Homepage");

        var vendor = await _vendorService.GetVendorByIdAsync(model.VendorId);
        if (vendor == null || !vendor.Active || vendor.Deleted)
            return RedirectToRoute("Homepage");

        //validate CAPTCHA
        if (_captchaSettings.Enabled && _captchaSettings.ShowOnContactUsPage && !captchaValid)
        {
            ModelState.AddModelError("", await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));
        }

        model = await _commonModelFactory.PrepareContactVendorModelAsync(model, vendor, true);

        if (ModelState.IsValid)
        {
            var subject = _commonSettings.SubjectFieldOnContactUsForm ? model.Subject : null;
            var body = _htmlFormatter.FormatText(model.Enquiry, false, true, false, false, false, false);

            await _workflowMessageService.SendContactVendorMessageAsync(vendor, (await _workContext.GetWorkingLanguageAsync()).Id,
                model.Email, model.FullName, subject, body);

            model.SuccessfullySent = true;
            model.Result = await _localizationService.GetResourceAsync("ContactVendor.YourEnquiryHasBeenSent");

            return View(model);
        }

        return View(model);
    }

    //sitemap page
    public virtual async Task<IActionResult> Sitemap(SitemapPageModel pageModel)
    {
        if (!_sitemapSettings.SitemapEnabled)
            return RedirectToRoute("Homepage");

        var model = await _sitemapModelFactory.PrepareSitemapModelAsync(pageModel);

        return View(model);
    }

    //SEO sitemap page
    //available even when a store is closed
    [CheckAccessClosedStore(ignore: true)]
    //available even when navigation is not allowed
    [CheckAccessPublicStore(ignore: true)]
    //ignore SEO friendly URLs checks
    [CheckLanguageSeoCode(ignore: true)]
    public virtual async Task<IActionResult> SitemapXml(int? id)
    {
        if (!_sitemapXmlSettings.SitemapXmlEnabled)
            return StatusCode(StatusCodes.Status403Forbidden);

        try
        {
            var sitemapXmlModel = await _sitemapModelFactory.PrepareSitemapXmlModelAsync(id ?? 0);
            return PhysicalFile(sitemapXmlModel.SitemapXmlPath, MimeTypes.ApplicationXml);
        }
        catch
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable);
        }
    }

    public virtual async Task<IActionResult> SetStoreTheme(string themeName, string returnUrl = "")
    {
        await _themeContext.SetWorkingThemeNameAsync(themeName);

        //home page
        if (string.IsNullOrEmpty(returnUrl))
            returnUrl = Url.RouteUrl("Homepage");

        //prevent open redirection attack
        if (!Url.IsLocalUrl(returnUrl))
            returnUrl = Url.RouteUrl("Homepage");

        return Redirect(returnUrl);
    }

    [HttpPost]
    //available even when a store is closed
    [CheckAccessClosedStore(ignore: true)]
    //available even when navigation is not allowed
    [CheckAccessPublicStore(ignore: true)]
    public virtual async Task<IActionResult> EuCookieLawAccept()
    {
        if (!_storeInformationSettings.DisplayEuCookieLawWarning)
            //disabled
            return Json(new { stored = false });

        //save setting
        var store = await _storeContext.GetCurrentStoreAsync();
        await _genericAttributeService.SaveAttributeAsync(await _workContext.GetCurrentCustomerAsync(), NopCustomerDefaults.EuCookieLawAcceptedAttribute, true, store.Id);
        return Json(new { stored = true });
    }

    //robots.txt file
    //available even when a store is closed
    [CheckAccessClosedStore(ignore: true)]
    //available even when navigation is not allowed
    [CheckAccessPublicStore(ignore: true)]
    //ignore SEO friendly URLs checks
    [CheckLanguageSeoCode(ignore: true)]
    public virtual async Task<IActionResult> RobotsTextFile()
    {
        var robotsFileContent = await _commonModelFactory.PrepareRobotsTextFileAsync();

        return Content(robotsFileContent, MimeTypes.TextPlain);
    }

    //available even when a store is closed
    [CheckAccessClosedStore(ignore: true)]
    //available even when navigation is not allowed
    [CheckAccessPublicStore(ignore: true)]
    public virtual IActionResult GenericUrl()
    {
        //seems that no entity was found
        return InvokeHttp404();
    }

    //store is closed
    //available even when a store is closed
    [CheckAccessClosedStore(ignore: true)]
    public virtual IActionResult StoreClosed()
    {
        return View();
    }

    //helper method to redirect users. Workaround for GenericPathRoute class where we're not allowed to do it
    public virtual IActionResult InternalRedirect(string url, bool permanentRedirect)
    {
        //ensure it's invoked from our GenericPathRoute class
        if (!HttpContext.Items.TryGetValue(NopHttpDefaults.GenericRouteInternalRedirect, out var value) || value is not bool redirect || !redirect)
        {
            url = Url.RouteUrl("Homepage");
            permanentRedirect = false;
        }

        //home page
        if (string.IsNullOrEmpty(url))
        {
            url = Url.RouteUrl("Homepage");
            permanentRedirect = false;
        }

        //prevent open redirection attack
        if (!Url.IsLocalUrl(url))
        {
            url = Url.RouteUrl("Homepage");
            permanentRedirect = false;
        }

        if (permanentRedirect)
            return RedirectPermanent(url);

        return Redirect(url);
    }

    //available even when a store is closed
    [CheckAccessClosedStore(ignore: true)]
    //available even when navigation is not allowed
    [CheckAccessPublicStore(ignore: true)]
    public virtual IActionResult FallbackRedirect()
    {
        //nothing was found
        return InvokeHttp404();
    }

    #endregion
}