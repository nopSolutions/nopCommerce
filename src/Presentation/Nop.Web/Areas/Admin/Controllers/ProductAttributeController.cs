using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Areas.Admin.Extensions;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Security;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class ProductAttributeController : BaseAdminController
    {
        #region Fields

        private readonly IProductService _productService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IPermissionService _permissionService;

        #endregion Fields

        #region Ctor

        public ProductAttributeController(IProductService productService,
            IProductAttributeService productAttributeService,
            ILanguageService languageService,
            ILocalizedEntityService localizedEntityService,
            ILocalizationService localizationService,
            ICustomerActivityService customerActivityService,
            IPermissionService permissionService)
        {
            this._productService = productService;
            this._productAttributeService = productAttributeService;
            this._languageService = languageService;
            this._localizedEntityService = localizedEntityService;
            this._localizationService = localizationService;
            this._customerActivityService = customerActivityService;
            this._permissionService = permissionService;
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

        //list
        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public virtual IActionResult List(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedKendoGridJson();

            var productAttributes = _productAttributeService
                .GetAllProductAttributes(command.Page - 1, command.PageSize);
            var gridModel = new DataSourceResult
            {
                Data = productAttributes.Select(x => x.ToModel()),
                Total = productAttributes.TotalCount
            };

            return Json(gridModel);
        }
        
        //create
        public virtual IActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var model = new ProductAttributeModel();
            //locales
            AddLocales(_languageService, model.Locales);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(ProductAttributeModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var productAttribute = model.ToEntity();
                _productAttributeService.InsertProductAttribute(productAttribute);
                UpdateLocales(productAttribute, model);

                //activity log
                _customerActivityService.InsertActivity("AddNewProductAttribute", _localizationService.GetResource("ActivityLog.AddNewProductAttribute"), productAttribute.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Attributes.ProductAttributes.Added"));

                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { id = productAttribute.Id });
                }
                return RedirectToAction("List");

            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        //edit
        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var productAttribute = _productAttributeService.GetProductAttributeById(id);
            if (productAttribute == null)
                //No product attribute found with the specified id
                return RedirectToAction("List");

            var model = productAttribute.ToModel();
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = productAttribute.GetLocalized(x => x.Name, languageId, false, false);
                locale.Description = productAttribute.GetLocalized(x => x.Description, languageId, false, false);
            });

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(ProductAttributeModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var productAttribute = _productAttributeService.GetProductAttributeById(model.Id);
            if (productAttribute == null)
                //No product attribute found with the specified id
                return RedirectToAction("List");
            
            if (ModelState.IsValid)
            {
                productAttribute = model.ToEntity(productAttribute);
                _productAttributeService.UpdateProductAttribute(productAttribute);

                UpdateLocales(productAttribute, model);

                //activity log
                _customerActivityService.InsertActivity("EditProductAttribute", _localizationService.GetResource("ActivityLog.EditProductAttribute"), productAttribute.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Attributes.ProductAttributes.Updated"));
                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { id = productAttribute.Id });
                }
                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        //delete
        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var productAttribute = _productAttributeService.GetProductAttributeById(id);
            if (productAttribute == null)
                //No product attribute found with the specified id
                return RedirectToAction("List");

            _productAttributeService.DeleteProductAttribute(productAttribute);

            //activity log
            _customerActivityService.InsertActivity("DeleteProductAttribute", _localizationService.GetResource("ActivityLog.DeleteProductAttribute"), productAttribute.Name);

            SuccessNotification(_localizationService.GetResource("Admin.Catalog.Attributes.ProductAttributes.Deleted"));
            return RedirectToAction("List");
        }

        #endregion

        #region Used by products

        //used by products
        [HttpPost]
        public virtual IActionResult UsedByProducts(DataSourceRequest command, int productAttributeId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedKendoGridJson();

            var products = _productService.GetProductsByProductAtributeId(
                productAttributeId: productAttributeId,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize);
            var gridModel = new DataSourceResult
            {
                Data = products.Select(x =>
                {
                    return new ProductAttributeModel.UsedByProductModel
                    {
                        Id = x.Id,
                        ProductName = x.Name,
                        Published = x.Published
                    };
                }),
                Total = products.TotalCount
            };

            return Json(gridModel);
        }
        
        #endregion

        #region Predefined values

        [HttpPost]
        public virtual IActionResult PredefinedProductAttributeValueList(int productAttributeId, DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedKendoGridJson();

            var values = _productAttributeService.GetPredefinedProductAttributeValues(productAttributeId);
            var gridModel = new DataSourceResult
            {
                Data = values.Select(x =>
                {
                    return new PredefinedProductAttributeValueModel
                    {
                        Id = x.Id,
                        ProductAttributeId = x.ProductAttributeId,
                        Name = x.Name,
                        PriceAdjustment = x.PriceAdjustment,
                        PriceAdjustmentStr = x.PriceAdjustment.ToString("G29"),
                        WeightAdjustment = x.WeightAdjustment,
                        WeightAdjustmentStr = x.WeightAdjustment.ToString("G29"),
                        Cost = x.Cost,
                        IsPreSelected = x.IsPreSelected,
                        DisplayOrder = x.DisplayOrder
                    };
                }),
                Total = values.Count()
            };

            return Json(gridModel);
        }

        //create
        public virtual IActionResult PredefinedProductAttributeValueCreatePopup(int productAttributeId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var productAttribute = _productAttributeService.GetProductAttributeById(productAttributeId);
            if (productAttribute == null)
                throw new ArgumentException("No product attribute found with the specified id");

            var model = new PredefinedProductAttributeValueModel
            {
                ProductAttributeId = productAttributeId
            };

            //locales
            AddLocales(_languageService, model.Locales);

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult PredefinedProductAttributeValueCreatePopup(PredefinedProductAttributeValueModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var productAttribute = _productAttributeService.GetProductAttributeById(model.ProductAttributeId);
            if (productAttribute == null)
                throw new ArgumentException("No product attribute found with the specified id");

            if (ModelState.IsValid)
            {
                var ppav = new PredefinedProductAttributeValue
                {
                    ProductAttributeId = model.ProductAttributeId,
                    Name = model.Name,
                    PriceAdjustment = model.PriceAdjustment,
                    WeightAdjustment = model.WeightAdjustment,
                    Cost = model.Cost,
                    IsPreSelected = model.IsPreSelected,
                    DisplayOrder = model.DisplayOrder
                };

                _productAttributeService.InsertPredefinedProductAttributeValue(ppav);
                UpdateLocales(ppav, model);

                ViewBag.RefreshPage = true;
                return View(model);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        //edit
        public virtual IActionResult PredefinedProductAttributeValueEditPopup(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var ppav = _productAttributeService.GetPredefinedProductAttributeValueById(id);
            if (ppav == null)
                throw new ArgumentException("No product attribute value found with the specified id");

            var model = new PredefinedProductAttributeValueModel
            {
                ProductAttributeId = ppav.ProductAttributeId,
                Name = ppav.Name,
                PriceAdjustment = ppav.PriceAdjustment,
                WeightAdjustment = ppav.WeightAdjustment,
                Cost = ppav.Cost,
                IsPreSelected = ppav.IsPreSelected,
                DisplayOrder = ppav.DisplayOrder
            };
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = ppav.GetLocalized(x => x.Name, languageId, false, false);
            });
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult PredefinedProductAttributeValueEditPopup(PredefinedProductAttributeValueModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var ppav = _productAttributeService.GetPredefinedProductAttributeValueById(model.Id);
            if (ppav == null)
                throw new ArgumentException("No product attribute value found with the specified id");

            if (ModelState.IsValid)
            {
                ppav.Name = model.Name;
                ppav.PriceAdjustment = model.PriceAdjustment;
                ppav.WeightAdjustment = model.WeightAdjustment;
                ppav.Cost = model.Cost;
                ppav.IsPreSelected = model.IsPreSelected;
                ppav.DisplayOrder = model.DisplayOrder;
                _productAttributeService.UpdatePredefinedProductAttributeValue(ppav);

                UpdateLocales(ppav, model);

                ViewBag.RefreshPage = true;
                return View(model);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        //delete
        [HttpPost]
        public virtual IActionResult PredefinedProductAttributeValueDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var ppav = _productAttributeService.GetPredefinedProductAttributeValueById(id);
            if (ppav == null)
                throw new ArgumentException("No predefined product attribute value found with the specified id");

            _productAttributeService.DeletePredefinedProductAttributeValue(ppav);

            return new NullJsonResult();
        }

        #endregion

        #endregion
    }
}