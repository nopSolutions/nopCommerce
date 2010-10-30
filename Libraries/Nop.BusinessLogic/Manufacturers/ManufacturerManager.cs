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
    public partial class ManufacturerManager : IManufacturerManager
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

        #region Fields

        /// <summary>
        /// object context
        /// </summary>
        protected NopObjectContext _context;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="context">Object context</param>
        public ManufacturerManager(NopObjectContext context)
        {
            _context = context;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Marks a manufacturer as deleted
        /// </summary>
        /// <param name="manufacturerId">Manufacturer identifer</param>
        public void MarkManufacturerAsDeleted(int manufacturerId)
        {
            var manufacturer = GetManufacturerById(manufacturerId);
            if (manufacturer != null)
            {
                manufacturer.Deleted = true;
                UpdateManufacturer(manufacturer);
            }
        }

        /// <summary>
        /// Gets all manufacturers
        /// </summary>
        /// <returns>Manufacturer collection</returns>
        public List<Manufacturer> GetAllManufacturers()
        {
            bool showHidden = NopContext.Current.IsAdmin;
            return GetAllManufacturers(showHidden);
        }

        /// <summary>
        /// Gets all manufacturers
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Manufacturer collection</returns>
        public List<Manufacturer> GetAllManufacturers(bool showHidden)
        {
            string key = string.Format(MANUFACTURERS_ALL_KEY, showHidden);
            object obj2 = NopRequestCache.Get(key);
            if (this.ManufacturersCacheEnabled && (obj2 != null))
            {
                return (List<Manufacturer>)obj2;
            }

            
            var query = from m in _context.Manufacturers
                        orderby m.DisplayOrder
                        where (showHidden || m.Published) &&
                        !m.Deleted
                        select m;
            var manufacturers = query.ToList();

            if (this.ManufacturersCacheEnabled)
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
        public Manufacturer GetManufacturerById(int manufacturerId)
        {
            if (manufacturerId == 0)
                return null;

            string key = string.Format(MANUFACTURERS_BY_ID_KEY, manufacturerId);
            object obj2 = NopRequestCache.Get(key);
            if (this.ManufacturersCacheEnabled && (obj2 != null))
            {
                return (Manufacturer)obj2;
            }

            
            var query = from m in _context.Manufacturers
                        where m.ManufacturerId == manufacturerId
                        select m;
            var manufacturer = query.SingleOrDefault();

            if (this.ManufacturersCacheEnabled)
            {
                NopRequestCache.Add(key, manufacturer);
            }
            return manufacturer;
        }

        /// <summary>
        /// Inserts a manufacturer
        /// </summary>
        /// <param name="manufacturer">Manufacturer</param>
        public void InsertManufacturer(Manufacturer manufacturer)
        {
            if (manufacturer == null)
                throw new ArgumentNullException("manufacturer");
            
            manufacturer.Name = CommonHelper.EnsureNotNull(manufacturer.Name);
            manufacturer.Name = CommonHelper.EnsureMaximumLength(manufacturer.Name, 400);
            manufacturer.Description = CommonHelper.EnsureNotNull(manufacturer.Description);
            manufacturer.MetaKeywords = CommonHelper.EnsureNotNull(manufacturer.MetaKeywords);
            manufacturer.MetaKeywords = CommonHelper.EnsureMaximumLength(manufacturer.MetaKeywords, 400);
            manufacturer.MetaDescription = CommonHelper.EnsureNotNull(manufacturer.MetaDescription);
            manufacturer.MetaDescription = CommonHelper.EnsureMaximumLength(manufacturer.MetaDescription, 4000);
            manufacturer.MetaTitle = CommonHelper.EnsureNotNull(manufacturer.MetaTitle);
            manufacturer.MetaTitle = CommonHelper.EnsureMaximumLength(manufacturer.MetaTitle, 400);
            manufacturer.SEName = CommonHelper.EnsureNotNull(manufacturer.SEName);
            manufacturer.SEName = CommonHelper.EnsureMaximumLength(manufacturer.SEName, 100);
            manufacturer.PriceRanges = CommonHelper.EnsureNotNull(manufacturer.PriceRanges);
            manufacturer.PriceRanges = CommonHelper.EnsureMaximumLength(manufacturer.PriceRanges, 400);

            
            
            _context.Manufacturers.AddObject(manufacturer);
            _context.SaveChanges();

            if (this.ManufacturersCacheEnabled || this.MappingsCacheEnabled)
            {
                NopRequestCache.RemoveByPattern(MANUFACTURERS_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(PRODUCTMANUFACTURERS_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Updates the manufacturer
        /// </summary>
        /// <param name="manufacturer">Manufacturer</param>
        public void UpdateManufacturer(Manufacturer manufacturer)
        {
            if (manufacturer == null)
                throw new ArgumentNullException("manufacturer");

            manufacturer.Name = CommonHelper.EnsureNotNull(manufacturer.Name);
            manufacturer.Name = CommonHelper.EnsureMaximumLength(manufacturer.Name, 400);
            manufacturer.Description = CommonHelper.EnsureNotNull(manufacturer.Description);
            manufacturer.MetaKeywords = CommonHelper.EnsureNotNull(manufacturer.MetaKeywords);
            manufacturer.MetaKeywords = CommonHelper.EnsureMaximumLength(manufacturer.MetaKeywords, 400);
            manufacturer.MetaDescription = CommonHelper.EnsureNotNull(manufacturer.MetaDescription);
            manufacturer.MetaDescription = CommonHelper.EnsureMaximumLength(manufacturer.MetaDescription, 4000);
            manufacturer.MetaTitle = CommonHelper.EnsureNotNull(manufacturer.MetaTitle);
            manufacturer.MetaTitle = CommonHelper.EnsureMaximumLength(manufacturer.MetaTitle, 400);
            manufacturer.SEName = CommonHelper.EnsureNotNull(manufacturer.SEName);
            manufacturer.SEName = CommonHelper.EnsureMaximumLength(manufacturer.SEName, 100);
            manufacturer.PriceRanges = CommonHelper.EnsureNotNull(manufacturer.PriceRanges);
            manufacturer.PriceRanges = CommonHelper.EnsureMaximumLength(manufacturer.PriceRanges, 400);

            
            if (!_context.IsAttached(manufacturer))
                _context.Manufacturers.Attach(manufacturer);

            _context.SaveChanges();

            if (this.ManufacturersCacheEnabled || this.MappingsCacheEnabled)
            {
                NopRequestCache.RemoveByPattern(MANUFACTURERS_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(PRODUCTMANUFACTURERS_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Gets localized manufacturer by id
        /// </summary>
        /// <param name="manufacturerLocalizedId">Localized manufacturer identifier</param>
        /// <returns>Manufacturer content</returns>
        public ManufacturerLocalized GetManufacturerLocalizedById(int manufacturerLocalizedId)
        {
            if (manufacturerLocalizedId == 0)
                return null;

            
            var query = from ml in _context.ManufacturerLocalized
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
        public List<ManufacturerLocalized> GetManufacturerLocalizedByManufacturerId(int manufacturerId)
        {
            if (manufacturerId == 0)
                return new List<ManufacturerLocalized>();

            
            var query = from ml in _context.ManufacturerLocalized
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
        public ManufacturerLocalized GetManufacturerLocalizedByManufacturerIdAndLanguageId(int manufacturerId, int languageId)
        {
            if (manufacturerId == 0 || languageId == 0)
                return null;

            
            var query = from ml in _context.ManufacturerLocalized
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
        /// <param name="manufacturerLocalized">Manufacturer content</param>
        public void InsertManufacturerLocalized(ManufacturerLocalized manufacturerLocalized)
        {
            if (manufacturerLocalized == null)
                throw new ArgumentNullException("manufacturerLocalized");

            manufacturerLocalized.Name = CommonHelper.EnsureNotNull(manufacturerLocalized.Name);
            manufacturerLocalized.Name = CommonHelper.EnsureMaximumLength(manufacturerLocalized.Name, 400);
            manufacturerLocalized.Description = CommonHelper.EnsureNotNull(manufacturerLocalized.Description);
            manufacturerLocalized.MetaKeywords = CommonHelper.EnsureNotNull(manufacturerLocalized.MetaKeywords);
            manufacturerLocalized.MetaKeywords = CommonHelper.EnsureMaximumLength(manufacturerLocalized.MetaKeywords, 400);
            manufacturerLocalized.MetaDescription = CommonHelper.EnsureNotNull(manufacturerLocalized.MetaDescription);
            manufacturerLocalized.MetaDescription = CommonHelper.EnsureMaximumLength(manufacturerLocalized.MetaDescription, 4000);
            manufacturerLocalized.MetaTitle = CommonHelper.EnsureNotNull(manufacturerLocalized.MetaTitle);
            manufacturerLocalized.MetaTitle = CommonHelper.EnsureMaximumLength(manufacturerLocalized.MetaTitle, 400);
            manufacturerLocalized.SEName = CommonHelper.EnsureNotNull(manufacturerLocalized.SEName);
            manufacturerLocalized.SEName = CommonHelper.EnsureMaximumLength(manufacturerLocalized.SEName, 100);

            

            _context.ManufacturerLocalized.AddObject(manufacturerLocalized);
            _context.SaveChanges();

            if (this.ManufacturersCacheEnabled)
            {
                NopRequestCache.RemoveByPattern(MANUFACTURERS_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Update a localized manufacturer
        /// </summary>
        /// <param name="manufacturerLocalized">Manufacturer content</param>
        public void UpdateManufacturerLocalized(ManufacturerLocalized manufacturerLocalized)
        {
            if (manufacturerLocalized == null)
                throw new ArgumentNullException("manufacturerLocalized");

            manufacturerLocalized.Name = CommonHelper.EnsureNotNull(manufacturerLocalized.Name);
            manufacturerLocalized.Name = CommonHelper.EnsureMaximumLength(manufacturerLocalized.Name, 400);
            manufacturerLocalized.Description = CommonHelper.EnsureNotNull(manufacturerLocalized.Description);
            manufacturerLocalized.MetaKeywords = CommonHelper.EnsureNotNull(manufacturerLocalized.MetaKeywords);
            manufacturerLocalized.MetaKeywords = CommonHelper.EnsureMaximumLength(manufacturerLocalized.MetaKeywords, 400);
            manufacturerLocalized.MetaDescription = CommonHelper.EnsureNotNull(manufacturerLocalized.MetaDescription);
            manufacturerLocalized.MetaDescription = CommonHelper.EnsureMaximumLength(manufacturerLocalized.MetaDescription, 4000);
            manufacturerLocalized.MetaTitle = CommonHelper.EnsureNotNull(manufacturerLocalized.MetaTitle);
            manufacturerLocalized.MetaTitle = CommonHelper.EnsureMaximumLength(manufacturerLocalized.MetaTitle, 400);
            manufacturerLocalized.SEName = CommonHelper.EnsureNotNull(manufacturerLocalized.SEName);
            manufacturerLocalized.SEName = CommonHelper.EnsureMaximumLength(manufacturerLocalized.SEName, 100);

            bool allFieldsAreEmpty = string.IsNullOrEmpty(manufacturerLocalized.Name) &&
                string.IsNullOrEmpty(manufacturerLocalized.Description) &&
                string.IsNullOrEmpty(manufacturerLocalized.MetaKeywords) &&
                string.IsNullOrEmpty(manufacturerLocalized.MetaDescription) &&
                string.IsNullOrEmpty(manufacturerLocalized.MetaTitle) &&
                string.IsNullOrEmpty(manufacturerLocalized.SEName);

            
            if (!_context.IsAttached(manufacturerLocalized))
                _context.ManufacturerLocalized.Attach(manufacturerLocalized);

            if (allFieldsAreEmpty)
            {
                //delete if all fields are empty
                _context.DeleteObject(manufacturerLocalized);
                _context.SaveChanges();
            }
            else
            {
                _context.SaveChanges();
            }

            if (this.ManufacturersCacheEnabled)
            {
                NopRequestCache.RemoveByPattern(MANUFACTURERS_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Deletes a product manufacturer mapping
        /// </summary>
        /// <param name="productManufacturerId">Product manufacturer mapping identifer</param>
        public void DeleteProductManufacturer(int productManufacturerId)
        {
            if (productManufacturerId == 0)
                return;

            var productManufacturer = GetProductManufacturerById(productManufacturerId);
            if (productManufacturer == null)
                return;

            
            if (!_context.IsAttached(productManufacturer))
                _context.ProductManufacturers.Attach(productManufacturer);
            _context.DeleteObject(productManufacturer);
            _context.SaveChanges();

            if (this.ManufacturersCacheEnabled || this.MappingsCacheEnabled)
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
        public List<ProductManufacturer> GetProductManufacturersByManufacturerId(int manufacturerId)
        {
            if (manufacturerId == 0)
                return new List<ProductManufacturer>();

            bool showHidden = NopContext.Current.IsAdmin;
            string key = string.Format(PRODUCTMANUFACTURERS_ALLBYMANUFACTURERID_KEY, showHidden, manufacturerId);
            object obj2 = NopRequestCache.Get(key);
            if (this.MappingsCacheEnabled && (obj2 != null))
            {
                return (List<ProductManufacturer>)obj2;
            }

            
            var query = from pm in _context.ProductManufacturers
                        join p in _context.Products on pm.ProductId equals p.ProductId
                        where pm.ManufacturerId == manufacturerId &&
                        !p.Deleted &&
                        (showHidden || p.Published)
                        orderby pm.DisplayOrder
                        select pm;
            var productManufacturers = query.ToList();

            if (this.MappingsCacheEnabled)
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
        public List<ProductManufacturer> GetProductManufacturersByProductId(int productId)
        {
            if (productId == 0)
                return new List<ProductManufacturer>();

            bool showHidden = NopContext.Current.IsAdmin;
            string key = string.Format(PRODUCTMANUFACTURERS_ALLBYPRODUCTID_KEY, showHidden, productId);
            object obj2 = NopRequestCache.Get(key);
            if (this.MappingsCacheEnabled && (obj2 != null))
            {
                return (List<ProductManufacturer>)obj2;
            }

            
            var query = from pm in _context.ProductManufacturers
                        join m in _context.Manufacturers on pm.ManufacturerId equals m.ManufacturerId
                        where pm.ProductId == productId &&
                        !m.Deleted &&
                        (showHidden || m.Published)
                        orderby pm.DisplayOrder
                        select pm;
            var productManufacturers = query.ToList();

            if (this.MappingsCacheEnabled)
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
        public ProductManufacturer GetProductManufacturerById(int productManufacturerId)
        {
            if (productManufacturerId == 0)
                return null;

            string key = string.Format(PRODUCTMANUFACTURERS_BY_ID_KEY, productManufacturerId);
            object obj2 = NopRequestCache.Get(key);
            if (this.MappingsCacheEnabled && (obj2 != null))
            {
                return (ProductManufacturer)obj2;
            }

            
            var query = from pm in _context.ProductManufacturers
                        where pm.ProductManufacturerId == productManufacturerId
                        select pm;
            var productManufacturer = query.SingleOrDefault();

            if (this.MappingsCacheEnabled)
            {
                NopRequestCache.Add(key, productManufacturer);
            }
            return productManufacturer;
        }

        /// <summary>
        /// Inserts a product manufacturer mapping
        /// </summary>
        /// <param name="productManufacturer">Product manufacturer mapping</param>
        public void InsertProductManufacturer(ProductManufacturer productManufacturer)
        {
            if (productManufacturer == null)
                throw new ArgumentNullException("productManufacturer");

            

            _context.ProductManufacturers.AddObject(productManufacturer);
            _context.SaveChanges();

            if (this.ManufacturersCacheEnabled || this.MappingsCacheEnabled)
            {
                NopRequestCache.RemoveByPattern(MANUFACTURERS_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(PRODUCTMANUFACTURERS_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Updates the product manufacturer mapping
        /// </summary>
        /// <param name="productManufacturer">Product manufacturer mapping</param>
        public void UpdateProductManufacturer(ProductManufacturer productManufacturer)
        {
            if (productManufacturer == null)
                throw new ArgumentNullException("productManufacturer");

            
            if (!_context.IsAttached(productManufacturer))
                _context.ProductManufacturers.Attach(productManufacturer);

            _context.SaveChanges();

            if (this.ManufacturersCacheEnabled || this.MappingsCacheEnabled)
            {
                NopRequestCache.RemoveByPattern(MANUFACTURERS_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(PRODUCTMANUFACTURERS_PATTERN_KEY);
            }
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether manufacturers cache is enabled
        /// </summary>
        public bool ManufacturersCacheEnabled
        {
            get
            {
                return IoCFactory.Resolve<ISettingManager>().GetSettingValueBoolean("Cache.ManufacturerManager.ManufacturersCacheEnabled");
            }
        }

        /// <summary>
        /// Gets a value indicating whether mappings cache is enabled
        /// </summary>
        public bool MappingsCacheEnabled
        {
            get
            {
                return IoCFactory.Resolve<ISettingManager>().GetSettingValueBoolean("Cache.ManufacturerManager.MappingsCacheEnabled");
            }
        }
        #endregion
    }
}
