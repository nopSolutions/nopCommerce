using System;
using System.IO;
using System.Web.Mvc;
using Nop.Core;
using Nop.Plugin.Feed.Become.Models;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.PromotionFeed;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Feed.Become.Controllers
{
    [AdminAuthorize]
    public class FeedBecomeController : Controller
    {
        private readonly ICurrencyService _currencyService;
        private readonly ILocalizationService _localizationService;
        private readonly IPromotionFeedService _promotionFeedService;
        private readonly ILogger _logger;
        private readonly IWebHelper _webHelper;
        private readonly BecomeSettings _becomeSettings;
        private readonly ISettingService _settingService;

        public FeedBecomeController(ICurrencyService currencyService,
            ILocalizationService localizationService, IPromotionFeedService promotionFeedService, 
            ILogger logger, IWebHelper webHelper,
            BecomeSettings becomeSettings, ISettingService settingService)
        {
            this._currencyService = currencyService;
            this._localizationService = localizationService;
            this._promotionFeedService = promotionFeedService;
            this._logger = logger;
            this._webHelper = webHelper;
            this._becomeSettings = becomeSettings;
            this._settingService = settingService;
        }

        public ActionResult Configure()
        {
            var model = new FeedBecomeModel();
            model.ProductPictureSize = _becomeSettings.ProductPictureSize;
            model.CurrencyId = _becomeSettings.CurrencyId;
            foreach (var c in _currencyService.GetAllCurrencies(false))
            {
                model.AvailableCurrencies.Add(new SelectListItem()
                    {
                         Text = c.Name,
                         Value = c.Id.ToString()
                    });
            }

            return View("Nop.Plugin.Feed.Become.Views.FeedBecome.Configure", model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public ActionResult Configure(FeedBecomeModel model)
        {
            if (!ModelState.IsValid)
            {
                return Configure();
            }
            
            //save settings
            _becomeSettings.ProductPictureSize = model.ProductPictureSize;
            _becomeSettings.CurrencyId = model.CurrencyId;
            _settingService.SaveSetting(_becomeSettings);

            //redisplay the form
            foreach (var c in _currencyService.GetAllCurrencies(false))
            {
                model.AvailableCurrencies.Add(new SelectListItem()
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                });
            }
            return View("Nop.Plugin.Feed.Become.Views.FeedBecome.Configure", model);
        }

        [HttpPost, ActionName("Configure")]
        [FormValueRequired("generate")]
        public ActionResult GenerateFeed(FeedBecomeModel model)
        {
            if (!ModelState.IsValid)
            {
                return Configure();
            }


            try
            {
                string fileName = string.Format("become_{0}_{1}.csv", DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"), CommonHelper.GenerateRandomDigitCode(4));
                string filePath = string.Format("{0}content\\files\\exportimport\\{1}", Request.PhysicalApplicationPath, fileName);
                using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                {
                    var feed = _promotionFeedService.LoadPromotionFeedBySystemName("PromotionFeed.Become");
                    feed.GenerateFeed(fs);
                }

                string clickhereStr = string.Format("<a href=\"{0}content/files/exportimport/{1}\" target=\"_blank\">{2}</a>", _webHelper.GetStoreLocation(false), fileName, _localizationService.GetResource("Plugins.Feed.Become.ClickHere"));
                string result = string.Format(_localizationService.GetResource("Plugins.Feed.Become.SuccessResult"), clickhereStr);
                model.GenerateFeedResult = result;
            }
            catch (Exception exc)
            {
                model.GenerateFeedResult = exc.Message;
                _logger.Error(exc.Message, exc);
            }


            foreach (var c in _currencyService.GetAllCurrencies(false))
            {
                model.AvailableCurrencies.Add(new SelectListItem()
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                });
            }
            return View("Nop.Plugin.Feed.Become.Views.FeedBecome.Configure", model);
        }
    }
}
