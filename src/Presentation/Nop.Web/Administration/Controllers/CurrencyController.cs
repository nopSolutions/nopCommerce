using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Admin;
using Nop.Admin.Models;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Web.Framework.Controllers;
using Nop.Services.Directory;
using Telerik.Web.Mvc;

namespace Nop.Admin.Controllers
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

        #region List

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            var currencies = _currencyService.GetAllCurrencies(true);
            var gridModel = new GridModel<CurrencyModel>
            {
                Data = currencies.Select(x => x.ToModel()),
                Total = currencies.Count()
            };
            return View(gridModel);
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

        #endregion

        #region Edit
        public ActionResult Edit(int id)
        {
            var currency = _currencyService.GetCurrencyById(id);
            if (currency == null) throw new ArgumentException("No currency found with the specified id", "id");
            var model = currency.ToModel();
            return View(model);
        }
        
        [HttpPost, FormValueExists("save", "save-continue", "continueEditing")]
        public ActionResult Edit(CurrencyModel currencyModel, bool continueEditing)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var currency = _currencyService.GetCurrencyById(currencyModel.Id);
            currency = currencyModel.ToEntity(currency);
            currency.UpdatedOnUtc = DateTime.UtcNow;
            _currencyService.UpdateCurrency(currency);
            return continueEditing ? RedirectToAction("Edit", new { id = currency.Id }) : RedirectToAction("List");
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

        [HttpPost, FormValueExists("save", "save-continue", "continueEditing")]
        public ActionResult Create(CurrencyModel model, bool continueEditing)
        {
            model.CreatedOnUtc = DateTime.UtcNow;
            model.UpdatedOnUtc = DateTime.UtcNow;
            var currency = model.ToEntity();
            _currencyService.InsertCurrency(currency);
            return continueEditing ? RedirectToAction("Edit", new { id = currency.Id }) : RedirectToAction("List");
        }

        #endregion

        #region Delete


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
