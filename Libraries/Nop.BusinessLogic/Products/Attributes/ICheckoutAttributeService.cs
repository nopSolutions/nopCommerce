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
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using NopSolutions.NopCommerce.BusinessLogic.Caching;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Data;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

namespace NopSolutions.NopCommerce.BusinessLogic.Products.Attributes
{
    /// <summary>
    /// Checkout attribute service interface
    /// </summary>
    public partial interface ICheckoutAttributeService
    {
        #region Checkout attributes

        /// <summary>
        /// Deletes a checkout attribute
        /// </summary>
        /// <param name="checkoutAttributeId">Checkout attribute identifier</param>
        void DeleteCheckoutAttribute(int checkoutAttributeId);

        /// <summary>
        /// Gets all checkout attributes
        /// </summary>
        /// <param name="dontLoadShippableProductRequired">Value indicating whether to do not load attributes for checkout attibutes which require shippable products</param>
        /// <returns>Checkout attribute collection</returns>
        List<CheckoutAttribute> GetAllCheckoutAttributes(bool dontLoadShippableProductRequired);

        /// <summary>
        /// Gets a checkout attribute 
        /// </summary>
        /// <param name="checkoutAttributeId">Checkout attribute identifier</param>
        /// <returns>Checkout attribute</returns>
        CheckoutAttribute GetCheckoutAttributeById(int checkoutAttributeId);

        /// <summary>
        /// Inserts a checkout attribute
        /// </summary>
        /// <param name="checkoutAttribute">Checkout attribute</param>
        void InsertCheckoutAttribute(CheckoutAttribute checkoutAttribute);

        /// <summary>
        /// Updates the checkout attribute
        /// </summary>
        /// <param name="checkoutAttribute">Checkout attribute</param>
        /// <param name="displayOrder">Display order</param>
        void UpdateCheckoutAttribute(CheckoutAttribute checkoutAttribute);

        /// <summary>
        /// Gets localized checkout attribute by id
        /// </summary>
        /// <param name="checkoutAttributeLocalizedId">Localized checkout attribute identifier</param>
        /// <returns>Checkout attribute content</returns>
        CheckoutAttributeLocalized GetCheckoutAttributeLocalizedById(int checkoutAttributeLocalizedId);

        /// <summary>
        /// Gets localized checkout attribute by category id
        /// </summary>
        /// <param name="checkoutAttributeId">Checkout attribute identifier</param>
        /// <returns>Checkout attribute content</returns>
        List<CheckoutAttributeLocalized> GetCheckoutAttributeLocalizedByCheckoutAttributeId(int checkoutAttributeId);

        /// <summary>
        /// Gets localized checkout attribute by checkout attribute id and language id
        /// </summary>
        /// <param name="checkoutAttributeId">Checkout attribute identifier</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Checkout attribute content</returns>
        CheckoutAttributeLocalized GetCheckoutAttributeLocalizedByCheckoutAttributeIdAndLanguageId(int checkoutAttributeId, int languageId);
        
        /// <summary>
        /// Inserts a localized checkout attribute
        /// </summary>
        /// <param name="checkoutAttributeLocalized">Checkout attribute content</param>
        void InsertCheckoutAttributeLocalized(CheckoutAttributeLocalized checkoutAttributeLocalized);

        /// <summary>
        /// Update a localized checkout attribute
        /// </summary>
        /// <param name="checkoutAttributeLocalized">Checkout attribute content</param>
        void UpdateCheckoutAttributeLocalized(CheckoutAttributeLocalized checkoutAttributeLocalized);

        #endregion

        #region Checkout variant attribute values

        /// <summary>
        /// Deletes a checkout attribute value
        /// </summary>
        /// <param name="checkoutAttributeValueId">Checkout attribute value identifier</param>
        void DeleteCheckoutAttributeValue(int checkoutAttributeValueId);

        /// <summary>
        /// Gets checkout attribute values by checkout attribute identifier
        /// </summary>
        /// <param name="checkoutAttributeId">The checkout attribute identifier</param>
        /// <returns>Checkout attribute value collection</returns>
        List<CheckoutAttributeValue> GetCheckoutAttributeValues(int checkoutAttributeId);

        /// <summary>
        /// Gets a checkout attribute value
        /// </summary>
        /// <param name="checkoutAttributeValueId">Checkout attribute value identifier</param>
        /// <returns>Checkout attribute value</returns>
        CheckoutAttributeValue GetCheckoutAttributeValueById(int checkoutAttributeValueId);

        /// <summary>
        /// Inserts a checkout attribute value
        /// </summary>
        /// <param name="checkoutAttributeValue">Checkout attribute value</param>
        void InsertCheckoutAttributeValue(CheckoutAttributeValue checkoutAttributeValue);

        /// <summary>
        /// Updates the checkout attribute value
        /// </summary>
        /// <param name="checkoutAttributeValue">Checkout attribute value</param>
        void UpdateCheckoutAttributeValue(CheckoutAttributeValue checkoutAttributeValue);

        /// <summary>
        /// Gets localized checkout attribute value by id
        /// </summary>
        /// <param name="checkoutAttributeValueLocalizedId">Localized checkout attribute value identifier</param>
        /// <returns>Localized checkout attribute value</returns>
        CheckoutAttributeValueLocalized GetCheckoutAttributeValueLocalizedById(int checkoutAttributeValueLocalizedId);

        /// <summary>
        /// Gets localized checkout attribute value by checkout attribute value id
        /// </summary>
        /// <param name="checkoutAttributeValueId">Checkout attribute value identifier</param>
        /// <returns>Localized checkout attribute value</returns>
        List<CheckoutAttributeValueLocalized> GetCheckoutAttributeValueLocalizedByCheckoutAttributeValueId(int checkoutAttributeValueId);

        /// <summary>
        /// Gets localized checkout attribute value by checkout attribute value id and language id
        /// </summary>
        /// <param name="checkoutAttributeValueId">Checkout attribute value identifier</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Localized checkout attribute value</returns>
        CheckoutAttributeValueLocalized GetCheckoutAttributeValueLocalizedByCheckoutAttributeValueIdAndLanguageId(int checkoutAttributeValueId, int languageId);

        /// <summary>
        /// Inserts a localized checkout attribute value
        /// </summary>
        /// <param name="checkoutAttributeValueLocalized">Localized checkout attribute value</param>
        void InsertCheckoutAttributeValueLocalized(CheckoutAttributeValueLocalized checkoutAttributeValueLocalized);

        /// <summary>
        /// Update a localized checkout attribute value
        /// </summary>
        /// <param name="checkoutAttributeValueLocalized">Localized checkout attribute value</param>
        void UpdateCheckoutAttributeValueLocalized(CheckoutAttributeValueLocalized checkoutAttributeValueLocalized);

        #endregion
    }
}
