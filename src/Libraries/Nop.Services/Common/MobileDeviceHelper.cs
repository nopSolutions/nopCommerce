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

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="storeInformationSettings">Store information settings</param>
        /// <param name="workContext">Work context</param>
        public MobileDeviceHelper(StoreInformationSettings storeInformationSettings,
            IWorkContext workContext)
        {
            this._storeInformationSettings = storeInformationSettings;
            this._workContext = workContext;
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
            return httpContext.Request.Browser.IsMobileDevice ||
                _storeInformationSettings.EmulateMobileDevice;
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
            return _workContext.CurrentCustomer.GetAttribute<bool>(SystemCustomerAttributeNames.DontUseMobileVersion);
        }

        #endregion
    }
}