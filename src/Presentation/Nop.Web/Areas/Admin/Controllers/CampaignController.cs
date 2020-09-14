using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Messages;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Messages;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class CampaignController : BaseAdminController
    {
        #region Fields

        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly ICampaignModelFactory _campaignModelFactory;
        private readonly ICampaignService _campaignService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IEmailAccountService _emailAccountService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly IPermissionService _permissionService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;

        #endregion

        #region Ctor

        public CampaignController(EmailAccountSettings emailAccountSettings,
            ICampaignModelFactory campaignModelFactory,
            ICampaignService campaignService,
            ICustomerActivityService customerActivityService,
            IDateTimeHelper dateTimeHelper,
            IEmailAccountService emailAccountService,
            ILocalizationService localizationService,
            INotificationService notificationService,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            IPermissionService permissionService,
            IStoreContext storeContext,
            IStoreService storeService)
        {
            _emailAccountSettings = emailAccountSettings;
            _campaignModelFactory = campaignModelFactory;
            _campaignService = campaignService;
            _customerActivityService = customerActivityService;
            _dateTimeHelper = dateTimeHelper;
            _emailAccountService = emailAccountService;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _newsLetterSubscriptionService = newsLetterSubscriptionService;
            _permissionService = permissionService;
            _storeContext = storeContext;
            _storeService = storeService;
        }

        #endregion

        #region Utilities

        protected virtual async Task<EmailAccount> GetEmailAccount(int emailAccountId)
        {
            return await _emailAccountService.GetEmailAccountById(emailAccountId)
                ?? await _emailAccountService.GetEmailAccountById(_emailAccountSettings.DefaultEmailAccountId)
                ?? throw new NopException("Email account could not be loaded");
        }

        #endregion

        #region Methods

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual async Task<IActionResult> List()
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageCampaigns))
                return AccessDeniedView();

            //prepare model
            var model = await _campaignModelFactory.PrepareCampaignSearchModel(new CampaignSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(CampaignSearchModel searchModel)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageCampaigns))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = await _campaignModelFactory.PrepareCampaignListModel(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageCampaigns))
                return AccessDeniedView();

            //prepare model
            var model = await _campaignModelFactory.PrepareCampaignModel(new CampaignModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Create(CampaignModel model, bool continueEditing)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageCampaigns))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var campaign = model.ToEntity<Campaign>();

                campaign.CreatedOnUtc = DateTime.UtcNow;
                campaign.DontSendBeforeDateUtc = model.DontSendBeforeDate.HasValue ?
                    (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.DontSendBeforeDate.Value) : null;

                await _campaignService.InsertCampaign(campaign);

                //activity log
                await _customerActivityService.InsertActivity("AddNewCampaign",
                    string.Format(await _localizationService.GetResource("ActivityLog.AddNewCampaign"), campaign.Id), campaign);

                _notificationService.SuccessNotification(await _localizationService.GetResource("Admin.Promotions.Campaigns.Added"));

                return continueEditing ? RedirectToAction("Edit", new { id = campaign.Id }) : RedirectToAction("List");
            }

            //prepare model
            model = await _campaignModelFactory.PrepareCampaignModel(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageCampaigns))
                return AccessDeniedView();

            //try to get a campaign with the specified id
            var campaign = await _campaignService.GetCampaignById(id);
            if (campaign == null)
                return RedirectToAction("List");

            //prepare model
            var model = await _campaignModelFactory.PrepareCampaignModel(null, campaign);

            return View(model);
        }

        [HttpPost]
        [ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual async Task<IActionResult> Edit(CampaignModel model, bool continueEditing)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageCampaigns))
                return AccessDeniedView();

            //try to get a campaign with the specified id
            var campaign = await _campaignService.GetCampaignById(model.Id);
            if (campaign == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                campaign = model.ToEntity(campaign);

                campaign.DontSendBeforeDateUtc = model.DontSendBeforeDate.HasValue ?
                    (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.DontSendBeforeDate.Value) : null;

                await _campaignService.UpdateCampaign(campaign);

                //activity log
                await _customerActivityService.InsertActivity("EditCampaign",
                    string.Format(await _localizationService.GetResource("ActivityLog.EditCampaign"), campaign.Id), campaign);

                _notificationService.SuccessNotification(await _localizationService.GetResource("Admin.Promotions.Campaigns.Updated"));

                return continueEditing ? RedirectToAction("Edit", new { id = campaign.Id }) : RedirectToAction("List");
            }

            //prepare model
            model = await _campaignModelFactory.PrepareCampaignModel(model, campaign, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("send-test-email")]
        public virtual async Task<IActionResult> SendTestEmail(CampaignModel model)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageCampaigns))
                return AccessDeniedView();

            //try to get a campaign with the specified id
            var campaign = await _campaignService.GetCampaignById(model.Id);
            if (campaign == null)
                return RedirectToAction("List");

            //prepare model
            model = await _campaignModelFactory.PrepareCampaignModel(model, campaign);

            //ensure that the entered email is valid
            if (!CommonHelper.IsValidEmail(model.TestEmail))
            {
                _notificationService.ErrorNotification(await _localizationService.GetResource("Admin.Common.WrongEmail"));
                return View(model);
            }

            try
            {
                var emailAccount = await GetEmailAccount(model.EmailAccountId);
                var subscription = await _newsLetterSubscriptionService
                    .GetNewsLetterSubscriptionByEmailAndStoreId(model.TestEmail, (await _storeContext.GetCurrentStore()).Id);
                if (subscription != null)
                {
                    //there's a subscription. let's use it
                    await _campaignService.SendCampaign(campaign, emailAccount, new List<NewsLetterSubscription> { subscription });
                }
                else
                {
                    //no subscription found
                    await _campaignService.SendCampaign(campaign, emailAccount, model.TestEmail);
                }

                _notificationService.SuccessNotification(await _localizationService.GetResource("Admin.Promotions.Campaigns.TestEmailSentToCustomers"));

                return View(model);
            }
            catch (Exception exc)
            {
                _notificationService.ErrorNotification(exc);
            }

            //prepare model
            model = await _campaignModelFactory.PrepareCampaignModel(model, campaign, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("send-mass-email")]
        public virtual async Task<IActionResult> SendMassEmail(CampaignModel model)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageCampaigns))
                return AccessDeniedView();

            //try to get a campaign with the specified id
            var campaign = await _campaignService.GetCampaignById(model.Id);
            if (campaign == null)
                return RedirectToAction("List");

            //prepare model
            model = await _campaignModelFactory.PrepareCampaignModel(model, campaign);

            try
            {
                var emailAccount = await GetEmailAccount(model.EmailAccountId);

                //subscribers of certain store?
                var storeId = (await _storeService.GetStoreById(campaign.StoreId))?.Id ?? 0;
                var subscriptions = await _newsLetterSubscriptionService.GetAllNewsLetterSubscriptions(storeId: storeId,
                    customerRoleId: model.CustomerRoleId,
                    isActive: true);
                var totalEmailsSent = await _campaignService.SendCampaign(campaign, emailAccount, subscriptions);

                _notificationService.SuccessNotification(string.Format(await _localizationService.GetResource("Admin.Promotions.Campaigns.MassEmailSentToCustomers"), totalEmailsSent));

                return View(model);
            }
            catch (Exception exc)
            {
                _notificationService.ErrorNotification(exc);
            }

            //prepare model
            model = await _campaignModelFactory.PrepareCampaignModel(model, campaign, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageCampaigns))
                return AccessDeniedView();

            //try to get a campaign with the specified id
            var campaign = await _campaignService.GetCampaignById(id);
            if (campaign == null)
                return RedirectToAction("List");

            await _campaignService.DeleteCampaign(campaign);

            //activity log
            await _customerActivityService.InsertActivity("DeleteCampaign",
                string.Format(await _localizationService.GetResource("ActivityLog.DeleteCampaign"), campaign.Id), campaign);

            _notificationService.SuccessNotification(await _localizationService.GetResource("Admin.Promotions.Campaigns.Deleted"));

            return RedirectToAction("List");
        }

        #endregion
    }
}