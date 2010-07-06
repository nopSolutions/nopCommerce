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
using System.Text;
using NopSolutions.NopCommerce.BusinessLogic.Products;



namespace NopSolutions.NopCommerce.BusinessLogic.Manufacturers
{
    /// <summary>
    /// Represents a product manufacturer mapping
    /// </summary>
    public partial class ProductManufacturer : BaseEntity
    {
        #region Ctor
        /// <summary>
        /// Creates a new instance of the ProductManufacturer class
        /// </summary>
        public ProductManufacturer()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the ProductManufacturer identifier
        /// </summary>
        public int ProductManufacturerId { get; set; }

        /// <summary>
        /// Gets or sets the product identifier
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Gets or sets the manufacturer identifier
        /// </summary>
        public int ManufacturerId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product is featured
        /// </summary>
        public bool IsFeaturedProduct { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }
        #endregion 

        #region Custom Properties
        /// <summary>
        /// Gets the manufacturer
        /// </summary>
        public Manufacturer Manufacturer
        {
            get
            {
                return ManufacturerManager.GetManufacturerById(this.ManufacturerId);
            }
        }

        /// <summary>
        /// Gets the product
        /// </summary>
        public Product Product
        {
            get
            {
                return ProductManager.GetProductById(this.ProductId);
            }
        }

        #endregion

        #region Navigation Properties

        /// <summary>
        /// Gets the manufacturer
        /// </summary>
        public virtual Manufacturer NpManufacturer { get; set; }

        /// <summary>
        /// Gets the product
        /// </summary>
        public virtual Product NpProduct { get; set; }

        #endregion
    }

}
