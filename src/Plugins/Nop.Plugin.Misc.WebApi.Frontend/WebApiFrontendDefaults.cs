namespace Nop.Plugin.Misc.WebApi.Frontend;

/// <summary>
/// Represents plugin constants
/// </summary>
public class WebApiFrontendDefaults
{
    /// <summary>
    /// Gets a plugin system name
    /// </summary>
    public static string SystemName => "Misc.WebApi.Frontend";

    #region Route names

    /// <summary>
    /// Represents the route names
    /// </summary>
    public class Route
    {
        /// <summary>
        /// Gets the products API route name
        /// </summary>
        public static string Products => "Plugin.Misc.WebApi.Frontend.ApiProducts";

        /// <summary>
        /// Gets the customers API route name
        /// </summary>
        public static string Customers => "Plugin.Misc.WebApi.Frontend.ApiCustomers";

        /// <summary>
        /// Gets the orders API route name
        /// </summary>
        public static string Orders => "Plugin.Misc.WebApi.Frontend.ApiOrders";

        /// <summary>
        /// Gets the authentication API route name
        /// </summary>
        public static string Authentication => "Plugin.Misc.WebApi.Frontend.ApiAuthentication";
    }

    #endregion
}