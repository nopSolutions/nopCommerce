using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using Nop.Core.Domain;
using Nop.Core.Domain.Common;
using Nop.Web.Framework.Controllers;

namespace Nop.Admin.Controllers
{
    [AdminAuthorize]
    public class HomeController : BaseNopController
    {
        #region Fields
        private readonly StoreInformationSettings _storeInformationSettings;
        private readonly CommonSettings _commonSettings;

        #endregion

        #region Ctor

        public HomeController(StoreInformationSettings storeInformationSettings, CommonSettings commonSettings)
        {
            this._storeInformationSettings = storeInformationSettings;
            this._commonSettings = commonSettings;
        }

        #endregion

        #region Methods

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult NopCommerceNews()
        {
            try
            {
                string feedUrl = string.Format("http://www.nopCommerce.com/NewsRSS.aspx?Version={0}&Localhost={1}&HideAdvertisements={2}&StoreURL={3}", _storeInformationSettings.CurrentVersion, Request.Url.IsLoopback, _commonSettings.HideAdvertisementsOnAdminArea, _storeInformationSettings.StoreUrl);
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

        #endregion
    }
}
