<<<<<<< HEAD
﻿using System.Threading.Tasks;
using Nop.Services.Plugins;

namespace Nop.Services.Tax
{
    /// <summary>
    /// Provides an interface for creating tax providers
    /// </summary>
    public partial interface ITaxProvider : IPlugin
    {
        /// <summary>
        /// Gets tax rate
        /// </summary>
        /// <param name="taxRateRequest">Tax rate request</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the ax
        /// </returns>
        Task<TaxRateResult> GetTaxRateAsync(TaxRateRequest taxRateRequest);

        /// <summary>
        /// Gets tax total
        /// </summary>
        /// <param name="taxTotalRequest">Tax total request</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the ax total
        /// </returns>
        Task<TaxTotalResult> GetTaxTotalAsync(TaxTotalRequest taxTotalRequest);
    }
}
=======
﻿using System.Threading.Tasks;
using Nop.Services.Plugins;

namespace Nop.Services.Tax
{
    /// <summary>
    /// Provides an interface for creating tax providers
    /// </summary>
    public partial interface ITaxProvider : IPlugin
    {
        /// <summary>
        /// Gets tax rate
        /// </summary>
        /// <param name="taxRateRequest">Tax rate request</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the ax
        /// </returns>
        Task<TaxRateResult> GetTaxRateAsync(TaxRateRequest taxRateRequest);

        /// <summary>
        /// Gets tax total
        /// </summary>
        /// <param name="taxTotalRequest">Tax total request</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the ax total
        /// </returns>
        Task<TaxTotalResult> GetTaxTotalAsync(TaxTotalRequest taxTotalRequest);
    }
}
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
