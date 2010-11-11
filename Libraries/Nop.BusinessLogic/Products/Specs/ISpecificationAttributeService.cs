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
using System.Linq;
using System.Xml;
using NopSolutions.NopCommerce.BusinessLogic.Caching;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Data;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

namespace NopSolutions.NopCommerce.BusinessLogic.Products.Specs
{
    /// <summary>
    /// Specification attribute service interface
    /// </summary>
    public partial interface ISpecificationAttributeService
    {
        #region Specification attribute

        /// <summary>
        /// Gets a specification attribute
        /// </summary>
        /// <param name="specificationAttributeId">The specification attribute identifier</param>
        /// <returns>Specification attribute</returns>
        SpecificationAttribute GetSpecificationAttributeById(int specificationAttributeId);

        /// <summary>
        /// Gets specification attribute collection
        /// </summary>
        /// <returns>Specification attribute collection</returns>
        List<SpecificationAttribute> GetSpecificationAttributes();

        /// <summary>
        /// Gets specification attribute collection
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Specification attribute collection</returns>
        List<SpecificationAttribute> GetSpecificationAttributes(int languageId);

        /// <summary>
        /// Deletes a specification attribute
        /// </summary>
        /// <param name="specificationAttributeId">The specification attribute identifier</param>
        void DeleteSpecificationAttribute(int specificationAttributeId);

        /// <summary>
        /// Inserts a specification attribute
        /// </summary>
        /// <param name="specificationAttribute">The specification attribute</param>
        void InsertSpecificationAttribute(SpecificationAttribute specificationAttribute);

        /// <summary>
        /// Updates the specification attribute
        /// </summary>
        /// <param name="specificationAttribute">The specification attribute</param>
        void UpdateSpecificationAttribute(SpecificationAttribute specificationAttribute);

        /// <summary>
        /// Gets localized specification attribute by id
        /// </summary>
        /// <param name="specificationAttributeLocalizedId">Localized specification identifier</param>
        /// <returns>Specification attribute content</returns>
        SpecificationAttributeLocalized GetSpecificationAttributeLocalizedById(int specificationAttributeLocalizedId);

        /// <summary>
        /// Gets localized specification attribute by specification attribute id
        /// </summary>
        /// <param name="specificationAttributeId">Specification attribute identifier</param>
        /// <returns>Secification attribute content</returns>
        List<SpecificationAttributeLocalized> GetSpecificationAttributeLocalizedBySpecificationAttributeId(int specificationAttributeId);

        /// <summary>
        /// Gets localized specification attribute by specification attribute id and language id
        /// </summary>
        /// <param name="specificationAttributeId">Specification attribute identifier</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Specification attribute content</returns>
        SpecificationAttributeLocalized GetSpecificationAttributeLocalizedBySpecificationAttributeIdAndLanguageId(int specificationAttributeId, int languageId);

        /// <summary>
        /// Inserts a localized specification attribute
        /// </summary>
        /// <param name="specificationAttributeLocalized">Localized specification attribute</param>
        void InsertSpecificationAttributeLocalized(SpecificationAttributeLocalized specificationAttributeLocalized);

        /// <summary>
        /// Update a localized specification attribute
        /// </summary>
        /// <param name="specificationAttributeLocalized">Localized specification attribute</param>
        void UpdateSpecificationAttributeLocalized(SpecificationAttributeLocalized specificationAttributeLocalized);

        #endregion

        #region Specification attribute option

        /// <summary>
        /// Gets a specification attribute option
        /// </summary>
        /// <param name="specificationAttributeOptionId">The specification attribute option identifier</param>
        /// <returns>Specification attribute option</returns>
        SpecificationAttributeOption GetSpecificationAttributeOptionById(int specificationAttributeOptionId);

        /// <summary>
        /// Gets a specification attribute option by specification attribute id
        /// </summary>
        /// <param name="specificationAttributeId">The specification attribute identifier</param>
        /// <returns>Specification attribute option</returns>
        List<SpecificationAttributeOption> GetSpecificationAttributeOptionsBySpecificationAttribute(int specificationAttributeId);

        /// <summary>
        /// Deletes a specification attribute option
        /// </summary>
        /// <param name="specificationAttributeOptionId">The specification attribute option identifier</param>
        void DeleteSpecificationAttributeOption(int specificationAttributeOptionId);

        /// <summary>
        /// Inserts a specification attribute option
        /// </summary>
        /// <param name="specificationAttributeOption">The specification attribute option</param>
        void InsertSpecificationAttributeOption(SpecificationAttributeOption specificationAttributeOption);

        /// <summary>
        /// Updates the specification attribute
        /// </summary>
        /// <param name="specificationAttributeOption">The specification attribute option</param>
        void UpdateSpecificationAttributeOptions(SpecificationAttributeOption specificationAttributeOption);

        /// <summary>
        /// Gets localized specification attribute option by id
        /// </summary>
        /// <param name="specificationAttributeOptionLocalizedId">Localized specification attribute option identifier</param>
        /// <returns>Localized specification attribute option</returns>
        SpecificationAttributeOptionLocalized GetSpecificationAttributeOptionLocalizedById(int specificationAttributeOptionLocalizedId);

        /// <summary>
        /// Gets localized specification attribute option by category id
        /// </summary>
        /// <param name="specificationAttributeOptionId">Specification attribute option identifier</param>
        /// <returns>Localized specification attribute option content</returns>
        List<SpecificationAttributeOptionLocalized> GetSpecificationAttributeOptionLocalizedBySpecificationAttributeOptionId(int specificationAttributeOptionId);


        /// <summary>
        /// Gets localized specification attribute option by specification attribute option id and language id
        /// </summary>
        /// <param name="specificationAttributeOptionId">Specification attribute option identifier</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Localized specification attribute option</returns>
        SpecificationAttributeOptionLocalized GetSpecificationAttributeOptionLocalizedBySpecificationAttributeOptionIdAndLanguageId(int specificationAttributeOptionId, int languageId);

        /// <summary>
        /// Inserts a localized specification attribute option
        /// </summary>
        /// <param name="specificationAttributeOptionLocalized">Localized specification attribute option</param>
        /// <returns>Localized specification attribute option</returns>
        void InsertSpecificationAttributeOptionLocalized(SpecificationAttributeOptionLocalized specificationAttributeOptionLocalized);

        /// <summary>
        /// Update a localized specification attribute option
        /// </summary>
        /// <param name="specificationAttributeOptionLocalized">Localized specification attribute option</param>
        /// <returns>Localized specification attribute option</returns>
        void UpdateSpecificationAttributeOptionLocalized(SpecificationAttributeOptionLocalized specificationAttributeOptionLocalized);

        #endregion

        #region Product specification attribute

        /// <summary>
        /// Deletes a product specification attribute mapping
        /// </summary>
        /// <param name="productSpecificationAttributeId">Product specification attribute identifier</param>
        void DeleteProductSpecificationAttribute(int productSpecificationAttributeId);

        /// <summary>
        /// Gets a product specification attribute mapping collection
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <returns>Product specification attribute mapping collection</returns>
        List<ProductSpecificationAttribute> GetProductSpecificationAttributesByProductId(int productId);

        /// <summary>
        /// Gets a product specification attribute mapping collection
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <param name="allowFiltering">0 to load attributes with AllowFiltering set to false, 0 to load attributes with AllowFiltering set to true, null to load all attributes</param>
        /// <param name="showOnProductPage">0 to load attributes with ShowOnProductPage set to false, 0 to load attributes with ShowOnProductPage set to true, null to load all attributes</param>
        /// <returns>Product specification attribute mapping collection</returns>
        List<ProductSpecificationAttribute> GetProductSpecificationAttributesByProductId(int productId,
            bool? allowFiltering, bool? showOnProductPage);

        /// <summary>
        /// Gets a product specification attribute mapping 
        /// </summary>
        /// <param name="productSpecificationAttributeId">Product specification attribute mapping identifier</param>
        /// <returns>Product specification attribute mapping</returns>
        ProductSpecificationAttribute GetProductSpecificationAttributeById(int productSpecificationAttributeId);

        /// <summary>
        /// Inserts a product specification attribute mapping
        /// </summary>
        /// <param name="productSpecificationAttribute">Product specification attribute mapping</param>
        void InsertProductSpecificationAttribute(ProductSpecificationAttribute productSpecificationAttribute);

        /// <summary>
        /// Updates the product specification attribute mapping
        /// </summary>
        /// <param name="productSpecificationAttribute">Product specification attribute mapping</param>
        void UpdateProductSpecificationAttribute(ProductSpecificationAttribute productSpecificationAttribute);

        #endregion

        #region Specification attribute option filter

        /// <summary>
        /// Gets a filtered product specification attribute mapping collection by category id
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <returns>Product specification attribute mapping collection</returns>
        List<SpecificationAttributeOptionFilter> GetSpecificationAttributeOptionFilter(int categoryId);

        /// <summary>
        /// Gets a filtered product specification attribute mapping collection by category id
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Product specification attribute mapping collection</returns>
        List<SpecificationAttributeOptionFilter> GetSpecificationAttributeOptionFilter(int categoryId, int languageId);

        #endregion
    }
}
