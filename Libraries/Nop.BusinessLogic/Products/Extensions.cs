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
using System.Text;
using NopSolutions.NopCommerce.Common.Utils.Html;


namespace NopSolutions.NopCommerce.BusinessLogic.Products
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Finds a related product item by specified identifiers
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="productId1">The first product identifier</param>
        /// <param name="productId2">The second product identifier</param>
        /// <returns>Related product</returns>
        public static RelatedProduct FindRelatedProduct(this List<RelatedProduct> source,
            int productId1, int productId2)
        {
           foreach (RelatedProduct relatedProduct in source)
                if (relatedProduct.ProductId1 == productId1 && relatedProduct.ProductId2 == productId2)
                    return relatedProduct;
            return null;
        }

        /// <summary>
        /// Finds a cross-sell product item by specified identifiers
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="productId1">The first product identifier</param>
        /// <param name="productId2">The second product identifier</param>
        /// <returns>Cross-sell product</returns>
        public static CrossSellProduct FindCrossSellProduct(this List<CrossSellProduct> source,
            int productId1, int productId2)
        {
            foreach (CrossSellProduct crossSellProduct in source)
                if (crossSellProduct.ProductId1 == productId1 && crossSellProduct.ProductId2 == productId2)
                    return crossSellProduct;
            return null;
        }
        
        /// <summary>
        ///  Formats the product review text
        /// </summary>
        /// <param name="productReview">Product review</param>
        /// <returns>Formatted text</returns>
        public static string FormatProductReviewText(this ProductReview productReview)
        {
            if (productReview == null || String.IsNullOrEmpty(productReview.ReviewText))
                return string.Empty;

            string result = HtmlHelper.FormatText(productReview.ReviewText, false, true, false, false, false, false);
            return result;
        }

        /// <summary>
        ///  Formats the email a friend text
        /// </summary>
        /// <param name="productReview">Product review</param>
        /// <returns>Formatted text</returns>
        public static string FormatEmailAFriendText(this string text)
        {
            if (String.IsNullOrEmpty(text))
                return string.Empty;

            string result = HtmlHelper.FormatText(text, false, true, false, false, false, false);
            return result;
        }
    }
}
