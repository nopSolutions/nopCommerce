using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Vendors;
using Nop.Web.Factories;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Themes;
using Nop.Web.Models.Common;

namespace Nop.Web.Controllers
{
    [AutoValidateAntiforgeryToken]
    public partial class CommonController : BasePublicController
    {
        #region Fields

        protected CaptchaSettings CaptchaSettings { get; }
        protected CommonSettings CommonSettings { get; }
        protected ICommonModelFactory CommonModelFactory { get; }
        protected ICurrencyService CurrencyService { get; }
        protected ICustomerActivityService CustomerActivityService { get; }
        protected IGenericAttributeService GenericAttributeService { get; }
        protected ILanguageService LanguageService { get; }
        protected ILocalizationService LocalizationService { get; }
        protected IStoreContext StoreContext { get; }
        protected IThemeContext ThemeContext { get; }
        protected IVendorService VendorService { get; }
        protected IWorkContext WorkContext { get; }
        protected IWorkflowMessageService WorkflowMessageService { get; }
        protected LocalizationSettings LocalizationSettings { get; }
        protected SitemapSettings SitemapSettings { get; }
        protected SitemapXmlSettings SitemapXmlSettings { get; }
        protected StoreInformationSettings StoreInformationSettings { get; }
        protected VendorSettings VendorSettings { get; }

        #endregion

        #region Ctor

        public CommonController(CaptchaSettings captchaSettings,
            CommonSettings commonSettings,
            ICommonModelFactory commonModelFactory,
            ICurrencyService currencyService,
            ICustomerActivityService customerActivityService,
            IGenericAttributeService genericAttributeService,
            ILanguageService languageService,
            ILocalizationService localizationService,
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
            CaptchaSettings = captchaSettings;
            CommonSettings = commonSettings;
            CommonModelFactory = commonModelFactory;
            CurrencyService = currencyService;
            CustomerActivityService = customerActivityService;
            GenericAttributeService = genericAttributeService;
            LanguageService = languageService;
            LocalizationService = localizationService;
            StoreContext = storeContext;
            ThemeContext = themeContext;
            VendorService = vendorService;
            WorkContext = workContext;
            WorkflowMessageService = workflowMessageService;
            LocalizationSettings = localizationSettings;
            SitemapSettings = sitemapSettings;
            SitemapXmlSettings = sitemapXmlSettings;
            StoreInformationSettings = storeInformationSettings;
            VendorSettings = vendorSettings;
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
        [CheckAccessClosedStore(true)]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual async Task<IActionResult> SetLanguage(int langid, string returnUrl = "")
        {
            var language = await LanguageService.GetLanguageByIdAsync(langid);
            if (!language?.Published ?? false)
                language = await WorkContext.GetWorkingLanguageAsync();

            //home page
            if (string.IsNullOrEmpty(returnUrl))
                returnUrl = Url.RouteUrl("Homepage");

            //language part in URL
            if (LocalizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
            {
                //remove current language code if it's already localized URL
                if ((await returnUrl.IsLocalizedUrlAsync(Request.PathBase, true)).IsLocalized)
                    returnUrl = returnUrl.RemoveLanguageSeoCodeFromUrl(Request.PathBase, true);

                //and add code of passed language
                returnUrl = returnUrl.AddLanguageSeoCodeToUrl(Request.PathBase, true, language);
            }

            await WorkContext.SetWorkingLanguageAsync(language);

            //prevent open redirection attack
            if (!Url.IsLocalUrl(returnUrl))
                returnUrl = Url.RouteUrl("Homepage");

            return Redirect(returnUrl);
        }

        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual async Task<IActionResult> SetCurrency(int customerCurrency, string returnUrl = "")
        {
            var currency = await CurrencyService.GetCurrencyByIdAsync(customerCurrency);
            if (currency != null)
                await WorkContext.SetWorkingCurrencyAsync(currency);

            //home page
            if (string.IsNullOrEmpty(returnUrl))
                returnUrl = Url.RouteUrl("Homepage");

            //prevent open redirection attack
            if (!Url.IsLocalUrl(returnUrl))
                returnUrl = Url.RouteUrl("Homepage");

            return Redirect(returnUrl);
        }

        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual async Task<IActionResult> SetTaxType(int customerTaxType, string returnUrl = "")
        {
            var taxDisplayType = (TaxDisplayType)Enum.ToObject(typeof(TaxDisplayType), customerTaxType);
            await WorkContext.SetTaxDisplayTypeAsync(taxDisplayType);

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
        [CheckAccessClosedStore(true)]
        public virtual async Task<IActionResult> ContactUs()
        {
            var model = new ContactUsModel();
            model = await CommonModelFactory.PrepareContactUsModelAsync(model, false);
            
            return View(model);
        }

        [HttpPost, ActionName("ContactUs")]        
        [ValidateCaptcha]
        //available even when a store is closed
        [CheckAccessClosedStore(true)]
        public virtual async Task<IActionResult> ContactUsSend(ContactUsModel model, bool captchaValid)
        {
            //validate CAPTCHA
            if (CaptchaSettings.Enabled && CaptchaSettings.ShowOnContactUsPage && !captchaValid)
            {
                ModelState.AddModelError("", await LocalizationService.GetResourceAsync("Common.WrongCaptchaMessage"));
            }

            model = await CommonModelFactory.PrepareContactUsModelAsync(model, true);

            if (ModelState.IsValid)
            {
                var subject = CommonSettings.SubjectFieldOnContactUsForm ? model.Subject : null;
                var body = Core.Html.HtmlHelper.FormatText(model.Enquiry, false, true, false, false, false, false);

                await WorkflowMessageService.SendContactUsMessageAsync((await WorkContext.GetWorkingLanguageAsync()).Id,
                    model.Email.Trim(), model.FullName, subject, body);

                model.SuccessfullySent = true;
                model.Result = await LocalizationService.GetResourceAsync("ContactUs.YourEnquiryHasBeenSent");

                //activity log
                await CustomerActivityService.InsertActivityAsync("PublicStore.ContactUs",
                    await LocalizationService.GetResourceAsync("ActivityLog.PublicStore.ContactUs"));

                return View(model);
            }

            return View(model);
        }

        //contact vendor page
        public virtual async Task<IActionResult> ContactVendor(int vendorId)
        {
            if (!VendorSettings.AllowCustomersToContactVendors)
                return RedirectToRoute("Homepage");

            var vendor = await VendorService.GetVendorByIdAsync(vendorId);
            if (vendor == null || !vendor.Active || vendor.Deleted)
                return RedirectToRoute("Homepage");

            var model = new ContactVendorModel();
            model = await CommonModelFactory.PrepareContactVendorModelAsync(model, vendor, false);
            
            return View(model);
        }

        [HttpPost, ActionName("ContactVendor")]        
        [ValidateCaptcha]
        public virtual async Task<IActionResult> ContactVendorSend(ContactVendorModel model, bool captchaValid)
        {
            if (!VendorSettings.AllowCustomersToContactVendors)
                return RedirectToRoute("Homepage");

            var vendor = await VendorService.GetVendorByIdAsync(model.VendorId);
            if (vendor == null || !vendor.Active || vendor.Deleted)
                return RedirectToRoute("Homepage");

            //validate CAPTCHA
            if (CaptchaSettings.Enabled && CaptchaSettings.ShowOnContactUsPage && !captchaValid)
            {
                ModelState.AddModelError("", await LocalizationService.GetResourceAsync("Common.WrongCaptchaMessage"));
            }

            model = await CommonModelFactory.PrepareContactVendorModelAsync(model, vendor, true);

            if (ModelState.IsValid)
            {
                var subject = CommonSettings.SubjectFieldOnContactUsForm ? model.Subject : null;
                var body = Core.Html.HtmlHelper.FormatText(model.Enquiry, false, true, false, false, false, false);

                await WorkflowMessageService.SendContactVendorMessageAsync(vendor, (await WorkContext.GetWorkingLanguageAsync()).Id,
                    model.Email.Trim(), model.FullName, subject, body);

                model.SuccessfullySent = true;
                model.Result = await LocalizationService.GetResourceAsync("ContactVendor.YourEnquiryHasBeenSent");

                return View(model);
            }

            return View(model);
        }

        //sitemap page
        public virtual async Task<IActionResult> Sitemap(SitemapPageModel pageModel)
        {
            if (!SitemapSettings.SitemapEnabled)
                return RedirectToRoute("Homepage");

            var model = await CommonModelFactory.PrepareSitemapModelAsync(pageModel);
            
            return View(model);
        }

        //SEO sitemap page
        //available even when a store is closed
        [CheckAccessClosedStore(true)]
        //ignore SEO friendly URLs checks
        [CheckLanguageSeoCode(true)]
        public virtual async Task<IActionResult> SitemapXml(int? id)
        {
            var siteMap = SitemapXmlSettings.SitemapXmlEnabled
                ? await CommonModelFactory.PrepareSitemapXmlAsync(id) : string.Empty;

            return Content(siteMap, "text/xml");
        }

        public virtual async Task<IActionResult> SetStoreTheme(string themeName, string returnUrl = "")
        {
            await ThemeContext.SetWorkingThemeNameAsync(themeName);

            //home page
            if (string.IsNullOrEmpty(returnUrl))
                returnUrl = Url.RouteUrl("Homepage");

            //prevent open redirection attack
            if (!Url.IsLocalUrl(returnUrl))
                returnUrl = Url.RouteUrl("Homepage");

            return Redirect(returnUrl);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        //available even when a store is closed
        [CheckAccessClosedStore(true)]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual async Task<IActionResult> EuCookieLawAccept()
        {
            if (!StoreInformationSettings.DisplayEuCookieLawWarning)
                //disabled
                return Json(new { stored = false });

            //save setting
            var store = await StoreContext.GetCurrentStoreAsync();
            await GenericAttributeService.SaveAttributeAsync(await WorkContext.GetCurrentCustomerAsync(), NopCustomerDefaults.EuCookieLawAcceptedAttribute, true, store.Id);
            return Json(new { stored = true });
        }

        //robots.txt file
        //available even when a store is closed
        [CheckAccessClosedStore(true)]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        //ignore SEO friendly URLs checks
        [CheckLanguageSeoCode(true)]
        public virtual async Task<IActionResult> RobotsTextFile()
        {
            var robotsFileContent = await CommonModelFactory.PrepareRobotsTextFileAsync();
            
            return Content(robotsFileContent, MimeTypes.TextPlain);
        }

        public virtual IActionResult GenericUrl()
        {
            //seems that no entity was found
            return InvokeHttp404();
        }

        //store is closed
        //available even when a store is closed
        [CheckAccessClosedStore(true)]
        public virtual IActionResult StoreClosed()
        {
            return View();
        }

        //helper method to redirect users. Workaround for GenericPathRoute class where we're not allowed to do it
        public virtual IActionResult InternalRedirect(string url, bool permanentRedirect)
        {
            //ensure it's invoked from our GenericPathRoute class
            if (HttpContext.Items["nop.RedirectFromGenericPathRoute"] == null ||
                !Convert.ToBoolean(HttpContext.Items["nop.RedirectFromGenericPathRoute"]))
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

        #endregion
    }
}