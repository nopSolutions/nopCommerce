using System;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Messages;
using Nop.Services.Helpers;
using Nop.Services.Messages;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Messages;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the campaign model factory implementation
    /// </summary>
    public partial class CampaignModelFactory : ICampaignModelFactory
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly ICampaignService _campaignService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IMessageTokenProvider _messageTokenProvider;

        #endregion

        #region Ctor

        public CampaignModelFactory(CatalogSettings catalogSettings,
            EmailAccountSettings emailAccountSettings,
            IBaseAdminModelFactory baseAdminModelFactory,
            ICampaignService campaignService,
            IDateTimeHelper dateTimeHelper,
            IMessageTokenProvider messageTokenProvider)
        {
            _catalogSettings = catalogSettings;
            _emailAccountSettings = emailAccountSettings;
            _baseAdminModelFactory = baseAdminModelFactory;
            _campaignService = campaignService;
            _dateTimeHelper = dateTimeHelper;
            _messageTokenProvider = messageTokenProvider;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare campaign search model
        /// </summary>
        /// <param name="searchModel">Campaign search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the campaign search model
        /// </returns>
        public virtual async Task<CampaignSearchModel> PrepareCampaignSearchModelAsync(CampaignSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare available stores
            await _baseAdminModelFactory.PrepareStoresAsync(searchModel.AvailableStores);

            searchModel.HideStoresList = _catalogSettings.IgnoreStoreLimitations || searchModel.AvailableStores.SelectionIsNotPossible();

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged campaign list model
        /// </summary>
        /// <param name="searchModel">Campaign search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the campaign list model
        /// </returns>
        public virtual async Task<CampaignListModel> PrepareCampaignListModelAsync(CampaignSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get campaigns
            var campaigns = (await _campaignService.GetAllCampaignsAsync(searchModel.StoreId)).ToPagedList(searchModel);

            //prepare grid model
            var model = await new CampaignListModel().PrepareToGridAsync(searchModel, campaigns, () =>
            {
                return campaigns.SelectAwait(async campaign =>
                {
                    //fill in model values from the entity
                    var campaignModel = campaign.ToModel<CampaignModel>();

                    //convert dates to the user time
                    campaignModel.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(campaign.CreatedOnUtc, DateTimeKind.Utc);
                    if (campaign.DontSendBeforeDateUtc.HasValue)
                    {
                        campaignModel.DontSendBeforeDate = await _dateTimeHelper
                            .ConvertToUserTimeAsync(campaign.DontSendBeforeDateUtc.Value, DateTimeKind.Utc);
                    }

                    return campaignModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare campaign model
        /// </summary>
        /// <param name="model">Campaign model</param>
        /// <param name="campaign">Campaign</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the campaign model
        /// </returns>
        public virtual async Task<CampaignModel> PrepareCampaignModelAsync(CampaignModel model, Campaign campaign, bool excludeProperties = false)
        {
            //fill in model values from the entity
            if (campaign != null)
            {
                model ??= campaign.ToModel<CampaignModel>();
                if (campaign.DontSendBeforeDateUtc.HasValue)
                    model.DontSendBeforeDate = await _dateTimeHelper.ConvertToUserTimeAsync(campaign.DontSendBeforeDateUtc.Value, DateTimeKind.Utc);
            }

            model.AllowedTokens = string.Join(", ", await _messageTokenProvider.GetListOfCampaignAllowedTokensAsync());

            //whether to fill in some of properties
            if (!excludeProperties)
                model.EmailAccountId = _emailAccountSettings.DefaultEmailAccountId;

            //prepare available stores
            await _baseAdminModelFactory.PrepareStoresAsync(model.AvailableStores);

            //prepare available customer roles
            await _baseAdminModelFactory.PrepareCustomerRolesAsync(model.AvailableCustomerRoles);

            //prepare available email accounts
            await _baseAdminModelFactory.PrepareEmailAccountsAsync(model.AvailableEmailAccounts, false);

            return model;
        }

        #endregion
    }
}