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
        Task<Customer> GetCurrentCustomer();

        /// <summary>
        /// Sets the current customer
        /// </summary>
        /// <param name="customer">Current customer</param>
        Task SetCustomer(Customer customer = null);

        /// <summary>
        /// Gets the original customer (in case the current one is impersonated)
        /// </summary>
        Customer OriginalCustomerIfImpersonated { get; }

        /// <summary>
        /// Gets the current vendor (logged-in manager)
        /// </summary>
        Task<Vendor> GetCurrentVendor();

        /// <summary>
        /// Gets current user working language
        /// </summary>
        Task<Language> GetWorkingLanguage();

        /// <summary>
        /// Sets current user working language
        /// </summary>
        /// <param name="language">Language</param>
        Task SetWorkingLanguage(Language language);

        /// <summary>
        /// Gets or sets current user working currency
        /// </summary>
        Task<Currency> GetWorkingCurrency();

        /// <summary>
        /// Sets current user working currency
        /// </summary>
        /// <param name="currency">Currency</param>
        Task SetWorkingCurrency(Currency currency);

        /// <summary>
        /// Gets or sets current tax display type
        /// </summary>
        Task<TaxDisplayType> GetTaxDisplayType();

        /// <summary>
        /// Sets current tax display type
        /// </summary>
        Task SetTaxDisplayType(TaxDisplayType taxDisplayType);

        /// <summary>
        /// Gets or sets value indicating whether we're in admin area
        /// </summary>
        bool IsAdmin { get; set; }
    }
}
