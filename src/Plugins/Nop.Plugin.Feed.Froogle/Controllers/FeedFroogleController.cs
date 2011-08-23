using System;
using System.IO;
using System.Net;
using System.Web.Mvc;
using Nop.Core;
using Nop.Plugin.Feed.Froogle.Models;
using Nop.Plugin.Feed.Froogle.Services;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.PromotionFeed;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Feed.Froogle.Controllers
{
    [AdminAuthorize]
    public class FeedFroogleController : Controller
    {
        private readonly IGoogleService _googleService;
        private readonly ICurrencyService _currencyService;
        private readonly ILocalizationService _localizationService;
        private readonly IPromotionFeedService _promotionFeedService;
        private readonly IWebHelper _webHelper;
        private readonly FroogleSettings _froogleSettings;
        private readonly ISettingService _settingService;

        public FeedFroogleController(IGoogleService googleService, 
            ICurrencyService currencyService,
            ILocalizationService localizationService, 
            IPromotionFeedService promotionFeedService, IWebHelper webHelper,
            FroogleSettings froogleSettings, ISettingService settingService)
        {
            this._googleService = googleService;
            this._currencyService = currencyService;
            this._localizationService = localizationService;
            this._promotionFeedService = promotionFeedService;
            this._webHelper = webHelper;
            this._froogleSettings = froogleSettings;
            this._settingService = settingService;
        }

        public ActionResult Configure()
        {
            var model = new FeedFroogleModel();
            //Picture
            model.ProductPictureSize = _froogleSettings.ProductPictureSize;
            //Currency
            model.CurrencyId = _froogleSettings.CurrencyId;
            foreach (var c in _currencyService.GetAllCurrencies(false))
            {
                model.AvailableCurrencies.Add(new SelectListItem()
                    {
                         Text = c.Name,
                         Value = c.Id.ToString()
                    });
            }
            //Google category
            model.DefaultGoogleCategory = _froogleSettings.DefaultGoogleCategory;
            model.AvailableGoogleCategories.Add(new SelectListItem()
            {
                Text = "Select a category",
                Value = ""
            });
            foreach (var gc in _googleService.GetTaxonomyList())
            {
                model.AvailableGoogleCategories.Add(new SelectListItem()
                {
                    Text = gc,
                    Value = gc
                });
            }
            //FTP settings
            model.FtpHostname = _froogleSettings.FtpHostname;
            model.FtpFilename = _froogleSettings.FtpFilename;
            model.FtpUsername = _froogleSettings.FtpUsername;
            model.FtpPassword = _froogleSettings.FtpPassword;

            return View("Nop.Plugin.Feed.Froogle.Views.FeedFroogle.Configure", model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public ActionResult Configure(FeedFroogleModel model)
        {
            if (!ModelState.IsValid)
            {
                return Configure();
            }
            
            //save settings
            _froogleSettings.ProductPictureSize = model.ProductPictureSize;
            _froogleSettings.CurrencyId = model.CurrencyId;
            _froogleSettings.DefaultGoogleCategory = model.DefaultGoogleCategory;
            _froogleSettings.FtpHostname = model.FtpHostname;
            _froogleSettings.FtpFilename = model.FtpFilename;
            _froogleSettings.FtpUsername = model.FtpUsername;
            _froogleSettings.FtpPassword = model.FtpPassword;
            _settingService.SaveSetting(_froogleSettings);

            //redisplay the form
            foreach (var c in _currencyService.GetAllCurrencies(false))
            {
                model.AvailableCurrencies.Add(new SelectListItem()
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                });
            }
            model.AvailableGoogleCategories.Add(new SelectListItem()
            {
                Text = "Select a category",
                Value = ""
            });
            foreach (var gc in _googleService.GetTaxonomyList())
            {
                model.AvailableGoogleCategories.Add(new SelectListItem()
                {
                    Text = gc,
                    Value = gc
                });
            }
            return View("Nop.Plugin.Feed.Froogle.Views.FeedFroogle.Configure", model);
        }

        [HttpPost, ActionName("Configure")]
        [FormValueRequired("generate")]
        public ActionResult GenerateFeed(FeedFroogleModel model)
        {
            if (!ModelState.IsValid)
            {
                return Configure();
            }


            try
            {
                string fileName = string.Format("froogle_{0}_{1}.xml", DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"), CommonHelper.GenerateRandomDigitCode(4));
                string filePath = string.Format("{0}content\\files\\exportimport\\{1}", Request.PhysicalApplicationPath, fileName);
                using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                {
                    var feed = _promotionFeedService.LoadPromotionFeedBySystemName("PromotionFeed.Froogle");
                    feed.GenerateFeed(fs);
                }

                string clickhereStr = string.Format("<a href=\"{0}content/files/exportimport/{1}\" target=\"_blank\">{2}</a>", _webHelper.GetStoreLocation(false), fileName, _localizationService.GetResource("Plugins.Feed.Froogle.ClickHere"));
                string result = string.Format(_localizationService.GetResource("Plugins.Feed.Froogle.SuccessResult"), clickhereStr);
                model.GenerateFeedResult = result;
            }
            catch (Exception exc)
            {
                model.GenerateFeedResult = exc.Message;
            }


            foreach (var c in _currencyService.GetAllCurrencies(false))
            {
                model.AvailableCurrencies.Add(new SelectListItem()
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                });
            }
            model.AvailableGoogleCategories.Add(new SelectListItem()
            {
                Text = "Select a category",
                Value = ""
            });
            foreach (var gc in _googleService.GetTaxonomyList())
            {
                model.AvailableGoogleCategories.Add(new SelectListItem()
                {
                    Text = gc,
                    Value = gc
                });
            }
            return View("Nop.Plugin.Feed.Froogle.Views.FeedFroogle.Configure", model);
        }

        [HttpPost, ActionName("Configure")]
        [FormValueRequired("uploadfeed")]
        public ActionResult UploadFeed(FeedFroogleModel model)
        {
            if (!ModelState.IsValid)
            {
                return Configure();
            }

            try
            {
                string uri = String.Format("{0}/{1}", _froogleSettings.FtpHostname, _froogleSettings.FtpFilename);
                FtpWebRequest req = WebRequest.Create(uri) as FtpWebRequest;
                req.Credentials = new NetworkCredential(_froogleSettings.FtpUsername, _froogleSettings.FtpPassword);
                req.KeepAlive = true;
                req.UseBinary = true;
                req.Method = WebRequestMethods.Ftp.UploadFile;

                using (Stream reqStream = req.GetRequestStream())
                {
                    var feed = _promotionFeedService.LoadPromotionFeedBySystemName("PromotionFeed.Froogle");
                    feed.GenerateFeed(reqStream);
                }

                var rsp = req.GetResponse() as FtpWebResponse;

                model.GenerateFeedResult = String.Format(_localizationService.GetResource("Plugins.Feed.Froogle.FtpUploadStatus"), rsp.StatusDescription);
            }
            catch (Exception exc)
            {
                model.GenerateFeedResult = exc.Message;
            }

            foreach (var c in _currencyService.GetAllCurrencies(false))
            {
                model.AvailableCurrencies.Add(new SelectListItem()
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                });
            }
            model.AvailableGoogleCategories.Add(new SelectListItem()
            {
                Text = "Select a category",
                Value = ""
            });
            foreach (var gc in _googleService.GetTaxonomyList())
            {
                model.AvailableGoogleCategories.Add(new SelectListItem()
                {
                    Text = gc,
                    Value = gc
                });
            }
            return View("Nop.Plugin.Feed.Froogle.Views.FeedFroogle.Configure", model);
        }
    }
}
