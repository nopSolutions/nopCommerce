using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Admin.Models;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Web.Framework.Controllers;
using Nop.Services.Directory;
using Nop.Web.MVC.Areas.Admin.Models;
using Nop.Web.MVC.Extensions;
using Telerik.Web.Mvc;

namespace Nop.Web.MVC.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class CurrencyController : Controller
    {
        private ICurrencyService _currencyService;
        private CurrencySettings _currencySettings;
        public CurrencyController(ICurrencyService currencyService, CurrencySettings currencySettings)
        {
            _currencyService = currencyService;
            _currencySettings = currencySettings;
        }

        #region Methods
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            var currencies = _currencyService.GetAllCurrencies(true);
            //var rates = _currencyService.GetCurrencyLiveRates(_currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode);
            var gridModel = new GridModel<CurrencyModel>
            {
                Data = currencies.Select(x => new CurrencyModel(x)),
                Total = currencies.Count()
            };
            return View(gridModel);
        }

        #endregion

        #region Edit
        public ActionResult Edit(int id)
        {
            var currency = _currencyService.GetCurrencyById(id);
            if (currency == null) throw new ArgumentException("No currency found with the specified id", "id");
            var model = currency.ToModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(CurrencyModel currencyModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var currency = _currencyService.GetCurrencyById(currencyModel.Id);
            currency = currencyModel.ToEntity(currency);
            currency.UpdatedOnUtc = DateTime.UtcNow;
            _currencyService.UpdateCurrency(currency);
            return Edit(currency.Id);
        }
        #endregion

        #region Create

        public ActionResult Create()
        {
            var currencyModel = new CurrencyModel();
            currencyModel.CreatedOnUtc = DateTime.UtcNow;
            currencyModel.UpdatedOnUtc = DateTime.UtcNow;
            return View(currencyModel);
        }

        [HttpPost]
        public ActionResult Create(CurrencyModel model)
        {
            model.CreatedOnUtc = DateTime.UtcNow;
            model.UpdatedOnUtc = DateTime.UtcNow;
            var currency = model.ToEntity();
            _currencyService.InsertCurrency(currency);
            return RedirectToAction("List");
        }

        #endregion

        #region Delete

        public ActionResult Delete(int id)
        {
            var currency = _currencyService.GetCurrencyById(id);
            if (currency == null)
            {
                return List();
            }
            var model = currency.ToModel();
            return Delete(model);
        }

        public ActionResult Delete(CurrencyModel model)
        {
            return PartialView(model);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var currency = _currencyService.GetCurrencyById(id);
            _currencyService.DeleteCurrency(currency);
            return RedirectToAction("List");
        }

        #endregion
    }
}
