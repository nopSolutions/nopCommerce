<<<<<<< HEAD
﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Compare products service interface
    /// </summary>
    public partial interface ICompareProductsService
    {
        /// <summary>
        /// Clears a "compare products" list
        /// </summary>
        void ClearCompareProducts();

        /// <summary>
        /// Gets a "compare products" list
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the "Compare products" list
        /// </returns>
        Task<IList<Product>> GetComparedProductsAsync();

        /// <summary>
        /// Removes a product from a "compare products" list
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task RemoveProductFromCompareListAsync(int productId);

        /// <summary>
        /// Adds a product to a "compare products" list
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task AddProductToCompareListAsync(int productId);
    }
}
=======
﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Compare products service interface
    /// </summary>
    public partial interface ICompareProductsService
    {
        /// <summary>
        /// Clears a "compare products" list
        /// </summary>
        void ClearCompareProducts();

        /// <summary>
        /// Gets a "compare products" list
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the "Compare products" list
        /// </returns>
        Task<IList<Product>> GetComparedProductsAsync();

        /// <summary>
        /// Removes a product from a "compare products" list
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task RemoveProductFromCompareListAsync(int productId);

        /// <summary>
        /// Adds a product to a "compare products" list
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task AddProductToCompareListAsync(int productId);
    }
}
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
