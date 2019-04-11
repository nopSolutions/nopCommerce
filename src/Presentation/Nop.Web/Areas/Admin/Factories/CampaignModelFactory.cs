using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Messages;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Messages;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Models.DataTables;
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
        private readonly ILocalizationService _localizationService;
        private readonly IMessageTokenProvider _messageTokenProvider;

        #endregion

        #region Ctor

        public CampaignModelFactory(CatalogSettings catalogSettings,
            EmailAccountSettings emailAccountSettings,
            IBaseAdminModelFactory baseAdminModelFactory,
            ICampaignService campaignService,
            IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService,
            IMessageTokenProvider messageTokenProvider)
        {
            _catalogSettings = catalogSettings;
            _emailAccountSettings = emailAccountSettings;
            _baseAdminModelFactory = baseAdminModelFactory;
            _campaignService = campaignService;
            _dateTimeHelper = dateTimeHelper;
            _localizationService = localizationService;
            _messageTokenProvider = messageTokenProvider;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare datatables model
        /// </summary>
        /// <param name="searchModel">Search model</param>
        /// <returns>Datatables model</returns>
        protected virtual DataTablesModel PrepareCampaignGridModel(CampaignSearchModel searchModel)
        {
            //prepare common properties
            var model = new DataTablesModel
            {
                Name = "campaigns-grid",
                UrlRead = new DataUrl("List", "Campaign", null),
                SearchButtonId = "search-campaigns",
                Length = searchModel.PageSize,
                LengthMenu = searchModel.AvailablePageSizes
            };

            //prepare filters to search
            model.Filters = new List<FilterParameter>()
            {
                new FilterParameter(nameof(searchModel.StoreId))
            };

            //prepare model columns
            model.ColumnCollection = new List<ColumnProperty>
            {
                new ColumnProperty(nameof(CampaignModel.Name))
                {
                    Title = _localizationService.GetResource("Admin.Promotions.Campaigns.Fields.Name")
                },
                new ColumnProperty(nameof(CampaignModel.CreatedOn))
                {
                    Title = _localizationService.GetResource("Admin.Promotions.Campaigns.Fields.CreatedOn"),
                    Width = "200",
                    Render = new RenderDate()
                },
                new ColumnProperty(nameof(CampaignModel.DontSendBeforeDate))
                {
                    Title = _localizationService.GetResource("Admin.Promotions.Campaigns.Fields.DontSendBeforeDate"),
                    Width = "200",
                    Render = new RenderDate()
                },
                new ColumnProperty(nameof(CampaignModel.Id))
                {
                    Title = _localizationService.GetResource("Admin.Common.Edit"),
                    Width = "100",
                    ClassName =  StyleColumn.CenterAll,
                    Render = new RenderButtonEdit(new DataUrl("Edit"))
                }
            };

            return model;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare campaign search model
        /// </summary>
        /// <param name="searchModel">Campaign search model</param>
        /// <returns>Campaign search model</returns>
        public virtual CampaignSearchModel PrepareCampaignSearchModel(CampaignSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare available stores
            _baseAdminModelFactory.PrepareStores(searchModel.AvailableStores);

            searchModel.HideStoresList = _catalogSettings.IgnoreStoreLimitations || searchModel.AvailableStores.SelectionIsNotPossible();

            //prepare page parameters
            searchModel.SetGridPageSize();
            searchModel.Grid = PrepareCampaignGridModel(searchModel);

            return searchModel;
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
            var campaigns = _campaignService.GetAllCampaigns(searchModel.StoreId).ToPagedList(searchModel);

            //prepare grid model
            var model = new CampaignListModel().PrepareToGrid(searchModel, campaigns, () =>
            {
                return campaigns.Select(campaign =>
                {
                    //fill in model values from the entity
                    var campaignModel = campaign.ToModel<CampaignModel>();

                    //convert dates to the user time
                    campaignModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(campaign.CreatedOnUtc, DateTimeKind.Utc);
                    if (campaign.DontSendBeforeDateUtc.HasValue)
                    {
                        campaignModel.DontSendBeforeDate = _dateTimeHelper
                            .ConvertToUserTime(campaign.DontSendBeforeDateUtc.Value, DateTimeKind.Utc);
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
        /// <returns>Campaign model</returns>
        public virtual CampaignModel PrepareCampaignModel(CampaignModel model, Campaign campaign, bool excludeProperties = false)
        {
            //fill in model values from the entity
            if (campaign != null)
            {
                model = model ?? campaign.ToModel<CampaignModel>();
                if (campaign.DontSendBeforeDateUtc.HasValue)
                    model.DontSendBeforeDate = _dateTimeHelper.ConvertToUserTime(campaign.DontSendBeforeDateUtc.Value, DateTimeKind.Utc);
            }

            model.AllowedTokens = string.Join(", ", _messageTokenProvider.GetListOfCampaignAllowedTokens());

            //whether to fill in some of properties
            if (!excludeProperties)
                model.EmailAccountId = _emailAccountSettings.DefaultEmailAccountId;

            //prepare available stores
            _baseAdminModelFactory.PrepareStores(model.AvailableStores);

            //prepare available customer roles
            _baseAdminModelFactory.PrepareCustomerRoles(model.AvailableCustomerRoles);

            //prepare available email accounts
            _baseAdminModelFactory.PrepareEmailAccounts(model.AvailableEmailAccounts, false);

            return model;
        }

        #endregion
    }
}