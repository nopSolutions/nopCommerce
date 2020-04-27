using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class CheckoutAttributeController : BaseAdminController
    {
        #region Fields

        private readonly CurrencySettings _currencySettings;
        private readonly ICheckoutAttributeModelFactory _checkoutAttributeModelFactory;
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly ICheckoutAttributeService _checkoutAttributeService;
        private readonly ICurrencyService _currencyService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly INotificationService _notificationService;
        private readonly IMeasureService _measureService;
        private readonly IPermissionService _permissionService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IStoreService _storeService;
        private readonly MeasureSettings _measureSettings;

        #endregion

        #region Ctor

        public CheckoutAttributeController(CurrencySettings currencySettings,
            ICheckoutAttributeModelFactory checkoutAttributeModelFactory,
            ICheckoutAttributeParser checkoutAttributeParser,
            ICheckoutAttributeService checkoutAttributeService,
            ICurrencyService currencyService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            INotificationService notificationService,
            IMeasureService measureService,
            IPermissionService permissionService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            MeasureSettings measureSettings)
        {
            _currencySettings = currencySettings;
            _checkoutAttributeModelFactory = checkoutAttributeModelFactory;
            _checkoutAttributeParser = checkoutAttributeParser;
            _checkoutAttributeService = checkoutAttributeService;
            _currencyService = currencyService;
            _customerActivityService = customerActivityService;
            _localizationService = localizationService;
            _localizedEntityService = localizedEntityService;
            _notificationService = notificationService;
            _measureService = measureService;
            _permissionService = permissionService;
            _storeMappingService = storeMappingService;
            _storeService = storeService;
            _measureSettings = measureSettings;
        }

        #endregion

        #region Utilities

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

                _localizedEntityService.SaveLocalizedValue(checkoutAttribute,
                    x => x.DefaultValue,
                    localized.DefaultValue,
                    localized.LanguageId);
            }
        }

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

        protected virtual void SaveStoreMappings(CheckoutAttribute checkoutAttribute, CheckoutAttributeModel model)
        {
            checkoutAttribute.LimitedToStores = model.SelectedStoreIds.Any();
            _checkoutAttributeService.UpdateCheckoutAttribute(checkoutAttribute);

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
                                var selectedValue = selectedAttribute?.SelectedValueId;

                                //for conditions we should empty values save even when nothing is selected
                                //otherwise "attributesXml" will be empty
                                //hence we won't be able to find a selected attribute
                                attributesXml = _checkoutAttributeParser.AddCheckoutAttribute(null, attribute, string.IsNullOrEmpty(selectedValue) ? string.Empty : selectedValue);
                            }
                            break;
                        case AttributeControlType.Checkboxes:
                            {
                                var selectedAttribute = model.ConditionModel.ConditionAttributes
                                    .FirstOrDefault(x => x.Id == model.ConditionModel.SelectedAttributeId);
                                var selectedValues = selectedAttribute?.Values
                                    .Where(x => x.Selected)
                                    .Select(x => x.Value)
                                    .ToList();

                                if (selectedValues?.Any() ?? false)
                                    foreach (var value in selectedValues)
                                        attributesXml = _checkoutAttributeParser.AddCheckoutAttribute(attributesXml, attribute, value);
                                else
                                    attributesXml = _checkoutAttributeParser.AddCheckoutAttribute(null, attribute, string.Empty);
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

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //prepare model
            var model = _checkoutAttributeModelFactory.PrepareCheckoutAttributeSearchModel(new CheckoutAttributeSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult List(CheckoutAttributeSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _checkoutAttributeModelFactory.PrepareCheckoutAttributeListModel(searchModel);

            return Json(model);
        }

        public virtual IActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //prepare model
            var model = _checkoutAttributeModelFactory.PrepareCheckoutAttributeModel(new CheckoutAttributeModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(CheckoutAttributeModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var checkoutAttribute = model.ToEntity<CheckoutAttribute>();
                _checkoutAttributeService.InsertCheckoutAttribute(checkoutAttribute);

                //locales
                UpdateAttributeLocales(checkoutAttribute, model);

                //stores
                SaveStoreMappings(checkoutAttribute, model);

                //activity log
                _customerActivityService.InsertActivity("AddNewCheckoutAttribute",
                    string.Format(_localizationService.GetResource("ActivityLog.AddNewCheckoutAttribute"), checkoutAttribute.Name), checkoutAttribute);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Catalog.Attributes.CheckoutAttributes.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");
                
                return RedirectToAction("Edit", new { id = checkoutAttribute.Id });
            }

            //prepare model
            model = _checkoutAttributeModelFactory.PrepareCheckoutAttributeModel(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a checkout attribute with the specified id
            var checkoutAttribute = _checkoutAttributeService.GetCheckoutAttributeById(id);
            if (checkoutAttribute == null)
                return RedirectToAction("List");

            //prepare model
            var model = _checkoutAttributeModelFactory.PrepareCheckoutAttributeModel(null, checkoutAttribute);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(CheckoutAttributeModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a checkout attribute with the specified id
            var checkoutAttribute = _checkoutAttributeService.GetCheckoutAttributeById(model.Id);
            if (checkoutAttribute == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                checkoutAttribute = model.ToEntity(checkoutAttribute);
                SaveConditionAttributes(checkoutAttribute, model);
                _checkoutAttributeService.UpdateCheckoutAttribute(checkoutAttribute);

                //locales
                UpdateAttributeLocales(checkoutAttribute, model);

                //stores
                SaveStoreMappings(checkoutAttribute, model);

                //activity log
                _customerActivityService.InsertActivity("EditCheckoutAttribute",
                    string.Format(_localizationService.GetResource("ActivityLog.EditCheckoutAttribute"), checkoutAttribute.Name), checkoutAttribute);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Catalog.Attributes.CheckoutAttributes.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");
                
                return RedirectToAction("Edit", new { id = checkoutAttribute.Id });
            }
            
            //prepare model
            model = _checkoutAttributeModelFactory.PrepareCheckoutAttributeModel(model, checkoutAttribute, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a checkout attribute with the specified id
            var checkoutAttribute = _checkoutAttributeService.GetCheckoutAttributeById(id);
            if (checkoutAttribute == null)
                return RedirectToAction("List");

            _checkoutAttributeService.DeleteCheckoutAttribute(checkoutAttribute);

            //activity log
            _customerActivityService.InsertActivity("DeleteCheckoutAttribute",
                string.Format(_localizationService.GetResource("ActivityLog.DeleteCheckoutAttribute"), checkoutAttribute.Name), checkoutAttribute);

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Catalog.Attributes.CheckoutAttributes.Deleted"));

            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual IActionResult DeleteSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            if (selectedIds != null)
            {
                var checkoutAttributes = _checkoutAttributeService.GetCheckoutAttributeByIds(selectedIds.ToArray());
                _checkoutAttributeService.DeleteCheckoutAttributes(checkoutAttributes);

                foreach (var checkoutAttribute in checkoutAttributes)
                {
                    //activity log
                    _customerActivityService.InsertActivity("DeleteCheckoutAttribute",
                        string.Format(_localizationService.GetResource("ActivityLog.DeleteCheckoutAttribute"), checkoutAttribute.Name), checkoutAttribute);
                }
            }

            return Json(new { Result = true });
        }

        #endregion

        #region Checkout attribute values

        [HttpPost]
        public virtual IActionResult ValueList(CheckoutAttributeValueSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedDataTablesJson();

            //try to get a checkout attribute with the specified id
            var checkoutAttribute = _checkoutAttributeService.GetCheckoutAttributeById(searchModel.CheckoutAttributeId)
                ?? throw new ArgumentException("No checkout attribute found with the specified id");

            //prepare model
            var model = _checkoutAttributeModelFactory.PrepareCheckoutAttributeValueListModel(searchModel, checkoutAttribute);

            return Json(model);
        }

        public virtual IActionResult ValueCreatePopup(int checkoutAttributeId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a checkout attribute with the specified id
            var checkoutAttribute = _checkoutAttributeService.GetCheckoutAttributeById(checkoutAttributeId);
            if (checkoutAttribute == null)
                return RedirectToAction("List");

            //prepare model
            var model = _checkoutAttributeModelFactory
                .PrepareCheckoutAttributeValueModel(new CheckoutAttributeValueModel(), checkoutAttribute, null);

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult ValueCreatePopup(CheckoutAttributeValueModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a checkout attribute with the specified id
            var checkoutAttribute = _checkoutAttributeService.GetCheckoutAttributeById(model.CheckoutAttributeId);
            if (checkoutAttribute == null)
                return RedirectToAction("List");

            model.PrimaryStoreCurrencyCode = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode;
            model.BaseWeightIn = _measureService.GetMeasureWeightById(_measureSettings.BaseWeightId).Name;

            if (checkoutAttribute.AttributeControlType == AttributeControlType.ColorSquares)
            {
                //ensure valid color is chosen/entered
                if (string.IsNullOrEmpty(model.ColorSquaresRgb))
                    ModelState.AddModelError(string.Empty, "Color is required");

                try
                {
                    //ensure color is valid (can be instantiated)
                    System.Drawing.ColorTranslator.FromHtml(model.ColorSquaresRgb);
                }
                catch (Exception exc)
                {
                    ModelState.AddModelError(string.Empty, exc.Message);
                }
            }

            if (ModelState.IsValid)
            {
                var checkoutAttributeValue = model.ToEntity<CheckoutAttributeValue>();
                _checkoutAttributeService.InsertCheckoutAttributeValue(checkoutAttributeValue);

                UpdateValueLocales(checkoutAttributeValue, model);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = _checkoutAttributeModelFactory.PrepareCheckoutAttributeValueModel(model, checkoutAttribute, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual IActionResult ValueEditPopup(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a checkout attribute value with the specified id
            var checkoutAttributeValue = _checkoutAttributeService.GetCheckoutAttributeValueById(id);
            if (checkoutAttributeValue == null)
                return RedirectToAction("List");

            //try to get a checkout attribute with the specified id
            var checkoutAttribute = _checkoutAttributeService.GetCheckoutAttributeById(checkoutAttributeValue.CheckoutAttributeId);
            if (checkoutAttribute == null)
                return RedirectToAction("List");

            //prepare model
            var model = _checkoutAttributeModelFactory.PrepareCheckoutAttributeValueModel(null, checkoutAttribute, checkoutAttributeValue);

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult ValueEditPopup(CheckoutAttributeValueModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a checkout attribute value with the specified id
            var checkoutAttributeValue = _checkoutAttributeService.GetCheckoutAttributeValueById(model.Id);
            if (checkoutAttributeValue == null)
                return RedirectToAction("List");

            //try to get a checkout attribute with the specified id
            var checkoutAttribute = _checkoutAttributeService.GetCheckoutAttributeById(checkoutAttributeValue.CheckoutAttributeId);
            if (checkoutAttribute == null)
                return RedirectToAction("List");

            model.PrimaryStoreCurrencyCode = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode;
            model.BaseWeightIn = _measureService.GetMeasureWeightById(_measureSettings.BaseWeightId).Name;

            if (checkoutAttribute.AttributeControlType == AttributeControlType.ColorSquares)
            {
                //ensure valid color is chosen/entered
                if (string.IsNullOrEmpty(model.ColorSquaresRgb))
                    ModelState.AddModelError(string.Empty, "Color is required");

                try
                {
                    //ensure color is valid (can be instantiated)
                    System.Drawing.ColorTranslator.FromHtml(model.ColorSquaresRgb);
                }
                catch (Exception exc)
                {
                    ModelState.AddModelError(string.Empty, exc.Message);
                }
            }

            if (ModelState.IsValid)
            {
                checkoutAttributeValue = model.ToEntity(checkoutAttributeValue);
                _checkoutAttributeService.UpdateCheckoutAttributeValue(checkoutAttributeValue);

                UpdateValueLocales(checkoutAttributeValue, model);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = _checkoutAttributeModelFactory.PrepareCheckoutAttributeValueModel(model, checkoutAttribute, checkoutAttributeValue, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult ValueDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a checkout attribute value with the specified id
            var checkoutAttributeValue = _checkoutAttributeService.GetCheckoutAttributeValueById(id)
                ?? throw new ArgumentException("No checkout attribute value found with the specified id", nameof(id));

            _checkoutAttributeService.DeleteCheckoutAttributeValue(checkoutAttributeValue);

            return new NullJsonResult();
        }

        #endregion
    }
}