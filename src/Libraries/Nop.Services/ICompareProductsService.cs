//------------------------------------------------------------------------------
// The contents of this file are subject to the nopCommerce Public License Version 1.0 ("License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at  http://www.nopCommerce.com/License.aspx. 
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// See the License for the specific language governing rights and limitations under the License.
// 
// The Original Code is nopCommerce.
// The Initial Developer of the Original Code is NopSolutions.
// All Rights Reserved.
// 
// Contributor(s): _______. 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain;
using System.Web;

namespace Nop.Services
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
        /// <returns>"Compare products" list</returns>
        List<Product> GetComparedProducts();


        /// <summary>
        /// Removes a product from a "compare products" list
        /// </summary>
        /// <param name="productId">Product identifer</param>
        void RemoveProductFromCompareList(int productId);

        /// <summary>
        /// Adds a product to a "compare products" list
        /// </summary>
        /// <param name="productId">Product identifer</param>
        void AddProductToCompareList(int productId);
    }
}
