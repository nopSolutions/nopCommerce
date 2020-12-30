using System.Threading.Tasks;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;

namespace Nop.Core
{
    /// <summary>
    /// Represents work context
    /// </summary>
    public interface IWorkContext
    {
        /// <summary>
        /// Gets the current customer
        /// </summary>
        Task<Customer> GetCurrentCustomerAsync();

        /// <summary>
        /// Sets the current customer
        /// </summary>
        /// <param name="customer">Current customer</param>
        Task SetCurrentCustomerAsync(Customer customer = null);

        /// <summary>
        /// Gets the original customer (in case the current one is impersonated)
        /// </summary>
        Customer OriginalCustomerIfImpersonated { get; }

        /// <summary>
        /// Gets the current vendor (logged-in manager)
        /// </summary>
        Task<Vendor> GetCurrentVendorAsync();

        /// <summary>
        /// Gets current user working language
        /// </summary>
        Task<Language> GetWorkingLanguageAsync();

        /// <summary>
        /// Sets current user working language
        /// </summary>
        /// <param name="language">Language</param>
        Task SetWorkingLanguageAsync(Language language);

        /// <summary>
        /// Gets or sets current user working currency
        /// </summary>
        Task<Currency> GetWorkingCurrencyAsync();

        /// <summary>
        /// Sets current user working currency
        /// </summary>
        /// <param name="currency">Currency</param>
        Task SetWorkingCurrencyAsync(Currency currency);

        /// <summary>
        /// Gets or sets current tax display type
        /// </summary>
        Task<TaxDisplayType> GetTaxDisplayTypeAsync();

        /// <summary>
        /// Sets current tax display type
        /// </summary>
        Task SetTaxDisplayTypeAsync(TaxDisplayType taxDisplayType);        
    }
}
