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

        protected CatalogSettings CatalogSettings { get; }
        protected EmailAccountSettings EmailAccountSettings { get; }
        protected IBaseAdminModelFactory BaseAdminModelFactory { get; }
        protected ICampaignService CampaignService { get; }
        protected IDateTimeHelper DateTimeHelper { get; }
        protected IMessageTokenProvider MessageTokenProvider { get; }

        #endregion

        #region Ctor

        public CampaignModelFactory(CatalogSettings catalogSettings,
            EmailAccountSettings emailAccountSettings,
            IBaseAdminModelFactory baseAdminModelFactory,
            ICampaignService campaignService,
            IDateTimeHelper dateTimeHelper,
            IMessageTokenProvider messageTokenProvider)
        {
            CatalogSettings = catalogSettings;
            EmailAccountSettings = emailAccountSettings;
            BaseAdminModelFactory = baseAdminModelFactory;
            CampaignService = campaignService;
            DateTimeHelper = dateTimeHelper;
            MessageTokenProvider = messageTokenProvider;
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
            await BaseAdminModelFactory.PrepareStoresAsync(searchModel.AvailableStores);

            searchModel.HideStoresList = CatalogSettings.IgnoreStoreLimitations || searchModel.AvailableStores.SelectionIsNotPossible();

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
            var campaigns = (await CampaignService.GetAllCampaignsAsync(searchModel.StoreId)).ToPagedList(searchModel);

            //prepare grid model
            var model = await new CampaignListModel().PrepareToGridAsync(searchModel, campaigns, () =>
            {
                return campaigns.SelectAwait(async campaign =>
                {
                    //fill in model values from the entity
                    var campaignModel = campaign.ToModel<CampaignModel>();

                    //convert dates to the user time
                    campaignModel.CreatedOn = await DateTimeHelper.ConvertToUserTimeAsync(campaign.CreatedOnUtc, DateTimeKind.Utc);
                    if (campaign.DontSendBeforeDateUtc.HasValue)
                    {
                        campaignModel.DontSendBeforeDate = await DateTimeHelper
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
                    model.DontSendBeforeDate = await DateTimeHelper.ConvertToUserTimeAsync(campaign.DontSendBeforeDateUtc.Value, DateTimeKind.Utc);
            }

            model.AllowedTokens = string.Join(", ", await MessageTokenProvider.GetListOfCampaignAllowedTokensAsync());

            //whether to fill in some of properties
            if (!excludeProperties)
                model.EmailAccountId = EmailAccountSettings.DefaultEmailAccountId;

            //prepare available stores
            await BaseAdminModelFactory.PrepareStoresAsync(model.AvailableStores);

            //prepare available customer roles
            await BaseAdminModelFactory.PrepareCustomerRolesAsync(model.AvailableCustomerRoles);

            //prepare available email accounts
            await BaseAdminModelFactory.PrepareEmailAccountsAsync(model.AvailableEmailAccounts, false);

            return model;
        }

        #endregion
    }
}