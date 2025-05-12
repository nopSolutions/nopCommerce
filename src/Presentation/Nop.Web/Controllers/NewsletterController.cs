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
    protected readonly INewsLetterModelFactory _newsletterModelFactory;
    protected readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
    protected readonly INewsLetterSubscriptionTypeService _newsLetterSubscriptionTypeService;
    protected readonly IStoreContext _storeContext;
    protected readonly IWorkContext _workContext;
    protected readonly IWorkflowMessageService _workflowMessageService;

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

            var newsLetterSubscriptionTypes = (await _newsLetterSubscriptionTypeService.GetAllNewsLetterSubscriptionTypesAsync(store.Id))
                .Where(s => s.TickedByDefault);

            if (subscription != null)
            {
                var currentSubscriptions = await _newsLetterSubscriptionService.GetNewsLetterSubscriptionByGuidAsync(subscription.NewsLetterSubscriptionGuid);
                subscription.LanguageId = subscription.LanguageId == 0 ? currentLanguage.Id : subscription.LanguageId;
                if (subscribe)
                {
                    //delete Subscriptions
                    foreach (var currentSubscription in currentSubscriptions)
                    {
                        await _newsLetterSubscriptionService.DeleteNewsLetterSubscriptionAsync(currentSubscription);
                    }

                    var newsLetterSubscriptionGuid = new Guid();
                    var isSendActivation = false;
                    foreach (var newsLetterSubscriptionType in newsLetterSubscriptionTypes)
                    {
                        var newsLetterSubscription = new NewsLetterSubscription
                        {
                            NewsLetterSubscriptionGuid = newsLetterSubscriptionGuid,
                            Email = email,
                            Active = false,
                            TypeId = newsLetterSubscriptionType.Id,
                            StoreId = store.Id,
                            LanguageId = currentLanguage.Id,
                            CreatedOnUtc = DateTime.UtcNow
                        };
                        await _newsLetterSubscriptionService.InsertNewsLetterSubscriptionAsync(newsLetterSubscription);
                        
                        if (!isSendActivation)
                        {
                            await _workflowMessageService.SendNewsLetterSubscriptionActivationMessageAsync(newsLetterSubscription);
                            isSendActivation = true;
                        }
                    }
                    result = await _localizationService.GetResourceAsync("Newsletter.SubscribeEmailSent");
                }
                else
                {
                    if (currentSubscriptions.Where(s => s.Active).Any())
                    {
                        await _workflowMessageService.SendNewsLetterSubscriptionDeactivationMessageAsync(subscription);
                    }
                    result = await _localizationService.GetResourceAsync("Newsletter.UnsubscribeEmailSent");
                }
            }
            else if (subscribe)
            {
                var newsLetterSubscriptionGuid = Guid.NewGuid();
                foreach (var newsLetterSubscriptionType in newsLetterSubscriptionTypes)
                {
                    var subscriber = new NewsLetterSubscription
                    {
                        NewsLetterSubscriptionGuid = newsLetterSubscriptionGuid,
                        Email = email,
                        Active = false,
                        TypeId = newsLetterSubscriptionType.Id,
                        StoreId = store.Id,
                        LanguageId = currentLanguage.Id,
                        CreatedOnUtc = DateTime.UtcNow
                    };
                    await _newsLetterSubscriptionService.InsertNewsLetterSubscriptionAsync(subscriber);
                }

                var currentSubscription = await _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreIdAsync(email, store.Id);
                await _workflowMessageService.SendNewsLetterSubscriptionActivationMessageAsync(currentSubscription);

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
        var subscriptionList = await _newsLetterSubscriptionService.GetNewsLetterSubscriptionByGuidAsync(token);
        if (subscriptionList == null)
            return RedirectToRoute("Homepage");

        if (active)
        {
            foreach(var subscription in subscriptionList)
            {
                subscription.Active = true;
                await _newsLetterSubscriptionService.UpdateNewsLetterSubscriptionAsync(subscription);

                if (!await _customerService.IsRegisteredAsync(await _workContext.GetCurrentCustomerAsync()) &&
                await _languageService.GetLanguageByIdAsync(subscription.LanguageId) is Language language)
                {
                    await _workContext.SetWorkingLanguageAsync(language);
                }
            }
        }
        else
        {
            foreach(var subscription in subscriptionList)
            {
                await _newsLetterSubscriptionService.DeleteNewsLetterSubscriptionAsync(subscription);
            }
        }

        var model = await _newsletterModelFactory.PrepareSubscriptionActivationModelAsync(active);
        return View(model);
    }
}