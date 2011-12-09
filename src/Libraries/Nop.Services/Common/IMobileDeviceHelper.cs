
using System.Web;

namespace Nop.Services.Common
{
    /// <summary>
    /// Mobile device helper interface
    /// </summary>
    public partial interface IMobileDeviceHelper
    {
        /// <summary>
        /// Returns a value indicating whether request is made by a mobile device
        /// </summary>
        /// <param name="httpContext">HTTP context</param>
        /// <returns>Result</returns>
        bool IsMobileDevice(HttpContextBase httpContext);

        /// <summary>
        /// Returns a value indicating whether mobile devices support is enabled
        /// </summary>
        bool MobileDevicesSupported();

        /// <summary>
        /// Returns a value indicating whether current customer prefer to use full desktop version (even request is made by a mobile device)
        /// </summary>
        bool CustomerDontUseMobileVersion();
    }
}