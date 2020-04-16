using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Vendors;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Seo;
using Nop.Services.Vendors;
using Nop.Web.Factories;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Models.Vendors;

namespace Nop.Web.Controllers
{
    public partial class VendorController : BasePublicController
    {
        #region Fields

        private readonly CaptchaSettings _captchaSettings;
        private readonly ICustomerService _customerService;
        private readonly IDownloadService _downloadService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IPictureService _pictureService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IVendorAttributeParser _vendorAttributeParser;
        private readonly IVendorAttributeService _vendorAttributeService;
        private readonly IVendorModelFactory _vendorModelFactory;
        private readonly IVendorService _vendorService;
        private readonly IWorkContext _workContext;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly LocalizationSettings _localizationSettings;
        private readonly VendorSettings _vendorSettings;

        #endregion

        #region Ctor

        public VendorController(CaptchaSettings captchaSettings,
            ICustomerService customerService,
            IDownloadService downloadService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IPictureService pictureService,
            IUrlRecordService urlRecordService,
            IVendorAttributeParser vendorAttributeParser,
            IVendorAttributeService vendorAttributeService,
            IVendorModelFactory vendorModelFactory,
            IVendorService vendorService,
            IWorkContext workContext,
            IWorkflowMessageService workflowMessageService,
            LocalizationSettings localizationSettings,
            VendorSettings vendorSettings)
        {
            _captchaSettings = captchaSettings;
            _customerService = customerService;
            _downloadService = downloadService;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _pictureService = pictureService;
            _urlRecordService = urlRecordService;
            _vendorAttributeParser = vendorAttributeParser;
            _vendorAttributeService = vendorAttributeService;
            _vendorModelFactory = vendorModelFactory;
            _vendorService = vendorService;
            _workContext = workContext;
            _workflowMessageService = workflowMessageService;
            _localizationSettings = localizationSettings;
            _vendorSettings = vendorSettings;
        }

        #endregion

        #region Utilities

        protected virtual void UpdatePictureSeoNames(Vendor vendor)
        {
            var picture = _pictureService.GetPictureById(vendor.PictureId);
            if (picture != null)
                _pictureService.SetSeoFilename(picture.Id, _pictureService.GetPictureSeName(vendor.Name));
        }

        protected virtual string ParseVendorAttributes(IFormCollection form)
        {
            if (form == null)
                throw new ArgumentNullException(nameof(form));

            var attributesXml = "";
            var attributes = _vendorAttributeService.GetAllVendorAttributes();
            foreach (var attribute in attributes)
            {
                var controlId = $"{NopVendorDefaults.VendorAttributePrefix}{attribute.Id}";
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                            {
                                var selectedAttributeId = int.Parse(ctrlAttributes);
                                if (selectedAttributeId > 0)
                                    attributesXml = _vendorAttributeParser.AddVendorAttribute(attributesXml,
                                        attribute, selectedAttributeId.ToString());
                            }
                        }
                        break;
                    case AttributeControlType.Checkboxes:
                        {
                            var cblAttributes = form[controlId];
                            if (!StringValues.IsNullOrEmpty(cblAttributes))
                            {
                                foreach (var item in cblAttributes.ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                )
                                {
                                    var selectedAttributeId = int.Parse(item);
                                    if (selectedAttributeId > 0)
                                        attributesXml = _vendorAttributeParser.AddVendorAttribute(attributesXml,
                                            attribute, selectedAttributeId.ToString());
                                }
                            }
                        }
                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        {
                            //load read-only (already server-side selected) values
                            var attributeValues = _vendorAttributeService.GetVendorAttributeValues(attribute.Id);
                            foreach (var selectedAttributeId in attributeValues
                                .Where(v => v.IsPreSelected)
                                .Select(v => v.Id)
                                .ToList())
                            {
                                attributesXml = _vendorAttributeParser.AddVendorAttribute(attributesXml,
                                    attribute, selectedAttributeId.ToString());
                            }
                        }
                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                            {
                                var enteredText = ctrlAttributes.ToString().Trim();
                                attributesXml = _vendorAttributeParser.AddVendorAttribute(attributesXml,
                                    attribute, enteredText);
                            }
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

        #region Methods

        [HttpsRequirement]
        public virtual IActionResult ApplyVendor()
        {
            if (!_vendorSettings.AllowCustomersToApplyForVendorAccount)
                return RedirectToRoute("Homepage");

            if (!_customerService.IsRegistered(_workContext.CurrentCustomer))
                return Challenge();

            var model = new ApplyVendorModel();
            model = _vendorModelFactory.PrepareApplyVendorModel(model, true, false, null);
            return View(model);
        }

        [HttpPost, ActionName("ApplyVendor")]
        [AutoValidateAntiforgeryToken]
        [ValidateCaptcha]
        public virtual IActionResult ApplyVendorSubmit(ApplyVendorModel model, bool captchaValid, IFormFile uploadedFile, IFormCollection form)
        {
            if (!_vendorSettings.AllowCustomersToApplyForVendorAccount)
                return RedirectToRoute("Homepage");

            if (!_customerService.IsRegistered(_workContext.CurrentCustomer))
                return Challenge();

            if (_customerService.IsAdmin(_workContext.CurrentCustomer))
                ModelState.AddModelError("", _localizationService.GetResource("Vendors.ApplyAccount.IsAdmin"));

            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnApplyVendorPage && !captchaValid)
            {
                ModelState.AddModelError("", _localizationService.GetResource("Common.WrongCaptchaMessage"));
            }

            var pictureId = 0;

            if (uploadedFile != null && !string.IsNullOrEmpty(uploadedFile.FileName))
            {
                try
                {
                    var contentType = uploadedFile.ContentType;
                    var vendorPictureBinary = _downloadService.GetDownloadBits(uploadedFile);
                    var picture = _pictureService.InsertPicture(vendorPictureBinary, contentType, null);

                    if (picture != null)
                        pictureId = picture.Id;
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", _localizationService.GetResource("Vendors.ApplyAccount.Picture.ErrorMessage"));
                }
            }

            //vendor attributes
            var vendorAttributesXml = ParseVendorAttributes(form);
            _vendorAttributeParser.GetAttributeWarnings(vendorAttributesXml).ToList()
                .ForEach(warning => ModelState.AddModelError(string.Empty, warning));

            if (ModelState.IsValid)
            {
                var description = Core.Html.HtmlHelper.FormatText(model.Description, false, false, true, false, false, false);
                //disabled by default
                var vendor = new Vendor
                {
                    Name = model.Name,
                    Email = model.Email,
                    //some default settings
                    PageSize = 6,
                    AllowCustomersToSelectPageSize = true,
                    PageSizeOptions = _vendorSettings.DefaultVendorPageSizeOptions,
                    PictureId = pictureId,
                    Description = description
                };
                _vendorService.InsertVendor(vendor);
                //search engine name (the same as vendor name)
                var seName = _urlRecordService.ValidateSeName(vendor, vendor.Name, vendor.Name, true);
                _urlRecordService.SaveSlug(vendor, seName, 0);

                //associate to the current customer
                //but a store owner will have to manually add this customer role to "Vendors" role
                //if he wants to grant access to admin area
                _workContext.CurrentCustomer.VendorId = vendor.Id;
                _customerService.UpdateCustomer(_workContext.CurrentCustomer);

                //update picture seo file name
                UpdatePictureSeoNames(vendor);

                //save vendor attributes
                _genericAttributeService.SaveAttribute(vendor, NopVendorDefaults.VendorAttributes, vendorAttributesXml);

                //notify store owner here (email)
                _workflowMessageService.SendNewVendorAccountApplyStoreOwnerNotification(_workContext.CurrentCustomer,
                    vendor, _localizationSettings.DefaultAdminLanguageId);

                model.DisableFormInput = true;
                model.Result = _localizationService.GetResource("Vendors.ApplyAccount.Submitted");
                return View(model);
            }

            //If we got this far, something failed, redisplay form
            model = _vendorModelFactory.PrepareApplyVendorModel(model, false, true, vendorAttributesXml);
            return View(model);
        }

        [HttpsRequirement]
        public virtual IActionResult Info()
        {
            if (!_customerService.IsRegistered(_workContext.CurrentCustomer))
                return Challenge();

            if (_workContext.CurrentVendor == null || !_vendorSettings.AllowVendorsToEditInfo)
                return RedirectToRoute("CustomerInfo");

            var model = new VendorInfoModel();
            model = _vendorModelFactory.PrepareVendorInfoModel(model, false);
            return View(model);
        }

        [HttpPost, ActionName("Info")]
        [AutoValidateAntiforgeryToken]
        [FormValueRequired("save-info-button")]
        public virtual IActionResult Info(VendorInfoModel model, IFormFile uploadedFile, IFormCollection form)
        {
            if (!_customerService.IsRegistered(_workContext.CurrentCustomer))
                return Challenge();

            if (_workContext.CurrentVendor == null || !_vendorSettings.AllowVendorsToEditInfo)
                return RedirectToRoute("CustomerInfo");

            Picture picture = null;

            if (uploadedFile != null && !string.IsNullOrEmpty(uploadedFile.FileName))
            {
                try
                {
                    var contentType = uploadedFile.ContentType;
                    var vendorPictureBinary = _downloadService.GetDownloadBits(uploadedFile);
                    picture = _pictureService.InsertPicture(vendorPictureBinary, contentType, null);
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", _localizationService.GetResource("Account.VendorInfo.Picture.ErrorMessage"));
                }
            }

            var vendor = _workContext.CurrentVendor;
            var prevPicture = _pictureService.GetPictureById(vendor.PictureId);

            //vendor attributes
            var vendorAttributesXml = ParseVendorAttributes(form);
            _vendorAttributeParser.GetAttributeWarnings(vendorAttributesXml).ToList()
                .ForEach(warning => ModelState.AddModelError(string.Empty, warning));

            if (ModelState.IsValid)
            {
                var description = Core.Html.HtmlHelper.FormatText(model.Description, false, false, true, false, false, false);

                vendor.Name = model.Name;
                vendor.Email = model.Email;
                vendor.Description = description;

                if (picture != null)
                {
                    vendor.PictureId = picture.Id;

                    if (prevPicture != null)
                        _pictureService.DeletePicture(prevPicture);
                }

                //update picture seo file name
                UpdatePictureSeoNames(vendor);

                _vendorService.UpdateVendor(vendor);

                //save vendor attributes
                _genericAttributeService.SaveAttribute(vendor, NopVendorDefaults.VendorAttributes, vendorAttributesXml);

                //notifications
                if (_vendorSettings.NotifyStoreOwnerAboutVendorInformationChange)
                    _workflowMessageService.SendVendorInformationChangeNotification(vendor, _localizationSettings.DefaultAdminLanguageId);

                return RedirectToAction("Info");
            }

            //If we got this far, something failed, redisplay form
            model = _vendorModelFactory.PrepareVendorInfoModel(model, true, vendorAttributesXml);
            return View(model);
        }

        [HttpPost, ActionName("Info")]
        [AutoValidateAntiforgeryToken]
        [FormValueRequired("remove-picture")]
        public virtual IActionResult RemovePicture()
        {
            if (!_customerService.IsRegistered(_workContext.CurrentCustomer))
                return Challenge();

            if (_workContext.CurrentVendor == null || !_vendorSettings.AllowVendorsToEditInfo)
                return RedirectToRoute("CustomerInfo");

            var vendor = _workContext.CurrentVendor;
            var picture = _pictureService.GetPictureById(vendor.PictureId);

            if (picture != null)
                _pictureService.DeletePicture(picture);

            vendor.PictureId = 0;
            _vendorService.UpdateVendor(vendor);

            //notifications
            if (_vendorSettings.NotifyStoreOwnerAboutVendorInformationChange)
                _workflowMessageService.SendVendorInformationChangeNotification(vendor, _localizationSettings.DefaultAdminLanguageId);

            return RedirectToAction("Info");
        }

        #endregion
    }
}