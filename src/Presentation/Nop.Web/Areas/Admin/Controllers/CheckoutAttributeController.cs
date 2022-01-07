using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        protected virtual async Task UpdateAttributeLocalesAsync(CheckoutAttribute checkoutAttribute, CheckoutAttributeModel model)
        {
            foreach (var localized in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValueAsync(checkoutAttribute,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);

                await _localizedEntityService.SaveLocalizedValueAsync(checkoutAttribute,
                    x => x.TextPrompt,
                    localized.TextPrompt,
                    localized.LanguageId);

                await _localizedEntityService.SaveLocalizedValueAsync(checkoutAttribute,
                    x => x.DefaultValue,
                    localized.DefaultValue,
                    localized.LanguageId);
            }
        }

        protected virtual async Task UpdateValueLocalesAsync(CheckoutAttributeValue checkoutAttributeValue, CheckoutAttributeValueModel model)
        {
            foreach (var localized in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValueAsync(checkoutAttributeValue,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);
            }
        }

        protected virtual async Task SaveStoreMappingsAsync(CheckoutAttribute checkoutAttribute, CheckoutAttributeModel model)
        {
            checkoutAttribute.LimitedToStores = model.SelectedStoreIds.Any();
            await _checkoutAttributeService.UpdateCheckoutAttributeAsync(checkoutAttribute);

            var existingStoreMappings =await _storeMappingService.GetStoreMappingsAsync(checkoutAttribute);
            var allStores = await _storeService.GetAllStoresAsync();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds.Contains(store.Id))
                {
                    //new store
                    if (!existingStoreMappings.Any(sm => sm.StoreId == store.Id))
                        await _storeMappingService.InsertStoreMappingAsync(checkoutAttribute, store.Id);
                }
                else
                {
                    //remove store
                    var storeMappingToDelete = existingStoreMappings.FirstOrDefault(sm => sm.StoreId == store.Id);
                    if (storeMappingToDelete != null)
                        await _storeMappingService.DeleteStoreMappingAsync(storeMappingToDelete);
                }
            }
        }

        protected virtual async Task SaveConditionAttributesAsync(CheckoutAttribute checkoutAttribute, CheckoutAttributeModel model)
        {
            string attributesXml = null;

            if (model.ConditionModel.EnableCondition)
            {
                var attribute = await _checkoutAttributeService.GetCheckoutAttributeByIdAsync(model.ConditionModel.SelectedAttributeId);
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

        public virtual async Task<IActionResult> List()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //prepare model
            var model = await _checkoutAttributeModelFactory.PrepareCheckoutAttributeSearchModelAsync(new CheckoutAttributeSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(CheckoutAttributeSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _checkoutAttributeModelFactory.PrepareCheckoutAttributeListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //prepare model
            var model = await _checkoutAttributeModelFactory.PrepareCheckoutAttributeModelAsync(new CheckoutAttributeModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Create(CheckoutAttributeModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var checkoutAttribute = model.ToEntity<CheckoutAttribute>();
                await _checkoutAttributeService.InsertCheckoutAttributeAsync(checkoutAttribute);

                //locales
                await UpdateAttributeLocalesAsync(checkoutAttribute, model);

                //stores
                await SaveStoreMappingsAsync(checkoutAttribute, model);

                //activity log
                await _customerActivityService.InsertActivityAsync("AddNewCheckoutAttribute",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewCheckoutAttribute"), checkoutAttribute.Name), checkoutAttribute);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Attributes.CheckoutAttributes.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");
                
                return RedirectToAction("Edit", new { id = checkoutAttribute.Id });
            }

            //prepare model
            model = await _checkoutAttributeModelFactory.PrepareCheckoutAttributeModelAsync(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a checkout attribute with the specified id
            var checkoutAttribute = await _checkoutAttributeService.GetCheckoutAttributeByIdAsync(id);
            if (checkoutAttribute == null)
                return RedirectToAction("List");

            //prepare model
            var model = await _checkoutAttributeModelFactory.PrepareCheckoutAttributeModelAsync(null, checkoutAttribute);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Edit(CheckoutAttributeModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a checkout attribute with the specified id
            var checkoutAttribute = await _checkoutAttributeService.GetCheckoutAttributeByIdAsync(model.Id);
            if (checkoutAttribute == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                checkoutAttribute = model.ToEntity(checkoutAttribute);
                await SaveConditionAttributesAsync(checkoutAttribute, model);
                await _checkoutAttributeService.UpdateCheckoutAttributeAsync(checkoutAttribute);

                //locales
                await UpdateAttributeLocalesAsync(checkoutAttribute, model);

                //stores
                await SaveStoreMappingsAsync(checkoutAttribute, model);

                //activity log
                await _customerActivityService.InsertActivityAsync("EditCheckoutAttribute",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditCheckoutAttribute"), checkoutAttribute.Name), checkoutAttribute);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Attributes.CheckoutAttributes.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");
                
                return RedirectToAction("Edit", new { id = checkoutAttribute.Id });
            }
            
            //prepare model
            model = await _checkoutAttributeModelFactory.PrepareCheckoutAttributeModelAsync(model, checkoutAttribute, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a checkout attribute with the specified id
            var checkoutAttribute = await _checkoutAttributeService.GetCheckoutAttributeByIdAsync(id);
            if (checkoutAttribute == null)
                return RedirectToAction("List");

            await _checkoutAttributeService.DeleteCheckoutAttributeAsync(checkoutAttribute);

            //activity log
            await _customerActivityService.InsertActivityAsync("DeleteCheckoutAttribute",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteCheckoutAttribute"), checkoutAttribute.Name), checkoutAttribute);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Attributes.CheckoutAttributes.Deleted"));

            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected(ICollection<int> selectedIds)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            if (selectedIds == null || selectedIds.Count == 0)
                return NoContent();

            var checkoutAttributes = await _checkoutAttributeService.GetCheckoutAttributeByIdsAsync(selectedIds.ToArray());
            await _checkoutAttributeService.DeleteCheckoutAttributesAsync(checkoutAttributes);

            foreach (var checkoutAttribute in checkoutAttributes)
            {
                //activity log
                await _customerActivityService.InsertActivityAsync("DeleteCheckoutAttribute",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteCheckoutAttribute"), checkoutAttribute.Name), checkoutAttribute);
            }

            return Json(new { Result = true });
        }

        #endregion

        #region Checkout attribute values

        [HttpPost]
        public virtual async Task<IActionResult> ValueList(CheckoutAttributeValueSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return await AccessDeniedDataTablesJson();

            //try to get a checkout attribute with the specified id
            var checkoutAttribute = await _checkoutAttributeService.GetCheckoutAttributeByIdAsync(searchModel.CheckoutAttributeId)
                ?? throw new ArgumentException("No checkout attribute found with the specified id");

            //prepare model
            var model = await _checkoutAttributeModelFactory.PrepareCheckoutAttributeValueListModelAsync(searchModel, checkoutAttribute);

            return Json(model);
        }

        public virtual async Task<IActionResult> ValueCreatePopup(int checkoutAttributeId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a checkout attribute with the specified id
            var checkoutAttribute = await _checkoutAttributeService.GetCheckoutAttributeByIdAsync(checkoutAttributeId);
            if (checkoutAttribute == null)
                return RedirectToAction("List");

            //prepare model
            var model = await _checkoutAttributeModelFactory
                .PrepareCheckoutAttributeValueModelAsync(new CheckoutAttributeValueModel(), checkoutAttribute, null);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ValueCreatePopup(CheckoutAttributeValueModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a checkout attribute with the specified id
            var checkoutAttribute = await _checkoutAttributeService.GetCheckoutAttributeByIdAsync(model.CheckoutAttributeId);
            if (checkoutAttribute == null)
                return RedirectToAction("List");

            model.PrimaryStoreCurrencyCode = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId)).CurrencyCode;
            model.BaseWeightIn = (await _measureService.GetMeasureWeightByIdAsync(_measureSettings.BaseWeightId)).Name;

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
                await _checkoutAttributeService.InsertCheckoutAttributeValueAsync(checkoutAttributeValue);

                await UpdateValueLocalesAsync(checkoutAttributeValue, model);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = await _checkoutAttributeModelFactory.PrepareCheckoutAttributeValueModelAsync(model, checkoutAttribute, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> ValueEditPopup(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a checkout attribute value with the specified id
            var checkoutAttributeValue = await _checkoutAttributeService.GetCheckoutAttributeValueByIdAsync(id);
            if (checkoutAttributeValue == null)
                return RedirectToAction("List");

            //try to get a checkout attribute with the specified id
            var checkoutAttribute = await _checkoutAttributeService.GetCheckoutAttributeByIdAsync(checkoutAttributeValue.CheckoutAttributeId);
            if (checkoutAttribute == null)
                return RedirectToAction("List");

            //prepare model
            var model = await _checkoutAttributeModelFactory.PrepareCheckoutAttributeValueModelAsync(null, checkoutAttribute, checkoutAttributeValue);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ValueEditPopup(CheckoutAttributeValueModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a checkout attribute value with the specified id
            var checkoutAttributeValue = await _checkoutAttributeService.GetCheckoutAttributeValueByIdAsync(model.Id);
            if (checkoutAttributeValue == null)
                return RedirectToAction("List");

            //try to get a checkout attribute with the specified id
            var checkoutAttribute = await _checkoutAttributeService.GetCheckoutAttributeByIdAsync(checkoutAttributeValue.CheckoutAttributeId);
            if (checkoutAttribute == null)
                return RedirectToAction("List");

            model.PrimaryStoreCurrencyCode = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId)).CurrencyCode;
            model.BaseWeightIn = (await _measureService.GetMeasureWeightByIdAsync(_measureSettings.BaseWeightId)).Name;

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
                await _checkoutAttributeService.UpdateCheckoutAttributeValueAsync(checkoutAttributeValue);

                await UpdateValueLocalesAsync(checkoutAttributeValue, model);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = await _checkoutAttributeModelFactory.PrepareCheckoutAttributeValueModelAsync(model, checkoutAttribute, checkoutAttributeValue, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ValueDelete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a checkout attribute value with the specified id
            var checkoutAttributeValue = await _checkoutAttributeService.GetCheckoutAttributeValueByIdAsync(id)
                ?? throw new ArgumentException("No checkout attribute value found with the specified id", nameof(id));

            await _checkoutAttributeService.DeleteCheckoutAttributeValueAsync(checkoutAttributeValue);

            return new NullJsonResult();
        }

        #endregion
    }
}