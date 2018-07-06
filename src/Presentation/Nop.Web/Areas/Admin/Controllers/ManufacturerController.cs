using System;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Discounts;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.ExportImport;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class ManufacturerController : BaseAdminController
    {
        #region Fields

        private readonly IAclService _aclService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICustomerService _customerService;
        private readonly IDiscountService _discountService;
        private readonly IExportManager _exportManager;
        private readonly IImportManager _importManager;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IManufacturerModelFactory _manufacturerModelFactory;
        private readonly IManufacturerService _manufacturerService;
        private readonly IPermissionService _permissionService;
        private readonly IPictureService _pictureService;
        private readonly IProductService _productService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IStoreService _storeService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public ManufacturerController(IAclService aclService,
            ICustomerActivityService customerActivityService,
            ICustomerService customerService,
            IDiscountService discountService,
            IExportManager exportManager,
            IImportManager importManager,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            IManufacturerModelFactory manufacturerModelFactory,
            IManufacturerService manufacturerService,
            IPermissionService permissionService,
            IPictureService pictureService,
            IProductService productService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IUrlRecordService urlRecordService,
            IWorkContext workContext)
        {
            this._aclService = aclService;
            this._customerActivityService = customerActivityService;
            this._customerService = customerService;
            this._discountService = discountService;
            this._exportManager = exportManager;
            this._importManager = importManager;
            this._localizationService = localizationService;
            this._localizedEntityService = localizedEntityService;
            this._manufacturerModelFactory = manufacturerModelFactory;
            this._manufacturerService = manufacturerService;
            this._permissionService = permissionService;
            this._pictureService = pictureService;
            this._productService = productService;
            this._storeMappingService = storeMappingService;
            this._storeService = storeService;
            this._urlRecordService = urlRecordService;
            this._workContext = workContext;
        }

        #endregion

        #region Utilities

        protected virtual void UpdateLocales(Manufacturer manufacturer, ManufacturerModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(manufacturer,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(manufacturer,
                    x => x.Description,
                    localized.Description,
                    localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(manufacturer,
                    x => x.MetaKeywords,
                    localized.MetaKeywords,
                    localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(manufacturer,
                    x => x.MetaDescription,
                    localized.MetaDescription,
                    localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(manufacturer,
                    x => x.MetaTitle,
                    localized.MetaTitle,
                    localized.LanguageId);

                //search engine name
                var seName = _urlRecordService.ValidateSeName(manufacturer, localized.SeName, localized.Name, false);
                _urlRecordService.SaveSlug(manufacturer, seName, localized.LanguageId);
            }
        }

        protected virtual void UpdatePictureSeoNames(Manufacturer manufacturer)
        {
            var picture = _pictureService.GetPictureById(manufacturer.PictureId);
            if (picture != null)
                _pictureService.SetSeoFilename(picture.Id, _pictureService.GetPictureSeName(manufacturer.Name));
        }

        protected virtual void SaveManufacturerAcl(Manufacturer manufacturer, ManufacturerModel model)
        {
            manufacturer.SubjectToAcl = model.SelectedCustomerRoleIds.Any();

            var existingAclRecords = _aclService.GetAclRecords(manufacturer);
            var allCustomerRoles = _customerService.GetAllCustomerRoles(true);
            foreach (var customerRole in allCustomerRoles)
            {
                if (model.SelectedCustomerRoleIds.Contains(customerRole.Id))
                {
                    //new role
                    if (existingAclRecords.Count(acl => acl.CustomerRoleId == customerRole.Id) == 0)
                        _aclService.InsertAclRecord(manufacturer, customerRole.Id);
                }
                else
                {
                    //remove role
                    var aclRecordToDelete = existingAclRecords.FirstOrDefault(acl => acl.CustomerRoleId == customerRole.Id);
                    if (aclRecordToDelete != null)
                        _aclService.DeleteAclRecord(aclRecordToDelete);
                }
            }
        }

        protected virtual void SaveStoreMappings(Manufacturer manufacturer, ManufacturerModel model)
        {
            manufacturer.LimitedToStores = model.SelectedStoreIds.Any();

            var existingStoreMappings = _storeMappingService.GetStoreMappings(manufacturer);
            var allStores = _storeService.GetAllStores();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds.Contains(store.Id))
                {
                    //new store
                    if (existingStoreMappings.Count(sm => sm.StoreId == store.Id) == 0)
                        _storeMappingService.InsertStoreMapping(manufacturer, store.Id);
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

        #endregion

        #region List

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            //prepare model
            var model = _manufacturerModelFactory.PrepareManufacturerSearchModel(new ManufacturerSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult List(ManufacturerSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedKendoGridJson();

            //prepare model
            var model = _manufacturerModelFactory.PrepareManufacturerListModel(searchModel);

            return Json(model);
        }

        #endregion

        #region Create / Edit / Delete

        public virtual IActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            //prepare model
            var model = _manufacturerModelFactory.PrepareManufacturerModel(new ManufacturerModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(ManufacturerModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var manufacturer = model.ToEntity<Manufacturer>();
                manufacturer.CreatedOnUtc = DateTime.UtcNow;
                manufacturer.UpdatedOnUtc = DateTime.UtcNow;
                _manufacturerService.InsertManufacturer(manufacturer);

                //search engine name
                model.SeName = _urlRecordService.ValidateSeName(manufacturer, model.SeName, manufacturer.Name, true);
                _urlRecordService.SaveSlug(manufacturer, model.SeName, 0);

                //locales
                UpdateLocales(manufacturer, model);

                //discounts
                var allDiscounts = _discountService.GetAllDiscounts(DiscountType.AssignedToManufacturers, showHidden: true);
                foreach (var discount in allDiscounts)
                {
                    if (model.SelectedDiscountIds != null && model.SelectedDiscountIds.Contains(discount.Id))
                        //manufacturer.AppliedDiscounts.Add(discount);
                        manufacturer.DiscountManufacturerMappings.Add(new DiscountManufacturerMapping { Discount = discount });
                }

                _manufacturerService.UpdateManufacturer(manufacturer);

                //update picture seo file name
                UpdatePictureSeoNames(manufacturer);

                //ACL (customer roles)
                SaveManufacturerAcl(manufacturer, model);

                //stores
                SaveStoreMappings(manufacturer, model);

                //activity log
                _customerActivityService.InsertActivity("AddNewManufacturer",
                    string.Format(_localizationService.GetResource("ActivityLog.AddNewManufacturer"), manufacturer.Name), manufacturer);

                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Manufacturers.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");

                //selected tab
                SaveSelectedTabName();

                return RedirectToAction("Edit", new { id = manufacturer.Id });
            }

            //prepare model
            model = _manufacturerModelFactory.PrepareManufacturerModel(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            //try to get a manufacturer with the specified id
            var manufacturer = _manufacturerService.GetManufacturerById(id);
            if (manufacturer == null || manufacturer.Deleted)
                return RedirectToAction("List");

            //prepare model
            var model = _manufacturerModelFactory.PrepareManufacturerModel(null, manufacturer);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(ManufacturerModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            //try to get a manufacturer with the specified id
            var manufacturer = _manufacturerService.GetManufacturerById(model.Id);
            if (manufacturer == null || manufacturer.Deleted)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                var prevPictureId = manufacturer.PictureId;
                manufacturer = model.ToEntity(manufacturer);
                manufacturer.UpdatedOnUtc = DateTime.UtcNow;
                _manufacturerService.UpdateManufacturer(manufacturer);

                //search engine name
                model.SeName = _urlRecordService.ValidateSeName(manufacturer, model.SeName, manufacturer.Name, true);
                _urlRecordService.SaveSlug(manufacturer, model.SeName, 0);

                //locales
                UpdateLocales(manufacturer, model);

                //discounts
                var allDiscounts = _discountService.GetAllDiscounts(DiscountType.AssignedToManufacturers, showHidden: true);
                foreach (var discount in allDiscounts)
                {
                    if (model.SelectedDiscountIds != null && model.SelectedDiscountIds.Contains(discount.Id))
                    {
                        //new discount
                        if (manufacturer.DiscountManufacturerMappings.Count(mapping => mapping.DiscountId == discount.Id) == 0)
                            manufacturer.DiscountManufacturerMappings.Add(new DiscountManufacturerMapping { Discount = discount });
                    }
                    else
                    {
                        //remove discount
                        if (manufacturer.DiscountManufacturerMappings.Count(mapping => mapping.DiscountId == discount.Id) > 0)
                            manufacturer.DiscountManufacturerMappings
                                .Remove(manufacturer.DiscountManufacturerMappings.FirstOrDefault(mapping => mapping.DiscountId == discount.Id));
                    }
                }

                _manufacturerService.UpdateManufacturer(manufacturer);

                //delete an old picture (if deleted or updated)
                if (prevPictureId > 0 && prevPictureId != manufacturer.PictureId)
                {
                    var prevPicture = _pictureService.GetPictureById(prevPictureId);
                    if (prevPicture != null)
                        _pictureService.DeletePicture(prevPicture);
                }

                //update picture seo file name
                UpdatePictureSeoNames(manufacturer);

                //ACL
                SaveManufacturerAcl(manufacturer, model);

                //stores
                SaveStoreMappings(manufacturer, model);

                //activity log
                _customerActivityService.InsertActivity("EditManufacturer",
                    string.Format(_localizationService.GetResource("ActivityLog.EditManufacturer"), manufacturer.Name), manufacturer);

                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Manufacturers.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");

                //selected tab
                SaveSelectedTabName();

                return RedirectToAction("Edit", new { id = manufacturer.Id });
            }

            //prepare model
            model = _manufacturerModelFactory.PrepareManufacturerModel(model, manufacturer, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            //try to get a manufacturer with the specified id
            var manufacturer = _manufacturerService.GetManufacturerById(id);
            if (manufacturer == null)
                return RedirectToAction("List");

            _manufacturerService.DeleteManufacturer(manufacturer);

            //activity log
            _customerActivityService.InsertActivity("DeleteManufacturer",
                string.Format(_localizationService.GetResource("ActivityLog.DeleteManufacturer"), manufacturer.Name), manufacturer);

            SuccessNotification(_localizationService.GetResource("Admin.Catalog.Manufacturers.Deleted"));

            return RedirectToAction("List");
        }

        #endregion

        #region Export / Import

        public virtual IActionResult ExportXml()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            try
            {
                var manufacturers = _manufacturerService.GetAllManufacturers(showHidden: true);
                var xml = _exportManager.ExportManufacturersToXml(manufacturers);
                return File(Encoding.UTF8.GetBytes(xml), "application/xml", "manufacturers.xml");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        public virtual IActionResult ExportXlsx()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            try
            {
                var bytes = _exportManager.ExportManufacturersToXlsx(_manufacturerService.GetAllManufacturers(showHidden: true).Where(p => !p.Deleted));

                return File(bytes, MimeTypes.TextXlsx, "manufacturers.xlsx");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        public virtual IActionResult ImportFromXlsx(IFormFile importexcelfile)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            //a vendor cannot import manufacturers
            if (_workContext.CurrentVendor != null)
                return AccessDeniedView();

            try
            {
                if (importexcelfile != null && importexcelfile.Length > 0)
                {
                    _importManager.ImportManufacturersFromXlsx(importexcelfile.OpenReadStream());
                }
                else
                {
                    ErrorNotification(_localizationService.GetResource("Admin.Common.UploadFile"));
                    return RedirectToAction("List");
                }

                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Manufacturers.Imported"));
                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        #endregion

        #region Products

        [HttpPost]
        public virtual IActionResult ProductList(ManufacturerProductSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedKendoGridJson();

            //try to get a manufacturer with the specified id
            var manufacturer = _manufacturerService.GetManufacturerById(searchModel.ManufacturerId)
                ?? throw new ArgumentException("No manufacturer found with the specified id");

            //prepare model
            var model = _manufacturerModelFactory.PrepareManufacturerProductListModel(searchModel, manufacturer);

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult ProductUpdate(ManufacturerProductModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            //try to get a product manufacturer with the specified id
            var productManufacturer = _manufacturerService.GetProductManufacturerById(model.Id)
                ?? throw new ArgumentException("No product manufacturer mapping found with the specified id");

            productManufacturer.IsFeaturedProduct = model.IsFeaturedProduct;
            productManufacturer.DisplayOrder = model.DisplayOrder;
            _manufacturerService.UpdateProductManufacturer(productManufacturer);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual IActionResult ProductDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            //try to get a product manufacturer with the specified id
            var productManufacturer = _manufacturerService.GetProductManufacturerById(id)
                ?? throw new ArgumentException("No product manufacturer mapping found with the specified id");

            _manufacturerService.DeleteProductManufacturer(productManufacturer);

            return new NullJsonResult();
        }

        public virtual IActionResult ProductAddPopup(int manufacturerId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            //prepare model
            var model = _manufacturerModelFactory.PrepareAddProductToManufacturerSearchModel(new AddProductToManufacturerSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult ProductAddPopupList(AddProductToManufacturerSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedKendoGridJson();

            //prepare model
            var model = _manufacturerModelFactory.PrepareAddProductToManufacturerListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual IActionResult ProductAddPopup(AddProductToManufacturerModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            //get selected products
            var selectedProducts = _productService.GetProductsByIds(model.SelectedProductIds.ToArray());
            if (selectedProducts.Any())
            {
                var existingProductmanufacturers = _manufacturerService
                    .GetProductManufacturersByManufacturerId(model.ManufacturerId, showHidden: true);
                foreach (var product in selectedProducts)
                {
                    //whether product manufacturer with such parameters already exists
                    if (_manufacturerService.FindProductManufacturer(existingProductmanufacturers, product.Id, model.ManufacturerId) != null)
                        continue;

                    //insert the new product manufacturer mapping
                    _manufacturerService.InsertProductManufacturer(new ProductManufacturer
                    {
                        ManufacturerId = model.ManufacturerId,
                        ProductId = product.Id,
                        IsFeaturedProduct = false,
                        DisplayOrder = 1
                    });
                }
            }

            ViewBag.RefreshPage = true;

            return View(new AddProductToManufacturerSearchModel());
        }

        #endregion
    }
}