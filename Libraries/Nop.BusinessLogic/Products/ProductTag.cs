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

using System.Collections.Generic;

namespace NopSolutions.NopCommerce.BusinessLogic.Products
{
    /// <summary>
    /// Represents a product tag
    /// </summary>
    public partial class ProductTag : BaseEntity
    {
        #region Ctor
        /// <summary>
        /// Creates a new instance of the ProductTag class
        /// </summary>
        public ProductTag()
        {
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the product identifier
        /// </summary>
        public int ProductTagId { get; set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the tagged product count
        /// </summary>
        public int ProductCount { get; set; }

        #endregion

        #region Navigation Properties

        /// <summary>
        /// Gets the products
        /// </summary>
        public virtual  ICollection<Product> NpProducts { get; set; }

        #endregion
    }
}
