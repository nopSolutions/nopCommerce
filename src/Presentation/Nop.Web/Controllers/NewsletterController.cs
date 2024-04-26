using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Security;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Web.Factories;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Controllers;

[AutoValidateAntiforgeryToken]
public partial class NewsletterController : BasePublicController
{
    protected readonly CaptchaSettings _captchaSettings;
    protected readonly ICustomerService _customerService;
    protected readonly ILanguageService _languageService;
    protected readonly ILocalizationService _localizationService;
    protected readonly INewsletterModelFactory _newsletterModelFactory;
    protected readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
    protected readonly IStoreContext _storeContext;
    protected readonly IWorkContext _workContext;
    protected readonly IWorkflowMessageService _workflowMessageService;

    public NewsletterController(CaptchaSettings captchaSettings,
        ICustomerService customerService,
        ILanguageService languageService,
        ILocalizationService localizationService,
        INewsletterModelFactory newsletterModelFactory,
        INewsLetterSubscriptionService newsLetterSubscriptionService,
        IStoreContext storeContext,
        IWorkContext workContext,
        IWorkflowMessageService workflowMessageService)
    {
        _captchaSettings = captchaSettings;
        _customerService = customerService;
        _languageService = languageService;
        _localizationService = localizationService;
        _newsletterModelFactory = newsletterModelFactory;
        _newsLetterSubscriptionService = newsLetterSubscriptionService;
        _storeContext = storeContext;
        _workContext = workContext;
        _workflowMessageService = workflowMessageService;
    }

    //available even when a store is closed
    [CheckAccessClosedStore(ignore: true)]
    [HttpPost]
    [ValidateCaptcha]
    public virtual async Task<IActionResult> SubscribeNewsletter(string email, bool subscribe, bool captchaValid)
    {
        string result;
        var success = false;

        //validate CAPTCHA
        if (_captchaSettings.Enabled && _captchaSettings.ShowOnNewsletterPage && !captchaValid)
        {
            result = await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage");
            return Json(new { Success = success, Result = result, });
        }

        if (!CommonHelper.IsValidEmail(email))
        {
            result = await _localizationService.GetResourceAsync("Newsletter.Email.Wrong");
        }
        else
        {
            email = email.Trim();
            var store = await _storeContext.GetCurrentStoreAsync();
            var subscription = await _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreIdAsync(email, store.Id);
            var currentLanguage = await _workContext.GetWorkingLanguageAsync();
            if (subscription != null)
            {
                subscription.LanguageId = subscription.LanguageId == 0 ? currentLanguage.Id : subscription.LanguageId;
                if (subscribe)
                {
                    if (!subscription.Active)
                    {
                        await _workflowMessageService.SendNewsLetterSubscriptionActivationMessageAsync(subscription);
                    }
                    result = await _localizationService.GetResourceAsync("Newsletter.SubscribeEmailSent");
                }
                else
                {
                    if (subscription.Active)
                    {
                        await _workflowMessageService.SendNewsLetterSubscriptionDeactivationMessageAsync(subscription);
                    }
                    result = await _localizationService.GetResourceAsync("Newsletter.UnsubscribeEmailSent");
                }
            }
            else if (subscribe)
            {
                subscription = new NewsLetterSubscription
                {
                    NewsLetterSubscriptionGuid = Guid.NewGuid(),
                    Email = email,
                    Active = false,
                    StoreId = store.Id,
                    LanguageId = currentLanguage.Id,
                    CreatedOnUtc = DateTime.UtcNow
                };
                await _newsLetterSubscriptionService.InsertNewsLetterSubscriptionAsync(subscription);
                await _workflowMessageService.SendNewsLetterSubscriptionActivationMessageAsync(subscription);

                result = await _localizationService.GetResourceAsync("Newsletter.SubscribeEmailSent");
            }
            else
            {
                result = await _localizationService.GetResourceAsync("Newsletter.UnsubscribeEmailSent");
            }
            success = true;
        }

        return Json(new
        {
            Success = success,
            Result = result,
        });
    }

    //available even when a store is closed
    [CheckAccessClosedStore(ignore: true)]
    public virtual async Task<IActionResult> SubscriptionActivation(Guid token, bool active)
    {
        var subscription = await _newsLetterSubscriptionService.GetNewsLetterSubscriptionByGuidAsync(token);
        if (subscription == null)
            return RedirectToRoute("Homepage");

        if (active)
        {
            subscription.Active = true;
            await _newsLetterSubscriptionService.UpdateNewsLetterSubscriptionAsync(subscription);

            if (!await _customerService.IsRegisteredAsync(await _workContext.GetCurrentCustomerAsync()) &&
                await _languageService.GetLanguageByIdAsync(subscription.LanguageId) is Language language)
            {
                await _workContext.SetWorkingLanguageAsync(language);
            }
        }
        else
            await _newsLetterSubscriptionService.DeleteNewsLetterSubscriptionAsync(subscription);

        var model = await _newsletterModelFactory.PrepareSubscriptionActivationModelAsync(active);
        return View(model);
    }
}