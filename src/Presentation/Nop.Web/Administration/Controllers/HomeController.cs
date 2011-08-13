using System;
using System.ServiceModel.Syndication;
using System.Web.Mvc;
using System.Xml;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.Common;
using Nop.Services.Configuration;
using Nop.Web.Framework.Controllers;

namespace Nop.Admin.Controllers
{
    [AdminAuthorize]
    public class HomeController : BaseNopController
    {
        #region Fields
        private readonly StoreInformationSettings _storeInformationSettings;
        private readonly CommonSettings _commonSettings;
        private readonly ISettingService _settingService;

        #endregion

        #region Ctor

        public HomeController(StoreInformationSettings storeInformationSettings,
            CommonSettings commonSettings, ISettingService settingService)
        {
            this._storeInformationSettings = storeInformationSettings;
            this._commonSettings = commonSettings;
            this._settingService = settingService;
        }

        #endregion

        #region Methods

        public ActionResult Index()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult NopCommerceNews()
        {
            try
            {
                string feedUrl = string.Format("http://www.nopCommerce.com/NewsRSS.aspx?Version={0}&Localhost={1}&HideAdvertisements={2}&StoreURL={3}",
                    NopVersion.CurrentVersion, 
                    Request.Url.IsLoopback, 
                    _commonSettings.HideAdvertisementsOnAdminArea, 
                    _storeInformationSettings.StoreUrl);
                //TODO cache results (based on feed URL)
                using (var reader = XmlReader.Create(feedUrl))
                {
                    var rssData = SyndicationFeed.Load(reader);
                    return PartialView(rssData);
                }
            }
            catch (Exception)
            {
                return Content("");
            }
        }

        [HttpPost]
        public ActionResult NopCommerceNewsHideAdv()
        {
            _commonSettings.HideAdvertisementsOnAdminArea = !_commonSettings.HideAdvertisementsOnAdminArea;
            _settingService.SaveSetting(_commonSettings);
            return Content("Setting changed");
        }

        #endregion
    }
}
