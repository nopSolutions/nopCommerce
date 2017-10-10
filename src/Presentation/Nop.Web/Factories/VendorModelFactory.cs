using System;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Vendors;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Web.Framework.Security.Captcha;
using Nop.Web.Models.Vendors;

namespace Nop.Web.Factories
{
    /// <summary>
    /// Represents the vendor model factory
    /// </summary>
    public partial class VendorModelFactory : IVendorModelFactory
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly ILocalizationService _localizationService;
        private readonly IPictureService _pictureService;
        
        private readonly CaptchaSettings _captchaSettings;
        private readonly CommonSettings _commonSettings;
        private readonly MediaSettings _mediaSettings;
        private readonly VendorSettings _vendorSettings;

        #endregion

        #region Ctor

        public VendorModelFactory(IWorkContext workContext,
            ILocalizationService localizationService,
            IPictureService pictureService,
            CaptchaSettings captchaSettings,
            CommonSettings commonSettings,
            MediaSettings mediaSettings,
            VendorSettings vendorSettings)
        {
            this._workContext = workContext;
            this._localizationService = localizationService;
            this._pictureService = pictureService;
            
            this._captchaSettings = captchaSettings;
            this._commonSettings = commonSettings;
            this._mediaSettings = mediaSettings;
            this._vendorSettings = vendorSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare the apply vendor model
        /// </summary>
        /// <param name="model">The apply vendor model</param>
        /// <param name="validateVendor">Whether to validate that the customer is already a vendor</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <returns>The apply vendor model</returns>
        public virtual ApplyVendorModel PrepareApplyVendorModel(ApplyVendorModel model, bool validateVendor, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (validateVendor && _workContext.CurrentCustomer.VendorId > 0)
            {
                //already applied for vendor account
                model.DisableFormInput = true;
                model.Result = _localizationService.GetResource("Vendors.ApplyAccount.AlreadyApplied");
            }

            model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnApplyVendorPage;
            model.TermsOfServiceEnabled = _vendorSettings.TermsOfServiceEnabled;
            model.TermsOfServicePopup = _commonSettings.PopupForTermsOfServiceLinks;

            if (!excludeProperties)
            {
                model.Email = _workContext.CurrentCustomer.Email;
            }

            return model;
        }

        /// <summary>
        /// Prepare the vendor info model
        /// </summary>
        /// <param name="model">Vendor info model</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <returns>Vendor info model</returns>
        public virtual VendorInfoModel PrepareVendorInfoModel(VendorInfoModel model, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var vendor = _workContext.CurrentVendor;
            if (!excludeProperties)
            {
                model.Description = vendor.Description;
                model.Email = vendor.Email;
                model.Name = vendor.Name;
            }

            var picture = _pictureService.GetPictureById(vendor.PictureId);
            var pictureSize = _mediaSettings.AvatarPictureSize;
            model.PictureUrl = picture != null ? _pictureService.GetPictureUrl(picture, pictureSize) : string.Empty;

            return model;
        }
        
        #endregion
    }
}
