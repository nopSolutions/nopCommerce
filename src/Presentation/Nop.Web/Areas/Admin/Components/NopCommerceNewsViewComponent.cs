using System.Linq;
using System.Net;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Common;
using Nop.Services.Configuration;
using Nop.Web.Areas.Admin.Infrastructure.Cache;
using Nop.Web.Areas.Admin.Models.Home;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Mvc.Rss;

namespace Nop.Web.Areas.Admin.Components
{
    public class NopCommerceNewsViewComponent : NopViewComponent
    {
        private readonly AdminAreaSettings _adminAreaSettings;
        private readonly IStoreContext _storeContext;
        private readonly IStaticCacheManager _cacheManager;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;

        public NopCommerceNewsViewComponent(IStoreContext storeContext,
            AdminAreaSettings adminAreaSettings,
            ISettingService settingService,
            IStaticCacheManager cacheManager,
            IWebHelper webHelper)
        {
            this._storeContext = storeContext;
            this._adminAreaSettings = adminAreaSettings;
            this._settingService = settingService;
            this._cacheManager = cacheManager;
            this._webHelper = webHelper;
        }

        public IViewComponentResult Invoke()
        {
            try
            {
                var feedUrl = $"http://www.nopCommerce.com/NewsRSS.aspx?Version={NopVersion.CurrentVersion}&Localhost={_webHelper.IsLocalRequest(Request)}&HideAdvertisements={_adminAreaSettings.HideAdvertisementsOnAdminArea}&StoreURL={_storeContext.CurrentStore.Url}"
                        .ToLowerInvariant();

                var rssData = _cacheManager.Get(ModelCacheEventConsumer.OFFICIAL_NEWS_MODEL_KEY, () =>
                {
                    //specify timeout (5 secs)
                    var request = WebRequest.Create(feedUrl);
                    request.Timeout = 5000;
                    using (var response = request.GetResponse())
                    using (var reader = XmlReader.Create(response.GetResponseStream()))
                    {
                        return RssFeed.Load(reader);
                    }
                });

                var model = new NopCommerceNewsModel
                {
                    HideAdvertisements = _adminAreaSettings.HideAdvertisementsOnAdminArea
                };

                for (var i = 0; i < rssData.Items.Count; i++)
                {
                    var item = rssData.Items.ElementAt(i);
                    var newsItem = new NopCommerceNewsModel.NewsDetailsModel
                    {
                        Title = item.TitleText,
                        Summary = item.ContentText,
                        Url = item.Url.OriginalString,
                        PublishDate = item.PublishDate
                    };
                    model.Items.Add(newsItem);

                    //has new items?
                    if (i == 0)
                    {
                        var firstRequest = string.IsNullOrEmpty(_adminAreaSettings.LastNewsTitleAdminArea);
                        if (_adminAreaSettings.LastNewsTitleAdminArea != newsItem.Title)
                        {
                            _adminAreaSettings.LastNewsTitleAdminArea = newsItem.Title;
                            _settingService.SaveSetting(_adminAreaSettings);

                            if (!firstRequest)
                            {
                                //new item
                                model.HasNewItems = true;
                            }
                        }
                    }
                }
                return View(model);
            }
            catch
            {
                return Content("");
            }
        }
    }
}
