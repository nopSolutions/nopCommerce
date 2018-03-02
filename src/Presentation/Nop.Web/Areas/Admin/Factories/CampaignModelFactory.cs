using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Messages;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Extensions;
using Nop.Web.Areas.Admin.Models.Messages;
using Nop.Web.Framework.Extensions;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the campaign model factory implementation
    /// </summary>
    public partial class CampaignModelFactory : ICampaignModelFactory
    {
        #region Fields

        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly ICampaignService _campaignService;
        private readonly ICustomerService _customerService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IEmailAccountService _emailAccountService;
        private readonly ILocalizationService _localizationService;
        private readonly IMessageTokenProvider _messageTokenProvider;
        private readonly IStoreService _storeService;

        #endregion

        #region Ctor

        public CampaignModelFactory(EmailAccountSettings emailAccountSettings,
            ICampaignService campaignService,
            ICustomerService customerService,
            IDateTimeHelper dateTimeHelper,
            IEmailAccountService emailAccountService,
            ILocalizationService localizationService,
            IMessageTokenProvider messageTokenProvider,
            IStoreService storeService)
        {
            this._emailAccountSettings = emailAccountSettings;
            this._campaignService = campaignService;
            this._customerService = customerService;
            this._dateTimeHelper = dateTimeHelper;
            this._emailAccountService = emailAccountService;
            this._localizationService = localizationService;
            this._messageTokenProvider = messageTokenProvider;
            this._storeService = storeService;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare available stores for the passed model
        /// </summary>
        /// <param name="model">Campaign model</param>
        /// <param name="campaign">Campaign</param>
        protected virtual void PrepareModelStores(CampaignModel model, Campaign campaign)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            //prepare available stores
            var availableStores = _storeService.GetAllStores();
            model.AvailableStores = availableStores
                .Select(store => new SelectListItem { Text = store.Name, Value = store.Id.ToString() }).ToList();

            //insert special store item for the "all" value
            model.AvailableStores
                .Insert(0, new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
        }

        /// <summary>
        /// Prepare available customer roles for the passed model
        /// </summary>
        /// <param name="model">Campaign model</param>
        /// <param name="campaign">Campaign</param>
        protected virtual void PrepareModelCustomerRoles(CampaignModel model, Campaign campaign)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            //prepare available customer roles
            var availableCustomerRoles = _customerService.GetAllCustomerRoles();
            model.AvailableCustomerRoles = availableCustomerRoles
                .Select(customerRole => new SelectListItem { Text = customerRole.Name, Value = customerRole.Id.ToString() }).ToList();

            //insert special customer role item for the "all" value
            model.AvailableCustomerRoles
                .Insert(0, new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
        }

        /// <summary>
        /// Prepare available email accounts for the passed model
        /// </summary>
        /// <param name="model">Campaign model</param>
        /// <param name="campaign">Campaign</param>
        protected virtual void PrepareModelEmailAccounts(CampaignModel model, Campaign campaign)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            //prepare available email accounts
            var availableEmailAccounts = _emailAccountService.GetAllEmailAccounts();
            model.AvailableEmailAccounts = availableEmailAccounts.Select(emailAccount => new SelectListItem
            {
                Value = emailAccount.Id.ToString(),
                Text = $"{emailAccount.DisplayName} ({emailAccount.Email})"
            }).ToList();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare campaign search model
        /// </summary>
        /// <param name="model">Campaign search model</param>
        /// <returns>Campaign search model</returns>
        public virtual CampaignSearchModel PrepareCampaignSearchModel(CampaignSearchModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            //prepare available stores
            var availableStores = _storeService.GetAllStores();
            model.AvailableStores = availableStores
                .Select(store => new SelectListItem { Text = store.Name, Value = store.Id.ToString() }).ToList();

            //insert special store item for the "all" value
            model.AvailableStores
                .Insert(0, new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

            return model;
        }

        /// <summary>
        /// Prepare paged campaign list model
        /// </summary>
        /// <param name="searchModel">Campaign search model</param>
        /// <returns>Campaign list model</returns>
        public virtual CampaignListModel PrepareCampaignListModel(CampaignSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));
            
            //get campaigns
            var campaigns = _campaignService.GetAllCampaigns(searchModel.StoreId);

            //prepare grid model
            var model = new CampaignListModel
            {
                Data = campaigns.PaginationByRequestModel(searchModel).Select(campaign =>
                {
                    //fill in model values from the entity
                    var campaignModel = campaign.ToModel();

                    //convert dates to the user time
                    campaignModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(campaign.CreatedOnUtc, DateTimeKind.Utc);
                    if (campaign.DontSendBeforeDateUtc.HasValue)
                    {
                        campaignModel.DontSendBeforeDate = _dateTimeHelper
                            .ConvertToUserTime(campaign.DontSendBeforeDateUtc.Value, DateTimeKind.Utc);
                    }

                    return campaignModel;
                }),
                Total = campaigns.Count
            };

            return model;
        }

        /// <summary>
        /// Prepare campaign model
        /// </summary>
        /// <param name="model">Campaign model</param>
        /// <param name="campaign">Campaign</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Campaign model</returns>
        public virtual CampaignModel PrepareCampaignModel(CampaignModel model, Campaign campaign, bool excludeProperties = false)
        {
            //fill in model values from the entity
            if (campaign != null)
            {
                model = model ?? campaign.ToModel();
                if (campaign.DontSendBeforeDateUtc.HasValue)
                    model.DontSendBeforeDate = _dateTimeHelper.ConvertToUserTime(campaign.DontSendBeforeDateUtc.Value, DateTimeKind.Utc);
            }

            model.AllowedTokens = string.Join(", ", _messageTokenProvider.GetListOfCampaignAllowedTokens());

            //whether to fill in some of properties
            if (!excludeProperties)
                model.EmailAccountId = _emailAccountSettings.DefaultEmailAccountId;

            //prepare model stores
            PrepareModelStores(model, campaign);

            //prepare model customer roles
            PrepareModelCustomerRoles(model, campaign);

            //prepare model email accounts
            PrepareModelEmailAccounts(model, campaign);

            return model;
        }

        #endregion
    }
}