using System;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Common;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Logging;
using Nop.Web.Areas.Admin.Infrastructure.Cache;
using Nop.Web.Areas.Admin.Models.Home;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the home models factory implementation
    /// </summary>
    public partial class HomeModelFactory : IHomeModelFactory
    {
        #region Fields

        protected AdminAreaSettings AdminAreaSettings { get; }
        protected ICommonModelFactory CommonModelFactory { get; }
        protected ILogger Logger { get; }
        protected IOrderModelFactory OrderModelFactory { get; }
        protected ISettingService SettingService { get; }
        protected IStaticCacheManager StaticCacheManager { get; }
        protected IWorkContext WorkContext { get; }
        protected NopHttpClient NopHttpClient { get; }

        #endregion

        #region Ctor

        public HomeModelFactory(AdminAreaSettings adminAreaSettings,
            ICommonModelFactory commonModelFactory,
            ILogger logger,
            IOrderModelFactory orderModelFactory,
            ISettingService settingService,
            IStaticCacheManager staticCacheManager,
            IWorkContext workContext,
            NopHttpClient nopHttpClient)
        {
            AdminAreaSettings = adminAreaSettings;
            CommonModelFactory = commonModelFactory;
            Logger = logger;
            OrderModelFactory = orderModelFactory;
            SettingService = settingService;
            StaticCacheManager = staticCacheManager;
            WorkContext = workContext;
            NopHttpClient = nopHttpClient;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare dashboard model
        /// </summary>
        /// <param name="model">Dashboard model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the dashboard model
        /// </returns>
        public virtual async Task<DashboardModel> PrepareDashboardModelAsync(DashboardModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            model.IsLoggedInAsVendor = await WorkContext.GetCurrentVendorAsync() != null;

            //prepare nested search models
            await CommonModelFactory.PreparePopularSearchTermSearchModelAsync(model.PopularSearchTerms);
            await OrderModelFactory.PrepareBestsellerBriefSearchModelAsync(model.BestsellersByAmount);
            await OrderModelFactory.PrepareBestsellerBriefSearchModelAsync(model.BestsellersByQuantity);

            return model;
        }

        /// <summary>
        /// Prepare nopCommerce news model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the nopCommerce news model
        /// </returns>
        public virtual async Task<NopCommerceNewsModel> PrepareNopCommerceNewsModelAsync()
        {
            var model = new NopCommerceNewsModel
            {
                HideAdvertisements = AdminAreaSettings.HideAdvertisementsOnAdminArea
            };

            try
            {
                //try to get news RSS feed
                var rssData = await StaticCacheManager.GetAsync(StaticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.OfficialNewsModelKey), async () =>
                {
                    try
                    {
                        return await NopHttpClient.GetNewsRssAsync();
                    }
                    catch (AggregateException exception)
                    {
                        //rethrow actual excepion
                        throw exception.InnerException;
                    }
                });

                for (var i = 0; i < rssData.Items.Count; i++)
                {
                    var item = rssData.Items.ElementAt(i);
                    var newsItem = new NopCommerceNewsDetailsModel
                    {
                        Title = item.TitleText,
                        Summary = XmlHelper.XmlDecode(item.Content?.Value ?? string.Empty),
                        Url = item.Url.OriginalString,
                        PublishDate = item.PublishDate
                    };
                    model.Items.Add(newsItem);

                    //has new items?
                    if (i != 0)
                        continue;

                    var firstRequest = string.IsNullOrEmpty(AdminAreaSettings.LastNewsTitleAdminArea);
                    if (AdminAreaSettings.LastNewsTitleAdminArea == newsItem.Title)
                        continue;

                    AdminAreaSettings.LastNewsTitleAdminArea = newsItem.Title;
                    await SettingService.SaveSettingAsync(AdminAreaSettings);

                    //new item
                    if (!firstRequest)
                        model.HasNewItems = true;
                }
            }
            catch (Exception ex)
            {
                await Logger.ErrorAsync("No access to the news. Website www.nopcommerce.com is not available.", ex);
            }

            return model;
        }

        #endregion
    }
}