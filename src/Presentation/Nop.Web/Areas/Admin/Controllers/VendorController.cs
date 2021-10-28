using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Vendors;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Vendors;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Vendors;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class VendorController : BaseAdminController
    {
        #region Fields

        protected IAddressService AddressService { get; }
        protected ICustomerActivityService CustomerActivityService { get; }
        protected ICustomerService CustomerService { get; }
        protected IGenericAttributeService GenericAttributeService { get; }
        protected ILocalizationService LocalizationService { get; }
        protected ILocalizedEntityService LocalizedEntityService { get; }
        protected INotificationService NotificationService { get; }
        protected IPermissionService PermissionService { get; }
        protected IPictureService PictureService { get; }
        protected IUrlRecordService UrlRecordService { get; }
        protected IVendorAttributeParser VendorAttributeParser { get; }
        protected IVendorAttributeService VendorAttributeService { get; }
        protected IVendorModelFactory VendorModelFactory { get; }
        protected IVendorService VendorService { get; }

        #endregion

        #region Ctor

        public VendorController(IAddressService addressService,
            ICustomerActivityService customerActivityService,
            ICustomerService customerService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            INotificationService notificationService,
            IPermissionService permissionService,
            IPictureService pictureService,
            IUrlRecordService urlRecordService,
            IVendorAttributeParser vendorAttributeParser,
            IVendorAttributeService vendorAttributeService,
            IVendorModelFactory vendorModelFactory,
            IVendorService vendorService)
        {
            AddressService = addressService;
            CustomerActivityService = customerActivityService;
            CustomerService = customerService;
            GenericAttributeService = genericAttributeService;
            LocalizationService = localizationService;
            LocalizedEntityService = localizedEntityService;
            NotificationService = notificationService;
            PermissionService = permissionService;
            PictureService = pictureService;
            UrlRecordService = urlRecordService;
            VendorAttributeParser = vendorAttributeParser;
            VendorAttributeService = vendorAttributeService;
            VendorModelFactory = vendorModelFactory;
            VendorService = vendorService;
        }

        #endregion

        #region Utilities

        protected virtual async Task UpdatePictureSeoNamesAsync(Vendor vendor)
        {
            var picture = await PictureService.GetPictureByIdAsync(vendor.PictureId);
            if (picture != null)
                await PictureService.SetSeoFilenameAsync(picture.Id, await PictureService.GetPictureSeNameAsync(vendor.Name));
        }

        protected virtual async Task UpdateLocalesAsync(Vendor vendor, VendorModel model)
        {
            foreach (var localized in model.Locales)
            {
                await LocalizedEntityService.SaveLocalizedValueAsync(vendor,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);

                await LocalizedEntityService.SaveLocalizedValueAsync(vendor,
                    x => x.Description,
                    localized.Description,
                    localized.LanguageId);

                await LocalizedEntityService.SaveLocalizedValueAsync(vendor,
                    x => x.MetaKeywords,
                    localized.MetaKeywords,
                    localized.LanguageId);

                await LocalizedEntityService.SaveLocalizedValueAsync(vendor,
                    x => x.MetaDescription,
                    localized.MetaDescription,
                    localized.LanguageId);

                await LocalizedEntityService.SaveLocalizedValueAsync(vendor,
                    x => x.MetaTitle,
                    localized.MetaTitle,
                    localized.LanguageId);

                //search engine name
                var seName = await UrlRecordService.ValidateSeNameAsync(vendor, localized.SeName, localized.Name, false);
                await UrlRecordService.SaveSlugAsync(vendor, seName, localized.LanguageId);
            }
        }

        protected virtual async Task<string> ParseVendorAttributesAsync(IFormCollection form)
        {
            if (form == null)
                throw new ArgumentNullException(nameof(form));

            var attributesXml = string.Empty;
            var vendorAttributes = await VendorAttributeService.GetAllVendorAttributesAsync();
            foreach (var attribute in vendorAttributes)
            {
                var controlId = $"{NopVendorDefaults.VendorAttributePrefix}{attribute.Id}";
                StringValues ctrlAttributes;
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                        ctrlAttributes = form[controlId];
                        if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                        {
                            var selectedAttributeId = int.Parse(ctrlAttributes);
                            if (selectedAttributeId > 0)
                                attributesXml = VendorAttributeParser.AddVendorAttribute(attributesXml,
                                    attribute, selectedAttributeId.ToString());
                        }

                        break;
                    case AttributeControlType.Checkboxes:
                        var cblAttributes = form[controlId];
                        if (!StringValues.IsNullOrEmpty(cblAttributes))
                        {
                            foreach (var item in cblAttributes.ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                var selectedAttributeId = int.Parse(item);
                                if (selectedAttributeId > 0)
                                    attributesXml = VendorAttributeParser.AddVendorAttribute(attributesXml,
                                        attribute, selectedAttributeId.ToString());
                            }
                        }

                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        //load read-only (already server-side selected) values
                        var attributeValues = await VendorAttributeService.GetVendorAttributeValuesAsync(attribute.Id);
                        foreach (var selectedAttributeId in attributeValues
                            .Where(v => v.IsPreSelected)
                            .Select(v => v.Id)
                            .ToList())
                        {
                            attributesXml = VendorAttributeParser.AddVendorAttribute(attributesXml,
                                attribute, selectedAttributeId.ToString());
                        }

                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        ctrlAttributes = form[controlId];
                        if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                        {
                            var enteredText = ctrlAttributes.ToString().Trim();
                            attributesXml = VendorAttributeParser.AddVendorAttribute(attributesXml,
                                attribute, enteredText);
                        }

                        break;
                    case AttributeControlType.Datepicker:
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.ImageSquares:
                    case AttributeControlType.FileUpload:
                    //not supported vendor attributes
                    default:
                        break;
                }
            }

            return attributesXml;
        }

        #endregion

        #region Vendors

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual async Task<IActionResult> List()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            //prepare model
            var model = await VendorModelFactory.PrepareVendorSearchModelAsync(new VendorSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(VendorSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageVendors))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await VendorModelFactory.PrepareVendorListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            //prepare model
            var model = await VendorModelFactory.PrepareVendorModelAsync(new VendorModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual async Task<IActionResult> Create(VendorModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            //parse vendor attributes
            var vendorAttributesXml = await ParseVendorAttributesAsync(model.Form);
            (await VendorAttributeParser.GetAttributeWarningsAsync(vendorAttributesXml)).ToList()
                .ForEach(warning => ModelState.AddModelError(string.Empty, warning));

            if (ModelState.IsValid)
            {
                var vendor = model.ToEntity<Vendor>();
                await VendorService.InsertVendorAsync(vendor);

                //activity log
                await CustomerActivityService.InsertActivityAsync("AddNewVendor",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.AddNewVendor"), vendor.Id), vendor);

                //search engine name
                model.SeName = await UrlRecordService.ValidateSeNameAsync(vendor, model.SeName, vendor.Name, true);
                await UrlRecordService.SaveSlugAsync(vendor, model.SeName, 0);

                //address
                var address = model.Address.ToEntity<Address>();
                address.CreatedOnUtc = DateTime.UtcNow;

                //some validation
                if (address.CountryId == 0)
                    address.CountryId = null;
                if (address.StateProvinceId == 0)
                    address.StateProvinceId = null;
                await AddressService.InsertAddressAsync(address);
                vendor.AddressId = address.Id;
                await VendorService.UpdateVendorAsync(vendor);

                //vendor attributes
                await GenericAttributeService.SaveAttributeAsync(vendor, NopVendorDefaults.VendorAttributes, vendorAttributesXml);

                //locales
                await UpdateLocalesAsync(vendor, model);

                //update picture seo file name
                await UpdatePictureSeoNamesAsync(vendor);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Vendors.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");
                
                return RedirectToAction("Edit", new { id = vendor.Id });
            }

            //prepare model
            model = await VendorModelFactory.PrepareVendorModelAsync(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            //try to get a vendor with the specified id
            var vendor = await VendorService.GetVendorByIdAsync(id);
            if (vendor == null || vendor.Deleted)
                return RedirectToAction("List");

            //prepare model
            var model = await VendorModelFactory.PrepareVendorModelAsync(null, vendor);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Edit(VendorModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            //try to get a vendor with the specified id
            var vendor = await VendorService.GetVendorByIdAsync(model.Id);
            if (vendor == null || vendor.Deleted)
                return RedirectToAction("List");

            //parse vendor attributes
            var vendorAttributesXml = await ParseVendorAttributesAsync(model.Form);
            (await VendorAttributeParser.GetAttributeWarningsAsync(vendorAttributesXml)).ToList()
                .ForEach(warning => ModelState.AddModelError(string.Empty, warning));

            if (ModelState.IsValid)
            {
                var prevPictureId = vendor.PictureId;
                vendor = model.ToEntity(vendor);
                await VendorService.UpdateVendorAsync(vendor);

                //vendor attributes
                await GenericAttributeService.SaveAttributeAsync(vendor, NopVendorDefaults.VendorAttributes, vendorAttributesXml);

                //activity log
                await CustomerActivityService.InsertActivityAsync("EditVendor",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.EditVendor"), vendor.Id), vendor);

                //search engine name
                model.SeName = await UrlRecordService.ValidateSeNameAsync(vendor, model.SeName, vendor.Name, true);
                await UrlRecordService.SaveSlugAsync(vendor, model.SeName, 0);

                //address
                var address = await AddressService.GetAddressByIdAsync(vendor.AddressId);
                if (address == null)
                {
                    address = model.Address.ToEntity<Address>();
                    address.CreatedOnUtc = DateTime.UtcNow;

                    //some validation
                    if (address.CountryId == 0)
                        address.CountryId = null;
                    if (address.StateProvinceId == 0)
                        address.StateProvinceId = null;

                    await AddressService.InsertAddressAsync(address);
                    vendor.AddressId = address.Id;
                    await VendorService.UpdateVendorAsync(vendor);
                }
                else
                {
                    address = model.Address.ToEntity(address);

                    //some validation
                    if (address.CountryId == 0)
                        address.CountryId = null;
                    if (address.StateProvinceId == 0)
                        address.StateProvinceId = null;

                    await AddressService.UpdateAddressAsync(address);
                }

                //locales
                await UpdateLocalesAsync(vendor, model);

                //delete an old picture (if deleted or updated)
                if (prevPictureId > 0 && prevPictureId != vendor.PictureId)
                {
                    var prevPicture = await PictureService.GetPictureByIdAsync(prevPictureId);
                    if (prevPicture != null)
                        await PictureService.DeletePictureAsync(prevPicture);
                }
                //update picture seo file name
                await UpdatePictureSeoNamesAsync(vendor);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Vendors.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");
                
                return RedirectToAction("Edit", new { id = vendor.Id });
            }

            //prepare model
            model = await VendorModelFactory.PrepareVendorModelAsync(model, vendor, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            //try to get a vendor with the specified id
            var vendor = await VendorService.GetVendorByIdAsync(id);
            if (vendor == null)
                return RedirectToAction("List");

            //clear associated customer references
            var associatedCustomers = await CustomerService.GetAllCustomersAsync(vendorId: vendor.Id);
            foreach (var customer in associatedCustomers)
            {
                customer.VendorId = 0;
                await CustomerService.UpdateCustomerAsync(customer);
            }

            //delete a vendor
            await VendorService.DeleteVendorAsync(vendor);

            //activity log
            await CustomerActivityService.InsertActivityAsync("DeleteVendor",
                string.Format(await LocalizationService.GetResourceAsync("ActivityLog.DeleteVendor"), vendor.Id), vendor);

            NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Vendors.Deleted"));

            return RedirectToAction("List");
        }

        #endregion

        #region Vendor notes

        [HttpPost]
        public virtual async Task<IActionResult> VendorNotesSelect(VendorNoteSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageVendors))
                return await AccessDeniedDataTablesJson();

            //try to get a vendor with the specified id
            var vendor = await VendorService.GetVendorByIdAsync(searchModel.VendorId)
                ?? throw new ArgumentException("No vendor found with the specified id");

            //prepare model
            var model = await VendorModelFactory.PrepareVendorNoteListModelAsync(searchModel, vendor);

            return Json(model);
        }

        public virtual async Task<IActionResult> VendorNoteAdd(int vendorId, string message)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            if (string.IsNullOrEmpty(message))
                return ErrorJson(await LocalizationService.GetResourceAsync("Admin.Vendors.VendorNotes.Fields.Note.Validation"));

            //try to get a vendor with the specified id
            var vendor = await VendorService.GetVendorByIdAsync(vendorId);
            if (vendor == null)
                return ErrorJson("Vendor cannot be loaded");

            await VendorService.InsertVendorNoteAsync(new VendorNote
            {
                Note = message,
                CreatedOnUtc = DateTime.UtcNow,
                VendorId = vendor.Id
            });

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual async Task<IActionResult> VendorNoteDelete(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            //try to get a vendor note with the specified id
            var vendorNote = await VendorService.GetVendorNoteByIdAsync(id)
                ?? throw new ArgumentException("No vendor note found with the specified id", nameof(id));

            await VendorService.DeleteVendorNoteAsync(vendorNote);

            return new NullJsonResult();
        }

        #endregion
    }
}