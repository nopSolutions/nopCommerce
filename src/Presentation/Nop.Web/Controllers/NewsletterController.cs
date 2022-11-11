using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Messages;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Web.Factories;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Controllers
{
    [AutoValidateAntiforgeryToken]
    public partial class NewsletterController : BasePublicController
    {
        private readonly ILocalizationService _localizationService;
        private readonly INewsletterModelFactory _newsletterModelFactory;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;
        private readonly IWorkflowMessageService _workflowMessageService;

        public NewsletterController(ILocalizationService localizationService,
            INewsletterModelFactory newsletterModelFactory,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            IStoreContext storeContext,
            IWorkContext workContext,
            IWorkflowMessageService workflowMessageService)
        {
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
        public virtual async Task<IActionResult> SubscribeNewsletter(string email, bool subscribe)
        {
            string result;
            var success = false;

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
                    if (subscribe)
                    {
                        if (!subscription.Active)
                        {
                            await _workflowMessageService.SendNewsLetterSubscriptionActivationMessageAsync(subscription, currentLanguage.Id);
                        }
                        result = await _localizationService.GetResourceAsync("Newsletter.SubscribeEmailSent");
                    }
                    else
                    {
                        if (subscription.Active)
                        {
                            await _workflowMessageService.SendNewsLetterSubscriptionDeactivationMessageAsync(subscription, currentLanguage.Id);
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
                        CreatedOnUtc = DateTime.UtcNow
                    };
                    await _newsLetterSubscriptionService.InsertNewsLetterSubscriptionAsync(subscription);
                    await _workflowMessageService.SendNewsLetterSubscriptionActivationMessageAsync(subscription, currentLanguage.Id);

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
            }
            else
                await _newsLetterSubscriptionService.DeleteNewsLetterSubscriptionAsync(subscription);

            var model = await _newsletterModelFactory.PrepareSubscriptionActivationModelAsync(active);
            return View(model);
        }
    }
}