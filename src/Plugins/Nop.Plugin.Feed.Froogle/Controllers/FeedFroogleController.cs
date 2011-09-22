using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Nop.Core;
using Nop.Plugin.Feed.Froogle.Domain;
using Nop.Plugin.Feed.Froogle.Models;
using Nop.Plugin.Feed.Froogle.Services;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.PromotionFeed;
using Nop.Services.Security;
using Nop.Web.Framework.Controllers;
using Telerik.Web.Mvc;

namespace Nop.Plugin.Feed.Froogle.Controllers
{
    [AdminAuthorize]
    public class FeedFroogleController : Controller
    {
        private readonly IGoogleService _googleService;
        private readonly IProductService _productService;
        private readonly ICurrencyService _currencyService;
        private readonly ILocalizationService _localizationService;
        private readonly IPromotionFeedService _promotionFeedService;
        private readonly ILogger _logger;
        private readonly IWebHelper _webHelper;
        private readonly FroogleSettings _froogleSettings;
        private readonly ISettingService _settingService;
        private readonly IPermissionService _permissionService;

        public FeedFroogleController(IGoogleService googleService, 
            IProductService productService, ICurrencyService currencyService,
            ILocalizationService localizationService, IPromotionFeedService promotionFeedService, 
            ILogger logger, IWebHelper webHelper,
            FroogleSettings froogleSettings, ISettingService settingService,
            IPermissionService permissionService)
        {
            this._googleService = googleService;
            this._productService = productService;
            this._currencyService = currencyService;
            this._localizationService = localizationService;
            this._promotionFeedService = promotionFeedService;
            this._logger = logger;
            this._webHelper = webHelper;
            this._froogleSettings = froogleSettings;
            this._settingService = settingService;
            this._permissionService = permissionService;
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







        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GoogleProductList(GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePromotionFeeds))
                throw new NopException("Not authorized");

            var productVariants = _productService.SearchProductVariants(command.Page - 1, command.PageSize, true);
            var productVariantsModel = productVariants
                .Select(x =>
                            {
                                var gModel = new FeedFroogleModel.GoogleProductModel()
                                {
                                    ProductVariantId = x.Id,
                                    FullProductVariantName = x.FullProductName
                                };
                                var googleProduct = _googleService.GetByProductVariantId(x.Id);
                                if (googleProduct != null)
                                    gModel.GoogleCategory = googleProduct.Taxonomy;

                                return gModel;
                            })
                .ToList();

            var model = new GridModel<FeedFroogleModel.GoogleProductModel>
            {
                Data = productVariantsModel,
                Total = productVariants.TotalCount
            };

            return new JsonResult
            {
                Data = model
            };
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult GoogleProductUpdate(GridCommand command, FeedFroogleModel.GoogleProductModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePromotionFeeds))
                throw new NopException("Not authorized");


            var googleProduct = _googleService.GetByProductVariantId(model.ProductVariantId);
            if (googleProduct != null)
            {
                //update
                googleProduct.Taxonomy = model.GoogleCategory;
                _googleService.UpdateGoogleProductRecord(googleProduct);
            }
            else
            {
                //insert
                googleProduct = new GoogleProductRecord()
                {
                    ProductVariantId = model.ProductVariantId,
                    Taxonomy = model.GoogleCategory
                };
                _googleService.InsertGoogleProductRecord(googleProduct);
            }
            
            return GoogleProductList(command);
        }
    }
}
