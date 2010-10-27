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
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.IoC;

namespace NopSolutions.NopCommerce.BusinessLogic.Manufacturers
{
    /// <summary>
    /// Manufacturer manager
    /// </summary>
    public partial interface IManufacturerManager
    {
        /// <summary>
        /// Marks a manufacturer as deleted
        /// </summary>
        /// <param name="manufacturerId">Manufacturer identifer</param>
        void MarkManufacturerAsDeleted(int manufacturerId);

        /// <summary>
        /// Gets all manufacturers
        /// </summary>
        /// <returns>Manufacturer collection</returns>
        List<Manufacturer> GetAllManufacturers();

        /// <summary>
        /// Gets all manufacturers
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Manufacturer collection</returns>
        List<Manufacturer> GetAllManufacturers(bool showHidden);

        /// <summary>
        /// Gets a manufacturer
        /// </summary>
        /// <param name="manufacturerId">Manufacturer identifier</param>
        /// <returns>Manufacturer</returns>
        Manufacturer GetManufacturerById(int manufacturerId);

        /// <summary>
        /// Inserts a manufacturer
        /// </summary>
        /// <param name="manufacturer">Manufacturer</param>
        void InsertManufacturer(Manufacturer manufacturer);

        /// <summary>
        /// Updates the manufacturer
        /// </summary>
        /// <param name="manufacturer">Manufacturer</param>
        void UpdateManufacturer(Manufacturer manufacturer);

        /// <summary>
        /// Gets localized manufacturer by id
        /// </summary>
        /// <param name="manufacturerLocalizedId">Localized manufacturer identifier</param>
        /// <returns>Manufacturer content</returns>
        ManufacturerLocalized GetManufacturerLocalizedById(int manufacturerLocalizedId);

        /// <summary>
        /// Gets localized manufacturer by manufacturer id
        /// </summary>
        /// <param name="manufacturerId">Manufacturer identifier</param>
        /// <returns>Manufacturer content</returns>
        List<ManufacturerLocalized> GetManufacturerLocalizedByManufacturerId(int manufacturerId);

        /// <summary>
        /// Gets localized manufacturer by manufacturer id and language id
        /// </summary>
        /// <param name="manufacturerId">Manufacturer identifier</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Manufacturer content</returns>
        ManufacturerLocalized GetManufacturerLocalizedByManufacturerIdAndLanguageId(int manufacturerId, int languageId);

        /// <summary>
        /// Inserts a localized manufacturer
        /// </summary>
        /// <param name="manufacturerLocalized">Manufacturer content</param>
        void InsertManufacturerLocalized(ManufacturerLocalized manufacturerLocalized);

        /// <summary>
        /// Update a localized manufacturer
        /// </summary>
        /// <param name="manufacturerLocalized">Manufacturer content</param>
        void UpdateManufacturerLocalized(ManufacturerLocalized manufacturerLocalized);

        /// <summary>
        /// Deletes a product manufacturer mapping
        /// </summary>
        /// <param name="productManufacturerId">Product manufacturer mapping identifer</param>
        void DeleteProductManufacturer(int productManufacturerId);

        /// <summary>
        /// Gets product manufacturer collection
        /// </summary>
        /// <param name="manufacturerId">Manufacturer identifier</param>
        /// <returns>Product manufacturer collection</returns>
        List<ProductManufacturer> GetProductManufacturersByManufacturerId(int manufacturerId);

        /// <summary>
        /// Gets a product manufacturer mapping collection
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <returns>Product manufacturer mapping collection</returns>
        List<ProductManufacturer> GetProductManufacturersByProductId(int productId);

        /// <summary>
        /// Gets a product manufacturer mapping 
        /// </summary>
        /// <param name="productManufacturerId">Product manufacturer mapping identifier</param>
        /// <returns>Product manufacturer mapping</returns>
        ProductManufacturer GetProductManufacturerById(int productManufacturerId);

        /// <summary>
        /// Inserts a product manufacturer mapping
        /// </summary>
        /// <param name="productManufacturer">Product manufacturer mapping</param>
        void InsertProductManufacturer(ProductManufacturer productManufacturer);

        /// <summary>
        /// Updates the product manufacturer mapping
        /// </summary>
        /// <param name="productManufacturer">Product manufacturer mapping</param>
        void UpdateProductManufacturer(ProductManufacturer productManufacturer);
    }
}
