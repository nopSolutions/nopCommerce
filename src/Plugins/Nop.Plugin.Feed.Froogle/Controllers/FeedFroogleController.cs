using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Tasks;
using Nop.Core.Plugins;
using Nop.Plugin.Feed.Froogle.Domain;
using Nop.Plugin.Feed.Froogle.Models;
using Nop.Plugin.Feed.Froogle.Services;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Tasks;
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
        private readonly IPluginFinder _pluginFinder;
        private readonly ILogger _logger;
        private readonly IWebHelper _webHelper;
        private readonly IScheduleTaskService _scheduleTaskService;
        private readonly FroogleSettings _froogleSettings;
        private readonly ISettingService _settingService;

        public FeedFroogleController(IGoogleService googleService, 
            IProductService productService, ICurrencyService currencyService,
            ILocalizationService localizationService, IPluginFinder pluginFinder, 
            ILogger logger, IWebHelper webHelper, IScheduleTaskService scheduleTaskService, 
            FroogleSettings froogleSettings, ISettingService settingService)
        {
            this._googleService = googleService;
            this._productService = productService;
            this._currencyService = currencyService;
            this._localizationService = localizationService;
            this._pluginFinder = pluginFinder;
            this._logger = logger;
            this._webHelper = webHelper;
            this._scheduleTaskService = scheduleTaskService;
            this._froogleSettings = froogleSettings;
            this._settingService = settingService;
        }

        [NonAction]
        private ScheduleTask FindScheduledTask()
        {
            return _scheduleTaskService.GetTaskByType("Nop.Plugin.Feed.Froogle.StaticFileGenerationTask, Nop.Plugin.Feed.Froogle");
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

            //task
            ScheduleTask task = FindScheduledTask();
            if (task != null)
            {
                model.GenerateStaticFileEachMinutes = task.Seconds / 60;
                model.TaskEnabled = task.Enabled;
            }
            //file path
            if (System.IO.File.Exists(System.IO.Path.Combine(HttpRuntime.AppDomainAppPath, "content\\files\\exportimport", _froogleSettings.StaticFileName)))
                model.StaticFilePath = string.Format("{0}content/files/exportimport/{1}", _webHelper.GetStoreLocation(false), _froogleSettings.StaticFileName);

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

            string saveResult = "";
            //save settings
            _froogleSettings.ProductPictureSize = model.ProductPictureSize;
            _froogleSettings.CurrencyId = model.CurrencyId;
            _froogleSettings.DefaultGoogleCategory = model.DefaultGoogleCategory;
            _settingService.SaveSetting(_froogleSettings);

            // Update the task
            var task = FindScheduledTask();
            if (task != null)
            {
                task.Enabled = model.TaskEnabled;
                task.Seconds = model.GenerateStaticFileEachMinutes * 60;
                _scheduleTaskService.UpdateTask(task);
                saveResult = _localizationService.GetResource("Plugins.Feed.Froogle.TaskRestart");
            }

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
            //file path
            if (System.IO.File.Exists(System.IO.Path.Combine(HttpRuntime.AppDomainAppPath, "content\\files\\exportimport", _froogleSettings.StaticFileName)))
                model.StaticFilePath = string.Format("{0}content/files/exportimport/{1}", _webHelper.GetStoreLocation(false), _froogleSettings.StaticFileName);

            //set result text
            model.SaveResult = saveResult;
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
                var pluginDescriptor = _pluginFinder.GetPluginDescriptorBySystemName("PromotionFeed.Froogle");
                if (pluginDescriptor == null)
                    throw new Exception("Cannot load the plugin");

                //plugin
                var plugin = pluginDescriptor.Instance() as FroogleService;
                if (plugin == null)
                    throw new Exception("Cannot load the plugin");

                plugin.GenerateStaticFile();

                string clickhereStr = string.Format("<a href=\"{0}content/files/exportimport/{1}\" target=\"_blank\">{2}</a>", _webHelper.GetStoreLocation(false), _froogleSettings.StaticFileName, _localizationService.GetResource("Plugins.Feed.Froogle.ClickHere"));
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

            //task
            ScheduleTask task = FindScheduledTask();
            if (task != null)
            {
                model.GenerateStaticFileEachMinutes = task.Seconds / 60;
                model.TaskEnabled = task.Enabled;
            }

            //file path
            if (System.IO.File.Exists(System.IO.Path.Combine(HttpRuntime.AppDomainAppPath, "content\\files\\exportimport", _froogleSettings.StaticFileName)))
                model.StaticFilePath = string.Format("{0}content/files/exportimport/{1}", _webHelper.GetStoreLocation(false), _froogleSettings.StaticFileName);

            return View("Nop.Plugin.Feed.Froogle.Views.FeedFroogle.Configure", model);
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GoogleProductList(GridCommand command)
        {
            var productVariants = _productService.SearchProductVariants(0, 0, "", false,
                command.Page - 1, command.PageSize, true);
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
                                {
                                    gModel.GoogleCategory = googleProduct.Taxonomy;
                                    gModel.Gender = googleProduct.Gender;
                                    gModel.AgeGroup = googleProduct.AgeGroup;
                                    gModel.Color = googleProduct.Color;
                                    gModel.GoogleSize = googleProduct.Size;
                                }

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
            var googleProduct = _googleService.GetByProductVariantId(model.ProductVariantId);
            if (googleProduct != null)
            {

                googleProduct.Taxonomy = model.GoogleCategory;
                googleProduct.Gender = model.Gender;
                googleProduct.AgeGroup = model.AgeGroup;
                googleProduct.Color = model.Color;
                googleProduct.Size = model.GoogleSize;
                _googleService.UpdateGoogleProductRecord(googleProduct);
            }
            else
            {
                //insert
                googleProduct = new GoogleProductRecord()
                {
                    ProductVariantId = model.ProductVariantId,
                    Taxonomy = model.GoogleCategory,
                    Gender = model.Gender,
                    AgeGroup = model.AgeGroup,
                    Color = model.Color,
                    Size = model.GoogleSize
                };
                _googleService.InsertGoogleProductRecord(googleProduct);
            }
            
            return GoogleProductList(command);
        }
    }
}
