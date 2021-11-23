using System;
using System.Linq;
using System.Threading.Tasks;
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
using Nop.Services.Html;
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
    [AutoValidateAntiforgeryToken]
    public partial class VendorController : BasePublicController
    {
        #region Fields

        protected CaptchaSettings CaptchaSettings { get; }
        protected ICustomerService CustomerService { get; }
        protected IDownloadService DownloadService { get; }
        protected IGenericAttributeService GenericAttributeService { get; }
        protected ILocalizationService LocalizationService { get; }
        private readonly INopHtmlHelper _nopHtmlHelper;
        protected IPictureService PictureService { get; }
        protected IUrlRecordService UrlRecordService { get; }
        protected IVendorAttributeParser VendorAttributeParser { get; }
        protected IVendorAttributeService VendorAttributeService { get; }
        protected IVendorModelFactory VendorModelFactory { get; }
        protected IVendorService VendorService { get; }
        protected IWorkContext WorkContext { get; }
        protected IWorkflowMessageService WorkflowMessageService { get; }
        protected LocalizationSettings LocalizationSettings { get; }
        protected VendorSettings VendorSettings { get; }

        #endregion

        #region Ctor

        public VendorController(CaptchaSettings captchaSettings,
            ICustomerService customerService,
            IDownloadService downloadService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            INopHtmlHelper nopHtmlHelper,
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
            CaptchaSettings = captchaSettings;
            CustomerService = customerService;
            DownloadService = downloadService;
            GenericAttributeService = genericAttributeService;
            LocalizationService = localizationService;
            _nopHtmlHelper = nopHtmlHelper;
            PictureService = pictureService;
            UrlRecordService = urlRecordService;
            VendorAttributeParser = vendorAttributeParser;
            VendorAttributeService = vendorAttributeService;
            VendorModelFactory = vendorModelFactory;
            VendorService = vendorService;
            WorkContext = workContext;
            WorkflowMessageService = workflowMessageService;
            LocalizationSettings = localizationSettings;
            VendorSettings = vendorSettings;
        }

        #endregion

        #region Utilities

        protected virtual async Task UpdatePictureSeoNamesAsync(Vendor vendor)
        {
            var picture = await PictureService.GetPictureByIdAsync(vendor.PictureId);
            if (picture != null)
                await PictureService.SetSeoFilenameAsync(picture.Id, await PictureService.GetPictureSeNameAsync(vendor.Name));
        }

        protected virtual async Task<string> ParseVendorAttributesAsync(IFormCollection form)
        {
            if (form == null)
                throw new ArgumentNullException(nameof(form));

            var attributesXml = "";
            var attributes = await VendorAttributeService.GetAllVendorAttributesAsync();
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
                                    attributesXml = VendorAttributeParser.AddVendorAttribute(attributesXml,
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
                                        attributesXml = VendorAttributeParser.AddVendorAttribute(attributesXml,
                                            attribute, selectedAttributeId.ToString());
                                }
                            }
                        }
                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        {
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
                        }
                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                            {
                                var enteredText = ctrlAttributes.ToString().Trim();
                                attributesXml = VendorAttributeParser.AddVendorAttribute(attributesXml,
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

        public virtual async Task<IActionResult> ApplyVendor()
        {
            if (!VendorSettings.AllowCustomersToApplyForVendorAccount)
                return RedirectToRoute("Homepage");

            if (!await CustomerService.IsRegisteredAsync(await WorkContext.GetCurrentCustomerAsync()))
                return Challenge();

            var model = new ApplyVendorModel();
            model = await VendorModelFactory.PrepareApplyVendorModelAsync(model, true, false, null);
            return View(model);
        }

        [HttpPost, ActionName("ApplyVendor")]
        [ValidateCaptcha]
        public virtual async Task<IActionResult> ApplyVendorSubmit(ApplyVendorModel model, bool captchaValid, IFormFile uploadedFile)
        {
            if (!VendorSettings.AllowCustomersToApplyForVendorAccount)
                return RedirectToRoute("Homepage");

            var customer = await WorkContext.GetCurrentCustomerAsync();
            if (!await CustomerService.IsRegisteredAsync(customer))
                return Challenge();

            if (await CustomerService.IsAdminAsync(customer))
                ModelState.AddModelError("", await LocalizationService.GetResourceAsync("Vendors.ApplyAccount.IsAdmin"));

            //validate CAPTCHA
            if (CaptchaSettings.Enabled && CaptchaSettings.ShowOnApplyVendorPage && !captchaValid)
            {
                ModelState.AddModelError("", await LocalizationService.GetResourceAsync("Common.WrongCaptchaMessage"));
            }

            var pictureId = 0;

            if (uploadedFile != null && !string.IsNullOrEmpty(uploadedFile.FileName))
            {
                try
                {
                    var contentType = uploadedFile.ContentType;
                    var vendorPictureBinary = await DownloadService.GetDownloadBitsAsync(uploadedFile);
                    var picture = await PictureService.InsertPictureAsync(vendorPictureBinary, contentType, null);

                    if (picture != null)
                        pictureId = picture.Id;
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", await LocalizationService.GetResourceAsync("Vendors.ApplyAccount.Picture.ErrorMessage"));
                }
            }

            //vendor attributes
            var vendorAttributesXml = await ParseVendorAttributesAsync(model.Form);
            (await VendorAttributeParser.GetAttributeWarningsAsync(vendorAttributesXml)).ToList()
                .ForEach(warning => ModelState.AddModelError(string.Empty, warning));

            if (ModelState.IsValid)
            {
                var description = _nopHtmlHelper.FormatText(model.Description, false, false, true, false, false, false);
                //disabled by default
                var vendor = new Vendor
                {
                    Name = model.Name,
                    Email = model.Email,
                    //some default settings
                    PageSize = 6,
                    AllowCustomersToSelectPageSize = true,
                    PageSizeOptions = VendorSettings.DefaultVendorPageSizeOptions,
                    PictureId = pictureId,
                    Description = description
                };
                await VendorService.InsertVendorAsync(vendor);
                //search engine name (the same as vendor name)
                var seName = await UrlRecordService.ValidateSeNameAsync(vendor, vendor.Name, vendor.Name, true);
                await UrlRecordService.SaveSlugAsync(vendor, seName, 0);

                //associate to the current customer
                //but a store owner will have to manually add this customer role to "Vendors" role
                //if he wants to grant access to admin area
                customer.VendorId = vendor.Id;
                await CustomerService.UpdateCustomerAsync(customer);

                //update picture seo file name
                await UpdatePictureSeoNamesAsync(vendor);

                //save vendor attributes
                await GenericAttributeService.SaveAttributeAsync(vendor, NopVendorDefaults.VendorAttributes, vendorAttributesXml);

                //notify store owner here (email)
                await WorkflowMessageService.SendNewVendorAccountApplyStoreOwnerNotificationAsync(customer,
                    vendor, LocalizationSettings.DefaultAdminLanguageId);

                model.DisableFormInput = true;
                model.Result = await LocalizationService.GetResourceAsync("Vendors.ApplyAccount.Submitted");
                return View(model);
            }

            //If we got this far, something failed, redisplay form
            model = await VendorModelFactory.PrepareApplyVendorModelAsync(model, false, true, vendorAttributesXml);
            return View(model);
        }

        public virtual async Task<IActionResult> Info()
        {
            if (!await CustomerService.IsRegisteredAsync(await WorkContext.GetCurrentCustomerAsync()))
                return Challenge();

            if (await WorkContext.GetCurrentVendorAsync() == null || !VendorSettings.AllowVendorsToEditInfo)
                return RedirectToRoute("CustomerInfo");

            var model = new VendorInfoModel();
            model = await VendorModelFactory.PrepareVendorInfoModelAsync(model, false);
            return View(model);
        }

        [HttpPost, ActionName("Info")]
        [FormValueRequired("save-info-button")]
        public virtual async Task<IActionResult> Info(VendorInfoModel model, IFormFile uploadedFile)
        {
            if (!await CustomerService.IsRegisteredAsync(await WorkContext.GetCurrentCustomerAsync()))
                return Challenge();

            var vendor = await WorkContext.GetCurrentVendorAsync();
            if (vendor == null || !VendorSettings.AllowVendorsToEditInfo)
                return RedirectToRoute("CustomerInfo");

            Picture picture = null;

            if (uploadedFile != null && !string.IsNullOrEmpty(uploadedFile.FileName))
            {
                try
                {
                    var contentType = uploadedFile.ContentType;
                    var vendorPictureBinary = await DownloadService.GetDownloadBitsAsync(uploadedFile);
                    picture = await PictureService.InsertPictureAsync(vendorPictureBinary, contentType, null);
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", await LocalizationService.GetResourceAsync("Account.VendorInfo.Picture.ErrorMessage"));
                }
            }

            var prevPicture = await PictureService.GetPictureByIdAsync(vendor.PictureId);

            //vendor attributes
            var vendorAttributesXml = await ParseVendorAttributesAsync(model.Form);
            (await VendorAttributeParser.GetAttributeWarningsAsync(vendorAttributesXml)).ToList()
                .ForEach(warning => ModelState.AddModelError(string.Empty, warning));

            if (ModelState.IsValid)
            {
                var description = _nopHtmlHelper.FormatText(model.Description, false, false, true, false, false, false);

                vendor.Name = model.Name;
                vendor.Email = model.Email;
                vendor.Description = description;

                if (picture != null)
                {
                    vendor.PictureId = picture.Id;

                    if (prevPicture != null)
                        await PictureService.DeletePictureAsync(prevPicture);
                }

                //update picture seo file name
                await UpdatePictureSeoNamesAsync(vendor);

                await VendorService.UpdateVendorAsync(vendor);

                //save vendor attributes
                await GenericAttributeService.SaveAttributeAsync(vendor, NopVendorDefaults.VendorAttributes, vendorAttributesXml);

                //notifications
                if (VendorSettings.NotifyStoreOwnerAboutVendorInformationChange)
                    await WorkflowMessageService.SendVendorInformationChangeNotificationAsync(vendor, LocalizationSettings.DefaultAdminLanguageId);

                return RedirectToAction("Info");
            }

            //If we got this far, something failed, redisplay form
            model = await VendorModelFactory.PrepareVendorInfoModelAsync(model, true, vendorAttributesXml);
            return View(model);
        }

        [HttpPost, ActionName("Info")]
        [FormValueRequired("remove-picture")]
        public virtual async Task<IActionResult> RemovePicture()
        {
            if (!await CustomerService.IsRegisteredAsync(await WorkContext.GetCurrentCustomerAsync()))
                return Challenge();

            var vendor = await WorkContext.GetCurrentVendorAsync();
            if (vendor == null || !VendorSettings.AllowVendorsToEditInfo)
                return RedirectToRoute("CustomerInfo");

            var picture = await PictureService.GetPictureByIdAsync(vendor.PictureId);

            if (picture != null)
                await PictureService.DeletePictureAsync(picture);

            vendor.PictureId = 0;
            await VendorService.UpdateVendorAsync(vendor);

            //notifications
            if (VendorSettings.NotifyStoreOwnerAboutVendorInformationChange)
                await WorkflowMessageService.SendVendorInformationChangeNotificationAsync(vendor, LocalizationSettings.DefaultAdminLanguageId);

            return RedirectToAction("Info");
        }

        #endregion
    }
}