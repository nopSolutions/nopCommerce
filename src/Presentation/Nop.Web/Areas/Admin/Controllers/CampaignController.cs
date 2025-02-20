﻿using Microsoft.AspNetCore.Mvc;
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

namespace Nop.Web.Areas.Admin.Controllers;

public partial class CampaignController : BaseAdminController
{
    #region Fields

    protected readonly EmailAccountSettings _emailAccountSettings;
    protected readonly ICampaignModelFactory _campaignModelFactory;
    protected readonly ICampaignService _campaignService;
    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly IEmailAccountService _emailAccountService;
    protected readonly ILocalizationService _localizationService;
    protected readonly INotificationService _notificationService;
    protected readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
    protected readonly IPermissionService _permissionService;
    protected readonly IStoreContext _storeContext;
    protected readonly IStoreService _storeService;
    protected readonly IWorkContext _workContext;

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
        IStoreService storeService,
        IWorkContext workContext)
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
        _workContext = workContext;
    }

    #endregion

    #region Utilities

    protected virtual async Task<EmailAccount> GetEmailAccountAsync(int emailAccountId)
    {
        return await _emailAccountService.GetEmailAccountByIdAsync(emailAccountId)
               ?? await _emailAccountService.GetEmailAccountByIdAsync(_emailAccountSettings.DefaultEmailAccountId)
               ?? throw new NopException("Email account could not be loaded");
    }

    #endregion

    #region Methods

    public virtual IActionResult Index()
    {
        return RedirectToAction("List");
    }

    [CheckPermission(StandardPermission.Promotions.CAMPAIGNS_VIEW)]
    public virtual async Task<IActionResult> List()
    {
        //prepare model
        var model = await _campaignModelFactory.PrepareCampaignSearchModelAsync(new CampaignSearchModel());

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Promotions.CAMPAIGNS_VIEW)]
    public virtual async Task<IActionResult> List(CampaignSearchModel searchModel)
    {
        //prepare model
        var model = await _campaignModelFactory.PrepareCampaignListModelAsync(searchModel);

        return Json(model);
    }

    [CheckPermission(StandardPermission.Promotions.CAMPAIGNS_CREATE_EDIT)]
    public virtual async Task<IActionResult> Create()
    {
        //prepare model
        var model = await _campaignModelFactory.PrepareCampaignModelAsync(new CampaignModel(), null);

        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Promotions.CAMPAIGNS_CREATE_EDIT)]
    public virtual async Task<IActionResult> Create(CampaignModel model, bool continueEditing)
    {
        if (ModelState.IsValid)
        {
            var campaign = model.ToEntity<Campaign>();

            campaign.CreatedOnUtc = DateTime.UtcNow;
            campaign.DontSendBeforeDateUtc = model.DontSendBeforeDate.HasValue ?
                (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.DontSendBeforeDate.Value) : null;

            await _campaignService.InsertCampaignAsync(campaign);

            //activity log
            await _customerActivityService.InsertActivityAsync("AddNewCampaign",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewCampaign"), campaign.Id), campaign);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Promotions.Campaigns.Added"));

            return continueEditing ? RedirectToAction("Edit", new { id = campaign.Id }) : RedirectToAction("List");
        }

        //prepare model
        model = await _campaignModelFactory.PrepareCampaignModelAsync(model, null, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [CheckPermission(StandardPermission.Promotions.CAMPAIGNS_VIEW)]
    public virtual async Task<IActionResult> Edit(int id)
    {
        //try to get a campaign with the specified id
        var campaign = await _campaignService.GetCampaignByIdAsync(id);
        if (campaign == null)
            return RedirectToAction("List");

        //prepare model
        var model = await _campaignModelFactory.PrepareCampaignModelAsync(null, campaign);

        return View(model);
    }

    [HttpPost]
    [ParameterBasedOnFormName("save-continue", "continueEditing")]
    [FormValueRequired("save", "save-continue")]
    [CheckPermission(StandardPermission.Promotions.CAMPAIGNS_CREATE_EDIT)]
    public virtual async Task<IActionResult> Edit(CampaignModel model, bool continueEditing)
    {
        //try to get a campaign with the specified id
        var campaign = await _campaignService.GetCampaignByIdAsync(model.Id);
        if (campaign == null)
            return RedirectToAction("List");

        if (ModelState.IsValid)
        {
            campaign = model.ToEntity(campaign);

            campaign.DontSendBeforeDateUtc = model.DontSendBeforeDate.HasValue ?
                (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.DontSendBeforeDate.Value) : null;

            await _campaignService.UpdateCampaignAsync(campaign);

            //activity log
            await _customerActivityService.InsertActivityAsync("EditCampaign",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditCampaign"), campaign.Id), campaign);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Promotions.Campaigns.Updated"));

            return continueEditing ? RedirectToAction("Edit", new { id = campaign.Id }) : RedirectToAction("List");
        }

        //prepare model
        model = await _campaignModelFactory.PrepareCampaignModelAsync(model, campaign, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [HttpPost, ActionName("Edit")]
    [FormValueRequired("send-test-email")]
    [CheckPermission(StandardPermission.Promotions.CAMPAIGNS_SEND_EMAILS)]
    public virtual async Task<IActionResult> SendTestEmail(CampaignModel model)
    {
        //try to get a campaign with the specified id
        var campaign = await _campaignService.GetCampaignByIdAsync(model.Id);
        if (campaign == null)
            return RedirectToAction("List");

        //prepare model
        model = await _campaignModelFactory.PrepareCampaignModelAsync(model, campaign);

        //ensure that the entered email is valid
        if (!CommonHelper.IsValidEmail(model.TestEmail))
        {
            _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Common.WrongEmail"));
            return View(model);
        }

        try
        {
            var emailAccount = await GetEmailAccountAsync(model.EmailAccountId);
            var store = await _storeContext.GetCurrentStoreAsync();
            var subscription = await _newsLetterSubscriptionService
                .GetNewsLetterSubscriptionByEmailAndStoreIdAsync(model.TestEmail, store.Id);
            if (subscription != null)
            {
                //there's a subscription. let's use it
                await _campaignService.SendCampaignAsync(campaign, emailAccount, new List<NewsLetterSubscription> { subscription });
            }
            else
            {
                var workingLanguage = await _workContext.GetWorkingLanguageAsync();

                //no subscription found
                await _campaignService.SendCampaignAsync(campaign, emailAccount, model.TestEmail, workingLanguage.Id);
            }

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Promotions.Campaigns.TestEmailSentToCustomers"));

            return RedirectToAction("Edit", new { id = campaign.Id });
        }
        catch (Exception exc)
        {
            await _notificationService.ErrorNotificationAsync(exc);
        }

        //prepare model
        model = await _campaignModelFactory.PrepareCampaignModelAsync(model, campaign, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [HttpPost, ActionName("Edit")]
    [FormValueRequired("send-mass-email")]
    [CheckPermission(StandardPermission.Promotions.CAMPAIGNS_SEND_EMAILS)]
    public virtual async Task<IActionResult> SendMassEmail(CampaignModel model)
    {
        //try to get a campaign with the specified id
        var campaign = await _campaignService.GetCampaignByIdAsync(model.Id);
        if (campaign == null)
            return RedirectToAction("List");

        //prepare model
        model = await _campaignModelFactory.PrepareCampaignModelAsync(model, campaign);

        try
        {
            var emailAccount = await GetEmailAccountAsync(model.EmailAccountId);

            //subscribers of certain store?
            var storeId = (await _storeService.GetStoreByIdAsync(campaign.StoreId))?.Id ?? 0;
            var subscriptions = await _newsLetterSubscriptionService.GetAllNewsLetterSubscriptionsAsync(storeId: storeId,
                customerRoleId: model.CustomerRoleId,
                isActive: true);
            var totalEmailsSent = await _campaignService.SendCampaignAsync(campaign, emailAccount, subscriptions);

            _notificationService.SuccessNotification(string.Format(await _localizationService.GetResourceAsync("Admin.Promotions.Campaigns.MassEmailSentToCustomers"), totalEmailsSent));

            return RedirectToAction("Edit", new { id = campaign.Id });
        }
        catch (Exception exc)
        {
            await _notificationService.ErrorNotificationAsync(exc);
        }

        //prepare model
        model = await _campaignModelFactory.PrepareCampaignModelAsync(model, campaign, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Promotions.CAMPAIGNS_DELETE)]
    public virtual async Task<IActionResult> Delete(int id)
    {
        //try to get a campaign with the specified id
        var campaign = await _campaignService.GetCampaignByIdAsync(id);
        if (campaign == null)
            return RedirectToAction("List");

        await _campaignService.DeleteCampaignAsync(campaign);

        //activity log
        await _customerActivityService.InsertActivityAsync("DeleteCampaign",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteCampaign"), campaign.Id), campaign);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Promotions.Campaigns.Deleted"));

        return RedirectToAction("List");
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Promotions.CAMPAIGNS_CREATE_EDIT)]
    public virtual async Task<IActionResult> CopyCampaign(CampaignModel model)
    {
        var copyModel = model.CopyCampaignModel;
        if (copyModel is null)
            return RedirectToAction("List");

        try
        {
            var originalCampaign = await _campaignService.GetCampaignByIdAsync(copyModel.OriginalCampaignId);

            if (originalCampaign is null)
                return RedirectToAction("List");

            var newCampaign = await _campaignService.CopyCampaignAsync(originalCampaign, copyModel.Name);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Promotions.Campaigns.Copied"));

            return RedirectToAction("Edit", new { id = newCampaign.Id });
        }
        catch (Exception exc)
        {
            _notificationService.ErrorNotification(exc.Message);
            return RedirectToAction("Edit", new { id = copyModel.OriginalCampaignId });
        }
    }

    #endregion
}