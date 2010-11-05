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
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Categories;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Media;
using NopSolutions.NopCommerce.BusinessLogic.Products.Attributes;
using NopSolutions.NopCommerce.BusinessLogic.Promo.Discounts;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Tax;
using NopSolutions.NopCommerce.BusinessLogic.Warehouses;
using NopSolutions.NopCommerce.BusinessLogic.IoC;

namespace NopSolutions.NopCommerce.BusinessLogic.Products
{
	/// <summary>
    /// Represents a product variant pricelist
	/// </summary>
    public partial class ProductVariantPricelist : BaseEntity
    {
        #region Ctor
        /// <summary>
        /// Creates a new instance of the ProductVariantPricelist class
        /// </summary>
        public ProductVariantPricelist()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the product variant pricelist identifier
        /// </summary>
        public int ProductVariantPricelistId { get; set; }

        /// <summary>
        /// Gets or sets the product variant identifer
        /// </summary>
        public int ProductVariantId { get; set; }

        /// <summary>
        /// Gets or sets the pricelist identifier
        /// </summary>
        public int PricelistId { get; set; }

        /// <summary>
        /// Gets or sets the type of price adjustment (if used) (relative or absolute)
        /// </summary>
        public int PriceAdjustmentTypeId { get; set; }

        /// <summary>
        /// Gets or sets the price will be adjusted by this amount (in accordance with PriceAdjustmentType)
        /// </summary>
        public decimal PriceAdjustment { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance update
        /// </summary>
        public DateTime UpdatedOn { get; set; }

        #endregion

        #region Custom Properties

        /// <summary>
        /// Gets the product variant
        /// </summary>
        public ProductVariant ProductVariant
        {
            get
            {
                return IoCFactory.Resolve<IProductService>().GetProductVariantById(this.ProductVariantId);
            }
        }

        /// <summary>
        /// Gets the pricelist
        /// </summary>
        public Pricelist Pricelist
        {
            get
            {
                return IoCFactory.Resolve<IProductService>().GetPricelistById(this.PricelistId);
            }
        }

        /// <summary>
        /// Gets the log type
        /// </summary>
        public PriceAdjustmentTypeEnum PriceAdjustmentType
        {
            get
            {
                return (PriceAdjustmentTypeEnum)this.PriceAdjustmentTypeId;
            }
            set
            {
                this.PriceAdjustmentTypeId = (int)value;
            }
        }

        #endregion

        #region Navigation Properties

        /// <summary>
        /// Gets the product variant
        /// </summary>
        public virtual ProductVariant NpProductVariant { get; set; }

        /// <summary>
        /// Gets the pricelist
        /// </summary>
        public virtual Pricelist NpPricelist { get; set; }

        #endregion
    }
}

