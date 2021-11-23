using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        protected ICustomerActivityService CustomerActivityService { get; }
        protected ILocalizationService LocalizationService { get; }
        protected ILocalizedEntityService LocalizedEntityService { get; }
        protected INotificationService NotificationService { get; }
        protected IPermissionService PermissionService { get; }
        protected IProductAttributeModelFactory ProductAttributeModelFactory { get; }
        protected IProductAttributeService ProductAttributeService { get; }

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
            CustomerActivityService = customerActivityService;
            LocalizationService = localizationService;
            LocalizedEntityService = localizedEntityService;
            NotificationService = notificationService;
            PermissionService = permissionService;
            ProductAttributeModelFactory = productAttributeModelFactory;
            ProductAttributeService = productAttributeService;
        }

        #endregion

        #region Utilities

        protected virtual async Task UpdateLocalesAsync(ProductAttribute productAttribute, ProductAttributeModel model)
        {
            foreach (var localized in model.Locales)
            {
                await LocalizedEntityService.SaveLocalizedValueAsync(productAttribute,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);

                await LocalizedEntityService.SaveLocalizedValueAsync(productAttribute,
                    x => x.Description,
                    localized.Description,
                    localized.LanguageId);
            }
        }

        protected virtual async Task UpdateLocalesAsync(PredefinedProductAttributeValue ppav, PredefinedProductAttributeValueModel model)
        {
            foreach (var localized in model.Locales)
            {
                await LocalizedEntityService.SaveLocalizedValueAsync(ppav,
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
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //prepare model
            var model = await ProductAttributeModelFactory.PrepareProductAttributeSearchModelAsync(new ProductAttributeSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(ProductAttributeSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await ProductAttributeModelFactory.PrepareProductAttributeListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //prepare model
            var model = await ProductAttributeModelFactory.PrepareProductAttributeModelAsync(new ProductAttributeModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Create(ProductAttributeModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var productAttribute = model.ToEntity<ProductAttribute>();
                await ProductAttributeService.InsertProductAttributeAsync(productAttribute);
                await UpdateLocalesAsync(productAttribute, model);

                //activity log
                await CustomerActivityService.InsertActivityAsync("AddNewProductAttribute",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.AddNewProductAttribute"), productAttribute.Name), productAttribute);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Catalog.Attributes.ProductAttributes.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");
                
                return RedirectToAction("Edit", new { id = productAttribute.Id });
            }

            //prepare model
            model = await ProductAttributeModelFactory.PrepareProductAttributeModelAsync(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a product attribute with the specified id
            var productAttribute = await ProductAttributeService.GetProductAttributeByIdAsync(id);
            if (productAttribute == null)
                return RedirectToAction("List");

            //prepare model
            var model = await ProductAttributeModelFactory.PrepareProductAttributeModelAsync(null, productAttribute);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Edit(ProductAttributeModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a product attribute with the specified id
            var productAttribute = await ProductAttributeService.GetProductAttributeByIdAsync(model.Id);
            if (productAttribute == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                productAttribute = model.ToEntity(productAttribute);
                await ProductAttributeService.UpdateProductAttributeAsync(productAttribute);

                await UpdateLocalesAsync(productAttribute, model);

                //activity log
                await CustomerActivityService.InsertActivityAsync("EditProductAttribute",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.EditProductAttribute"), productAttribute.Name), productAttribute);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Catalog.Attributes.ProductAttributes.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");
                
                return RedirectToAction("Edit", new { id = productAttribute.Id });
            }

            //prepare model
            model = await ProductAttributeModelFactory.PrepareProductAttributeModelAsync(model, productAttribute, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a product attribute with the specified id
            var productAttribute = await ProductAttributeService.GetProductAttributeByIdAsync(id);
            if (productAttribute == null)
                return RedirectToAction("List");

            await ProductAttributeService.DeleteProductAttributeAsync(productAttribute);

            //activity log
            await CustomerActivityService.InsertActivityAsync("DeleteProductAttribute",
                string.Format(await LocalizationService.GetResourceAsync("ActivityLog.DeleteProductAttribute"), productAttribute.Name), productAttribute);

            NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Catalog.Attributes.ProductAttributes.Deleted"));

            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected(ICollection<int> selectedIds)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            if (selectedIds == null || selectedIds.Count == 0)
                return NoContent();

            var productAttributes = await ProductAttributeService.GetProductAttributeByIdsAsync(selectedIds.ToArray());
            await ProductAttributeService.DeleteProductAttributesAsync(productAttributes);

            foreach (var productAttribute in productAttributes)
            {
                await CustomerActivityService.InsertActivityAsync("DeleteProductAttribute",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.DeleteProductAttribute"), productAttribute.Name), productAttribute);
            }

            return Json(new { Result = true });
        }

        #endregion

        #region Used by products

        [HttpPost]
        public virtual async Task<IActionResult> UsedByProducts(ProductAttributeProductSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return await AccessDeniedDataTablesJson();

            //try to get a product attribute with the specified id
            var productAttribute = await ProductAttributeService.GetProductAttributeByIdAsync(searchModel.ProductAttributeId)
                ?? throw new ArgumentException("No product attribute found with the specified id");

            //prepare model
            var model = await ProductAttributeModelFactory.PrepareProductAttributeProductListModelAsync(searchModel, productAttribute);

            return Json(model);
        }

        #endregion

        #region Predefined values

        [HttpPost]
        public virtual async Task<IActionResult> PredefinedProductAttributeValueList(PredefinedProductAttributeValueSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return await AccessDeniedDataTablesJson();

            //try to get a product attribute with the specified id
            var productAttribute = await ProductAttributeService.GetProductAttributeByIdAsync(searchModel.ProductAttributeId)
                ?? throw new ArgumentException("No product attribute found with the specified id");

            //prepare model
            var model = await ProductAttributeModelFactory.PreparePredefinedProductAttributeValueListModelAsync(searchModel, productAttribute);

            return Json(model);
        }

        public virtual async Task<IActionResult> PredefinedProductAttributeValueCreatePopup(int productAttributeId)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a product attribute with the specified id
            var productAttribute = await ProductAttributeService.GetProductAttributeByIdAsync(productAttributeId)
                ?? throw new ArgumentException("No product attribute found with the specified id", nameof(productAttributeId));

            //prepare model
            var model = await ProductAttributeModelFactory
                .PreparePredefinedProductAttributeValueModelAsync(new PredefinedProductAttributeValueModel(), productAttribute, null);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> PredefinedProductAttributeValueCreatePopup(PredefinedProductAttributeValueModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a product attribute with the specified id
            var productAttribute = await ProductAttributeService.GetProductAttributeByIdAsync(model.ProductAttributeId)
                ?? throw new ArgumentException("No product attribute found with the specified id");

            if (ModelState.IsValid)
            {
                //fill entity from model
                var ppav = model.ToEntity<PredefinedProductAttributeValue>();

                await ProductAttributeService.InsertPredefinedProductAttributeValueAsync(ppav);
                await UpdateLocalesAsync(ppav, model);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = await ProductAttributeModelFactory.PreparePredefinedProductAttributeValueModelAsync(model, productAttribute, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> PredefinedProductAttributeValueEditPopup(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a predefined product attribute value with the specified id
            var productAttributeValue = await ProductAttributeService.GetPredefinedProductAttributeValueByIdAsync(id)
                ?? throw new ArgumentException("No predefined product attribute value found with the specified id");

            //try to get a product attribute with the specified id
            var productAttribute = await ProductAttributeService.GetProductAttributeByIdAsync(productAttributeValue.ProductAttributeId)
                ?? throw new ArgumentException("No product attribute found with the specified id");

            //prepare model
            var model = await ProductAttributeModelFactory.PreparePredefinedProductAttributeValueModelAsync(null, productAttribute, productAttributeValue);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> PredefinedProductAttributeValueEditPopup(PredefinedProductAttributeValueModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a predefined product attribute value with the specified id
            var productAttributeValue = await ProductAttributeService.GetPredefinedProductAttributeValueByIdAsync(model.Id)
                ?? throw new ArgumentException("No predefined product attribute value found with the specified id");

            //try to get a product attribute with the specified id
            var productAttribute = await ProductAttributeService.GetProductAttributeByIdAsync(productAttributeValue.ProductAttributeId)
                ?? throw new ArgumentException("No product attribute found with the specified id");

            if (ModelState.IsValid)
            {
                productAttributeValue = model.ToEntity(productAttributeValue);
                await ProductAttributeService.UpdatePredefinedProductAttributeValueAsync(productAttributeValue);

                await UpdateLocalesAsync(productAttributeValue, model);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = await ProductAttributeModelFactory.PreparePredefinedProductAttributeValueModelAsync(model, productAttribute, productAttributeValue, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> PredefinedProductAttributeValueDelete(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a predefined product attribute value with the specified id
            var productAttributeValue = await ProductAttributeService.GetPredefinedProductAttributeValueByIdAsync(id)
                ?? throw new ArgumentException("No predefined product attribute value found with the specified id", nameof(id));

            await ProductAttributeService.DeletePredefinedProductAttributeValueAsync(productAttributeValue);

            return new NullJsonResult();
        }

        #endregion

        #endregion
    }
}