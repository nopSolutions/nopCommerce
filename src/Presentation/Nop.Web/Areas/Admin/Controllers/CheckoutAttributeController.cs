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

        protected CurrencySettings CurrencySettings { get; }
        protected ICheckoutAttributeModelFactory CheckoutAttributeModelFactory { get; }
        protected ICheckoutAttributeParser CheckoutAttributeParser { get; }
        protected ICheckoutAttributeService CheckoutAttributeService { get; }
        protected ICurrencyService CurrencyService { get; }
        protected ICustomerActivityService CustomerActivityService { get; }
        protected ILocalizationService LocalizationService { get; }
        protected ILocalizedEntityService LocalizedEntityService { get; }
        protected INotificationService NotificationService { get; }
        protected IMeasureService MeasureService { get; }
        protected IPermissionService PermissionService { get; }
        protected IStoreMappingService StoreMappingService { get; }
        protected IStoreService StoreService { get; }
        protected MeasureSettings MeasureSettings { get; }

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
            CurrencySettings = currencySettings;
            CheckoutAttributeModelFactory = checkoutAttributeModelFactory;
            CheckoutAttributeParser = checkoutAttributeParser;
            CheckoutAttributeService = checkoutAttributeService;
            CurrencyService = currencyService;
            CustomerActivityService = customerActivityService;
            LocalizationService = localizationService;
            LocalizedEntityService = localizedEntityService;
            NotificationService = notificationService;
            MeasureService = measureService;
            PermissionService = permissionService;
            StoreMappingService = storeMappingService;
            StoreService = storeService;
            MeasureSettings = measureSettings;
        }

        #endregion

        #region Utilities

        protected virtual async Task UpdateAttributeLocalesAsync(CheckoutAttribute checkoutAttribute, CheckoutAttributeModel model)
        {
            foreach (var localized in model.Locales)
            {
                await LocalizedEntityService.SaveLocalizedValueAsync(checkoutAttribute,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);

                await LocalizedEntityService.SaveLocalizedValueAsync(checkoutAttribute,
                    x => x.TextPrompt,
                    localized.TextPrompt,
                    localized.LanguageId);

                await LocalizedEntityService.SaveLocalizedValueAsync(checkoutAttribute,
                    x => x.DefaultValue,
                    localized.DefaultValue,
                    localized.LanguageId);
            }
        }

        protected virtual async Task UpdateValueLocalesAsync(CheckoutAttributeValue checkoutAttributeValue, CheckoutAttributeValueModel model)
        {
            foreach (var localized in model.Locales)
            {
                await LocalizedEntityService.SaveLocalizedValueAsync(checkoutAttributeValue,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);
            }
        }

        protected virtual async Task SaveStoreMappingsAsync(CheckoutAttribute checkoutAttribute, CheckoutAttributeModel model)
        {
            checkoutAttribute.LimitedToStores = model.SelectedStoreIds.Any();
            await CheckoutAttributeService.UpdateCheckoutAttributeAsync(checkoutAttribute);

            var existingStoreMappings =await StoreMappingService.GetStoreMappingsAsync(checkoutAttribute);
            var allStores = await StoreService.GetAllStoresAsync();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds.Contains(store.Id))
                {
                    //new store
                    if (existingStoreMappings.Count(sm => sm.StoreId == store.Id) == 0)
                        await StoreMappingService.InsertStoreMappingAsync(checkoutAttribute, store.Id);
                }
                else
                {
                    //remove store
                    var storeMappingToDelete = existingStoreMappings.FirstOrDefault(sm => sm.StoreId == store.Id);
                    if (storeMappingToDelete != null)
                        await StoreMappingService.DeleteStoreMappingAsync(storeMappingToDelete);
                }
            }
        }

        protected virtual async Task SaveConditionAttributesAsync(CheckoutAttribute checkoutAttribute, CheckoutAttributeModel model)
        {
            string attributesXml = null;

            if (model.ConditionModel.EnableCondition)
            {
                var attribute = await CheckoutAttributeService.GetCheckoutAttributeByIdAsync(model.ConditionModel.SelectedAttributeId);
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
                                attributesXml = CheckoutAttributeParser.AddCheckoutAttribute(null, attribute, string.IsNullOrEmpty(selectedValue) ? string.Empty : selectedValue);
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
                                        attributesXml = CheckoutAttributeParser.AddCheckoutAttribute(attributesXml, attribute, value);
                                else
                                    attributesXml = CheckoutAttributeParser.AddCheckoutAttribute(null, attribute, string.Empty);
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
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //prepare model
            var model = await CheckoutAttributeModelFactory.PrepareCheckoutAttributeSearchModelAsync(new CheckoutAttributeSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(CheckoutAttributeSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await CheckoutAttributeModelFactory.PrepareCheckoutAttributeListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //prepare model
            var model = await CheckoutAttributeModelFactory.PrepareCheckoutAttributeModelAsync(new CheckoutAttributeModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Create(CheckoutAttributeModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var checkoutAttribute = model.ToEntity<CheckoutAttribute>();
                await CheckoutAttributeService.InsertCheckoutAttributeAsync(checkoutAttribute);

                //locales
                await UpdateAttributeLocalesAsync(checkoutAttribute, model);

                //stores
                await SaveStoreMappingsAsync(checkoutAttribute, model);

                //activity log
                await CustomerActivityService.InsertActivityAsync("AddNewCheckoutAttribute",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.AddNewCheckoutAttribute"), checkoutAttribute.Name), checkoutAttribute);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Catalog.Attributes.CheckoutAttributes.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");
                
                return RedirectToAction("Edit", new { id = checkoutAttribute.Id });
            }

            //prepare model
            model = await CheckoutAttributeModelFactory.PrepareCheckoutAttributeModelAsync(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a checkout attribute with the specified id
            var checkoutAttribute = await CheckoutAttributeService.GetCheckoutAttributeByIdAsync(id);
            if (checkoutAttribute == null)
                return RedirectToAction("List");

            //prepare model
            var model = await CheckoutAttributeModelFactory.PrepareCheckoutAttributeModelAsync(null, checkoutAttribute);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Edit(CheckoutAttributeModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a checkout attribute with the specified id
            var checkoutAttribute = await CheckoutAttributeService.GetCheckoutAttributeByIdAsync(model.Id);
            if (checkoutAttribute == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                checkoutAttribute = model.ToEntity(checkoutAttribute);
                await SaveConditionAttributesAsync(checkoutAttribute, model);
                await CheckoutAttributeService.UpdateCheckoutAttributeAsync(checkoutAttribute);

                //locales
                await UpdateAttributeLocalesAsync(checkoutAttribute, model);

                //stores
                await SaveStoreMappingsAsync(checkoutAttribute, model);

                //activity log
                await CustomerActivityService.InsertActivityAsync("EditCheckoutAttribute",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.EditCheckoutAttribute"), checkoutAttribute.Name), checkoutAttribute);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Catalog.Attributes.CheckoutAttributes.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");
                
                return RedirectToAction("Edit", new { id = checkoutAttribute.Id });
            }
            
            //prepare model
            model = await CheckoutAttributeModelFactory.PrepareCheckoutAttributeModelAsync(model, checkoutAttribute, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a checkout attribute with the specified id
            var checkoutAttribute = await CheckoutAttributeService.GetCheckoutAttributeByIdAsync(id);
            if (checkoutAttribute == null)
                return RedirectToAction("List");

            await CheckoutAttributeService.DeleteCheckoutAttributeAsync(checkoutAttribute);

            //activity log
            await CustomerActivityService.InsertActivityAsync("DeleteCheckoutAttribute",
                string.Format(await LocalizationService.GetResourceAsync("ActivityLog.DeleteCheckoutAttribute"), checkoutAttribute.Name), checkoutAttribute);

            NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Catalog.Attributes.CheckoutAttributes.Deleted"));

            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected(ICollection<int> selectedIds)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            if (selectedIds == null || selectedIds.Count() == 0)
                return NoContent();

            var checkoutAttributes = await CheckoutAttributeService.GetCheckoutAttributeByIdsAsync(selectedIds.ToArray());
            await CheckoutAttributeService.DeleteCheckoutAttributesAsync(checkoutAttributes);

            foreach (var checkoutAttribute in checkoutAttributes)
            {
                //activity log
                await CustomerActivityService.InsertActivityAsync("DeleteCheckoutAttribute",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.DeleteCheckoutAttribute"), checkoutAttribute.Name), checkoutAttribute);
            }

            return Json(new { Result = true });
        }

        #endregion

        #region Checkout attribute values

        [HttpPost]
        public virtual async Task<IActionResult> ValueList(CheckoutAttributeValueSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return await AccessDeniedDataTablesJson();

            //try to get a checkout attribute with the specified id
            var checkoutAttribute = await CheckoutAttributeService.GetCheckoutAttributeByIdAsync(searchModel.CheckoutAttributeId)
                ?? throw new ArgumentException("No checkout attribute found with the specified id");

            //prepare model
            var model = await CheckoutAttributeModelFactory.PrepareCheckoutAttributeValueListModelAsync(searchModel, checkoutAttribute);

            return Json(model);
        }

        public virtual async Task<IActionResult> ValueCreatePopup(int checkoutAttributeId)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a checkout attribute with the specified id
            var checkoutAttribute = await CheckoutAttributeService.GetCheckoutAttributeByIdAsync(checkoutAttributeId);
            if (checkoutAttribute == null)
                return RedirectToAction("List");

            //prepare model
            var model = await CheckoutAttributeModelFactory
                .PrepareCheckoutAttributeValueModelAsync(new CheckoutAttributeValueModel(), checkoutAttribute, null);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ValueCreatePopup(CheckoutAttributeValueModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a checkout attribute with the specified id
            var checkoutAttribute = await CheckoutAttributeService.GetCheckoutAttributeByIdAsync(model.CheckoutAttributeId);
            if (checkoutAttribute == null)
                return RedirectToAction("List");

            model.PrimaryStoreCurrencyCode = (await CurrencyService.GetCurrencyByIdAsync(CurrencySettings.PrimaryStoreCurrencyId)).CurrencyCode;
            model.BaseWeightIn = (await MeasureService.GetMeasureWeightByIdAsync(MeasureSettings.BaseWeightId)).Name;

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
                await CheckoutAttributeService.InsertCheckoutAttributeValueAsync(checkoutAttributeValue);

                await UpdateValueLocalesAsync(checkoutAttributeValue, model);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = await CheckoutAttributeModelFactory.PrepareCheckoutAttributeValueModelAsync(model, checkoutAttribute, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> ValueEditPopup(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a checkout attribute value with the specified id
            var checkoutAttributeValue = await CheckoutAttributeService.GetCheckoutAttributeValueByIdAsync(id);
            if (checkoutAttributeValue == null)
                return RedirectToAction("List");

            //try to get a checkout attribute with the specified id
            var checkoutAttribute = await CheckoutAttributeService.GetCheckoutAttributeByIdAsync(checkoutAttributeValue.CheckoutAttributeId);
            if (checkoutAttribute == null)
                return RedirectToAction("List");

            //prepare model
            var model = await CheckoutAttributeModelFactory.PrepareCheckoutAttributeValueModelAsync(null, checkoutAttribute, checkoutAttributeValue);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ValueEditPopup(CheckoutAttributeValueModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a checkout attribute value with the specified id
            var checkoutAttributeValue = await CheckoutAttributeService.GetCheckoutAttributeValueByIdAsync(model.Id);
            if (checkoutAttributeValue == null)
                return RedirectToAction("List");

            //try to get a checkout attribute with the specified id
            var checkoutAttribute = await CheckoutAttributeService.GetCheckoutAttributeByIdAsync(checkoutAttributeValue.CheckoutAttributeId);
            if (checkoutAttribute == null)
                return RedirectToAction("List");

            model.PrimaryStoreCurrencyCode = (await CurrencyService.GetCurrencyByIdAsync(CurrencySettings.PrimaryStoreCurrencyId)).CurrencyCode;
            model.BaseWeightIn = (await MeasureService.GetMeasureWeightByIdAsync(MeasureSettings.BaseWeightId)).Name;

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
                await CheckoutAttributeService.UpdateCheckoutAttributeValueAsync(checkoutAttributeValue);

                await UpdateValueLocalesAsync(checkoutAttributeValue, model);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = await CheckoutAttributeModelFactory.PrepareCheckoutAttributeValueModelAsync(model, checkoutAttribute, checkoutAttributeValue, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ValueDelete(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a checkout attribute value with the specified id
            var checkoutAttributeValue = await CheckoutAttributeService.GetCheckoutAttributeValueByIdAsync(id)
                ?? throw new ArgumentException("No checkout attribute value found with the specified id", nameof(id));

            await CheckoutAttributeService.DeleteCheckoutAttributeValueAsync(checkoutAttributeValue);

            return new NullJsonResult();
        }

        #endregion
    }
}