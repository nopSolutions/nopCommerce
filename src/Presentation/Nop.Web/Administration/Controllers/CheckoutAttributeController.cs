using System;
using System.Linq;
using System.Web.Mvc;
using Nop.Admin.Extensions;
using Nop.Admin.Models.Orders;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Controllers
{
    public partial class CheckoutAttributeController : BaseAdminController
    {
        #region Fields

        private readonly ICheckoutAttributeService _checkoutAttributeService;
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly ILocalizationService _localizationService;
        private readonly ITaxCategoryService _taxCategoryService;
        private readonly IWorkContext _workContext;
        private readonly ICurrencyService _currencyService;
        private readonly CurrencySettings _currencySettings;
        private readonly IMeasureService _measureService;
        private readonly MeasureSettings _measureSettings;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IPermissionService _permissionService;
        private readonly IStoreService _storeService;
        private readonly IStoreMappingService _storeMappingService;

        #endregion

        #region Constructors

        public CheckoutAttributeController(ICheckoutAttributeService checkoutAttributeService,
            ICheckoutAttributeParser checkoutAttributeParser,
            ILanguageService languageService, 
            ILocalizedEntityService localizedEntityService,
            ILocalizationService localizationService,
            ITaxCategoryService taxCategoryService,
            IWorkContext workContext, 
            ICurrencyService currencyService, 
            ICustomerActivityService customerActivityService, 
            CurrencySettings currencySettings,
            IMeasureService measureService, 
            MeasureSettings measureSettings,
            IPermissionService permissionService,
            IStoreService storeService,
            IStoreMappingService storeMappingService)
        {
            this._checkoutAttributeService = checkoutAttributeService;
            this._checkoutAttributeParser = checkoutAttributeParser;
            this._languageService = languageService;
            this._localizedEntityService = localizedEntityService;
            this._localizationService = localizationService;
            this._taxCategoryService = taxCategoryService;
            this._workContext = workContext;
            this._currencyService = currencyService;
            this._customerActivityService = customerActivityService;
            this._currencySettings = currencySettings;
            this._measureService = measureService;
            this._measureSettings = measureSettings;
            this._permissionService = permissionService;
            this._storeService = storeService;
            this._storeMappingService = storeMappingService;
        }

        #endregion
        
        #region Utilities

        [NonAction]
        protected virtual void UpdateAttributeLocales(CheckoutAttribute checkoutAttribute, CheckoutAttributeModel model)
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
        protected virtual void UpdateValueLocales(CheckoutAttributeValue checkoutAttributeValue, CheckoutAttributeValueModel model)
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
        protected virtual void PrepareTaxCategories(CheckoutAttributeModel model, CheckoutAttribute checkoutAttribute, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            //tax categories
            var taxCategories = _taxCategoryService.GetAllTaxCategories();
            model.AvailableTaxCategories.Add(new SelectListItem { Text = "---", Value = "0" });
            foreach (var tc in taxCategories)
                model.AvailableTaxCategories.Add(new SelectListItem { Text = tc.Name, Value = tc.Id.ToString(), Selected = checkoutAttribute != null && !excludeProperties && tc.Id == checkoutAttribute.TaxCategoryId });
        }

        [NonAction]
        protected virtual void PrepareStoresMappingModel(CheckoutAttributeModel model, CheckoutAttribute checkoutAttribute, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (!excludeProperties && checkoutAttribute != null)
                model.SelectedStoreIds = _storeMappingService.GetStoresIdsWithAccess(checkoutAttribute).ToList();

            var allStores = _storeService.GetAllStores();
            foreach (var store in allStores)
            {
                model.AvailableStores.Add(new SelectListItem
                {
                    Text = store.Name,
                    Value = store.Id.ToString(),
                    Selected = model.SelectedStoreIds.Contains(store.Id)
                });
            }
        }

        [NonAction]
        protected virtual void SaveStoreMappings(CheckoutAttribute checkoutAttribute, CheckoutAttributeModel model)
        {
            checkoutAttribute.LimitedToStores = model.SelectedStoreIds.Any();

            var existingStoreMappings = _storeMappingService.GetStoreMappings(checkoutAttribute);
            var allStores = _storeService.GetAllStores();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds.Contains(store.Id))
                {
                    //new store
                    if (existingStoreMappings.Count(sm => sm.StoreId == store.Id) == 0)
                        _storeMappingService.InsertStoreMapping(checkoutAttribute, store.Id);
                }
                else
                {
                    //remove store
                    var storeMappingToDelete = existingStoreMappings.FirstOrDefault(sm => sm.StoreId == store.Id);
                    if (storeMappingToDelete != null)
                        _storeMappingService.DeleteStoreMapping(storeMappingToDelete);
                }
            }
        }

        [NonAction]
        protected virtual void PrepareConditionAttributes(CheckoutAttributeModel model, CheckoutAttribute checkoutAttribute)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            //currenty any checkout attribute can have condition.
            model.ConditionAllowed = true;

            if (checkoutAttribute == null)
                return;

            var selectedAttribute = _checkoutAttributeParser.ParseCheckoutAttributes(checkoutAttribute.ConditionAttributeXml).FirstOrDefault();
            var selectedValues = _checkoutAttributeParser.ParseCheckoutAttributeValues(checkoutAttribute.ConditionAttributeXml);

            model.ConditionModel = new ConditionModel()
            {
                EnableCondition = !string.IsNullOrEmpty(checkoutAttribute.ConditionAttributeXml),
                SelectedAttributeId = selectedAttribute != null ? selectedAttribute.Id : 0,
                ConditionAttributes = _checkoutAttributeService.GetAllCheckoutAttributes()
                    //ignore this attribute and non-combinable attributes
                    .Where(x => x.Id != checkoutAttribute.Id && x.CanBeUsedAsCondition())
                    .Select(x =>
                        new AttributeConditionModel()
                        {
                            Id = x.Id,
                            Name = x.Name,
                            AttributeControlType = x.AttributeControlType,
                            Values = _checkoutAttributeService.GetCheckoutAttributeValues(x.Id)
                                .Select(v => new SelectListItem() { Text = v.Name, Value = v.Id.ToString(),
                                    Selected = selectedAttribute != null && selectedAttribute.Id == x.Id && selectedValues.Any(sv => sv.Id == v.Id) }).ToList()
                        }).ToList()
            };
        }

        [NonAction]
        protected virtual void SaveConditionAttributes(CheckoutAttribute checkoutAttribute, CheckoutAttributeModel model)
        {
            string attributesXml = null;
            if (model.ConditionModel.EnableCondition)
            {
                var attribute = _checkoutAttributeService.GetCheckoutAttributeById(model.ConditionModel.SelectedAttributeId);
                if (attribute != null)
                {
                    switch (attribute.AttributeControlType)
                    {
                        case AttributeControlType.DropdownList:
                        case AttributeControlType.RadioList:
                        case AttributeControlType.ColorSquares:
                        case AttributeControlType.ImageSquares:
                            {
                                var selectedAttribute = model.ConditionModel.ConditionAttributes
                                    .FirstOrDefault(x => x.Id == model.ConditionModel.SelectedAttributeId);
                                var selectedValue = selectedAttribute != null ? selectedAttribute.SelectedValueId : null;
                                if (!String.IsNullOrEmpty(selectedValue))
                                    attributesXml = _checkoutAttributeParser.AddCheckoutAttribute(attributesXml, attribute, selectedValue);
                                else
                                    //for conditions we should empty values save even when nothing is selected
                                    //otherwise "attributesXml" will be empty
                                    //hence we won't be able to find a selected attribute
                                    attributesXml = _checkoutAttributeParser.AddCheckoutAttribute(attributesXml, attribute, string.Empty);
                            }
                            break;
                        case AttributeControlType.Checkboxes:
                            {
                                var selectedAttribute = model.ConditionModel.ConditionAttributes
                                    .FirstOrDefault(x => x.Id == model.ConditionModel.SelectedAttributeId);
                                var selectedValues = selectedAttribute != null ? selectedAttribute.Values.Where(x => x.Selected).Select(x => x.Value) : null;
                                if (selectedValues.Any())
                                    foreach (var value in selectedValues)
                                        attributesXml = _checkoutAttributeParser.AddCheckoutAttribute(attributesXml, attribute, value);
                                else
                                    attributesXml = _checkoutAttributeParser.AddCheckoutAttribute(attributesXml, attribute, string.Empty);
                            }
                            break;
                        case AttributeControlType.ReadonlyCheckboxes:
                        case AttributeControlType.TextBox:
                        case AttributeControlType.MultilineTextbox:
                        case AttributeControlType.Datepicker:
                        case AttributeControlType.FileUpload:
                        default:
                            //these attribute types are not supported as conditions
                            break;
                    }
                }
            }
            checkoutAttribute.ConditionAttributeXml = attributesXml;
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
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public ActionResult List(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var checkoutAttributes = _checkoutAttributeService.GetAllCheckoutAttributes();
            var gridModel = new DataSourceResult
            {
                Data = checkoutAttributes.Select(x =>
                {
                    var attributeModel = x.ToModel();
                    attributeModel.AttributeControlTypeName = x.AttributeControlType.GetLocalizedEnum(_localizationService, _workContext);
                    return attributeModel;
                }),
                Total = checkoutAttributes.Count()
            };
            return Json(gridModel);
        }
        
        //create
        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var model = new CheckoutAttributeModel();
            //locales
            AddLocales(_languageService, model.Locales);
            //tax categories
            PrepareTaxCategories(model, null, true);
            //Stores
            PrepareStoresMappingModel(model, null, false);
            //condition
            PrepareConditionAttributes(model, null);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Create(CheckoutAttributeModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var checkoutAttribute = model.ToEntity();
                _checkoutAttributeService.InsertCheckoutAttribute(checkoutAttribute);
                //locales
                UpdateAttributeLocales(checkoutAttribute, model);
                //Stores
                SaveStoreMappings(checkoutAttribute, model);

                //activity log
                _customerActivityService.InsertActivity("AddNewCheckoutAttribute", _localizationService.GetResource("ActivityLog.AddNewCheckoutAttribute"), checkoutAttribute.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Attributes.CheckoutAttributes.Added"));

                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { id = checkoutAttribute.Id });
                }
                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form

            //tax categories
            PrepareTaxCategories(model, null, true);
            //Stores
            PrepareStoresMappingModel(model, null, true);
            return View(model);
        }

        //edit
        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var checkoutAttribute = _checkoutAttributeService.GetCheckoutAttributeById(id);
            if (checkoutAttribute == null)
                //No checkout attribute found with the specified id
                return RedirectToAction("List");

            var model = checkoutAttribute.ToModel();
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = checkoutAttribute.GetLocalized(x => x.Name, languageId, false, false);
                locale.TextPrompt = checkoutAttribute.GetLocalized(x => x.TextPrompt, languageId, false, false);
            });
            //tax categories
            PrepareTaxCategories(model, checkoutAttribute, false);
            //Stores
            PrepareStoresMappingModel(model, checkoutAttribute, false);
            //condition
            PrepareConditionAttributes(model, checkoutAttribute);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Edit(CheckoutAttributeModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var checkoutAttribute = _checkoutAttributeService.GetCheckoutAttributeById(model.Id);
            if (checkoutAttribute == null)
                //No checkout attribute found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                checkoutAttribute = model.ToEntity(checkoutAttribute);
                SaveConditionAttributes(checkoutAttribute, model);
                _checkoutAttributeService.UpdateCheckoutAttribute(checkoutAttribute);
                //locales
                UpdateAttributeLocales(checkoutAttribute, model);
                //Stores
                SaveStoreMappings(checkoutAttribute, model);

                //activity log
                _customerActivityService.InsertActivity("EditCheckoutAttribute", _localizationService.GetResource("ActivityLog.EditCheckoutAttribute"), checkoutAttribute.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Attributes.CheckoutAttributes.Updated"));
                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new {id = checkoutAttribute.Id});
                }
                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form

            //tax categories
            PrepareTaxCategories(model, checkoutAttribute, true);
            //Stores
            PrepareStoresMappingModel(model, checkoutAttribute, true);

            return View(model);
        }

        //delete
        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var checkoutAttribute = _checkoutAttributeService.GetCheckoutAttributeById(id);
            _checkoutAttributeService.DeleteCheckoutAttribute(checkoutAttribute);

            //activity log
            _customerActivityService.InsertActivity("DeleteCheckoutAttribute", _localizationService.GetResource("ActivityLog.DeleteCheckoutAttribute"), checkoutAttribute.Name);

            SuccessNotification(_localizationService.GetResource("Admin.Catalog.Attributes.CheckoutAttributes.Deleted"));
            return RedirectToAction("List");
        }

        #endregion

        #region Checkout attribute values

        //list
        [HttpPost]
        public ActionResult ValueList(int checkoutAttributeId, DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var values = _checkoutAttributeService.GetCheckoutAttributeValues(checkoutAttributeId);
            var gridModel = new DataSourceResult
            {
                Data = values.Select(x => new CheckoutAttributeValueModel
                {
                    Id = x.Id,
                    CheckoutAttributeId = x.CheckoutAttributeId,
                    Name = x.CheckoutAttribute.AttributeControlType != AttributeControlType.ColorSquares ? x.Name : string.Format("{0} - {1}", x.Name, x.ColorSquaresRgb),
                    ColorSquaresRgb = x.ColorSquaresRgb,
                    PriceAdjustment = x.PriceAdjustment,
                    WeightAdjustment = x.WeightAdjustment,
                    IsPreSelected = x.IsPreSelected,
                    DisplayOrder = x.DisplayOrder,
                }),
                Total = values.Count()
            };
            return Json(gridModel);
        }

        //create
        public ActionResult ValueCreatePopup(int checkoutAttributeId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var checkoutAttribute = _checkoutAttributeService.GetCheckoutAttributeById(checkoutAttributeId);
            var model = new CheckoutAttributeValueModel();
            model.CheckoutAttributeId = checkoutAttributeId;
            model.PrimaryStoreCurrencyCode = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode;
            model.BaseWeightIn = _measureService.GetMeasureWeightById(_measureSettings.BaseWeightId).Name;

            //color squares
            model.DisplayColorSquaresRgb = checkoutAttribute.AttributeControlType == AttributeControlType.ColorSquares;
            model.ColorSquaresRgb = "#000000";

            //locales
            AddLocales(_languageService, model.Locales);
            return View(model);
        }

        [HttpPost]
        public ActionResult ValueCreatePopup(string btnId, string formId, CheckoutAttributeValueModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var checkoutAttribute = _checkoutAttributeService.GetCheckoutAttributeById(model.CheckoutAttributeId);
            if (checkoutAttribute == null)
                //No checkout attribute found with the specified id
                return RedirectToAction("List");

            model.PrimaryStoreCurrencyCode = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode;
            model.BaseWeightIn = _measureService.GetMeasureWeightById(_measureSettings.BaseWeightId).Name;

            if (checkoutAttribute.AttributeControlType == AttributeControlType.ColorSquares)
            {
                //ensure valid color is chosen/entered
                if (String.IsNullOrEmpty(model.ColorSquaresRgb))
                    ModelState.AddModelError("", "Color is required");
                try
                {
                    //ensure color is valid (can be instanciated)
                    System.Drawing.ColorTranslator.FromHtml(model.ColorSquaresRgb);
                }
                catch (Exception exc)
                {
                    ModelState.AddModelError("", exc.Message);
                }
            }

            if (ModelState.IsValid)
            {
                var cav = new CheckoutAttributeValue
                {
                    CheckoutAttributeId = model.CheckoutAttributeId,
                    Name = model.Name,
                    ColorSquaresRgb = model.ColorSquaresRgb,
                    PriceAdjustment = model.PriceAdjustment,
                    WeightAdjustment = model.WeightAdjustment,
                    IsPreSelected = model.IsPreSelected,
                    DisplayOrder = model.DisplayOrder
                };

                _checkoutAttributeService.InsertCheckoutAttributeValue(cav);
                UpdateValueLocales(cav, model);

                ViewBag.RefreshPage = true;
                ViewBag.btnId = btnId;
                ViewBag.formId = formId;
                return View(model);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        //edit
        public ActionResult ValueEditPopup(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var cav = _checkoutAttributeService.GetCheckoutAttributeValueById(id);
            if (cav == null)
                //No checkout attribute value found with the specified id
                return RedirectToAction("List");

            var model = new CheckoutAttributeValueModel
            {
                CheckoutAttributeId = cav.CheckoutAttributeId,
                Name = cav.Name,
                ColorSquaresRgb = cav.ColorSquaresRgb,
                DisplayColorSquaresRgb = cav.CheckoutAttribute.AttributeControlType == AttributeControlType.ColorSquares,
                PriceAdjustment = cav.PriceAdjustment,
                WeightAdjustment = cav.WeightAdjustment,
                IsPreSelected = cav.IsPreSelected,
                DisplayOrder = cav.DisplayOrder,
                PrimaryStoreCurrencyCode = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode,
                BaseWeightIn = _measureService.GetMeasureWeightById(_measureSettings.BaseWeightId).Name
            };

            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = cav.GetLocalized(x => x.Name, languageId, false, false);
            });

            return View(model);
        }

        [HttpPost]
        public ActionResult ValueEditPopup(string btnId, string formId, CheckoutAttributeValueModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var cav = _checkoutAttributeService.GetCheckoutAttributeValueById(model.Id);
            if (cav == null)
                //No checkout attribute value found with the specified id
                return RedirectToAction("List");

            model.PrimaryStoreCurrencyCode = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode;
            model.BaseWeightIn = _measureService.GetMeasureWeightById(_measureSettings.BaseWeightId).Name;

            if (cav.CheckoutAttribute.AttributeControlType == AttributeControlType.ColorSquares)
            {
                //ensure valid color is chosen/entered
                if (String.IsNullOrEmpty(model.ColorSquaresRgb))
                    ModelState.AddModelError("", "Color is required");
                try
                {
                    //ensure color is valid (can be instanciated)
                    System.Drawing.ColorTranslator.FromHtml(model.ColorSquaresRgb);
                }
                catch (Exception exc)
                {
                    ModelState.AddModelError("", exc.Message);
                }
            }

            if (ModelState.IsValid)
            {
                cav.Name = model.Name;
                cav.ColorSquaresRgb = model.ColorSquaresRgb;
                cav.PriceAdjustment = model.PriceAdjustment;
                cav.WeightAdjustment = model.WeightAdjustment;
                cav.IsPreSelected = model.IsPreSelected;
                cav.DisplayOrder = model.DisplayOrder;
                _checkoutAttributeService.UpdateCheckoutAttributeValue(cav);

                UpdateValueLocales(cav, model);

                ViewBag.RefreshPage = true;
                ViewBag.btnId = btnId;
                ViewBag.formId = formId;
                return View(model);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        //delete
        [HttpPost]
        public ActionResult ValueDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var cav = _checkoutAttributeService.GetCheckoutAttributeValueById(id);
            if (cav == null)
                throw new ArgumentException("No checkout attribute value found with the specified id");
            _checkoutAttributeService.DeleteCheckoutAttributeValue(cav);

            return new NullJsonResult();
        }


        #endregion
    }
}
