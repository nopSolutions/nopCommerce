using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Admin;
using Nop.Admin.Models;
using Nop.Admin.Models.Directory;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Services.Configuration;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Web.Framework.Controllers;
using Nop.Services.Directory;
using Telerik.Web.Mvc;

namespace Nop.Admin.Controllers
{
    [AdminAuthorize]
    public class CurrencyController :  BaseNopController
    {
        #region Fields

        private readonly ICurrencyService _currencyService;
        private readonly CurrencySettings _currencySettings;
        private readonly ISettingService _settingService;
        private readonly IDateTimeHelper _dateTimeHelper; 
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Constructors

        public CurrencyController(ICurrencyService currencyService, 
            CurrencySettings currencySettings, ISettingService settingService,
            IDateTimeHelper dateTimeHelper, ILocalizationService localizationService)
        {
            this._currencyService = currencyService;
            this._currencySettings = currencySettings;
            this._settingService = settingService;
            this._dateTimeHelper = dateTimeHelper;
            this._localizationService = localizationService;
        }
        
        #endregion

        #region Methods
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List(bool liveRates = false)
        {
            var currenciesModel = _currencyService.GetAllCurrencies(true).Select(x => x.ToModel()).ToList();
            foreach (var currency in currenciesModel)
                currency.IsPrimaryExchangeRateCurrency = currency.Id == _currencySettings.PrimaryExchangeRateCurrencyId ? true : false;
            foreach (var currency in currenciesModel)
                currency.IsPrimaryStoreCurrency = currency.Id == _currencySettings.PrimaryStoreCurrencyId ? true : false;
            if (liveRates) 
                ViewBag.Rates = _currencyService.GetCurrencyLiveRates(_currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode);
            ViewBag.ExchangeRateProviders = new SelectList(_currencyService.LoadAllExchangeRateProviders(), "SystemName", "FriendlyName", _currencySettings.ActiveExchangeRateProviderSystemName); ;
            ViewBag.AutoUpdateEnabled = _currencySettings.AutoUpdateEnabled;
            var gridModel = new GridModel<CurrencyModel>
            {
                Data = currenciesModel,
                Total = currenciesModel.Count()
            };
            return View(gridModel);
        }

        public ActionResult ApplyRate(string currencyCode, decimal rate)
        {
            Currency currency = _currencyService.GetCurrencyByCode(currencyCode);
            if (currency != null)
            {
                currency.Rate = rate;
                currency.UpdatedOnUtc = DateTime.UtcNow;
                _currencyService.UpdateCurrency(currency);
            }
            return RedirectToAction("List","Currency", new { liveRates=true });
        }

        [HttpPost]
        public ActionResult Save(FormCollection formValues)
        {
            _currencySettings.ActiveExchangeRateProviderSystemName = formValues["exchangeRateProvider"];
            _currencySettings.AutoUpdateEnabled = formValues["autoUpdateEnabled"].Equals("false")?false:true;
            _settingService.SaveSetting(_currencySettings);
            return RedirectToAction("List","Currency");
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult List(GridCommand command)
        {
            var currencies = _currencyService.GetAllCurrencies(true);
            var gridModel = new GridModel<CurrencyModel>
            {
                Data = currencies.Select(x => x.ToModel()),
                Total = currencies.Count()
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }


        public ActionResult MarkAsPrimaryExchangeRateCurrency(int id)
        {
            _currencySettings.PrimaryExchangeRateCurrencyId = id;
            _settingService.SaveSetting(_currencySettings);
            return RedirectToAction("List");
        }

        public ActionResult MarkAsPrimaryStoreCurrency(int id)
        {
            _currencySettings.PrimaryStoreCurrencyId = id;
            _settingService.SaveSetting(_currencySettings);
            return RedirectToAction("List");
        }

        #endregion

        #region Create / Edit / Delete

        public ActionResult Create()
        {
            var model = new CurrencyModel();
            //default values
            model.Published = true;
            model.Rate = 1;
            return View(model);
        }

        [HttpPost, FormValueExists("save", "save-continue", "continueEditing")]
        public ActionResult Create(CurrencyModel model, bool continueEditing)
        {
            if (ModelState.IsValid)
            {
                var currency = model.ToEntity();
                currency.CreatedOnUtc = DateTime.UtcNow;
                currency.UpdatedOnUtc = DateTime.UtcNow;
                _currencyService.InsertCurrency(currency);

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Currencies.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = currency.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }
        
        public ActionResult Edit(int id)
        {
            var currency = _currencyService.GetCurrencyById(id);
            if (currency == null) 
                throw new ArgumentException("No currency found with the specified id", "id");
            var model = currency.ToModel();
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(currency.CreatedOnUtc, DateTimeKind.Utc);
            return View(model);
        }

        [HttpPost, FormValueExists("save", "save-continue", "continueEditing")]
        public ActionResult Edit(CurrencyModel model, bool continueEditing)
        {
            var currency = _currencyService.GetCurrencyById(model.Id);
            if (currency == null)
                throw new ArgumentException("No currency found with the specified id");

            if (ModelState.IsValid)
            {
                currency = model.ToEntity(currency);
                currency.UpdatedOnUtc = DateTime.UtcNow;
                _currencyService.UpdateCurrency(currency);

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Currencies.Updated"));
                return continueEditing ? RedirectToAction("Edit", new { id = currency.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(currency.CreatedOnUtc, DateTimeKind.Utc);
            return View(model);
        }
        
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var currency = _currencyService.GetCurrencyById(id);
            _currencyService.DeleteCurrency(currency);

            SuccessNotification(_localizationService.GetResource("Admin.Configuration.Currencies.Deleted"));
            return RedirectToAction("List");
        }

        #endregion
    }
}
