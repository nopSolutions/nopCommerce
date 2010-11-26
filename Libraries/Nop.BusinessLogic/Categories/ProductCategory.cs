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

using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;
using NopSolutions.NopCommerce.BusinessLogic.Products;

namespace NopSolutions.NopCommerce.BusinessLogic.Categories
{
    /// <summary>
    /// Represents a product category mapping
    /// </summary>
    public partial class ProductCategory : BaseEntity
    {
        #region Properties
        /// <summary>
        /// Gets or sets the ProductCategory identifier
        /// </summary>
        public int ProductCategoryId { get; set; }

        /// <summary>
        /// Gets or sets the product identifier
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Gets or sets the category identifier
        /// </summary>
        public int CategoryId { get; set; }

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
        /// Gets the category
        /// </summary>
        public Category Category
        {
            get
            {
                return IoC.Resolve<ICategoryService>().GetCategoryById(this.CategoryId);
            }
        }

        /// <summary>
        /// Gets the product
        /// </summary>
        public Product Product
        {
            get
            {
                return IoC.Resolve<IProductService>().GetProductById(this.ProductId);
            }
        }

        #endregion

        #region Navigation Properties

        /// <summary>
        /// Gets the category
        /// </summary>
        public virtual Category NpCategory { get; set; }

        /// <summary>
        /// Gets the product
        /// </summary>
        public virtual Product NpProduct { get; set; }

        #endregion
    }

}
