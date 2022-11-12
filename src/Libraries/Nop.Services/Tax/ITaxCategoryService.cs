<<<<<<< HEAD
<<<<<<< HEAD
﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core.Domain.Tax;

namespace Nop.Services.Tax
{
    /// <summary>
    /// Tax category service interface
    /// </summary>
    public partial interface ITaxCategoryService
    {
        /// <summary>
        /// Deletes a tax category
        /// </summary>
        /// <param name="taxCategory">Tax category</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteTaxCategoryAsync(TaxCategory taxCategory);

        /// <summary>
        /// Gets all tax categories
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the ax categories
        /// </returns>
        Task<IList<TaxCategory>> GetAllTaxCategoriesAsync();

        /// <summary>
        /// Gets a tax category
        /// </summary>
        /// <param name="taxCategoryId">Tax category identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the ax category
        /// </returns>
        Task<TaxCategory> GetTaxCategoryByIdAsync(int taxCategoryId);

        /// <summary>
        /// Inserts a tax category
        /// </summary>
        /// <param name="taxCategory">Tax category</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertTaxCategoryAsync(TaxCategory taxCategory);

        /// <summary>
        /// Updates the tax category
        /// </summary>
        /// <param name="taxCategory">Tax category</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdateTaxCategoryAsync(TaxCategory taxCategory);
    }
}
=======
=======
=======
<<<<<<< HEAD
﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core.Domain.Tax;

namespace Nop.Services.Tax
{
    /// <summary>
    /// Tax category service interface
    /// </summary>
    public partial interface ITaxCategoryService
    {
        /// <summary>
        /// Deletes a tax category
        /// </summary>
        /// <param name="taxCategory">Tax category</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteTaxCategoryAsync(TaxCategory taxCategory);

        /// <summary>
        /// Gets all tax categories
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the ax categories
        /// </returns>
        Task<IList<TaxCategory>> GetAllTaxCategoriesAsync();

        /// <summary>
        /// Gets a tax category
        /// </summary>
        /// <param name="taxCategoryId">Tax category identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the ax category
        /// </returns>
        Task<TaxCategory> GetTaxCategoryByIdAsync(int taxCategoryId);

        /// <summary>
        /// Inserts a tax category
        /// </summary>
        /// <param name="taxCategory">Tax category</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertTaxCategoryAsync(TaxCategory taxCategory);

        /// <summary>
        /// Updates the tax category
        /// </summary>
        /// <param name="taxCategory">Tax category</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdateTaxCategoryAsync(TaxCategory taxCategory);
    }
}
=======
>>>>>>> cf758b6c548f45d8d46cc74e51253de0619d95dc
>>>>>>> 974287325803649b246516d81982b95e372d09b9
﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core.Domain.Tax;

namespace Nop.Services.Tax
{
    /// <summary>
    /// Tax category service interface
    /// </summary>
    public partial interface ITaxCategoryService
    {
        /// <summary>
        /// Deletes a tax category
        /// </summary>
        /// <param name="taxCategory">Tax category</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteTaxCategoryAsync(TaxCategory taxCategory);

        /// <summary>
        /// Gets all tax categories
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the ax categories
        /// </returns>
        Task<IList<TaxCategory>> GetAllTaxCategoriesAsync();

        /// <summary>
        /// Gets a tax category
        /// </summary>
        /// <param name="taxCategoryId">Tax category identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the ax category
        /// </returns>
        Task<TaxCategory> GetTaxCategoryByIdAsync(int taxCategoryId);

        /// <summary>
        /// Inserts a tax category
        /// </summary>
        /// <param name="taxCategory">Tax category</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertTaxCategoryAsync(TaxCategory taxCategory);

        /// <summary>
        /// Updates the tax category
        /// </summary>
        /// <param name="taxCategory">Tax category</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdateTaxCategoryAsync(TaxCategory taxCategory);
    }
}
<<<<<<< HEAD
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
=======
<<<<<<< HEAD
=======
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
>>>>>>> cf758b6c548f45d8d46cc74e51253de0619d95dc
>>>>>>> 974287325803649b246516d81982b95e372d09b9
