using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using Nop.Services.Messages;
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
        private readonly INotificationService _notificationService;
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
            INotificationService notificationService,
            IPermissionService permissionService,
            IPictureService pictureService,
            IProductService productService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IUrlRecordService urlRecordService,
            IWorkContext workContext)
        {
            _aclService = aclService;
            _customerActivityService = customerActivityService;
            _customerService = customerService;
            _discountService = discountService;
            _exportManager = exportManager;
            _importManager = importManager;
            _localizationService = localizationService;
            _localizedEntityService = localizedEntityService;
            _manufacturerModelFactory = manufacturerModelFactory;
            _manufacturerService = manufacturerService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _pictureService = pictureService;
            _productService = productService;
            _storeMappingService = storeMappingService;
            _storeService = storeService;
            _urlRecordService = urlRecordService;
            _workContext = workContext;
        }

        #endregion

        #region Utilities

        protected virtual async Task UpdateLocales(Manufacturer manufacturer, ManufacturerModel model)
        {
            foreach (var localized in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValue(manufacturer,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);

                await _localizedEntityService.SaveLocalizedValue(manufacturer,
                    x => x.Description,
                    localized.Description,
                    localized.LanguageId);

                await _localizedEntityService.SaveLocalizedValue(manufacturer,
                    x => x.MetaKeywords,
                    localized.MetaKeywords,
                    localized.LanguageId);

                await _localizedEntityService.SaveLocalizedValue(manufacturer,
                    x => x.MetaDescription,
                    localized.MetaDescription,
                    localized.LanguageId);

                await _localizedEntityService.SaveLocalizedValue(manufacturer,
                    x => x.MetaTitle,
                    localized.MetaTitle,
                    localized.LanguageId);

                //search engine name
                var seName = await _urlRecordService.ValidateSeName(manufacturer, localized.SeName, localized.Name, false);
                await _urlRecordService.SaveSlug(manufacturer, seName, localized.LanguageId);
            }
        }

        protected virtual async Task UpdatePictureSeoNames(Manufacturer manufacturer)
        {
            var picture = await _pictureService.GetPictureById(manufacturer.PictureId);
            if (picture != null)
                await _pictureService.SetSeoFilename(picture.Id, await _pictureService.GetPictureSeName(manufacturer.Name));
        }

        protected virtual async Task SaveManufacturerAcl(Manufacturer manufacturer, ManufacturerModel model)
        {
            manufacturer.SubjectToAcl = model.SelectedCustomerRoleIds.Any();
            await _manufacturerService.UpdateManufacturer(manufacturer);

            var existingAclRecords = await _aclService.GetAclRecords(manufacturer);
            var allCustomerRoles = await _customerService.GetAllCustomerRoles(true);
            foreach (var customerRole in allCustomerRoles)
            {
                if (model.SelectedCustomerRoleIds.Contains(customerRole.Id))
                {
                    //new role
                    if (existingAclRecords.Count(acl => acl.CustomerRoleId == customerRole.Id) == 0)
                        await _aclService.InsertAclRecord(manufacturer, customerRole.Id);
                }
                else
                {
                    //remove role
                    var aclRecordToDelete = existingAclRecords.FirstOrDefault(acl => acl.CustomerRoleId == customerRole.Id);
                    if (aclRecordToDelete != null)
                        await _aclService.DeleteAclRecord(aclRecordToDelete);
                }
            }
        }

        protected virtual async Task SaveStoreMappings(Manufacturer manufacturer, ManufacturerModel model)
        {
            manufacturer.LimitedToStores = model.SelectedStoreIds.Any();
            await _manufacturerService.UpdateManufacturer(manufacturer);

            var existingStoreMappings = await _storeMappingService.GetStoreMappings(manufacturer);
            var allStores = await _storeService.GetAllStores();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds.Contains(store.Id))
                {
                    //new store
                    if (existingStoreMappings.Count(sm => sm.StoreId == store.Id) == 0)
                        await _storeMappingService.InsertStoreMapping(manufacturer, store.Id);
                }
                else
                {
                    //remove store
                    var storeMappingToDelete = existingStoreMappings.FirstOrDefault(sm => sm.StoreId == store.Id);
                    if (storeMappingToDelete != null)
                        await _storeMappingService.DeleteStoreMapping(storeMappingToDelete);
                }
            }
        }

        #endregion

        #region List

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual async Task<IActionResult> List()
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            //prepare model
            var model = await _manufacturerModelFactory.PrepareManufacturerSearchModel(new ManufacturerSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(ManufacturerSearchModel searchModel)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = await _manufacturerModelFactory.PrepareManufacturerListModel(searchModel);

            return Json(model);
        }

        #endregion

        #region Create / Edit / Delete

        public virtual async Task<IActionResult> Create()
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            //prepare model
            var model = await _manufacturerModelFactory.PrepareManufacturerModel(new ManufacturerModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Create(ManufacturerModel model, bool continueEditing)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var manufacturer = model.ToEntity<Manufacturer>();
                manufacturer.CreatedOnUtc = DateTime.UtcNow;
                manufacturer.UpdatedOnUtc = DateTime.UtcNow;
                await _manufacturerService.InsertManufacturer(manufacturer);

                //search engine name
                model.SeName = await _urlRecordService.ValidateSeName(manufacturer, model.SeName, manufacturer.Name, true);
                await _urlRecordService.SaveSlug(manufacturer, model.SeName, 0);

                //locales
                await UpdateLocales(manufacturer, model);

                //discounts
                var allDiscounts = await _discountService.GetAllDiscounts(DiscountType.AssignedToManufacturers, showHidden: true);
                foreach (var discount in allDiscounts)
                {
                    if (model.SelectedDiscountIds != null && model.SelectedDiscountIds.Contains(discount.Id))
                        //manufacturer.AppliedDiscounts.Add(discount);
                        await _manufacturerService.InsertDiscountManufacturerMapping(new DiscountManufacturerMapping { EntityId = manufacturer.Id, DiscountId = discount.Id });

                }

                await _manufacturerService.UpdateManufacturer(manufacturer);

                //update picture seo file name
                await UpdatePictureSeoNames(manufacturer);

                //ACL (customer roles)
                await SaveManufacturerAcl(manufacturer, model);

                //stores
                await SaveStoreMappings(manufacturer, model);

                //activity log
                await _customerActivityService.InsertActivity("AddNewManufacturer",
                    string.Format(await _localizationService.GetResource("ActivityLog.AddNewManufacturer"), manufacturer.Name), manufacturer);

                _notificationService.SuccessNotification(await _localizationService.GetResource("Admin.Catalog.Manufacturers.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");
                
                return RedirectToAction("Edit", new { id = manufacturer.Id });
            }

            //prepare model
            model = await _manufacturerModelFactory.PrepareManufacturerModel(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            //try to get a manufacturer with the specified id
            var manufacturer = await _manufacturerService.GetManufacturerById(id);
            if (manufacturer == null || manufacturer.Deleted)
                return RedirectToAction("List");

            //prepare model
            var model = await _manufacturerModelFactory.PrepareManufacturerModel(null, manufacturer);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Edit(ManufacturerModel model, bool continueEditing)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            //try to get a manufacturer with the specified id
            var manufacturer = await _manufacturerService.GetManufacturerById(model.Id);
            if (manufacturer == null || manufacturer.Deleted)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                var prevPictureId = manufacturer.PictureId;
                manufacturer = model.ToEntity(manufacturer);
                manufacturer.UpdatedOnUtc = DateTime.UtcNow;
                await _manufacturerService.UpdateManufacturer(manufacturer);

                //search engine name
                model.SeName = await _urlRecordService.ValidateSeName(manufacturer, model.SeName, manufacturer.Name, true);
                await _urlRecordService.SaveSlug(manufacturer, model.SeName, 0);

                //locales
                await UpdateLocales(manufacturer, model);

                //discounts
                var allDiscounts = await _discountService.GetAllDiscounts(DiscountType.AssignedToManufacturers, showHidden: true);
                foreach (var discount in allDiscounts)
                {
                    if (model.SelectedDiscountIds != null && model.SelectedDiscountIds.Contains(discount.Id))
                    {
                        //new discount
                        if (_manufacturerService.GetDiscountAppliedToManufacturer(manufacturer.Id, discount.Id) is null)
                            await _manufacturerService.InsertDiscountManufacturerMapping(new DiscountManufacturerMapping { EntityId = manufacturer.Id, DiscountId = discount.Id });
                    }
                    else
                    {
                        //remove discount
                        if (await _manufacturerService.GetDiscountAppliedToManufacturer(manufacturer.Id, discount.Id) is DiscountManufacturerMapping discountManufacturerMapping)
                            await _manufacturerService.DeleteDiscountManufacturerMapping(discountManufacturerMapping);
                    }
                }

                await _manufacturerService.UpdateManufacturer(manufacturer);

                //delete an old picture (if deleted or updated)
                if (prevPictureId > 0 && prevPictureId != manufacturer.PictureId)
                {
                    var prevPicture = await _pictureService.GetPictureById(prevPictureId);
                    if (prevPicture != null)
                        await _pictureService.DeletePicture(prevPicture);
                }

                //update picture seo file name
                await UpdatePictureSeoNames(manufacturer);

                //ACL
                await SaveManufacturerAcl(manufacturer, model);

                //stores
                await SaveStoreMappings(manufacturer, model);

                //activity log
                await _customerActivityService.InsertActivity("EditManufacturer",
                    string.Format(await _localizationService.GetResource("ActivityLog.EditManufacturer"), manufacturer.Name), manufacturer);

                _notificationService.SuccessNotification(await _localizationService.GetResource("Admin.Catalog.Manufacturers.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");
                
                return RedirectToAction("Edit", new { id = manufacturer.Id });
            }

            //prepare model
            model = await _manufacturerModelFactory.PrepareManufacturerModel(model, manufacturer, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            //try to get a manufacturer with the specified id
            var manufacturer = await _manufacturerService.GetManufacturerById(id);
            if (manufacturer == null)
                return RedirectToAction("List");

            await _manufacturerService.DeleteManufacturer(manufacturer);

            //activity log
            await _customerActivityService.InsertActivity("DeleteManufacturer",
                string.Format(await _localizationService.GetResource("ActivityLog.DeleteManufacturer"), manufacturer.Name), manufacturer);

            _notificationService.SuccessNotification(await _localizationService.GetResource("Admin.Catalog.Manufacturers.Deleted"));

            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected(ICollection<int> selectedIds)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            if (selectedIds != null)
            {
                var manufacturers = await _manufacturerService.GetManufacturersByIds(selectedIds.ToArray());
                await _manufacturerService.DeleteManufacturers(manufacturers);

                manufacturers.ForEach(manufacturer => 
                {
                    //activity log
                    _customerActivityService.InsertActivity("DeleteManufacturer",
                        string.Format(_localizationService.GetResource("ActivityLog.DeleteManufacturer").Result, manufacturer.Name), manufacturer).Wait();
                });
            }

            return Json(new { Result = true });
        }

        #endregion

        #region Export / Import

        public virtual async Task<IActionResult> ExportXml()
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            try
            {
                var manufacturers = await _manufacturerService.GetAllManufacturers(showHidden: true);
                var xml = await _exportManager.ExportManufacturersToXml(manufacturers);
                return File(Encoding.UTF8.GetBytes(xml), "application/xml", "manufacturers.xml");
            }
            catch (Exception exc)
            {
                _notificationService.ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        public virtual async Task<IActionResult> ExportXlsx()
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            try
            {
                var bytes = await _exportManager.ExportManufacturersToXlsx((await _manufacturerService.GetAllManufacturers(showHidden: true)).Where(p => !p.Deleted));

                return File(bytes, MimeTypes.TextXlsx, "manufacturers.xlsx");
            }
            catch (Exception exc)
            {
                _notificationService.ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> ImportFromXlsx(IFormFile importexcelfile)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            //a vendor cannot import manufacturers
            if (await _workContext.GetCurrentVendor() != null)
                return AccessDeniedView();

            try
            {
                if (importexcelfile != null && importexcelfile.Length > 0)
                {
                    await _importManager.ImportManufacturersFromXlsx(importexcelfile.OpenReadStream());
                }
                else
                {
                    _notificationService.ErrorNotification(await _localizationService.GetResource("Admin.Common.UploadFile"));
                    return RedirectToAction("List");
                }

                _notificationService.SuccessNotification(await _localizationService.GetResource("Admin.Catalog.Manufacturers.Imported"));
                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                _notificationService.ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        #endregion

        #region Products

        [HttpPost]
        public virtual async Task<IActionResult> ProductList(ManufacturerProductSearchModel searchModel)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedDataTablesJson();

            //try to get a manufacturer with the specified id
            var manufacturer = await _manufacturerService.GetManufacturerById(searchModel.ManufacturerId)
                ?? throw new ArgumentException("No manufacturer found with the specified id");

            //prepare model
            var model = await _manufacturerModelFactory.PrepareManufacturerProductListModel(searchModel, manufacturer);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ProductUpdate(ManufacturerProductModel model)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            //try to get a product manufacturer with the specified id
            var productManufacturer = await _manufacturerService.GetProductManufacturerById(model.Id)
                ?? throw new ArgumentException("No product manufacturer mapping found with the specified id");

            //fill entity from model
            productManufacturer = model.ToEntity(productManufacturer);
            await _manufacturerService.UpdateProductManufacturer(productManufacturer);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual async Task<IActionResult> ProductDelete(int id)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            //try to get a product manufacturer with the specified id
            var productManufacturer = await _manufacturerService.GetProductManufacturerById(id)
                ?? throw new ArgumentException("No product manufacturer mapping found with the specified id");

            await _manufacturerService.DeleteProductManufacturer(productManufacturer);

            return new NullJsonResult();
        }

        public virtual async Task<IActionResult> ProductAddPopup(int manufacturerId)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            //prepare model
            var model = await _manufacturerModelFactory.PrepareAddProductToManufacturerSearchModel(new AddProductToManufacturerSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ProductAddPopupList(AddProductToManufacturerSearchModel searchModel)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = await _manufacturerModelFactory.PrepareAddProductToManufacturerListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual async Task<IActionResult> ProductAddPopup(AddProductToManufacturerModel model)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            //get selected products
            var selectedProducts = await _productService.GetProductsByIds(model.SelectedProductIds.ToArray());
            if (selectedProducts.Any())
            {
                var existingProductmanufacturers = await _manufacturerService
                    .GetProductManufacturersByManufacturerId(model.ManufacturerId, showHidden: true);
                foreach (var product in selectedProducts)
                {
                    //whether product manufacturer with such parameters already exists
                    if (_manufacturerService.FindProductManufacturer(existingProductmanufacturers, product.Id, model.ManufacturerId) != null)
                        continue;

                    //insert the new product manufacturer mapping
                    await _manufacturerService.InsertProductManufacturer(new ProductManufacturer
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