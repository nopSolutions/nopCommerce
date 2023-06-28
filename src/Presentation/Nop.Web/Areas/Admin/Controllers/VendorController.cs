using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Vendors;
using Nop.Services.Attributes;
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

        protected readonly IAddressService _addressService;
        protected readonly IAttributeParser<AddressAttribute, AddressAttributeValue> _addressAttributeParser;
        protected readonly IAttributeParser<VendorAttribute, VendorAttributeValue> _vendorAttributeParser;
        protected readonly IAttributeService<VendorAttribute, VendorAttributeValue> _vendorAttributeService;
        protected readonly ICustomerActivityService _customerActivityService;
        protected readonly ICustomerService _customerService;
        protected readonly IGenericAttributeService _genericAttributeService;
        protected readonly ILocalizationService _localizationService;
        protected readonly ILocalizedEntityService _localizedEntityService;
        protected readonly INotificationService _notificationService;
        protected readonly IPermissionService _permissionService;
        protected readonly IPictureService _pictureService;
        protected readonly IUrlRecordService _urlRecordService;
        protected readonly IVendorModelFactory _vendorModelFactory;
        protected readonly IVendorService _vendorService;

        #endregion

        #region Ctor

        public VendorController(IAddressService addressService,
            IAttributeParser<AddressAttribute, AddressAttributeValue> addressAttributeParser,
            IAttributeParser<VendorAttribute, VendorAttributeValue> vendorAttributeParser,
            IAttributeService<VendorAttribute, VendorAttributeValue> vendorAttributeService,
            ICustomerActivityService customerActivityService,
            ICustomerService customerService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            INotificationService notificationService,
            IPermissionService permissionService,
            IPictureService pictureService,
            IUrlRecordService urlRecordService,
            IVendorModelFactory vendorModelFactory,
            IVendorService vendorService)
        {
            _addressService = addressService;
            _addressAttributeParser = addressAttributeParser;
            _vendorAttributeParser = vendorAttributeParser;
            _vendorAttributeService = vendorAttributeService;
            _customerActivityService = customerActivityService;
            _customerService = customerService;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _localizedEntityService = localizedEntityService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _pictureService = pictureService;
            _urlRecordService = urlRecordService;
            _vendorModelFactory = vendorModelFactory;
            _vendorService = vendorService;
        }

        #endregion

        #region Utilities

        protected virtual async Task UpdatePictureSeoNamesAsync(Vendor vendor)
        {
            var picture = await _pictureService.GetPictureByIdAsync(vendor.PictureId);
            if (picture != null)
                await _pictureService.SetSeoFilenameAsync(picture.Id, await _pictureService.GetPictureSeNameAsync(vendor.Name));
        }

        protected virtual async Task UpdateLocalesAsync(Vendor vendor, VendorModel model)
        {
            foreach (var localized in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValueAsync(vendor,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);

                await _localizedEntityService.SaveLocalizedValueAsync(vendor,
                    x => x.Description,
                    localized.Description,
                    localized.LanguageId);

                await _localizedEntityService.SaveLocalizedValueAsync(vendor,
                    x => x.MetaKeywords,
                    localized.MetaKeywords,
                    localized.LanguageId);

                await _localizedEntityService.SaveLocalizedValueAsync(vendor,
                    x => x.MetaDescription,
                    localized.MetaDescription,
                    localized.LanguageId);

                await _localizedEntityService.SaveLocalizedValueAsync(vendor,
                    x => x.MetaTitle,
                    localized.MetaTitle,
                    localized.LanguageId);

                //search engine name
                var seName = await _urlRecordService.ValidateSeNameAsync(vendor, localized.SeName, localized.Name, false);
                await _urlRecordService.SaveSlugAsync(vendor, seName, localized.LanguageId);
            }
        }

        protected virtual async Task<string> ParseVendorAttributesAsync(IFormCollection form)
        {
            if (form == null)
                throw new ArgumentNullException(nameof(form));

            var attributesXml = string.Empty;
            var vendorAttributes = await _vendorAttributeService.GetAllAttributesAsync();
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
                                attributesXml = _vendorAttributeParser.AddAttribute(attributesXml,
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
                                    attributesXml = _vendorAttributeParser.AddAttribute(attributesXml,
                                        attribute, selectedAttributeId.ToString());
                            }
                        }

                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        //load read-only (already server-side selected) values
                        var attributeValues = await _vendorAttributeService.GetAttributeValuesAsync(attribute.Id);
                        foreach (var selectedAttributeId in attributeValues
                            .Where(v => v.IsPreSelected)
                            .Select(v => v.Id)
                            .ToList())
                        {
                            attributesXml = _vendorAttributeParser.AddAttribute(attributesXml,
                                attribute, selectedAttributeId.ToString());
                        }

                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        ctrlAttributes = form[controlId];
                        if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                        {
                            var enteredText = ctrlAttributes.ToString().Trim();
                            attributesXml = _vendorAttributeParser.AddAttribute(attributesXml,
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
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            //prepare model
            var model = await _vendorModelFactory.PrepareVendorSearchModelAsync(new VendorSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(VendorSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageVendors))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _vendorModelFactory.PrepareVendorListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            //prepare model
            var model = await _vendorModelFactory.PrepareVendorModelAsync(new VendorModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual async Task<IActionResult> Create(VendorModel model, bool continueEditing, IFormCollection form)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            //parse vendor attributes
            var vendorAttributesXml = await ParseVendorAttributesAsync(form);
            var warnings = (await _vendorAttributeParser.GetAttributeWarningsAsync(vendorAttributesXml)).ToList();
            foreach (var warning in warnings)
            {
                ModelState.AddModelError(string.Empty, warning);
            }

            if (ModelState.IsValid)
            {
                var vendor = model.ToEntity<Vendor>();
                await _vendorService.InsertVendorAsync(vendor);

                //activity log
                await _customerActivityService.InsertActivityAsync("AddNewVendor",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewVendor"), vendor.Id), vendor);

                //search engine name
                model.SeName = await _urlRecordService.ValidateSeNameAsync(vendor, model.SeName, vendor.Name, true);
                await _urlRecordService.SaveSlugAsync(vendor, model.SeName, 0);

                //address
                var address = model.Address.ToEntity<Address>();
                address.CreatedOnUtc = DateTime.UtcNow;

                //some validation
                if (address.CountryId == 0)
                    address.CountryId = null;
                if (address.StateProvinceId == 0)
                    address.StateProvinceId = null;
                await _addressService.InsertAddressAsync(address);
                vendor.AddressId = address.Id;
                await _vendorService.UpdateVendorAsync(vendor);

                //vendor attributes
                await _genericAttributeService.SaveAttributeAsync(vendor, NopVendorDefaults.VendorAttributes, vendorAttributesXml);

                //locales
                await UpdateLocalesAsync(vendor, model);

                //update picture seo file name
                await UpdatePictureSeoNamesAsync(vendor);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Vendors.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = vendor.Id });
            }

            //prepare model
            model = await _vendorModelFactory.PrepareVendorModelAsync(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            //try to get a vendor with the specified id
            var vendor = await _vendorService.GetVendorByIdAsync(id);
            if (vendor == null || vendor.Deleted)
                return RedirectToAction("List");

            //prepare model
            var model = await _vendorModelFactory.PrepareVendorModelAsync(null, vendor);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Edit(VendorModel model, bool continueEditing, IFormCollection form)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            //try to get a vendor with the specified id
            var vendor = await _vendorService.GetVendorByIdAsync(model.Id);
            if (vendor == null || vendor.Deleted)
                return RedirectToAction("List");

            //parse vendor attributes
            var vendorAttributesXml = await ParseVendorAttributesAsync(form);
            var warnings = (await _vendorAttributeParser.GetAttributeWarningsAsync(vendorAttributesXml)).ToList();
            foreach (var warning in warnings)
            {
                ModelState.AddModelError(string.Empty, warning);
            }

            //custom address attributes
            var customAttributes = await _addressAttributeParser.ParseCustomAttributesAsync(form, NopCommonDefaults.AddressAttributeControlName);
            var customAttributeWarnings = await _addressAttributeParser.GetAttributeWarningsAsync(customAttributes);
            foreach (var error in customAttributeWarnings)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            if (ModelState.IsValid)
            {
                var prevPictureId = vendor.PictureId;
                vendor = model.ToEntity(vendor);
                await _vendorService.UpdateVendorAsync(vendor);

                //vendor attributes
                await _genericAttributeService.SaveAttributeAsync(vendor, NopVendorDefaults.VendorAttributes, vendorAttributesXml);

                //activity log
                await _customerActivityService.InsertActivityAsync("EditVendor",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditVendor"), vendor.Id), vendor);

                //search engine name
                model.SeName = await _urlRecordService.ValidateSeNameAsync(vendor, model.SeName, vendor.Name, true);
                await _urlRecordService.SaveSlugAsync(vendor, model.SeName, 0);

                //address
                var address = await _addressService.GetAddressByIdAsync(vendor.AddressId);
                if (address == null)
                {
                    address = model.Address.ToEntity<Address>();
                    address.CustomAttributes = customAttributes;
                    address.CreatedOnUtc = DateTime.UtcNow;

                    //some validation
                    if (address.CountryId == 0)
                        address.CountryId = null;
                    if (address.StateProvinceId == 0)
                        address.StateProvinceId = null;

                    await _addressService.InsertAddressAsync(address);
                    vendor.AddressId = address.Id;
                    await _vendorService.UpdateVendorAsync(vendor);
                }
                else
                {
                    address = model.Address.ToEntity(address);
                    address.CustomAttributes = customAttributes;

                    //some validation
                    if (address.CountryId == 0)
                        address.CountryId = null;
                    if (address.StateProvinceId == 0)
                        address.StateProvinceId = null;

                    await _addressService.UpdateAddressAsync(address);
                }

                //locales
                await UpdateLocalesAsync(vendor, model);

                //delete an old picture (if deleted or updated)
                if (prevPictureId > 0 && prevPictureId != vendor.PictureId)
                {
                    var prevPicture = await _pictureService.GetPictureByIdAsync(prevPictureId);
                    if (prevPicture != null)
                        await _pictureService.DeletePictureAsync(prevPicture);
                }
                //update picture seo file name
                await UpdatePictureSeoNamesAsync(vendor);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Vendors.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = vendor.Id });
            }

            //prepare model
            model = await _vendorModelFactory.PrepareVendorModelAsync(model, vendor, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            //try to get a vendor with the specified id
            var vendor = await _vendorService.GetVendorByIdAsync(id);
            if (vendor == null)
                return RedirectToAction("List");

            //clear associated customer references
            var associatedCustomers = await _customerService.GetAllCustomersAsync(vendorId: vendor.Id);
            foreach (var customer in associatedCustomers)
            {
                customer.VendorId = 0;
                await _customerService.UpdateCustomerAsync(customer);
            }

            //delete a vendor
            await _vendorService.DeleteVendorAsync(vendor);

            //activity log
            await _customerActivityService.InsertActivityAsync("DeleteVendor",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteVendor"), vendor.Id), vendor);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Vendors.Deleted"));

            return RedirectToAction("List");
        }

        #endregion

        #region Vendor notes

        [HttpPost]
        public virtual async Task<IActionResult> VendorNotesSelect(VendorNoteSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageVendors))
                return await AccessDeniedDataTablesJson();

            //try to get a vendor with the specified id
            var vendor = await _vendorService.GetVendorByIdAsync(searchModel.VendorId)
                ?? throw new ArgumentException("No vendor found with the specified id");

            //prepare model
            var model = await _vendorModelFactory.PrepareVendorNoteListModelAsync(searchModel, vendor);

            return Json(model);
        }

        public virtual async Task<IActionResult> VendorNoteAdd(int vendorId, string message)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            if (string.IsNullOrEmpty(message))
                return ErrorJson(await _localizationService.GetResourceAsync("Admin.Vendors.VendorNotes.Fields.Note.Validation"));

            //try to get a vendor with the specified id
            var vendor = await _vendorService.GetVendorByIdAsync(vendorId);
            if (vendor == null)
                return ErrorJson("Vendor cannot be loaded");

            await _vendorService.InsertVendorNoteAsync(new VendorNote
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
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            //try to get a vendor note with the specified id
            var vendorNote = await _vendorService.GetVendorNoteByIdAsync(id)
                ?? throw new ArgumentException("No vendor note found with the specified id", nameof(id));

            await _vendorService.DeleteVendorNoteAsync(vendorNote);

            return new NullJsonResult();
        }

        #endregion
    }
}