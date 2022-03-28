using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Messages;
using Nop.Plugin.Misc.FreshAddressNewsletterIntegration.Services;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Web.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.FreshAddressNewsletterIntegration.Controllers
{
    /* hooks into the Newsletter route to override functionality */
    public class CustomNewsletterController : BasePublicController
    {
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IStoreContext _storeContext;
        private readonly CustomerSettings _customerSettings;
        private readonly ILogger _logger;
        private readonly IFreshAddressService _freshAddressService;

        public CustomNewsletterController(ILocalizationService localizationService,
            IWorkContext workContext,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            IWorkflowMessageService workflowMessageService,
            IStoreContext storeContext,
            CustomerSettings customerSettings,
            ILogger logger,
            IFreshAddressService freshAddressService)
        {
            _localizationService = localizationService;
            _workContext = workContext;
            _newsLetterSubscriptionService = newsLetterSubscriptionService;
            _workflowMessageService = workflowMessageService;
            _storeContext = storeContext;
            _customerSettings = customerSettings;
            _logger = logger;
            _freshAddressService = freshAddressService;
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
                result = await _localizationService.GetResourceAsync("Newsletter.Email.Wrong");
            }
            else
            {
                email = email.Trim();

                var subscription = await _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreIdAsync(email, (await _storeContext.GetCurrentStoreAsync()).Id);
                if (subscription != null)
                {
                    if (subscribe)
                    {
                        if (!subscription.Active)
                        {
                            await _workflowMessageService.SendNewsLetterSubscriptionActivationMessageAsync(
                                subscription,
                                (await _workContext.GetWorkingLanguageAsync()).Id
                            );
                        }
                        result = await _localizationService.GetResourceAsync("Newsletter.SubscribeEmailSent");
                    }
                    else
                    {
                        if (subscription.Active)
                        {
                            await _workflowMessageService.SendNewsLetterSubscriptionDeactivationMessageAsync(
                                subscription,
                                (await _workContext.GetWorkingLanguageAsync()).Id
                            );
                        }
                        result = await _localizationService.GetResourceAsync("Newsletter.UnsubscribeEmailSent");
                    }
                }
                else if (subscribe)
                {
                    try
                    {
                        var freshAddressResponse = await _freshAddressService.ValidateEmailAsync(email);
                        if (!freshAddressResponse.IsValid)
                        {
                            return Json(new
                            {
                                Success = false,
                                Result = freshAddressResponse.ErrorResponse,
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        await _logger.ErrorAsync($"An error occured while trying to validate a Newsletter signup through FreshAddress: {ex}");
                    }

                    subscription = new NewsLetterSubscription
                    {
                        NewsLetterSubscriptionGuid = Guid.NewGuid(),
                        Email = email,
                        Active = false,
                        StoreId = (await _storeContext.GetCurrentStoreAsync()).Id,
                        CreatedOnUtc = DateTime.UtcNow
                    };
                    await _newsLetterSubscriptionService.InsertNewsLetterSubscriptionAsync(subscription);
                    await _workflowMessageService.SendNewsLetterSubscriptionActivationMessageAsync(subscription, (_workContext.GetWorkingLanguageAsync()).Id);

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
    }
}
