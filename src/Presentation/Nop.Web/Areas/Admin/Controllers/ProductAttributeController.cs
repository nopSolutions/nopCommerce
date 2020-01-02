using System;
using System.Collections.Generic;
using System.Linq;
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

        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly IProductAttributeModelFactory _productAttributeModelFactory;
        private readonly IProductAttributeService _productAttributeService;

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

        protected virtual void UpdateLocales(ProductAttribute productAttribute, ProductAttributeModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(productAttribute,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(productAttribute,
                    x => x.Description,
                    localized.Description,
                    localized.LanguageId);
            }
        }

        protected virtual void UpdateLocales(PredefinedProductAttributeValue ppav, PredefinedProductAttributeValueModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(ppav,
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

        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //prepare model
            var model = _productAttributeModelFactory.PrepareProductAttributeSearchModel(new ProductAttributeSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult List(ProductAttributeSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _productAttributeModelFactory.PrepareProductAttributeListModel(searchModel);

            return Json(model);
        }

        public virtual IActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //prepare model
            var model = _productAttributeModelFactory.PrepareProductAttributeModel(new ProductAttributeModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(ProductAttributeModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var productAttribute = model.ToEntity<ProductAttribute>();
                _productAttributeService.InsertProductAttribute(productAttribute);
                UpdateLocales(productAttribute, model);

                //activity log
                _customerActivityService.InsertActivity("AddNewProductAttribute",
                    string.Format(_localizationService.GetResource("ActivityLog.AddNewProductAttribute"), productAttribute.Name), productAttribute);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Catalog.Attributes.ProductAttributes.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");
                
                return RedirectToAction("Edit", new { id = productAttribute.Id });
            }

            //prepare model
            model = _productAttributeModelFactory.PrepareProductAttributeModel(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a product attribute with the specified id
            var productAttribute = _productAttributeService.GetProductAttributeById(id);
            if (productAttribute == null)
                return RedirectToAction("List");

            //prepare model
            var model = _productAttributeModelFactory.PrepareProductAttributeModel(null, productAttribute);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(ProductAttributeModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a product attribute with the specified id
            var productAttribute = _productAttributeService.GetProductAttributeById(model.Id);
            if (productAttribute == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                productAttribute = model.ToEntity(productAttribute);
                _productAttributeService.UpdateProductAttribute(productAttribute);

                UpdateLocales(productAttribute, model);

                //activity log
                _customerActivityService.InsertActivity("EditProductAttribute",
                    string.Format(_localizationService.GetResource("ActivityLog.EditProductAttribute"), productAttribute.Name), productAttribute);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Catalog.Attributes.ProductAttributes.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");
                
                return RedirectToAction("Edit", new { id = productAttribute.Id });
            }

            //prepare model
            model = _productAttributeModelFactory.PrepareProductAttributeModel(model, productAttribute, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a product attribute with the specified id
            var productAttribute = _productAttributeService.GetProductAttributeById(id);
            if (productAttribute == null)
                return RedirectToAction("List");

            _productAttributeService.DeleteProductAttribute(productAttribute);

            //activity log
            _customerActivityService.InsertActivity("DeleteProductAttribute",
                string.Format(_localizationService.GetResource("ActivityLog.DeleteProductAttribute"), productAttribute.Name), productAttribute);

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Catalog.Attributes.ProductAttributes.Deleted"));

            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual IActionResult DeleteSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            if (selectedIds != null)
            {
                var productAttributes = _productAttributeService.GetProductAttributeByIds(selectedIds.ToArray());
                _productAttributeService.DeleteProductAttributes(productAttributes);

                foreach (var productAttribute in productAttributes)
                {
                    _customerActivityService.InsertActivity("DeleteProductAttribute",
                        string.Format(_localizationService.GetResource("ActivityLog.DeleteProductAttribute"), productAttribute.Name), productAttribute);
                }
            }

            return Json(new { Result = true });
        }

        #endregion

        #region Used by products

        [HttpPost]
        public virtual IActionResult UsedByProducts(ProductAttributeProductSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedDataTablesJson();

            //try to get a product attribute with the specified id
            var productAttribute = _productAttributeService.GetProductAttributeById(searchModel.ProductAttributeId)
                ?? throw new ArgumentException("No product attribute found with the specified id");

            //prepare model
            var model = _productAttributeModelFactory.PrepareProductAttributeProductListModel(searchModel, productAttribute);

            return Json(model);
        }

        #endregion

        #region Predefined values

        [HttpPost]
        public virtual IActionResult PredefinedProductAttributeValueList(PredefinedProductAttributeValueSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedDataTablesJson();

            //try to get a product attribute with the specified id
            var productAttribute = _productAttributeService.GetProductAttributeById(searchModel.ProductAttributeId)
                ?? throw new ArgumentException("No product attribute found with the specified id");

            //prepare model
            var model = _productAttributeModelFactory.PreparePredefinedProductAttributeValueListModel(searchModel, productAttribute);

            return Json(model);
        }

        public virtual IActionResult PredefinedProductAttributeValueCreatePopup(int productAttributeId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a product attribute with the specified id
            var productAttribute = _productAttributeService.GetProductAttributeById(productAttributeId)
                ?? throw new ArgumentException("No product attribute found with the specified id", nameof(productAttributeId));

            //prepare model
            var model = _productAttributeModelFactory
                .PreparePredefinedProductAttributeValueModel(new PredefinedProductAttributeValueModel(), productAttribute, null);

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult PredefinedProductAttributeValueCreatePopup(PredefinedProductAttributeValueModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a product attribute with the specified id
            var productAttribute = _productAttributeService.GetProductAttributeById(model.ProductAttributeId)
                ?? throw new ArgumentException("No product attribute found with the specified id");

            if (ModelState.IsValid)
            {
                //fill entity from model
                var ppav = model.ToEntity<PredefinedProductAttributeValue>();

                _productAttributeService.InsertPredefinedProductAttributeValue(ppav);
                UpdateLocales(ppav, model);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = _productAttributeModelFactory.PreparePredefinedProductAttributeValueModel(model, productAttribute, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual IActionResult PredefinedProductAttributeValueEditPopup(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a predefined product attribute value with the specified id
            var productAttributeValue = _productAttributeService.GetPredefinedProductAttributeValueById(id)
                ?? throw new ArgumentException("No predefined product attribute value found with the specified id");

            //try to get a product attribute with the specified id
            var productAttribute = _productAttributeService.GetProductAttributeById(productAttributeValue.ProductAttributeId)
                ?? throw new ArgumentException("No product attribute found with the specified id");

            //prepare model
            var model = _productAttributeModelFactory.PreparePredefinedProductAttributeValueModel(null, productAttribute, productAttributeValue);

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult PredefinedProductAttributeValueEditPopup(PredefinedProductAttributeValueModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a predefined product attribute value with the specified id
            var productAttributeValue = _productAttributeService.GetPredefinedProductAttributeValueById(model.Id)
                ?? throw new ArgumentException("No predefined product attribute value found with the specified id");

            //try to get a product attribute with the specified id
            var productAttribute = _productAttributeService.GetProductAttributeById(productAttributeValue.ProductAttributeId)
                ?? throw new ArgumentException("No product attribute found with the specified id");

            if (ModelState.IsValid)
            {
                productAttributeValue = model.ToEntity(productAttributeValue);
                _productAttributeService.UpdatePredefinedProductAttributeValue(productAttributeValue);

                UpdateLocales(productAttributeValue, model);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = _productAttributeModelFactory.PreparePredefinedProductAttributeValueModel(model, productAttribute, productAttributeValue, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult PredefinedProductAttributeValueDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a predefined product attribute value with the specified id
            var productAttributeValue = _productAttributeService.GetPredefinedProductAttributeValueById(id)
                ?? throw new ArgumentException("No predefined product attribute value found with the specified id", nameof(id));

            _productAttributeService.DeletePredefinedProductAttributeValue(productAttributeValue);

            return new NullJsonResult();
        }

        #endregion

        #endregion
    }
}