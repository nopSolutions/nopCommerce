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

        protected IAclService AclService { get; }
        protected ICustomerActivityService CustomerActivityService { get; }
        protected ICustomerService CustomerService { get; }
        protected IDiscountService DiscountService { get; }
        protected IExportManager ExportManager { get; }
        protected IImportManager ImportManager { get; }
        protected ILocalizationService LocalizationService { get; }
        protected ILocalizedEntityService LocalizedEntityService { get; }
        protected IManufacturerModelFactory ManufacturerModelFactory { get; }
        protected IManufacturerService ManufacturerService { get; }
        protected INotificationService NotificationService { get; }
        protected IPermissionService PermissionService { get; }
        protected IPictureService PictureService { get; }
        protected IProductService ProductService { get; }
        protected IStoreMappingService StoreMappingService { get; }
        protected IStoreService StoreService { get; }
        protected IUrlRecordService UrlRecordService { get; }
        protected IWorkContext WorkContext { get; }

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
            AclService = aclService;
            CustomerActivityService = customerActivityService;
            CustomerService = customerService;
            DiscountService = discountService;
            ExportManager = exportManager;
            ImportManager = importManager;
            LocalizationService = localizationService;
            LocalizedEntityService = localizedEntityService;
            ManufacturerModelFactory = manufacturerModelFactory;
            ManufacturerService = manufacturerService;
            NotificationService = notificationService;
            PermissionService = permissionService;
            PictureService = pictureService;
            ProductService = productService;
            StoreMappingService = storeMappingService;
            StoreService = storeService;
            UrlRecordService = urlRecordService;
            WorkContext = workContext;
        }

        #endregion

        #region Utilities

        protected virtual async Task UpdateLocalesAsync(Manufacturer manufacturer, ManufacturerModel model)
        {
            foreach (var localized in model.Locales)
            {
                await LocalizedEntityService.SaveLocalizedValueAsync(manufacturer,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);

                await LocalizedEntityService.SaveLocalizedValueAsync(manufacturer,
                    x => x.Description,
                    localized.Description,
                    localized.LanguageId);

                await LocalizedEntityService.SaveLocalizedValueAsync(manufacturer,
                    x => x.MetaKeywords,
                    localized.MetaKeywords,
                    localized.LanguageId);

                await LocalizedEntityService.SaveLocalizedValueAsync(manufacturer,
                    x => x.MetaDescription,
                    localized.MetaDescription,
                    localized.LanguageId);

                await LocalizedEntityService.SaveLocalizedValueAsync(manufacturer,
                    x => x.MetaTitle,
                    localized.MetaTitle,
                    localized.LanguageId);

                //search engine name
                var seName = await UrlRecordService.ValidateSeNameAsync(manufacturer, localized.SeName, localized.Name, false);
                await UrlRecordService.SaveSlugAsync(manufacturer, seName, localized.LanguageId);
            }
        }

        protected virtual async Task UpdatePictureSeoNamesAsync(Manufacturer manufacturer)
        {
            var picture = await PictureService.GetPictureByIdAsync(manufacturer.PictureId);
            if (picture != null)
                await PictureService.SetSeoFilenameAsync(picture.Id, await PictureService.GetPictureSeNameAsync(manufacturer.Name));
        }

        protected virtual async Task SaveManufacturerAclAsync(Manufacturer manufacturer, ManufacturerModel model)
        {
            manufacturer.SubjectToAcl = model.SelectedCustomerRoleIds.Any();
            await ManufacturerService.UpdateManufacturerAsync(manufacturer);

            var existingAclRecords = await AclService.GetAclRecordsAsync(manufacturer);
            var allCustomerRoles = await CustomerService.GetAllCustomerRolesAsync(true);
            foreach (var customerRole in allCustomerRoles)
            {
                if (model.SelectedCustomerRoleIds.Contains(customerRole.Id))
                {
                    //new role
                    if (existingAclRecords.Count(acl => acl.CustomerRoleId == customerRole.Id) == 0)
                        await AclService.InsertAclRecordAsync(manufacturer, customerRole.Id);
                }
                else
                {
                    //remove role
                    var aclRecordToDelete = existingAclRecords.FirstOrDefault(acl => acl.CustomerRoleId == customerRole.Id);
                    if (aclRecordToDelete != null)
                        await AclService.DeleteAclRecordAsync(aclRecordToDelete);
                }
            }
        }

        protected virtual async Task SaveStoreMappingsAsync(Manufacturer manufacturer, ManufacturerModel model)
        {
            manufacturer.LimitedToStores = model.SelectedStoreIds.Any();
            await ManufacturerService.UpdateManufacturerAsync(manufacturer);

            var existingStoreMappings = await StoreMappingService.GetStoreMappingsAsync(manufacturer);
            var allStores = await StoreService.GetAllStoresAsync();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds.Contains(store.Id))
                {
                    //new store
                    if (existingStoreMappings.Count(sm => sm.StoreId == store.Id) == 0)
                        await StoreMappingService.InsertStoreMappingAsync(manufacturer, store.Id);
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

        #endregion

        #region List

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual async Task<IActionResult> List()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            //prepare model
            var model = await ManufacturerModelFactory.PrepareManufacturerSearchModelAsync(new ManufacturerSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(ManufacturerSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageManufacturers))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await ManufacturerModelFactory.PrepareManufacturerListModelAsync(searchModel);

            return Json(model);
        }

        #endregion

        #region Create / Edit / Delete

        public virtual async Task<IActionResult> Create()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            //prepare model
            var model = await ManufacturerModelFactory.PrepareManufacturerModelAsync(new ManufacturerModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Create(ManufacturerModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var manufacturer = model.ToEntity<Manufacturer>();
                manufacturer.CreatedOnUtc = DateTime.UtcNow;
                manufacturer.UpdatedOnUtc = DateTime.UtcNow;
                await ManufacturerService.InsertManufacturerAsync(manufacturer);

                //search engine name
                model.SeName = await UrlRecordService.ValidateSeNameAsync(manufacturer, model.SeName, manufacturer.Name, true);
                await UrlRecordService.SaveSlugAsync(manufacturer, model.SeName, 0);

                //locales
                await UpdateLocalesAsync(manufacturer, model);

                //discounts
                var allDiscounts = await DiscountService.GetAllDiscountsAsync(DiscountType.AssignedToManufacturers, showHidden: true);
                foreach (var discount in allDiscounts)
                {
                    if (model.SelectedDiscountIds != null && model.SelectedDiscountIds.Contains(discount.Id))
                        //manufacturer.AppliedDiscounts.Add(discount);
                        await ManufacturerService.InsertDiscountManufacturerMappingAsync(new DiscountManufacturerMapping { EntityId = manufacturer.Id, DiscountId = discount.Id });

                }

                await ManufacturerService.UpdateManufacturerAsync(manufacturer);

                //update picture seo file name
                await UpdatePictureSeoNamesAsync(manufacturer);

                //ACL (customer roles)
                await SaveManufacturerAclAsync(manufacturer, model);

                //stores
                await SaveStoreMappingsAsync(manufacturer, model);

                //activity log
                await CustomerActivityService.InsertActivityAsync("AddNewManufacturer",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.AddNewManufacturer"), manufacturer.Name), manufacturer);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Catalog.Manufacturers.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");
                
                return RedirectToAction("Edit", new { id = manufacturer.Id });
            }

            //prepare model
            model = await ManufacturerModelFactory.PrepareManufacturerModelAsync(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            //try to get a manufacturer with the specified id
            var manufacturer = await ManufacturerService.GetManufacturerByIdAsync(id);
            if (manufacturer == null || manufacturer.Deleted)
                return RedirectToAction("List");

            //prepare model
            var model = await ManufacturerModelFactory.PrepareManufacturerModelAsync(null, manufacturer);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Edit(ManufacturerModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            //try to get a manufacturer with the specified id
            var manufacturer = await ManufacturerService.GetManufacturerByIdAsync(model.Id);
            if (manufacturer == null || manufacturer.Deleted)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                var prevPictureId = manufacturer.PictureId;
                manufacturer = model.ToEntity(manufacturer);
                manufacturer.UpdatedOnUtc = DateTime.UtcNow;
                await ManufacturerService.UpdateManufacturerAsync(manufacturer);

                //search engine name
                model.SeName = await UrlRecordService.ValidateSeNameAsync(manufacturer, model.SeName, manufacturer.Name, true);
                await UrlRecordService.SaveSlugAsync(manufacturer, model.SeName, 0);

                //locales
                await UpdateLocalesAsync(manufacturer, model);

                //discounts
                var allDiscounts = await DiscountService.GetAllDiscountsAsync(DiscountType.AssignedToManufacturers, showHidden: true);
                foreach (var discount in allDiscounts)
                {
                    if (model.SelectedDiscountIds != null && model.SelectedDiscountIds.Contains(discount.Id))
                    {
                        //new discount
                        if (await ManufacturerService.GetDiscountAppliedToManufacturerAsync(manufacturer.Id, discount.Id) is null)
                            await ManufacturerService.InsertDiscountManufacturerMappingAsync(new DiscountManufacturerMapping { EntityId = manufacturer.Id, DiscountId = discount.Id });
                    }
                    else
                    {
                        //remove discount
                        if (await ManufacturerService.GetDiscountAppliedToManufacturerAsync(manufacturer.Id, discount.Id) is DiscountManufacturerMapping discountManufacturerMapping)
                            await ManufacturerService.DeleteDiscountManufacturerMappingAsync(discountManufacturerMapping);
                    }
                }

                await ManufacturerService.UpdateManufacturerAsync(manufacturer);

                //delete an old picture (if deleted or updated)
                if (prevPictureId > 0 && prevPictureId != manufacturer.PictureId)
                {
                    var prevPicture = await PictureService.GetPictureByIdAsync(prevPictureId);
                    if (prevPicture != null)
                        await PictureService.DeletePictureAsync(prevPicture);
                }

                //update picture seo file name
                await UpdatePictureSeoNamesAsync(manufacturer);

                //ACL
                await SaveManufacturerAclAsync(manufacturer, model);

                //stores
                await SaveStoreMappingsAsync(manufacturer, model);

                //activity log
                await CustomerActivityService.InsertActivityAsync("EditManufacturer",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.EditManufacturer"), manufacturer.Name), manufacturer);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Catalog.Manufacturers.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");
                
                return RedirectToAction("Edit", new { id = manufacturer.Id });
            }

            //prepare model
            model = await ManufacturerModelFactory.PrepareManufacturerModelAsync(model, manufacturer, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            //try to get a manufacturer with the specified id
            var manufacturer = await ManufacturerService.GetManufacturerByIdAsync(id);
            if (manufacturer == null)
                return RedirectToAction("List");

            await ManufacturerService.DeleteManufacturerAsync(manufacturer);

            //activity log
            await CustomerActivityService.InsertActivityAsync("DeleteManufacturer",
                string.Format(await LocalizationService.GetResourceAsync("ActivityLog.DeleteManufacturer"), manufacturer.Name), manufacturer);

            NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Catalog.Manufacturers.Deleted"));

            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected(ICollection<int> selectedIds)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            if (selectedIds == null || selectedIds.Count() == 0)
                return NoContent();

            var manufacturers = await ManufacturerService.GetManufacturersByIdsAsync(selectedIds.ToArray());
            await ManufacturerService.DeleteManufacturersAsync(manufacturers);

            var locale = await LocalizationService.GetResourceAsync("ActivityLog.DeleteManufacturer");
            foreach (var manufacturer in manufacturers)
            {
                //activity log
                await CustomerActivityService.InsertActivityAsync("DeleteManufacturer", string.Format(locale, manufacturer.Name), manufacturer);
            }

            return Json(new { Result = true });
        }

        #endregion

        #region Export / Import

        public virtual async Task<IActionResult> ExportXml()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            try
            {
                var manufacturers = await ManufacturerService.GetAllManufacturersAsync(showHidden: true);
                var xml = await ExportManager.ExportManufacturersToXmlAsync(manufacturers);
                return File(Encoding.UTF8.GetBytes(xml), "application/xml", "manufacturers.xml");
            }
            catch (Exception exc)
            {
                await NotificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("List");
            }
        }

        public virtual async Task<IActionResult> ExportXlsx()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            try
            {
                var bytes = await ExportManager.ExportManufacturersToXlsxAsync((await ManufacturerService.GetAllManufacturersAsync(showHidden: true)).Where(p => !p.Deleted));

                return File(bytes, MimeTypes.TextXlsx, "manufacturers.xlsx");
            }
            catch (Exception exc)
            {
                await NotificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> ImportFromXlsx(IFormFile importexcelfile)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            //a vendor cannot import manufacturers
            if (await WorkContext.GetCurrentVendorAsync() != null)
                return AccessDeniedView();

            try
            {
                if (importexcelfile != null && importexcelfile.Length > 0)
                {
                    await ImportManager.ImportManufacturersFromXlsxAsync(importexcelfile.OpenReadStream());
                }
                else
                {
                    NotificationService.ErrorNotification(await LocalizationService.GetResourceAsync("Admin.Common.UploadFile"));
                    return RedirectToAction("List");
                }

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Catalog.Manufacturers.Imported"));
                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                await NotificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("List");
            }
        }

        #endregion

        #region Products

        [HttpPost]
        public virtual async Task<IActionResult> ProductList(ManufacturerProductSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageManufacturers))
                return await AccessDeniedDataTablesJson();

            //try to get a manufacturer with the specified id
            var manufacturer = await ManufacturerService.GetManufacturerByIdAsync(searchModel.ManufacturerId)
                ?? throw new ArgumentException("No manufacturer found with the specified id");

            //prepare model
            var model = await ManufacturerModelFactory.PrepareManufacturerProductListModelAsync(searchModel, manufacturer);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ProductUpdate(ManufacturerProductModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            //try to get a product manufacturer with the specified id
            var productManufacturer = await ManufacturerService.GetProductManufacturerByIdAsync(model.Id)
                ?? throw new ArgumentException("No product manufacturer mapping found with the specified id");

            //fill entity from model
            productManufacturer = model.ToEntity(productManufacturer);
            await ManufacturerService.UpdateProductManufacturerAsync(productManufacturer);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual async Task<IActionResult> ProductDelete(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            //try to get a product manufacturer with the specified id
            var productManufacturer = await ManufacturerService.GetProductManufacturerByIdAsync(id)
                ?? throw new ArgumentException("No product manufacturer mapping found with the specified id");

            await ManufacturerService.DeleteProductManufacturerAsync(productManufacturer);

            return new NullJsonResult();
        }

        public virtual async Task<IActionResult> ProductAddPopup(int manufacturerId)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            //prepare model
            var model = await ManufacturerModelFactory.PrepareAddProductToManufacturerSearchModelAsync(new AddProductToManufacturerSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ProductAddPopupList(AddProductToManufacturerSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageManufacturers))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await ManufacturerModelFactory.PrepareAddProductToManufacturerListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual async Task<IActionResult> ProductAddPopup(AddProductToManufacturerModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            //get selected products
            var selectedProducts = await ProductService.GetProductsByIdsAsync(model.SelectedProductIds.ToArray());
            if (selectedProducts.Any())
            {
                var existingProductmanufacturers = await ManufacturerService
                    .GetProductManufacturersByManufacturerIdAsync(model.ManufacturerId, showHidden: true);
                foreach (var product in selectedProducts)
                {
                    //whether product manufacturer with such parameters already exists
                    if (ManufacturerService.FindProductManufacturer(existingProductmanufacturers, product.Id, model.ManufacturerId) != null)
                        continue;

                    //insert the new product manufacturer mapping
                    await ManufacturerService.InsertProductManufacturerAsync(new ProductManufacturer
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