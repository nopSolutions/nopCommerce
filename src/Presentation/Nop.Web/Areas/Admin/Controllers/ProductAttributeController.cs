using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class ProductAttributeController : BaseAdminController
    {
        #region Fields

        protected readonly ICustomerActivityService _customerActivityService;
        protected readonly ILocalizationService _localizationService;
        protected readonly ILocalizedEntityService _localizedEntityService;
        protected readonly INotificationService _notificationService;
        protected readonly IPermissionService _permissionService;
        protected readonly IProductAttributeModelFactory _productAttributeModelFactory;
        protected readonly IProductAttributeService _productAttributeService;

        #endregion Fields

        #region Ctor

        public ProductAttributeController(ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            INotificationService notificationService,
            IPermissionService permissionService,
            IProductAttributeModelFactory productAttributeModelFactory,
            IProductAttributeService productAttributeService)
        {
            _customerActivityService = customerActivityService;
            _localizationService = localizationService;
            _localizedEntityService = localizedEntityService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _productAttributeModelFactory = productAttributeModelFactory;
            _productAttributeService = productAttributeService;
        }

        #endregion

        #region Utilities

        protected virtual async Task UpdateLocalesAsync(ProductAttribute productAttribute, ProductAttributeModel model)
        {
            foreach (var localized in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValueAsync(productAttribute,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);

                await _localizedEntityService.SaveLocalizedValueAsync(productAttribute,
                    x => x.Description,
                    localized.Description,
                    localized.LanguageId);
            }
        }

        protected virtual async Task UpdateLocalesAsync(PredefinedProductAttributeValue ppav, PredefinedProductAttributeValueModel model)
        {
            foreach (var localized in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValueAsync(ppav,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);
            }
        }

        #endregion

        #region Methods

        #region Attribute list / create / edit / delete

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual async Task<IActionResult> List()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //prepare model
            var model = await _productAttributeModelFactory.PrepareProductAttributeSearchModelAsync(new ProductAttributeSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(ProductAttributeSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _productAttributeModelFactory.PrepareProductAttributeListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //prepare model
            var model = await _productAttributeModelFactory.PrepareProductAttributeModelAsync(new ProductAttributeModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Create(ProductAttributeModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var productAttribute = model.ToEntity<ProductAttribute>();
                await _productAttributeService.InsertProductAttributeAsync(productAttribute);
                await UpdateLocalesAsync(productAttribute, model);

                //activity log
                await _customerActivityService.InsertActivityAsync("AddNewProductAttribute",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewProductAttribute"), productAttribute.Name), productAttribute);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Attributes.ProductAttributes.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = productAttribute.Id });
            }

            //prepare model
            model = await _productAttributeModelFactory.PrepareProductAttributeModelAsync(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a product attribute with the specified id
            var productAttribute = await _productAttributeService.GetProductAttributeByIdAsync(id);
            if (productAttribute == null)
                return RedirectToAction("List");

            //prepare model
            var model = await _productAttributeModelFactory.PrepareProductAttributeModelAsync(null, productAttribute);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Edit(ProductAttributeModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a product attribute with the specified id
            var productAttribute = await _productAttributeService.GetProductAttributeByIdAsync(model.Id);
            if (productAttribute == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                productAttribute = model.ToEntity(productAttribute);
                await _productAttributeService.UpdateProductAttributeAsync(productAttribute);

                await UpdateLocalesAsync(productAttribute, model);

                //activity log
                await _customerActivityService.InsertActivityAsync("EditProductAttribute",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditProductAttribute"), productAttribute.Name), productAttribute);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Attributes.ProductAttributes.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = productAttribute.Id });
            }

            //prepare model
            model = await _productAttributeModelFactory.PrepareProductAttributeModelAsync(model, productAttribute, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a product attribute with the specified id
            var productAttribute = await _productAttributeService.GetProductAttributeByIdAsync(id);
            if (productAttribute == null)
                return RedirectToAction("List");

            await _productAttributeService.DeleteProductAttributeAsync(productAttribute);

            //activity log
            await _customerActivityService.InsertActivityAsync("DeleteProductAttribute",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteProductAttribute"), productAttribute.Name), productAttribute);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Attributes.ProductAttributes.Deleted"));

            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected(ICollection<int> selectedIds)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            if (selectedIds == null || selectedIds.Count == 0)
                return NoContent();

            var productAttributes = await _productAttributeService.GetProductAttributeByIdsAsync(selectedIds.ToArray());
            await _productAttributeService.DeleteProductAttributesAsync(productAttributes);

            foreach (var productAttribute in productAttributes)
            {
                await _customerActivityService.InsertActivityAsync("DeleteProductAttribute",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteProductAttribute"), productAttribute.Name), productAttribute);
            }

            return Json(new { Result = true });
        }

        #endregion

        #region Used by products

        [HttpPost]
        public virtual async Task<IActionResult> UsedByProducts(ProductAttributeProductSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return await AccessDeniedDataTablesJson();

            //try to get a product attribute with the specified id
            var productAttribute = await _productAttributeService.GetProductAttributeByIdAsync(searchModel.ProductAttributeId)
                ?? throw new ArgumentException("No product attribute found with the specified id");

            //prepare model
            var model = await _productAttributeModelFactory.PrepareProductAttributeProductListModelAsync(searchModel, productAttribute);

            return Json(model);
        }

        #endregion

        #region Predefined values

        [HttpPost]
        public virtual async Task<IActionResult> PredefinedProductAttributeValueList(PredefinedProductAttributeValueSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return await AccessDeniedDataTablesJson();

            //try to get a product attribute with the specified id
            var productAttribute = await _productAttributeService.GetProductAttributeByIdAsync(searchModel.ProductAttributeId)
                ?? throw new ArgumentException("No product attribute found with the specified id");

            //prepare model
            var model = await _productAttributeModelFactory.PreparePredefinedProductAttributeValueListModelAsync(searchModel, productAttribute);

            return Json(model);
        }

        public virtual async Task<IActionResult> PredefinedProductAttributeValueCreatePopup(int productAttributeId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a product attribute with the specified id
            var productAttribute = await _productAttributeService.GetProductAttributeByIdAsync(productAttributeId)
                ?? throw new ArgumentException("No product attribute found with the specified id", nameof(productAttributeId));

            //prepare model
            var model = await _productAttributeModelFactory
                .PreparePredefinedProductAttributeValueModelAsync(new PredefinedProductAttributeValueModel(), productAttribute, null);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> PredefinedProductAttributeValueCreatePopup(PredefinedProductAttributeValueModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a product attribute with the specified id
            var productAttribute = await _productAttributeService.GetProductAttributeByIdAsync(model.ProductAttributeId)
                ?? throw new ArgumentException("No product attribute found with the specified id");

            if (ModelState.IsValid)
            {
                //fill entity from model
                var ppav = model.ToEntity<PredefinedProductAttributeValue>();

                await _productAttributeService.InsertPredefinedProductAttributeValueAsync(ppav);
                await UpdateLocalesAsync(ppav, model);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = await _productAttributeModelFactory.PreparePredefinedProductAttributeValueModelAsync(model, productAttribute, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> PredefinedProductAttributeValueEditPopup(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a predefined product attribute value with the specified id
            var productAttributeValue = await _productAttributeService.GetPredefinedProductAttributeValueByIdAsync(id)
                ?? throw new ArgumentException("No predefined product attribute value found with the specified id");

            //try to get a product attribute with the specified id
            var productAttribute = await _productAttributeService.GetProductAttributeByIdAsync(productAttributeValue.ProductAttributeId)
                ?? throw new ArgumentException("No product attribute found with the specified id");

            //prepare model
            var model = await _productAttributeModelFactory.PreparePredefinedProductAttributeValueModelAsync(null, productAttribute, productAttributeValue);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> PredefinedProductAttributeValueEditPopup(PredefinedProductAttributeValueModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a predefined product attribute value with the specified id
            var productAttributeValue = await _productAttributeService.GetPredefinedProductAttributeValueByIdAsync(model.Id)
                ?? throw new ArgumentException("No predefined product attribute value found with the specified id");

            //try to get a product attribute with the specified id
            var productAttribute = await _productAttributeService.GetProductAttributeByIdAsync(productAttributeValue.ProductAttributeId)
                ?? throw new ArgumentException("No product attribute found with the specified id");

            if (ModelState.IsValid)
            {
                productAttributeValue = model.ToEntity(productAttributeValue);
                await _productAttributeService.UpdatePredefinedProductAttributeValueAsync(productAttributeValue);

                await UpdateLocalesAsync(productAttributeValue, model);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = await _productAttributeModelFactory.PreparePredefinedProductAttributeValueModelAsync(model, productAttribute, productAttributeValue, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> PredefinedProductAttributeValueDelete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a predefined product attribute value with the specified id
            var productAttributeValue = await _productAttributeService.GetPredefinedProductAttributeValueByIdAsync(id)
                ?? throw new ArgumentException("No predefined product attribute value found with the specified id", nameof(id));

            await _productAttributeService.DeletePredefinedProductAttributeValueAsync(productAttributeValue);

            return new NullJsonResult();
        }

        #endregion

        #endregion
    }
}