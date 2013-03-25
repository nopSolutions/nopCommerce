using System;
using System.Net;
using System.ServiceModel.Syndication;
using System.Web.Mvc;
using System.Xml;
using Nop.Admin.Models.Home;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Services.Configuration;
using Nop.Web.Framework.Controllers;

namespace Nop.Admin.Controllers
{
    [AdminAuthorize]
    public partial class HomeController : BaseNopController
    {
        #region Fields
        private readonly IStoreContext _storeContext;
        private readonly CommonSettings _commonSettings;
        private readonly ISettingService _settingService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public HomeController(IStoreContext storeContext, 
            CommonSettings commonSettings, 
            ISettingService settingService,
            IWorkContext workContext)
        {
            this._storeContext = storeContext;
            this._commonSettings = commonSettings;
            this._settingService = settingService;
            this._workContext = workContext;
        }

        #endregion

        #region Methods

        public ActionResult Index()
        {
            var model = new DashboardModel();
            model.IsLoggedInAsVendor = _workContext.CurrentVendor != null;
            return View(model);
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
                    _storeContext.CurrentStore.Url);

                //specify timeout (5 secs)
                var request = WebRequest.Create(feedUrl);
                request.Timeout = 5000;
                using (WebResponse response = request.GetResponse())
                using (var reader = XmlReader.Create(response.GetResponseStream()))
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
