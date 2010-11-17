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
using NopSolutions.NopCommerce.BusinessLogic.Caching;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Data;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

namespace NopSolutions.NopCommerce.BusinessLogic.Templates
{
    /// <summary>
    /// Category template service
    /// </summary>
    public partial class TemplateService : ITemplateService
    {
        #region Constants
        private const string CATEGORYTEMPLATES_ALL_KEY = "Nop.categorytemplate.all";
        private const string CATEGORYTEMPLATES_BY_ID_KEY = "Nop.categorytemplate.id-{0}";
        private const string MANUFACTURERTEMPLATES_ALL_KEY = "Nop.manufacturertemplate.all";
        private const string MANUFACTURERTEMPLATES_BY_ID_KEY = "Nop.manufacturertemplate.id-{0}";
        private const string PRODUCTTEMPLATES_ALL_KEY = "Nop.producttemplate.all";
        private const string PRODUCTTEMPLATES_BY_ID_KEY = "Nop.producttemplate.id-{0}";
        private const string CATEGORYTEMPLATES_PATTERN_KEY = "Nop.categorytemplate.";
        private const string MANUFACTURERTEMPLATES_PATTERN_KEY = "Nop.manufacturertemplate.";
        private const string PRODUCTTEMPLATES_PATTERN_KEY = "Nop.producttemplate.";
        #endregion

        #region Fields

        /// <summary>
        /// Object context
        /// </summary>
        protected readonly NopObjectContext _context;

        /// <summary>
        /// Cache manager
        /// </summary>
        protected readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="context">Object context</param>
        public TemplateService(NopObjectContext context)
        {
            this._context = context;
            this._cacheManager = new NopRequestCache();
        }

        #endregion

        #region Methods
        /// <summary>
        /// Deletes a category template
        /// </summary>
        /// <param name="categoryTemplateId">Category template identifier</param>
        public void DeleteCategoryTemplate(int categoryTemplateId)
        {
            var categoryTemplate = GetCategoryTemplateById(categoryTemplateId);
            if (categoryTemplate == null)
                return;

            
            if (!_context.IsAttached(categoryTemplate))
                _context.CategoryTemplates.Attach(categoryTemplate);
            _context.DeleteObject(categoryTemplate);
            _context.SaveChanges();
            
            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(CATEGORYTEMPLATES_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Gets all category templates
        /// </summary>
        /// <returns>Category template collection</returns>
        public List<CategoryTemplate> GetAllCategoryTemplates()
        {
            string key = string.Format(CATEGORYTEMPLATES_ALL_KEY);
            object obj2 = _cacheManager.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (List<CategoryTemplate>)obj2;
            }

            
            var query = from ct in _context.CategoryTemplates
                        orderby ct.DisplayOrder, ct.Name
                        select ct;
            var categoryTemplates = query.ToList();

            if (this.CacheEnabled)
            {
                _cacheManager.Add(key, categoryTemplates);
            }
            return categoryTemplates;
        }

        /// <summary>
        /// Gets a category template
        /// </summary>
        /// <param name="categoryTemplateId">Category template identifier</param>
        /// <returns>A category template</returns>
        public CategoryTemplate GetCategoryTemplateById(int categoryTemplateId)
        {
            if (categoryTemplateId == 0)
                return null;

            string key = string.Format(CATEGORYTEMPLATES_BY_ID_KEY, categoryTemplateId);
            object obj2 = _cacheManager.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (CategoryTemplate)obj2;
            }

            
            var query = from ct in _context.CategoryTemplates
                        where ct.CategoryTemplateId == categoryTemplateId
                        select ct;
            var categoryTemplate = query.SingleOrDefault();

            if (this.CacheEnabled)
            {
                _cacheManager.Add(key, categoryTemplate);
            }
            return categoryTemplate;
        }

        /// <summary>
        /// Inserts a category template
        /// </summary>
        /// <param name="categoryTemplate">Category template</param>
        public void InsertCategoryTemplate(CategoryTemplate categoryTemplate)
        {
            if (categoryTemplate == null)
                throw new ArgumentNullException("categoryTemplate");

            categoryTemplate.Name = CommonHelper.EnsureNotNull(categoryTemplate.Name);
            categoryTemplate.Name = CommonHelper.EnsureMaximumLength(categoryTemplate.Name, 100);
            categoryTemplate.TemplatePath = CommonHelper.EnsureNotNull(categoryTemplate.TemplatePath);
            categoryTemplate.TemplatePath = CommonHelper.EnsureMaximumLength(categoryTemplate.TemplatePath, 200);

            

            _context.CategoryTemplates.AddObject(categoryTemplate);
            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(CATEGORYTEMPLATES_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Updates the category template
        /// </summary>
        /// <param name="categoryTemplate">Category template</param>
        public void UpdateCategoryTemplate(CategoryTemplate categoryTemplate)
        {
            if (categoryTemplate == null)
                throw new ArgumentNullException("categoryTemplate");

            categoryTemplate.Name = CommonHelper.EnsureNotNull(categoryTemplate.Name);
            categoryTemplate.Name = CommonHelper.EnsureMaximumLength(categoryTemplate.Name, 100);
            categoryTemplate.TemplatePath = CommonHelper.EnsureNotNull(categoryTemplate.TemplatePath);
            categoryTemplate.TemplatePath = CommonHelper.EnsureMaximumLength(categoryTemplate.TemplatePath, 200);

            
            if (!_context.IsAttached(categoryTemplate))
                _context.CategoryTemplates.Attach(categoryTemplate);

            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(CATEGORYTEMPLATES_PATTERN_KEY);
            }
        }
        
        /// <summary>
        /// Deletes a manufacturer template
        /// </summary>
        /// <param name="manufacturerTemplateId">Manufacturer template identifier</param>
        public void DeleteManufacturerTemplate(int manufacturerTemplateId)
        {
            var manufacturerTemplate = GetManufacturerTemplateById(manufacturerTemplateId);
            if (manufacturerTemplate == null)
                return;

            
            if (!_context.IsAttached(manufacturerTemplate))
                _context.ManufacturerTemplates.Attach(manufacturerTemplate);
            _context.DeleteObject(manufacturerTemplate);
            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(MANUFACTURERTEMPLATES_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Gets all manufacturer templates
        /// </summary>
        /// <returns>Manufacturer template collection</returns>
        public List<ManufacturerTemplate> GetAllManufacturerTemplates()
        {
            string key = string.Format(MANUFACTURERTEMPLATES_ALL_KEY);
            object obj2 = _cacheManager.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (List<ManufacturerTemplate>)obj2;
            }

            
            var query = from mt in _context.ManufacturerTemplates
                        orderby mt.DisplayOrder, mt.Name
                        select mt;
            var manufacturerTemplates = query.ToList();

            if (this.CacheEnabled)
            {
                _cacheManager.Add(key, manufacturerTemplates);
            }
            return manufacturerTemplates;
        }

        /// <summary>
        /// Gets a manufacturer template
        /// </summary>
        /// <param name="manufacturerTemplateId">Manufacturer template identifier</param>
        /// <returns>Manufacturer template</returns>
        public ManufacturerTemplate GetManufacturerTemplateById(int manufacturerTemplateId)
        {
            if (manufacturerTemplateId == 0)
                return null;

            string key = string.Format(MANUFACTURERTEMPLATES_BY_ID_KEY, manufacturerTemplateId);
            object obj2 = _cacheManager.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (ManufacturerTemplate)obj2;
            }

            
            var query = from mt in _context.ManufacturerTemplates
                        where mt.ManufacturerTemplateId == manufacturerTemplateId
                        select mt;
            var manufacturerTemplate = query.SingleOrDefault();

            if (this.CacheEnabled)
            {
                _cacheManager.Add(key, manufacturerTemplate);
            }
            return manufacturerTemplate;
        }

        /// <summary>
        /// Inserts a manufacturer template
        /// </summary>
        /// <param name="manufacturerTemplate">Manufacturer template</param>
        public void InsertManufacturerTemplate(ManufacturerTemplate manufacturerTemplate)
        {
            if (manufacturerTemplate == null)
                throw new ArgumentNullException("manufacturerTemplate");

            manufacturerTemplate.Name = CommonHelper.EnsureNotNull(manufacturerTemplate.Name);
            manufacturerTemplate.Name = CommonHelper.EnsureMaximumLength(manufacturerTemplate.Name, 100);
            manufacturerTemplate.TemplatePath = CommonHelper.EnsureNotNull(manufacturerTemplate.TemplatePath);
            manufacturerTemplate.TemplatePath = CommonHelper.EnsureMaximumLength(manufacturerTemplate.TemplatePath, 200);

            
            
            _context.ManufacturerTemplates.AddObject(manufacturerTemplate);
            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(MANUFACTURERTEMPLATES_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Updates the manufacturer template
        /// </summary>
        /// <param name="manufacturerTemplate">Manufacturer template</param>
        public void UpdateManufacturerTemplate(ManufacturerTemplate manufacturerTemplate)
        {
            if (manufacturerTemplate == null)
                throw new ArgumentNullException("manufacturerTemplate");

            manufacturerTemplate.Name = CommonHelper.EnsureNotNull(manufacturerTemplate.Name);
            manufacturerTemplate.Name = CommonHelper.EnsureMaximumLength(manufacturerTemplate.Name, 100);
            manufacturerTemplate.TemplatePath = CommonHelper.EnsureNotNull(manufacturerTemplate.TemplatePath);
            manufacturerTemplate.TemplatePath = CommonHelper.EnsureMaximumLength(manufacturerTemplate.TemplatePath, 200);

            
            if (!_context.IsAttached(manufacturerTemplate))
                _context.ManufacturerTemplates.Attach(manufacturerTemplate);

            _context.SaveChanges();
            
            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(MANUFACTURERTEMPLATES_PATTERN_KEY);
            }
        }
        
        /// <summary>
        /// Deletes a product template
        /// </summary>
        /// <param name="productTemplateId">Product template identifier</param>
        public void DeleteProductTemplate(int productTemplateId)
        {
            var productTemplate = GetProductTemplateById(productTemplateId);
            if (productTemplate == null)
                return;

            
            if (!_context.IsAttached(productTemplate))
                _context.ProductTemplates.Attach(productTemplate);
            _context.DeleteObject(productTemplate);
            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(PRODUCTTEMPLATES_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Gets all product templates
        /// </summary>
        /// <returns>Product template collection</returns>
        public List<ProductTemplate> GetAllProductTemplates()
        {
            string key = string.Format(PRODUCTTEMPLATES_ALL_KEY);
            object obj2 = _cacheManager.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (List<ProductTemplate>)obj2;
            }

            
            var query = from pt in _context.ProductTemplates
                        orderby pt.DisplayOrder, pt.Name
                        select pt;
            var productTemplates = query.ToList();

            if (this.CacheEnabled)
            {
                _cacheManager.Add(key, productTemplates);
            }
            return productTemplates;
        }

        /// <summary>
        /// Gets a product template
        /// </summary>
        /// <param name="productTemplateId">Product template identifier</param>
        /// <returns>Product template</returns>
        public ProductTemplate GetProductTemplateById(int productTemplateId)
        {
            if (productTemplateId == 0)
                return null;

            string key = string.Format(PRODUCTTEMPLATES_BY_ID_KEY, productTemplateId);
            object obj2 = _cacheManager.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (ProductTemplate)obj2;
            }

            
            var query = from pt in _context.ProductTemplates
                        where pt.ProductTemplateId == productTemplateId
                        select pt;
            var productTemplate = query.SingleOrDefault();

            if (this.CacheEnabled)
            {
                _cacheManager.Add(key, productTemplate);
            }
            return productTemplate;
        }

        /// <summary>
        /// Inserts a product template
        /// </summary>
        /// <param name="productTemplate">Product template</param>
        public void InsertProductTemplate(ProductTemplate productTemplate)
        {
            if (productTemplate == null)
                throw new ArgumentNullException("productTemplate");

            productTemplate.Name = CommonHelper.EnsureNotNull(productTemplate.Name);
            productTemplate.Name = CommonHelper.EnsureMaximumLength(productTemplate.Name, 100);
            productTemplate.TemplatePath = CommonHelper.EnsureNotNull(productTemplate.TemplatePath);
            productTemplate.TemplatePath = CommonHelper.EnsureMaximumLength(productTemplate.TemplatePath, 200);

            
            
            _context.ProductTemplates.AddObject(productTemplate);
            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(PRODUCTTEMPLATES_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Updates the product template
        /// </summary>
        /// <param name="productTemplate">Product template</param>
        /// <returns>Product template</returns>
        public void UpdateProductTemplate(ProductTemplate productTemplate)
        {
            if (productTemplate == null)
                throw new ArgumentNullException("productTemplate");

            productTemplate.Name = CommonHelper.EnsureNotNull(productTemplate.Name);
            productTemplate.Name = CommonHelper.EnsureMaximumLength(productTemplate.Name, 100);
            productTemplate.TemplatePath = CommonHelper.EnsureNotNull(productTemplate.TemplatePath);
            productTemplate.TemplatePath = CommonHelper.EnsureMaximumLength(productTemplate.TemplatePath, 200);

            
            if (!_context.IsAttached(productTemplate))
                _context.ProductTemplates.Attach(productTemplate);

            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(PRODUCTTEMPLATES_PATTERN_KEY);
            }
        }

        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether cache is enabled
        /// </summary>
        public bool CacheEnabled
        {
            get
            {
                return IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Cache.TemplateManager.CacheEnabled");
            }
        }
        #endregion
    }
}