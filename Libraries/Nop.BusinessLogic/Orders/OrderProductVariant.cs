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
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;
using NopSolutions.NopCommerce.BusinessLogic.Media;
using NopSolutions.NopCommerce.BusinessLogic.Products;

namespace NopSolutions.NopCommerce.BusinessLogic.Orders
{
    /// <summary>
    /// Represents an order product variant
    /// </summary>
    public partial class OrderProductVariant : BaseEntity
    {
        #region Fields
        private Order _order;
        private ProductVariant _pv;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the order product variant identifier
        /// </summary>
        public int OrderProductVariantId { get; set; }

        /// <summary>
        /// Gets or sets the order product variant identifier
        /// </summary>
        public Guid OrderProductVariantGuid { get; set; }

        /// <summary>
        /// Gets or sets the order identifier
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// Gets or sets the product variant identifier
        /// </summary>
        public int ProductVariantId { get; set; }

        /// <summary>
        /// Gets or sets the unit price in primary store currency (incl tax)
        /// </summary>
        public decimal UnitPriceInclTax { get; set; }

        /// <summary>
        /// Gets or sets the unit price in primary store currency (excl tax)
        /// </summary>
        public decimal UnitPriceExclTax { get; set; }

        /// <summary>
        /// Gets or sets the price in primary store currency (incl tax)
        /// </summary>
        public decimal PriceInclTax { get; set; }

        /// <summary>
        /// Gets or sets the price in primary store currency (excl tax)
        /// </summary>
        public decimal PriceExclTax { get; set; }

        /// <summary>
        /// Gets or sets the unit price in customer currency (incl tax)
        /// </summary>
        public decimal UnitPriceInclTaxInCustomerCurrency { get; set; }

        /// <summary>
        /// Gets or sets the unit price in customer currency (excl tax)
        /// </summary>
        public decimal UnitPriceExclTaxInCustomerCurrency { get; set; }

        /// <summary>
        /// Gets or sets the price in customer currency (incl tax)
        /// </summary>
        public decimal PriceInclTaxInCustomerCurrency { get; set; }

        /// <summary>
        /// Gets or sets the price in customer currency (excl tax)
        /// </summary>
        public decimal PriceExclTaxInCustomerCurrency { get; set; }

        /// <summary>
        /// Gets or sets the attribute description
        /// </summary>
        public string AttributeDescription { get; set; }

        /// <summary>
        /// Gets or sets the product variant attributes in XML format
        /// </summary>
        public string AttributesXml { get; set; }

        /// <summary>
        /// Gets or sets the quantity
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the discount amount (incl tax)
        /// </summary>
        public decimal DiscountAmountInclTax { get; set; }

        /// <summary>
        /// Gets or sets the discount amount (excl tax)
        /// </summary>
        public decimal DiscountAmountExclTax { get; set; }

        /// <summary>
        /// Gets or sets the download count
        /// </summary>
        public int DownloadCount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether download is activated
        /// </summary>
        public bool IsDownloadActivated { get; set; }

        /// <summary>
        /// Gets or sets a license download identifier (in case this is a downloadable product)
        /// </summary>
        public int LicenseDownloadId { get; set; }

        #endregion

        #region Custom Properties
        /// <summary>
        /// Gets the order
        /// </summary>
        public Order Order
        {
            get
            {
                if (_order == null)
                    _order = IoC.Resolve<IOrderService>().GetOrderById(this.OrderId);
                return _order;
            }
        }

        /// <summary>
        /// Gets the product variant
        /// </summary>
        public ProductVariant ProductVariant
        {
            get
            {
                if (_pv == null)
                    _pv = IoC.Resolve<IProductService>().GetProductVariantById(this.ProductVariantId);
                return _pv;
            }
        }

        /// <summary>
        /// Gets the license download
        /// </summary>
        public Download LicenseDownload
        {
            get
            {
                return IoC.Resolve<IDownloadService>().GetDownloadById(this.LicenseDownloadId);
            }
        }
        #endregion

        #region Navigation Properties

        /// <summary>
        /// Gets the order
        /// </summary>
        public virtual Order NpOrder { get; set; }

        /// <summary>
        /// Gets the product variant
        /// </summary>
        public virtual ProductVariant NpProductVariant { get; set; }

        /// <summary>
        /// Gets the purchased gift cards
        /// </summary>
        public virtual ICollection<GiftCard> NpGiftCard { get; set; }
        
        #endregion
    }
}
