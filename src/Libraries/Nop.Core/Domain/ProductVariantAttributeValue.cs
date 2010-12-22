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

namespace Nop.Core.Domain
{
    /// <summary>
    /// Represents a product variant attribute value
    /// </summary>
    public partial class ProductVariantAttributeValue : BaseEntity
    {
        /// <summary>
        /// Gets or sets the product variant attribute mapping identifier
        /// </summary>
        public int ProductVariantAttributeId { get; set; }

        /// <summary>
        /// Gets or sets the product variant attribute name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the price adjustment
        /// </summary>
        public decimal PriceAdjustment { get; set; }

        /// <summary>
        /// Gets or sets the weight adjustment
        /// </summary>
        public decimal WeightAdjustment { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the value is pre-selected
        /// </summary>
        public bool IsPreSelected { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }
        
        /// <summary>
        /// Gets the product variant attribute
        /// </summary>
        public virtual ProductVariantAttribute ProductVariantAttribute { get; set; }

        /// <summary>
        /// Gets the localized product variant attribute values
        /// </summary>
        public virtual ICollection<LocalizedProductVariantAttributeValue> LocalizedProductVariantAttributeValues { get; set; }
    }

}
