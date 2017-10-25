using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Areas.Admin.Extensions;
using Nop.Web.Areas.Admin.Models.Messages;
using Nop.Core;
using Nop.Core.Domain.Messages;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
	public partial class CampaignController : BaseAdminController
	{
	    #region Fields

        private readonly ICampaignService _campaignService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IEmailAccountService _emailAccountService;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly ILocalizationService _localizationService;
        private readonly IMessageTokenProvider _messageTokenProvider;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly IPermissionService _permissionService;
	    private readonly ICustomerService _customerService;
        private readonly ICustomerActivityService _customerActivityService;

        #endregion

	    #region Ctor

        public CampaignController(ICampaignService campaignService,
            IDateTimeHelper dateTimeHelper, 
            IEmailAccountService emailAccountService,
            EmailAccountSettings emailAccountSettings,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            ILocalizationService localizationService, 
            IMessageTokenProvider messageTokenProvider,
            IStoreContext storeContext,
            IStoreService storeService,
            IPermissionService permissionService, 
            ICustomerService customerService,
            ICustomerActivityService customerActivityService)
		{
            this._campaignService = campaignService;
            this._dateTimeHelper = dateTimeHelper;
            this._emailAccountService = emailAccountService;
            this._emailAccountSettings = emailAccountSettings;
            this._newsLetterSubscriptionService = newsLetterSubscriptionService;
            this._localizationService = localizationService;
            this._messageTokenProvider = messageTokenProvider;
            this._storeContext = storeContext;
            this._storeService = storeService;
            this._permissionService = permissionService;
            this._customerService = customerService;
            this._customerActivityService = customerActivityService;
		}

        #endregion

	    #region Utilities

        protected virtual void PrepareStoresModel(CampaignModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            model.AvailableStores.Add(new SelectListItem
            {
                Text = _localizationService.GetResource("Admin.Common.All"),
                Value = "0"
            });
            var stores = _storeService.GetAllStores();
            foreach (var store in stores)
            {
                model.AvailableStores.Add(new SelectListItem
                {
                    Text = store.Name,
                    Value = store.Id.ToString()
                });
            }
        }
        
        protected virtual void PrepareCustomerRolesModel(CampaignModel model)
	    {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            model.AvailableCustomerRoles.Add(new SelectListItem
            {
                Text = _localizationService.GetResource("Admin.Common.All"),
                Value = "0"
            });
            var roles = _customerService.GetAllCustomerRoles();
            foreach (var customerRole in roles)
            {
                model.AvailableCustomerRoles.Add(new SelectListItem
                {
                    Text = customerRole.Name,
                    Value = customerRole.Id.ToString()
                });
            }
        }

        protected virtual void PrepareEmailAccountsModel(CampaignModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            model.AvailableEmailAccounts = _emailAccountService.GetAllEmailAccounts().Select(emailAccount => new SelectListItem
            {
                Value = emailAccount.Id.ToString(),
                Text = $"{emailAccount.DisplayName} ({emailAccount.Email})"
            }).ToList();
        }

        protected virtual EmailAccount GetEmailAccount(int emailAccountId)
        {
            var emailAccount = _emailAccountService.GetEmailAccountById(emailAccountId)
                ?? _emailAccountService.GetEmailAccountById(_emailAccountSettings.DefaultEmailAccountId);

            if (emailAccount == null)
                throw new NopException("Email account could not be loaded");

            return emailAccount;
        }

        #endregion

	    #region Methods

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

		public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCampaigns))
                return AccessDeniedView();

            var stores = _storeService.GetAllStores();
            var model = new CampaignListModel();

            model.AvailableStores.Add(new SelectListItem
            {
                Text = _localizationService.GetResource("Admin.Common.All"),
                Value = "0"
            });
            
            foreach (var store in stores)
            {
                model.AvailableStores.Add(new SelectListItem
                {
                    Text = store.Name,
                    Value = store.Id.ToString()
                });
            }

            return View(model);
		}

        [HttpPost]
        public virtual IActionResult List(DataSourceRequest command, CampaignListModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCampaigns))
                return AccessDeniedKendoGridJson();

            var campaigns = _campaignService.GetAllCampaigns(searchModel.StoreId);
            var gridModel = new DataSourceResult
            {
                Data = campaigns.Select(x =>
                {
                    var model = x.ToModel();
                    model.CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc);
                    if (x.DontSendBeforeDateUtc.HasValue)
                        model.DontSendBeforeDate = _dateTimeHelper.ConvertToUserTime(x.DontSendBeforeDateUtc.Value, DateTimeKind.Utc);
                    return model;
                }),
                Total = campaigns.Count
            };
            return Json(gridModel);
        }

        public virtual IActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCampaigns))
                return AccessDeniedView();

            var model = new CampaignModel
            {
                AllowedTokens = string.Join(", ", _messageTokenProvider.GetListOfCampaignAllowedTokens())
            };
            //stores
            PrepareStoresModel(model);
            //customer roles
            PrepareCustomerRolesModel(model);
            //email accounts
            PrepareEmailAccountsModel(model);
            model.EmailAccountId = _emailAccountSettings.DefaultEmailAccountId;

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(CampaignModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCampaigns))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var campaign = model.ToEntity();
                campaign.CreatedOnUtc = DateTime.UtcNow;
                campaign.DontSendBeforeDateUtc = model.DontSendBeforeDate.HasValue ?
                    (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.DontSendBeforeDate.Value) : null;
                _campaignService.InsertCampaign(campaign);

                //activity log
                _customerActivityService.InsertActivity("AddNewCampaign", _localizationService.GetResource("ActivityLog.AddNewCampaign"), campaign.Id);

                SuccessNotification(_localizationService.GetResource("Admin.Promotions.Campaigns.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = campaign.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            model.AllowedTokens = string.Join(", ", _messageTokenProvider.GetListOfCampaignAllowedTokens());
            //stores
            PrepareStoresModel(model);
            //customer roles
            PrepareCustomerRolesModel(model);
            //email accounts
            PrepareEmailAccountsModel(model);

            return View(model);
        }

		public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCampaigns))
                return AccessDeniedView();

            var campaign = _campaignService.GetCampaignById(id);
            if (campaign == null)
                //No campaign found with the specified id
                return RedirectToAction("List");

            var model = campaign.ToModel();
            if (campaign.DontSendBeforeDateUtc.HasValue)
                model.DontSendBeforeDate = _dateTimeHelper.ConvertToUserTime(campaign.DontSendBeforeDateUtc.Value, DateTimeKind.Utc);
            model.AllowedTokens = string.Join(", ", _messageTokenProvider.GetListOfCampaignAllowedTokens());
            //stores
            PrepareStoresModel(model);
            //customer roles
            PrepareCustomerRolesModel(model);
            //email accounts
            PrepareEmailAccountsModel(model);
            model.EmailAccountId = _emailAccountSettings.DefaultEmailAccountId;

            return View(model);
		}

        [HttpPost]
        [ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual IActionResult Edit(CampaignModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCampaigns))
                return AccessDeniedView();

            var campaign = _campaignService.GetCampaignById(model.Id);
            if (campaign == null)
                //No campaign found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                campaign = model.ToEntity(campaign);
                campaign.DontSendBeforeDateUtc = model.DontSendBeforeDate.HasValue ?
                    (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.DontSendBeforeDate.Value) : null;
                _campaignService.UpdateCampaign(campaign);

                //activity log
                _customerActivityService.InsertActivity("EditCampaign", _localizationService.GetResource("ActivityLog.EditCampaign"), campaign.Id);

                SuccessNotification(_localizationService.GetResource("Admin.Promotions.Campaigns.Updated"));
                return continueEditing ? RedirectToAction("Edit", new { id = campaign.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            model.AllowedTokens = string.Join(", ", _messageTokenProvider.GetListOfCampaignAllowedTokens());
            //stores
            PrepareStoresModel(model);
            //customer roles
            PrepareCustomerRolesModel(model);
            //email accounts
            PrepareEmailAccountsModel(model);

            return View(model);
		}

        [HttpPost,ActionName("Edit")]
        [FormValueRequired("send-test-email")]
        public virtual IActionResult SendTestEmail(CampaignModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCampaigns))
                return AccessDeniedView();

            var campaign = _campaignService.GetCampaignById(model.Id);
            if (campaign == null)
                //No campaign found with the specified id
                return RedirectToAction("List");
            
            model.AllowedTokens = string.Join(", ", _messageTokenProvider.GetListOfCampaignAllowedTokens());
            //stores
            PrepareStoresModel(model);
            //customer roles
            PrepareCustomerRolesModel(model);
            //email accounts
            PrepareEmailAccountsModel(model);

            if (!CommonHelper.IsValidEmail(model.TestEmail))
            {
                ErrorNotification(_localizationService.GetResource("Admin.Common.WrongEmail"), false);
                return View(model);
            }

            try
            {

                var emailAccount = GetEmailAccount(model.EmailAccountId);
                var subscription = _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreId(model.TestEmail, _storeContext.CurrentStore.Id);
                if (subscription != null)
                {
                    //there's a subscription. let's use it
                    var subscriptions = new List<NewsLetterSubscription>();
                    subscriptions.Add(subscription);
                    _campaignService.SendCampaign(campaign, emailAccount, subscriptions);
                }
                else
                {
                    //no subscription found
                    _campaignService.SendCampaign(campaign, emailAccount, model.TestEmail);
                }

                SuccessNotification(_localizationService.GetResource("Admin.Promotions.Campaigns.TestEmailSentToCustomers"), false);
                return View(model);
            }
            catch (Exception exc)
            {
                ErrorNotification(exc, false);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("send-mass-email")]
        public virtual IActionResult SendMassEmail(CampaignModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCampaigns))
                return AccessDeniedView();

            var campaign = _campaignService.GetCampaignById(model.Id);
            if (campaign == null)
                //No campaign found with the specified id
                return RedirectToAction("List");

            model.AllowedTokens = string.Join(", ", _messageTokenProvider.GetListOfCampaignAllowedTokens());
            //stores
            PrepareStoresModel(model);
            //customer roles
            PrepareCustomerRolesModel(model);
            //email accounts
            PrepareEmailAccountsModel(model);

            try
            {
                var emailAccount = GetEmailAccount(model.EmailAccountId);

                //subscribers of certain store?
                var store = _storeService.GetStoreById(campaign.StoreId);
                var storeId = store != null ? store.Id : 0;
                var subscriptions = _newsLetterSubscriptionService.GetAllNewsLetterSubscriptions(storeId: storeId, 
                    customerRoleId: model.CustomerRoleId,
                    isActive: true);
                var totalEmailsSent = _campaignService.SendCampaign(campaign, emailAccount, subscriptions);
                SuccessNotification(string.Format(_localizationService.GetResource("Admin.Promotions.Campaigns.MassEmailSentToCustomers"), totalEmailsSent), false);
                return View(model);
            }
            catch (Exception exc)
            {
                ErrorNotification(exc, false);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

		[HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCampaigns))
                return AccessDeniedView();

            var campaign = _campaignService.GetCampaignById(id);
            if (campaign == null)
                //No campaign found with the specified id
                return RedirectToAction("List");

            _campaignService.DeleteCampaign(campaign);

            //activity log
            _customerActivityService.InsertActivity("DeleteCampaign", _localizationService.GetResource("ActivityLog.DeleteCampaign"), campaign.Id);

            SuccessNotification(_localizationService.GetResource("Admin.Promotions.Campaigns.Deleted"));

			return RedirectToAction("List");
		}

	    #endregion
    }
}