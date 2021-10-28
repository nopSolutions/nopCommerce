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
    public partial class NewsletterController : BasePublicController
    {
        protected ILocalizationService LocalizationService { get; }
        protected INewsletterModelFactory NewsletterModelFactory { get; }
        protected INewsLetterSubscriptionService NewsLetterSubscriptionService { get; }
        protected IStoreContext StoreContext { get; }
        protected IWorkContext WorkContext { get; }
        protected IWorkflowMessageService WorkflowMessageService { get; }

        public NewsletterController(ILocalizationService localizationService,
            INewsletterModelFactory newsletterModelFactory,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            IStoreContext storeContext,
            IWorkContext workContext,
            IWorkflowMessageService workflowMessageService)
        {
            LocalizationService = localizationService;
            NewsletterModelFactory = newsletterModelFactory;
            NewsLetterSubscriptionService = newsLetterSubscriptionService;
            StoreContext = storeContext;
            WorkContext = workContext;
            WorkflowMessageService = workflowMessageService;
        }

        //available even when a store is closed
        [CheckAccessClosedStore(true)]
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public virtual async Task<IActionResult> SubscribeNewsletter(string email, bool subscribe)
        {
            string result;
            var success = false;

            if (!CommonHelper.IsValidEmail(email))
            {
                result = await LocalizationService.GetResourceAsync("Newsletter.Email.Wrong");
            }
            else
            {
                email = email.Trim();
                var store = await StoreContext.GetCurrentStoreAsync();
                var subscription = await NewsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreIdAsync(email, store.Id);
                var currentLanguage = await WorkContext.GetWorkingLanguageAsync();
                if (subscription != null)
                {
                    if (subscribe)
                    {
                        if (!subscription.Active)
                        {
                            await WorkflowMessageService.SendNewsLetterSubscriptionActivationMessageAsync(subscription, currentLanguage.Id);
                        }
                        result = await LocalizationService.GetResourceAsync("Newsletter.SubscribeEmailSent");
                    }
                    else
                    {
                        if (subscription.Active)
                        {
                            await WorkflowMessageService.SendNewsLetterSubscriptionDeactivationMessageAsync(subscription, currentLanguage.Id);
                        }
                        result = await LocalizationService.GetResourceAsync("Newsletter.UnsubscribeEmailSent");
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
                    await NewsLetterSubscriptionService.InsertNewsLetterSubscriptionAsync(subscription);
                    await WorkflowMessageService.SendNewsLetterSubscriptionActivationMessageAsync(subscription, currentLanguage.Id);

                    result = await LocalizationService.GetResourceAsync("Newsletter.SubscribeEmailSent");
                }
                else
                {
                    result = await LocalizationService.GetResourceAsync("Newsletter.UnsubscribeEmailSent");
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
        [CheckAccessClosedStore(true)]
        public virtual async Task<IActionResult> SubscriptionActivation(Guid token, bool active)
        {
            var subscription = await NewsLetterSubscriptionService.GetNewsLetterSubscriptionByGuidAsync(token);
            if (subscription == null)
                return RedirectToRoute("Homepage");

            if (active)
            {
                subscription.Active = true;
                await NewsLetterSubscriptionService.UpdateNewsLetterSubscriptionAsync(subscription);
            }
            else
                await NewsLetterSubscriptionService.DeleteNewsLetterSubscriptionAsync(subscription);

            var model = await NewsletterModelFactory.PrepareSubscriptionActivationModelAsync(active);
            return View(model);
        }
    }
}