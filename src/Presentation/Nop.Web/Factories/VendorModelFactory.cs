using System;
using Nop.Core;
using Nop.Core.Domain.Media;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Web.Framework.Security.Captcha;
using Nop.Web.Models.Vendors;

namespace Nop.Web.Factories
{
    public partial class VendorModelFactory : IVendorModelFactory
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly ILocalizationService _localizationService;
        private readonly IPictureService _pictureService;
        
        private readonly CaptchaSettings _captchaSettings;
        private readonly MediaSettings _mediaSettings;

        #endregion

        #region Constructors

        public VendorModelFactory(IWorkContext workContext,
            ILocalizationService localizationService,
            IPictureService pictureService,
            CaptchaSettings captchaSettings,
            MediaSettings mediaSettings)
        {
            this._workContext = workContext;
            this._localizationService = localizationService;
            this._pictureService = pictureService;
            
            this._captchaSettings = captchaSettings;
            this._mediaSettings = mediaSettings;
        }

        #endregion
        
        #region Methods
        
        public virtual ApplyVendorModel PrepareApplyVendorModel(ApplyVendorModel model, bool validateVendor, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (validateVendor && _workContext.CurrentCustomer.VendorId > 0)
            {
                //already applied for vendor account
                model.DisableFormInput = true;
                model.Result = _localizationService.GetResource("Vendors.ApplyAccount.AlreadyApplied");
            }

            model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnApplyVendorPage;

            if (!excludeProperties)
            {
                model.Email = _workContext.CurrentCustomer.Email;
            }

            return model;
        }
        
        public virtual VendorInfoModel PrepareVendorInfoModel(VendorInfoModel model, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

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
