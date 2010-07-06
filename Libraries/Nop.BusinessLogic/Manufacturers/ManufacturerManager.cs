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

namespace NopSolutions.NopCommerce.BusinessLogic.Manufacturers
{
    /// <summary>
    /// Manufacturer manager
    /// </summary>
    public partial class ManufacturerManager
    {
        #region Constants
        private const string MANUFACTURERS_ALL_KEY = "Nop.manufacturer.all-{0}";
        private const string MANUFACTURERS_BY_ID_KEY = "Nop.manufacturer.id-{0}";
        private const string PRODUCTMANUFACTURERS_ALLBYMANUFACTURERID_KEY = "Nop.productmanufacturer.allbymanufacturerid-{0}-{1}";
        private const string PRODUCTMANUFACTURERS_ALLBYPRODUCTID_KEY = "Nop.productmanufacturer.allbyproductid-{0}-{1}";
        private const string PRODUCTMANUFACTURERS_BY_ID_KEY = "Nop.productmanufacturer.id-{0}";
        private const string MANUFACTURERS_PATTERN_KEY = "Nop.manufacturer.";
        private const string PRODUCTMANUFACTURERS_PATTERN_KEY = "Nop.productmanufacturer.";
        #endregion
        
        #region Methods

        /// <summary>
        /// Marks a manufacturer as deleted
        /// </summary>
        /// <param name="manufacturerId">Manufacturer identifer</param>
        public static void MarkManufacturerAsDeleted(int manufacturerId)
        {
            var manufacturer = GetManufacturerById(manufacturerId);
            if (manufacturer != null)
            {
                manufacturer = UpdateManufacturer(manufacturer.ManufacturerId, manufacturer.Name, manufacturer.Description,
                    manufacturer.TemplateId, manufacturer.MetaKeywords,
                    manufacturer.MetaDescription, manufacturer.MetaTitle,
                    manufacturer.SEName, manufacturer.PictureId, manufacturer.PageSize,
                    manufacturer.PriceRanges, manufacturer.Published,
                    true, manufacturer.DisplayOrder, manufacturer.CreatedOn, manufacturer.UpdatedOn);
            }
        }

        /// <summary>
        /// Removes a manufacturer picture
        /// </summary>
        /// <param name="manufacturerId">Manufacturer identifier</param>
        public static void RemoveManufacturerPicture(int manufacturerId)
        {
            var manufacturer = GetManufacturerById(manufacturerId);
            if (manufacturer != null)
            {
                UpdateManufacturer(manufacturer.ManufacturerId, manufacturer.Name, manufacturer.Description,
                    manufacturer.TemplateId, manufacturer.MetaKeywords,
                    manufacturer.MetaDescription, manufacturer.MetaTitle,
                    manufacturer.SEName, 0, manufacturer.PageSize, manufacturer.PriceRanges,
                    manufacturer.Published, manufacturer.Deleted, manufacturer.DisplayOrder, 
                    manufacturer.CreatedOn, manufacturer.UpdatedOn);
            }
        }

        /// <summary>
        /// Gets all manufacturers
        /// </summary>
        /// <returns>Manufacturer collection</returns>
        public static List<Manufacturer> GetAllManufacturers()
        {
            bool showHidden = NopContext.Current.IsAdmin;
            return GetAllManufacturers(showHidden);
        }

        /// <summary>
        /// Gets all manufacturers
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Manufacturer collection</returns>
        public static List<Manufacturer> GetAllManufacturers(bool showHidden)
        {
            string key = string.Format(MANUFACTURERS_ALL_KEY, showHidden);
            object obj2 = NopRequestCache.Get(key);
            if (ManufacturerManager.ManufacturersCacheEnabled && (obj2 != null))
            {
                return (List<Manufacturer>)obj2;
            }

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from m in context.Manufacturers
                        orderby m.DisplayOrder
                        where (showHidden || m.Published) &&
                        !m.Deleted
                        select m;
            var manufacturers = query.ToList();

            if (ManufacturerManager.ManufacturersCacheEnabled)
            {
                NopRequestCache.Add(key, manufacturers);
            }
            return manufacturers;
        }

        /// <summary>
        /// Gets a manufacturer
        /// </summary>
        /// <param name="manufacturerId">Manufacturer identifier</param>
        /// <returns>Manufacturer</returns>
        public static Manufacturer GetManufacturerById(int manufacturerId)
        {
            if (manufacturerId == 0)
                return null;

            string key = string.Format(MANUFACTURERS_BY_ID_KEY, manufacturerId);
            object obj2 = NopRequestCache.Get(key);
            if (ManufacturerManager.ManufacturersCacheEnabled && (obj2 != null))
            {
                return (Manufacturer)obj2;
            }

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from m in context.Manufacturers
                        where m.ManufacturerId == manufacturerId
                        select m;
            var manufacturer = query.SingleOrDefault();

            if (ManufacturerManager.ManufacturersCacheEnabled)
            {
                NopRequestCache.Add(key, manufacturer);
            }
            return manufacturer;
        }

        /// <summary>
        /// Inserts a manufacturer
        /// </summary>
        /// <param name="name">The name</param>
        /// <param name="description">The description</param>
        /// <param name="templateId">The template identifier</param>
        /// <param name="metaKeywords">The meta keywords</param>
        /// <param name="metaDescription">The meta description</param>
        /// <param name="metaTitle">The meta title</param>
        /// <param name="seName">The search-engine name</param>
        /// <param name="pictureId">The parent picture identifier</param>
        /// <param name="pageSize">The page size</param>
        /// <param name="priceRanges">The price ranges</param>
        /// <param name="published">A value indicating whether the entity is published</param>
        /// <param name="deleted">A value indicating whether the entity has been deleted</param>
        /// <param name="displayOrder">The display order</param>
        /// <param name="createdOn">The date and time of instance creation</param>
        /// <param name="updatedOn">The date and time of instance update</param>
        /// <returns>Manufacturer</returns>
        public static Manufacturer InsertManufacturer(string name, string description,
            int templateId, string metaKeywords, string metaDescription, string metaTitle,
            string seName, int pictureId, int pageSize, string priceRanges,
            bool published, bool deleted, int displayOrder,
            DateTime createdOn, DateTime updatedOn)
        {
            name = CommonHelper.EnsureMaximumLength(name, 400);
            metaKeywords = CommonHelper.EnsureMaximumLength(metaKeywords, 400);
            metaDescription = CommonHelper.EnsureMaximumLength(metaDescription, 4000);
            metaTitle = CommonHelper.EnsureMaximumLength(metaTitle, 400);
            seName = CommonHelper.EnsureMaximumLength(seName, 100);
            priceRanges = CommonHelper.EnsureMaximumLength(priceRanges, 400);

            var context = ObjectContextHelper.CurrentObjectContext;

            var manufacturer = context.Manufacturers.CreateObject();
            manufacturer.Name = name;
            manufacturer.Description = description;
            manufacturer.TemplateId = templateId;
            manufacturer.MetaKeywords = metaKeywords;
            manufacturer.MetaDescription = metaDescription;
            manufacturer.MetaTitle = metaTitle;
            manufacturer.SEName = seName;
            manufacturer.PictureId = pictureId;
            manufacturer.PageSize = pageSize;
            manufacturer.PriceRanges = priceRanges;
            manufacturer.Published = published;
            manufacturer.Deleted = deleted;
            manufacturer.DisplayOrder = displayOrder;
            manufacturer.CreatedOn = createdOn;
            manufacturer.UpdatedOn = updatedOn;

            context.Manufacturers.AddObject(manufacturer);
            context.SaveChanges();

            if (ManufacturerManager.ManufacturersCacheEnabled || ManufacturerManager.MappingsCacheEnabled)
            {
                NopRequestCache.RemoveByPattern(MANUFACTURERS_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(PRODUCTMANUFACTURERS_PATTERN_KEY);
            }

            return manufacturer;
        }

        /// <summary>
        /// Updates the manufacturer
        /// </summary>
        /// <param name="manufacturerId">Manufacturer identifier</param>
        /// <param name="name">The name</param>
        /// <param name="description">The description</param>
        /// <param name="templateId">The template identifier</param>
        /// <param name="metaKeywords">The meta keywords</param>
        /// <param name="metaDescription">The meta description</param>
        /// <param name="metaTitle">The meta title</param>
        /// <param name="seName">The search-engine name</param>
        /// <param name="pictureId">The parent picture identifier</param>
        /// <param name="pageSize">The page size</param>
        /// <param name="priceRanges">The price ranges</param>
        /// <param name="published">A value indicating whether the entity is published</param>
        /// <param name="deleted">A value indicating whether the entity has been deleted</param>
        /// <param name="displayOrder">The display order</param>
        /// <param name="createdOn">The date and time of instance creation</param>
        /// <param name="updatedOn">The date and time of instance update</param>
        /// <returns>Manufacturer</returns>
        public static Manufacturer UpdateManufacturer(int manufacturerId,
            string name, string description,
            int templateId, string metaKeywords, string metaDescription, string metaTitle,
            string seName, int pictureId, int pageSize, string priceRanges,
            bool published, bool deleted, int displayOrder,
            DateTime createdOn, DateTime updatedOn)
        {
            name = CommonHelper.EnsureMaximumLength(name, 400);
            metaKeywords = CommonHelper.EnsureMaximumLength(metaKeywords, 400);
            metaDescription = CommonHelper.EnsureMaximumLength(metaDescription, 4000);
            metaTitle = CommonHelper.EnsureMaximumLength(metaTitle, 400);
            seName = CommonHelper.EnsureMaximumLength(seName, 100);
            priceRanges = CommonHelper.EnsureMaximumLength(priceRanges, 400);

            var manufacturer = GetManufacturerById(manufacturerId);
            if (manufacturer == null)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(manufacturer))
                context.Manufacturers.Attach(manufacturer);

            manufacturer.Name = name;
            manufacturer.Description = description;
            manufacturer.TemplateId = templateId;
            manufacturer.MetaKeywords = metaKeywords;
            manufacturer.MetaDescription = metaDescription;
            manufacturer.MetaTitle = metaTitle;
            manufacturer.SEName = seName;
            manufacturer.PictureId = pictureId;
            manufacturer.PageSize = pageSize;
            manufacturer.PriceRanges = priceRanges;
            manufacturer.Published = published;
            manufacturer.Deleted = deleted;
            manufacturer.DisplayOrder = displayOrder;
            manufacturer.CreatedOn = createdOn;
            manufacturer.UpdatedOn = updatedOn;
            context.SaveChanges();

            if (ManufacturerManager.ManufacturersCacheEnabled || ManufacturerManager.MappingsCacheEnabled)
            {
                NopRequestCache.RemoveByPattern(MANUFACTURERS_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(PRODUCTMANUFACTURERS_PATTERN_KEY);
            }

            return manufacturer;
        }

        /// <summary>
        /// Gets localized manufacturer by id
        /// </summary>
        /// <param name="manufacturerLocalizedId">Localized manufacturer identifier</param>
        /// <returns>Manufacturer content</returns>
        public static ManufacturerLocalized GetManufacturerLocalizedById(int manufacturerLocalizedId)
        {
            if (manufacturerLocalizedId == 0)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from ml in context.ManufacturerLocalized
                        where ml.ManufacturerLocalizedId == manufacturerLocalizedId
                        select ml;
            var manufacturerLocalized = query.SingleOrDefault();
            return manufacturerLocalized;
        }

        /// <summary>
        /// Gets localized manufacturer by manufacturer id
        /// </summary>
        /// <param name="manufacturerId">Manufacturer identifier</param>
        /// <returns>Manufacturer content</returns>
        public static List<ManufacturerLocalized> GetManufacturerLocalizedByManufacturerId(int manufacturerId)
        {
            if (manufacturerId == 0)
                return new List<ManufacturerLocalized>();

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from ml in context.ManufacturerLocalized
                        where ml.ManufacturerId == manufacturerId
                        select ml;
            var content = query.ToList();
            return content;
        }

        /// <summary>
        /// Gets localized manufacturer by manufacturer id and language id
        /// </summary>
        /// <param name="manufacturerId">Manufacturer identifier</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Manufacturer content</returns>
        public static ManufacturerLocalized GetManufacturerLocalizedByManufacturerIdAndLanguageId(int manufacturerId, int languageId)
        {
            if (manufacturerId == 0 || languageId == 0)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from ml in context.ManufacturerLocalized
                        orderby ml.ManufacturerLocalizedId
                        where ml.ManufacturerId == manufacturerId &&
                        ml.LanguageId == languageId
                        select ml;
            var manufacturerLocalized = query.FirstOrDefault();
            return manufacturerLocalized;
        }

        /// <summary>
        /// Inserts a localized manufacturer
        /// </summary>
        /// <param name="manufacturerId">Manufacturer identifier</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="name">Name text</param>
        /// <param name="description">Description text</param>
        /// <param name="metaKeywords">Meta keywords text</param>
        /// <param name="metaDescription">Meta descriptions text</param>
        /// <param name="metaTitle">Metat title text</param>
        /// <param name="seName">Se name text</param>
        /// <returns>Manufacturer content</returns>
        public static ManufacturerLocalized InsertManufacturerLocalized(int manufacturerId,
            int languageId, string name, string description,
            string metaKeywords, string metaDescription, string metaTitle, string seName)
        {
            name = CommonHelper.EnsureMaximumLength(name, 400);
            metaKeywords = CommonHelper.EnsureMaximumLength(metaKeywords, 400);
            metaDescription = CommonHelper.EnsureMaximumLength(metaDescription, 4000);
            metaTitle = CommonHelper.EnsureMaximumLength(metaTitle, 400);
            seName = CommonHelper.EnsureMaximumLength(seName, 100);

            var context = ObjectContextHelper.CurrentObjectContext;

            var manufacturerLocalized = context.ManufacturerLocalized.CreateObject();
            manufacturerLocalized.ManufacturerId = manufacturerId;
            manufacturerLocalized.LanguageId = languageId;
            manufacturerLocalized.Name = name;
            manufacturerLocalized.Description = description;
            manufacturerLocalized.MetaKeywords = metaKeywords;
            manufacturerLocalized.MetaDescription = metaDescription;
            manufacturerLocalized.MetaTitle = metaTitle;
            manufacturerLocalized.SEName = seName;

            context.ManufacturerLocalized.AddObject(manufacturerLocalized);
            context.SaveChanges();

            if (ManufacturerManager.ManufacturersCacheEnabled)
            {
                NopRequestCache.RemoveByPattern(MANUFACTURERS_PATTERN_KEY);
            }

            return manufacturerLocalized;
        }

        /// <summary>
        /// Update a localized manufacturer
        /// </summary>
        /// <param name="manufacturerLocalizedId">Localized manufacturer identifier</param>
        /// <param name="manufacturerId">Manufacturer identifier</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="name">Name text</param>
        /// <param name="description">Description text</param>
        /// <param name="metaKeywords">Meta keywords text</param>
        /// <param name="metaDescription">Meta descriptions text</param>
        /// <param name="metaTitle">Metat title text</param>
        /// <param name="seName">Se name text</param>
        /// <returns>Manufacturer content</returns>
        public static ManufacturerLocalized UpdateManufacturerLocalized(int manufacturerLocalizedId,
            int manufacturerId, int languageId, string name, string description,
            string metaKeywords, string metaDescription, string metaTitle, string seName)
        {
            name = CommonHelper.EnsureMaximumLength(name, 400);
            metaKeywords = CommonHelper.EnsureMaximumLength(metaKeywords, 400);
            metaDescription = CommonHelper.EnsureMaximumLength(metaDescription, 4000);
            metaTitle = CommonHelper.EnsureMaximumLength(metaTitle, 400);
            seName = CommonHelper.EnsureMaximumLength(seName, 100);

            var manufacturerLocalized = GetManufacturerLocalizedById(manufacturerLocalizedId);
            if (manufacturerLocalized == null)
                return null;

            bool allFieldsAreEmpty = string.IsNullOrEmpty(name) &&
                string.IsNullOrEmpty(description) &&
                string.IsNullOrEmpty(metaKeywords) &&
                string.IsNullOrEmpty(metaDescription) &&
                string.IsNullOrEmpty(metaTitle) &&
                string.IsNullOrEmpty(seName);

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(manufacturerLocalized))
                context.ManufacturerLocalized.Attach(manufacturerLocalized);

            if (allFieldsAreEmpty)
            {
                //delete if all fields are empty
                context.DeleteObject(manufacturerLocalized);
                context.SaveChanges();
            }
            else
            {
                manufacturerLocalized.ManufacturerId = manufacturerId;
                manufacturerLocalized.LanguageId = languageId;
                manufacturerLocalized.Name = name;
                manufacturerLocalized.Description = description;
                manufacturerLocalized.MetaKeywords = metaKeywords;
                manufacturerLocalized.MetaDescription = metaDescription;
                manufacturerLocalized.MetaTitle = metaTitle;
                manufacturerLocalized.SEName = seName;
                context.SaveChanges();
            }

            if (ManufacturerManager.ManufacturersCacheEnabled)
            {
                NopRequestCache.RemoveByPattern(MANUFACTURERS_PATTERN_KEY);
            }

            return manufacturerLocalized;
        }

        /// <summary>
        /// Deletes a product manufacturer mapping
        /// </summary>
        /// <param name="productManufacturerId">Product manufacturer mapping identifer</param>
        public static void DeleteProductManufacturer(int productManufacturerId)
        {
            if (productManufacturerId == 0)
                return;

            var productManufacturer = GetProductManufacturerById(productManufacturerId);
            if (productManufacturer == null)
                return;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(productManufacturer))
                context.ProductManufacturers.Attach(productManufacturer);
            context.DeleteObject(productManufacturer);
            context.SaveChanges();

            if (ManufacturerManager.ManufacturersCacheEnabled || ManufacturerManager.MappingsCacheEnabled)
            {
                NopRequestCache.RemoveByPattern(MANUFACTURERS_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(PRODUCTMANUFACTURERS_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Gets product manufacturer collection
        /// </summary>
        /// <param name="manufacturerId">Manufacturer identifier</param>
        /// <returns>Product manufacturer collection</returns>
        public static List<ProductManufacturer> GetProductManufacturersByManufacturerId(int manufacturerId)
        {
            if (manufacturerId == 0)
                return new List<ProductManufacturer>();

            bool showHidden = NopContext.Current.IsAdmin;
            string key = string.Format(PRODUCTMANUFACTURERS_ALLBYMANUFACTURERID_KEY, showHidden, manufacturerId);
            object obj2 = NopRequestCache.Get(key);
            if (ManufacturerManager.MappingsCacheEnabled && (obj2 != null))
            {
                return (List<ProductManufacturer>)obj2;
            }

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from pm in context.ProductManufacturers
                        join p in context.Products on pm.ProductId equals p.ProductId
                        where pm.ManufacturerId == manufacturerId &&
                        !p.Deleted &&
                        (showHidden || p.Published)
                        orderby pm.DisplayOrder
                        select pm;
            var productManufacturers = query.ToList();

            if (ManufacturerManager.MappingsCacheEnabled)
            {
                NopRequestCache.Add(key, productManufacturers);
            }
            return productManufacturers;
        }

        /// <summary>
        /// Gets a product manufacturer mapping collection
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <returns>Product manufacturer mapping collection</returns>
        public static List<ProductManufacturer> GetProductManufacturersByProductId(int productId)
        {
            if (productId == 0)
                return new List<ProductManufacturer>();

            bool showHidden = NopContext.Current.IsAdmin;
            string key = string.Format(PRODUCTMANUFACTURERS_ALLBYPRODUCTID_KEY, showHidden, productId);
            object obj2 = NopRequestCache.Get(key);
            if (ManufacturerManager.MappingsCacheEnabled && (obj2 != null))
            {
                return (List<ProductManufacturer>)obj2;
            }

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from pm in context.ProductManufacturers
                        join m in context.Manufacturers on pm.ManufacturerId equals m.ManufacturerId
                        where pm.ProductId == productId &&
                        !m.Deleted &&
                        (showHidden || m.Published)
                        orderby pm.DisplayOrder
                        select pm;
            var productManufacturers = query.ToList();

            if (ManufacturerManager.MappingsCacheEnabled)
            {
                NopRequestCache.Add(key, productManufacturers);
            }
            return productManufacturers;
        }

        /// <summary>
        /// Gets a product manufacturer mapping 
        /// </summary>
        /// <param name="productManufacturerId">Product manufacturer mapping identifier</param>
        /// <returns>Product manufacturer mapping</returns>
        public static ProductManufacturer GetProductManufacturerById(int productManufacturerId)
        {
            if (productManufacturerId == 0)
                return null;

            string key = string.Format(PRODUCTMANUFACTURERS_BY_ID_KEY, productManufacturerId);
            object obj2 = NopRequestCache.Get(key);
            if (ManufacturerManager.MappingsCacheEnabled && (obj2 != null))
            {
                return (ProductManufacturer)obj2;
            }

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from pm in context.ProductManufacturers
                        where pm.ProductManufacturerId == productManufacturerId
                        select pm;
            var productManufacturer = query.SingleOrDefault();

            if (ManufacturerManager.MappingsCacheEnabled)
            {
                NopRequestCache.Add(key, productManufacturer);
            }
            return productManufacturer;
        }

        /// <summary>
        /// Inserts a product manufacturer mapping
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <param name="manufacturerId">Manufacturer identifier</param>
        /// <param name="isFeaturedProduct">A value indicating whether the product is featured</param>
        /// <param name="displayOrder">The display order</param>
        /// <returns>Product manufacturer mapping </returns>
        public static ProductManufacturer InsertProductManufacturer(int productId, 
            int manufacturerId, bool isFeaturedProduct, int displayOrder)
        {
            var context = ObjectContextHelper.CurrentObjectContext;

            var productManufacturer = context.ProductManufacturers.CreateObject();
            productManufacturer.ProductId = productId;
            productManufacturer.ManufacturerId = manufacturerId;
            productManufacturer.IsFeaturedProduct = isFeaturedProduct;
            productManufacturer.DisplayOrder = displayOrder;

            context.ProductManufacturers.AddObject(productManufacturer);
            context.SaveChanges();

            if (ManufacturerManager.ManufacturersCacheEnabled || ManufacturerManager.MappingsCacheEnabled)
            {
                NopRequestCache.RemoveByPattern(MANUFACTURERS_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(PRODUCTMANUFACTURERS_PATTERN_KEY);
            }

            return productManufacturer;
        }

        /// <summary>
        /// Updates the product manufacturer mapping
        /// </summary>
        /// <param name="productManufacturerId">Product manufacturer mapping identifier</param>
        /// <param name="productId">Product identifier</param>
        /// <param name="manufacturerId">Manufacturer identifier</param>
        /// <param name="isFeaturedProduct">A value indicating whether the product is featured</param>
        /// <param name="displayOrder">The display order</param>
        /// <returns>Product manufacturer mapping </returns>
        public static ProductManufacturer UpdateProductManufacturer(int productManufacturerId,
            int productId, int manufacturerId, bool isFeaturedProduct, int displayOrder)
        {
            var productManufacturer = GetProductManufacturerById(productManufacturerId);
            if (productManufacturer == null)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(productManufacturer))
                context.ProductManufacturers.Attach(productManufacturer);

            productManufacturer.ProductId = productId;
            productManufacturer.ManufacturerId = manufacturerId;
            productManufacturer.IsFeaturedProduct = isFeaturedProduct;
            productManufacturer.DisplayOrder = displayOrder;
            context.SaveChanges();

            if (ManufacturerManager.ManufacturersCacheEnabled || ManufacturerManager.MappingsCacheEnabled)
            {
                NopRequestCache.RemoveByPattern(MANUFACTURERS_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(PRODUCTMANUFACTURERS_PATTERN_KEY);
            }

            return productManufacturer;
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether manufacturers cache is enabled
        /// </summary>
        public static bool ManufacturersCacheEnabled
        {
            get
            {
                return SettingManager.GetSettingValueBoolean("Cache.ManufacturerManager.ManufacturersCacheEnabled");
            }
        }

        /// <summary>
        /// Gets a value indicating whether mappings cache is enabled
        /// </summary>
        public static bool MappingsCacheEnabled
        {
            get
            {
                return SettingManager.GetSettingValueBoolean("Cache.ManufacturerManager.MappingsCacheEnabled");
            }
        }
        #endregion
    }
}
