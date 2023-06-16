

namespace Nop.Plugin.GoogleAuth
{
    /// <summary>
    /// Represents plugin constants
    /// </summary>
    public class GoogleAuthenticationDefaults
    {
        /// <summary>
        /// Gets a plugin system name
        /// </summary>
        public static string SystemName => "GoogleAuth";

        /// <summary>
        /// Gets a name of the route to the data deletion callback
        /// </summary>
        public static string DataDeletionCallbackRoute => "Plugin.GoogleAuth.DataDeletionCallback";

        /// <summary>
        /// Gets a name of the route to the data deletion status check
        /// </summary>
        public static string DataDeletionStatusCheckRoute => "Plugin.GoogleAuth.DataDeletionStatusCheck";

        /// <summary>
        /// Gets a name of error callback method
        /// </summary>
        public static string ErrorCallback => "ErrorCallback";
    }
}