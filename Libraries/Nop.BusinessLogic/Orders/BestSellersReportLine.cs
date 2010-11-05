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
using NopSolutions.NopCommerce.BusinessLogic.IoC;


namespace NopSolutions.NopCommerce.BusinessLogic.Orders
{
    /// <summary>
    /// Represents a best sellers report line
    /// </summary>
    public partial class BestSellersReportLine : BaseEntity
    {
        #region Fields
        private ProductVariant _pv = null;
        #endregion

        #region Ctor
        /// <summary>
        /// Creates a new instance of the BestSellersReportLine class
        /// </summary>
        public BestSellersReportLine()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the product variant identifier
        /// </summary>
        public int ProductVariantId { get; set; }

        /// <summary>
        /// Gets or sets the total count
        /// </summary>
        public int SalesTotalCount { get; set; }

        /// <summary>
        /// Gets or sets the total amount
        /// </summary>
        public decimal SalesTotalAmount { get; set; }

        #endregion

        #region Custom Properties
        /// <summary>
        /// Gets a product variant
        /// </summary>
        public ProductVariant ProductVariant
        {
            get
            {
                if (_pv == null)
                    _pv = IoCFactory.Resolve<IProductService>().GetProductVariantById(this.ProductVariantId);
                return _pv;
            }
        }
        #endregion
    }

}
