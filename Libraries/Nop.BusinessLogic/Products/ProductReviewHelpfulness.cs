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

namespace NopSolutions.NopCommerce.BusinessLogic.Products
{
    /// <summary>
    /// Represents a product review helpfulness
    /// </summary>
    public partial class ProductReviewHelpfulness : BaseEntity
    {
        #region Ctor
        /// <summary>
        /// Creates a new instance of the ProductReviewHelpfulness class
        /// </summary>
        public ProductReviewHelpfulness()
        {
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the identifier
        /// </summary>
        public int ProductReviewHelpfulnessId { get; set; }

        /// <summary>
        /// Gets or sets the product review identifier
        /// </summary>
        public int ProductReviewId { get; set; }

        /// <summary>
        /// Gets or sets the customer identifier
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether the review was helpful
        /// </summary>
        public bool WasHelpful { get; set; }

        #endregion

        #region Navigation Properties

        /// <summary>
        /// Gets the product review
        /// </summary>
        public virtual ProductReview NpProductReview { get; set; }

        #endregion
    }
}