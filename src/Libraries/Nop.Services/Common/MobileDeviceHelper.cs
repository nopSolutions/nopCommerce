using System.Web;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.Customers;

namespace Nop.Services.Common
{
    /// <summary>
    /// Mobile device helper
    /// </summary>
    public partial class MobileDeviceHelper : IMobileDeviceHelper
    {
        #region Fields

        private readonly StoreInformationSettings _storeInformationSettings;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="storeInformationSettings">Store information settings</param>
        /// <param name="workContext">Work context</param>
        /// <param name="storeContext">Store context</param>
        public MobileDeviceHelper(StoreInformationSettings storeInformationSettings,
            IWorkContext workContext, IStoreContext storeContext)
        {
            this._storeInformationSettings = storeInformationSettings;
            this._workContext = workContext;
            this._storeContext = storeContext;
        }

        #endregion

        #region Methods
        
        /// <summary>
        /// Returns a value indicating whether request is made by a mobile device
        /// </summary>
        /// <param name="httpContext">HTTP context</param>
        /// <returns>Result</returns>
        public virtual bool IsMobileDevice(HttpContextBase httpContext)
        {
            if (_storeInformationSettings.EmulateMobileDevice)
                return true;

            //comment the code below if you want tablets to be recognized as mobile devices.
            //nopCommerce uses the free edition of the 51degrees.mobi library for detecting browser mobile properties.
            //by default this property (IsTablet) is always false. you will need the premium edition in order to get it supported.
            bool isTablet = false;
            if (bool.TryParse(httpContext.Request.Browser["IsTablet"], out isTablet) && isTablet)
                return false;

            if (httpContext.Request.Browser.IsMobileDevice)
                return true;

            return false;
        }

        /// <summary>
        /// Returns a value indicating whether mobile devices support is enabled
        /// </summary>
        public virtual bool MobileDevicesSupported()
        {
            return _storeInformationSettings.MobileDevicesSupported;
        }

        /// <summary>
        /// Returns a value indicating whether current customer prefer to use full desktop version (even request is made by a mobile device)
        /// </summary>
        public virtual bool CustomerDontUseMobileVersion()
        {
            return _workContext.CurrentCustomer.GetAttribute<bool>(SystemCustomerAttributeNames.DontUseMobileVersion, _storeContext.CurrentStore.Id);
        }

        #endregion
    }
}