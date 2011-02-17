
using Nop.Services.Configuration;
namespace Nop.Services.Tax
{
    /// <summary>
    /// Provides an interface for creating tax providers
    /// </summary>
    public partial interface ITaxProvider
    {
        /// <summary>
        /// Gets or sets the friendly name
        /// </summary>
        string FriendlyName { get; }

        /// <summary>
        /// Gets or sets the system name
        /// </summary>
        string SystemName { get; }

        /// <summary>
        /// Gets or sets the setting service
        /// </summary>
        ISettingService SettingService { get; set; }

        /// <summary>
        /// Gets tax rate
        /// </summary>
        /// <param name="calculateTaxRequest">Tax calculation request</param>
        /// <returns>Tax</returns>
        CalculateTaxResult GetTaxRate(CalculateTaxRequest calculateTaxRequest);

    }
}
