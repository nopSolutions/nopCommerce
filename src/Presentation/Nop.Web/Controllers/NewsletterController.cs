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
    #region Fields

    protected readonly CaptchaSettings _captchaSettings;
    protected readonly ICustomerService _customerService;
    protected readonly ILanguageService _languageService;
    protected readonly ILocalizationService _localizationService;
    protected readonly INewsLetterModelFactory _newsletterModelFactory;
    protected readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
    protected readonly INewsLetterSubscriptionTypeService _newsLetterSubscriptionTypeService;
    protected readonly IStoreContext _storeContext;
    protected readonly IWorkContext _workContext;
    protected readonly IWorkflowMessageService _workflowMessageService;

    #endregion

    #region Ctor

    public NewsletterController(CaptchaSettings captchaSettings,
        ICustomerService customerService,
        ILanguageService languageService,
        ILocalizationService localizationService,
        INewsLetterModelFactory newsletterModelFactory,
        INewsLetterSubscriptionService newsLetterSubscriptionService,
        INewsLetterSubscriptionTypeService newsLetterSubscriptionTypeService,
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
        _newsLetterSubscriptionTypeService = newsLetterSubscriptionTypeService;
        _storeContext = storeContext;
        _workContext = workContext;
        _workflowMessageService = workflowMessageService;
    }

    #endregion

    #region Methods

    //available even when a store is closed
    [CheckAccessClosedStore(ignore: true)]
    [CheckLanguageSeoCode(ignore: true)]
    [HttpPost]
    [ValidateCaptcha]
    public virtual async Task<IActionResult> SubscribeNewsletter(string email, bool subscribe, bool captchaValid)
    {
        //validate CAPTCHA
        if (_captchaSettings.Enabled && _captchaSettings.ShowOnNewsletterPage && !captchaValid)
            return Json(new { Success = false, Result = await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"), });

        //validate email
        if (!CommonHelper.IsValidEmail(email))
            return Json(new { Success = false, Result = await _localizationService.GetResourceAsync("Newsletter.Email.Wrong"), });

        //get current subscriptions
        email = email.Trim();
        var store = await _storeContext.GetCurrentStoreAsync();
        var currentSubscriptions = await _newsLetterSubscriptionService.GetNewsLetterSubscriptionsByEmailAsync(email, storeId: store.Id);

        //try to subscribe a customer for all 'ticked by default' subscriptions in the current store
        if (subscribe)
        {
            var subscriptionTypes = (await _newsLetterSubscriptionTypeService.GetAllNewsLetterSubscriptionTypesAsync(store.Id))
                .Where(type => type.TickedByDefault)
                .ToList();

            var language = await _workContext.GetWorkingLanguageAsync();

            //create new subscriptions
            var subscriptionGuid = currentSubscriptions.FirstOrDefault()?.NewsLetterSubscriptionGuid ?? Guid.NewGuid();
            var newSubscriptions = await subscriptionTypes.SelectAwait(async type =>
            {
                var existingSubscription = currentSubscriptions.FirstOrDefault(subscription => subscription.TypeId == type.Id);
                if (existingSubscription is not null)
                {
                    //update language if not set yet
                    if (existingSubscription.LanguageId == 0)
                    {
                        existingSubscription.LanguageId = language.Id;
                        await _newsLetterSubscriptionService.UpdateNewsLetterSubscriptionAsync(existingSubscription);
                    }

                    return null;
                }

                var newSubscription = new NewsLetterSubscription
                {
                    NewsLetterSubscriptionGuid = subscriptionGuid,
                    Email = email,
                    Active = false,
                    TypeId = type.Id,
                    StoreId = store.Id,
                    LanguageId = language.Id,
                    CreatedOnUtc = DateTime.UtcNow
                };
                await _newsLetterSubscriptionService.InsertNewsLetterSubscriptionAsync(newSubscription);

                return newSubscription;
            }).Where(subscription => subscription is not null).ToListAsync();

            //send an activation message
            if (newSubscriptions.FirstOrDefault() is NewsLetterSubscription subscription)
                await _workflowMessageService.SendNewsLetterSubscriptionActivationMessageAsync(subscription);
        }
        else
        {
            //send a deactivation message
            if (currentSubscriptions.FirstOrDefault(s => s.Active) is NewsLetterSubscription subscription)
                await _workflowMessageService.SendNewsLetterSubscriptionDeactivationMessageAsync(subscription);
        }

        return Json(new
        {
            Success = true,
            Result = subscribe
                ? await _localizationService.GetResourceAsync("Newsletter.SubscribeEmailSent")
                : await _localizationService.GetResourceAsync("Newsletter.UnsubscribeEmailSent")
        });
    }

    //available even when a store is closed
    [CheckAccessClosedStore(ignore: true)]
    public virtual async Task<IActionResult> SubscriptionActivation(Guid token, bool active)
    {
        var store = await _storeContext.GetCurrentStoreAsync();
        var subscriptions = (await _newsLetterSubscriptionService.GetNewsLetterSubscriptionsByGuidAsync(token))
            .Where(subscription => subscription.StoreId == store.Id)
            .ToList();
        if (!subscriptions.Any())
            return RedirectToRoute("Homepage");

        foreach (var subscription in subscriptions)
        {
            if (active)
            {
                //activate subscription if inactive
                if (!subscription.Active)
                {
                    subscription.Active = true;
                    await _newsLetterSubscriptionService.UpdateNewsLetterSubscriptionAsync(subscription);
                }

                //set language by subscription language (only for guests, as registered customers have the language set)
                var customer = await _workContext.GetCurrentCustomerAsync();
                if (!await _customerService.IsRegisteredAsync(customer) &&
                    await _languageService.GetLanguageByIdAsync(subscription.LanguageId) is Language language)
                {
                    await _workContext.SetWorkingLanguageAsync(language);
                }
            }
            else
            {
                //delete inactive subscription
                await _newsLetterSubscriptionService.DeleteNewsLetterSubscriptionAsync(subscription);
            }
        }

        var model = await _newsletterModelFactory.PrepareSubscriptionActivationModelAsync(active);

        return View(model);
    }

    #endregion
}