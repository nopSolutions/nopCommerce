using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Admin.Models;
using Nop.Admin.Models.Orders;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Tax;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Telerik.Web.Mvc;

namespace Nop.Admin.Controllers
{
    [AdminAuthorize]
    public class CheckoutAttributeController : BaseNopController
    {
        #region Fields

        private readonly ICheckoutAttributeService _checkoutAttributeService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly ILocalizationService _localizationService;
        private readonly ITaxCategoryService _taxCategoryService;
        private readonly IWorkContext _workContext;
        private readonly ICurrencyService _currencyService;
        private readonly CurrencySettings _currencySettings;
        private readonly IMeasureService _measureService;
        private readonly MeasureSettings _measureSettings;

        #endregion

        #region Constructors

        public CheckoutAttributeController(ICheckoutAttributeService checkoutAttributeService,
            ILanguageService languageService, ILocalizedEntityService localizedEntityService,
            ILocalizationService localizationService, ITaxCategoryService taxCategoryService,
            IWorkContext workContext, ICurrencyService currencyService, CurrencySettings currencySettings,
            IMeasureService measureService, MeasureSettings measureSettings)
        {
            this._checkoutAttributeService = checkoutAttributeService;
            this._languageService = languageService;
            this._localizedEntityService = localizedEntityService;
            this._localizationService = localizationService;
            this._taxCategoryService = taxCategoryService;
            this._workContext = workContext;
            this._currencyService = currencyService;
            this._currencySettings = currencySettings;
            this._measureService = measureService;
            this._measureSettings = measureSettings;
        }

        #endregion
        
        #region Utilities

        [NonAction]
        public void UpdateAttributeLocales(CheckoutAttribute checkoutAttribute, CheckoutAttributeModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(checkoutAttribute,
                                                               x => x.Name,
                                                               localized.Name,
                                                               localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(checkoutAttribute,
                                                               x => x.TextPrompt,
                                                               localized.TextPrompt,
                                                               localized.LanguageId);
            }
        }

        [NonAction]
        public void UpdateValueLocales(CheckoutAttributeValue checkoutAttributeValue, CheckoutAttributeValueModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(checkoutAttributeValue,
                                                               x => x.Name,
                                                               localized.Name,
                                                               localized.LanguageId);
            }
        }

        [NonAction]
        private void PrepareCheckoutAttributeModel(CheckoutAttributeModel model, CheckoutAttribute checkoutAttribute, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            //tax categories
            var taxCategories = _taxCategoryService.GetAllTaxCategories();
            model.AvailableTaxCategories.Add(new SelectListItem() { Text = "---", Value = "0" });
            foreach (var tc in taxCategories)
                model.AvailableTaxCategories.Add(new SelectListItem() { Text = tc.Name, Value = tc.Id.ToString(), Selected = checkoutAttribute != null && !excludeProperties && tc.Id == checkoutAttribute.TaxCategoryId });
        }

        #endregion
        
        #region Checkout attributes

        //list
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            var checkoutAttributes = _checkoutAttributeService.GetAllCheckoutAttributes(false);
            var gridModel = new GridModel<CheckoutAttributeModel>
            {
                Data = checkoutAttributes.Select(x => 
                {
                    var caModel = x.ToModel();
                    caModel.AttributeControlTypeName = x.AttributeControlType.GetLocalizedEnum(_localizationService, _workContext);
                    return caModel;
                }),
                Total = checkoutAttributes.Count()
            };
            return View(gridModel);
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult List(GridCommand command)
        {
            var checkoutAttributes = _checkoutAttributeService.GetAllCheckoutAttributes(false);
            var gridModel = new GridModel<CheckoutAttributeModel>
            {
                Data = checkoutAttributes.Select(x =>
                {
                    var caModel = x.ToModel();
                    caModel.AttributeControlTypeName = x.AttributeControlType.GetLocalizedEnum(_localizationService, _workContext);
                    return caModel;
                }),
                Total = checkoutAttributes.Count()
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }
        
        //create
        public ActionResult Create()
        {
            var model = new CheckoutAttributeModel();
            //locales
            AddLocales(_languageService, model.Locales);
            PrepareCheckoutAttributeModel(model, null, true);
            return View(model);
        }

        [HttpPost, FormValueExists("save", "save-continue", "continueEditing")]
        public ActionResult Create(CheckoutAttributeModel model, bool continueEditing)
        {
            if (ModelState.IsValid)
            {
                var checkoutAttribute = model.ToEntity();
                _checkoutAttributeService.InsertCheckoutAttribute(checkoutAttribute);
                UpdateAttributeLocales(checkoutAttribute, model);

                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Attributes.CheckoutAttributes.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = checkoutAttribute.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            PrepareCheckoutAttributeModel(model, null, true);
            return View(model);
        }

        //edit
        public ActionResult Edit(int id)
        {
            var checkoutAttribute = _checkoutAttributeService.GetCheckoutAttributeById(id);
            if (checkoutAttribute == null)
                throw new ArgumentException("No checkout attribute found with the specified id", "id");
            var model = checkoutAttribute.ToModel();
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = checkoutAttribute.GetLocalized(x => x.Name, languageId, false, false);
                locale.TextPrompt = checkoutAttribute.GetLocalized(x => x.TextPrompt, languageId, false, false);
            });
            PrepareCheckoutAttributeModel(model, checkoutAttribute, false);

            return View(model);
        }

        [HttpPost, FormValueExists("save", "save-continue", "continueEditing")]
        public ActionResult Edit(CheckoutAttributeModel model, bool continueEditing)
        {
            var checkoutAttribute = _checkoutAttributeService.GetCheckoutAttributeById(model.Id);
            if (checkoutAttribute == null)
                throw new ArgumentException("No checkout attribute found with the specified id");
            if (ModelState.IsValid)
            {
                checkoutAttribute = model.ToEntity(checkoutAttribute);
                _checkoutAttributeService.UpdateCheckoutAttribute(checkoutAttribute);

                UpdateAttributeLocales(checkoutAttribute, model);


                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Attributes.CheckoutAttributes.Updated"));
                return continueEditing ? RedirectToAction("Edit", checkoutAttribute.Id) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            PrepareCheckoutAttributeModel(model, checkoutAttribute, true);
            return View(model);
        }

        //delete
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var checkoutAttribute = _checkoutAttributeService.GetCheckoutAttributeById(id);
            _checkoutAttributeService.DeleteCheckoutAttribute(checkoutAttribute);

            SuccessNotification(_localizationService.GetResource("Admin.Catalog.Attributes.CheckoutAttributes.Deleted"));
            return RedirectToAction("List");
        }

        #endregion

        #region Checkout attribute values

        //list
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult ValueList(int checkoutAttributeId, GridCommand command)
        {
            var values = _checkoutAttributeService.GetCheckoutAttributeValues(checkoutAttributeId);
            var gridModel = new GridModel<CheckoutAttributeValueModel>
            {
                Data = values.Select(x => 
                    {
                        var model = x.ToModel();
                        //locales
                        //AddLocales(_languageService, model.Locales, (locale, languageId) =>
                        //{
                        //    locale.Name = x.GetLocalized(y => y.Name, languageId, false, false);
                        //});
                        return model;
                    }),
                Total = values.Count()
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        //create
        public ActionResult ValueCreatePopup(int checkoutAttributeId)
        {
            var model = new CheckoutAttributeValueModel();
            model.CheckoutAttributeId = checkoutAttributeId;
            model.PrimaryStoreCurrencyCode = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode;
            model.BaseWeightIn = _measureService.GetMeasureWeightById(_measureSettings.BaseWeightId).Name;

            //locales
            AddLocales(_languageService, model.Locales);
            return View(model);
        }

        [HttpPost]
        public ActionResult ValueCreatePopup(string btnId, CheckoutAttributeValueModel model)
        {
            var checkoutAttribute = _checkoutAttributeService.GetCheckoutAttributeById(model.CheckoutAttributeId);
            if (checkoutAttribute == null)
                throw new ArgumentException("No checkout attribute found with the specified id");

            model.PrimaryStoreCurrencyCode = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode;
            model.BaseWeightIn = _measureService.GetMeasureWeightById(_measureSettings.BaseWeightId).Name;

            if (ModelState.IsValid)
            {
                var sao = model.ToEntity();

                _checkoutAttributeService.InsertCheckoutAttributeValue(sao);
                UpdateValueLocales(sao, model);

                ViewBag.RefreshPage = true;
                ViewBag.btnId = btnId;
                return View(model);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        //edit
        public ActionResult ValueEditPopup(int id)
        {
            var cav = _checkoutAttributeService.GetCheckoutAttributeValueById(id);
            if (cav == null)
                throw new ArgumentException("No checkout attribute value found with the specified id", "id");
            var model = cav.ToModel();
            model.PrimaryStoreCurrencyCode = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode;
            model.BaseWeightIn = _measureService.GetMeasureWeightById(_measureSettings.BaseWeightId).Name;

            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = cav.GetLocalized(x => x.Name, languageId, false, false);
            });

            return View(model);
        }

        [HttpPost]
        public ActionResult ValueEditPopup(string btnId, CheckoutAttributeValueModel model)
        {
            var cav = _checkoutAttributeService.GetCheckoutAttributeValueById(model.Id);
            if (cav == null)
                throw new ArgumentException("No checkout attribute value found with the specified id");
            model.PrimaryStoreCurrencyCode = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode;
            model.BaseWeightIn = _measureService.GetMeasureWeightById(_measureSettings.BaseWeightId).Name;

            if (ModelState.IsValid)
            {
                cav = model.ToEntity(cav);
                _checkoutAttributeService.UpdateCheckoutAttributeValue(cav);

                UpdateValueLocales(cav, model);

                ViewBag.RefreshPage = true;
                ViewBag.btnId = btnId;
                return View(model);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        //delete
        [GridAction(EnableCustomBinding = true)]
        public ActionResult ValueDelete(int valueId, int checkoutAttributeId, GridCommand command)
        {
            var cav = _checkoutAttributeService.GetCheckoutAttributeValueById(valueId);
            _checkoutAttributeService.DeleteCheckoutAttributeValue(cav);

            return ValueList(checkoutAttributeId, command);
        }


        #endregion
    }
}
