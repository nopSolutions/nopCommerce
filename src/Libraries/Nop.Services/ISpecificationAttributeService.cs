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

using System.Collections.Generic;
using Nop.Core.Domain;

namespace Nop.Services
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
        /// Gets specification attributes
        /// </summary>
        /// <returns>Specification attributes</returns>
        List<SpecificationAttribute> GetSpecificationAttributes();

        /// <summary>
        /// Deletes a specification attribute
        /// </summary>
        /// <param name="specificationAttribute">The specification attribute</param>
        void DeleteSpecificationAttribute(SpecificationAttribute specificationAttribute);

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
        /// <param name="localizedSpecificationAttributeId">Localized specification identifier</param>
        /// <returns>Specification attribute content</returns>
        LocalizedSpecificationAttribute GetSpecificationAttributeLocalizedById(int localizedSpecificationAttributeId);

        /// <summary>
        /// Gets localized specification attribute by specification attribute id
        /// </summary>
        /// <param name="specificationAttributeId">Specification attribute identifier</param>
        /// <returns>Secification attribute content</returns>
        List<LocalizedSpecificationAttribute> GetSpecificationAttributeLocalizedBySpecificationAttributeId(int specificationAttributeId);

        /// <summary>
        /// Gets localized specification attribute by specification attribute id and language id
        /// </summary>
        /// <param name="specificationAttributeId">Specification attribute identifier</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Specification attribute content</returns>
        LocalizedSpecificationAttribute GetSpecificationAttributeLocalizedBySpecificationAttributeIdAndLanguageId(int specificationAttributeId, int languageId);

        /// <summary>
        /// Inserts a localized specification attribute
        /// </summary>
        /// <param name="localizedSpecificationAttribute">Localized specification attribute</param>
        void InsertSpecificationAttributeLocalized(LocalizedSpecificationAttribute localizedSpecificationAttribute);

        /// <summary>
        /// Update a localized specification attribute
        /// </summary>
        /// <param name="localizedSpecificationAttribute">Localized specification attribute</param>
        void UpdateSpecificationAttributeLocalized(LocalizedSpecificationAttribute localizedSpecificationAttribute);

        #endregion

        #region Specification attribute option

        /// <summary>
        /// Gets a specification attribute option
        /// </summary>
        /// <param name="specificationAttributeOption">The specification attribute option</param>
        /// <returns>Specification attribute option</returns>
        SpecificationAttributeOption GetSpecificationAttributeOptionById(int specificationAttributeOption);

        /// <summary>
        /// Gets a specification attribute option by specification attribute id
        /// </summary>
        /// <param name="specificationAttributeId">The specification attribute identifier</param>
        /// <returns>Specification attribute option</returns>
        List<SpecificationAttributeOption> GetSpecificationAttributeOptionsBySpecificationAttribute(int specificationAttributeId);

        /// <summary>
        /// Deletes a specification attribute option
        /// </summary>
        /// <param name="specificationAttributeOption">The specification attribute option</param>
        void DeleteSpecificationAttributeOption(SpecificationAttributeOption specificationAttributeOption);

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
        /// <param name="localizedSpecificationAttributeOptionId">Localized specification attribute option identifier</param>
        /// <returns>Localized specification attribute option</returns>
        LocalizedSpecificationAttributeOption GetSpecificationAttributeOptionLocalizedById(int localizedSpecificationAttributeOptionId);

        /// <summary>
        /// Gets localized specification attribute option by category id
        /// </summary>
        /// <param name="specificationAttributeOptionId">Specification attribute option identifier</param>
        /// <returns>Localized specification attribute option content</returns>
        List<LocalizedSpecificationAttributeOption> GetSpecificationAttributeOptionLocalizedBySpecificationAttributeOptionId(int specificationAttributeOptionId);
        
        /// <summary>
        /// Gets localized specification attribute option by specification attribute option id and language id
        /// </summary>
        /// <param name="specificationAttributeOptionId">Specification attribute option identifier</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Localized specification attribute option</returns>
        LocalizedSpecificationAttributeOption GetSpecificationAttributeOptionLocalizedBySpecificationAttributeOptionIdAndLanguageId(int specificationAttributeOptionId, int languageId);

        /// <summary>
        /// Inserts a localized specification attribute option
        /// </summary>
        /// <param name="localizedSpecificationAttributeOption">Localized specification attribute option</param>
        /// <returns>Localized specification attribute option</returns>
        void InsertSpecificationAttributeOptionLocalized(LocalizedSpecificationAttributeOption localizedSpecificationAttributeOption);

        /// <summary>
        /// Update a localized specification attribute option
        /// </summary>
        /// <param name="localizedSpecificationAttributeOption">Localized specification attribute option</param>
        /// <returns>Localized specification attribute option</returns>
        void UpdateSpecificationAttributeOptionLocalized(LocalizedSpecificationAttributeOption localizedSpecificationAttributeOption);

        #endregion

        #region Product specification attribute

        /// <summary>
        /// Deletes a product specification attribute mapping
        /// </summary>
        /// <param name="productSpecificationAttribute">Product specification attribute</param>
        void DeleteProductSpecificationAttribute(ProductSpecificationAttribute productSpecificationAttribute);

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
    }
}
