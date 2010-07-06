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

namespace NopSolutions.NopCommerce.BusinessLogic.Templates
{
    /// <summary>
    /// Category template manager
    /// </summary>
    public partial class TemplateManager
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

        #region Methods
        /// <summary>
        /// Deletes a category template
        /// </summary>
        /// <param name="categoryTemplateId">Category template identifier</param>
        public static void DeleteCategoryTemplate(int categoryTemplateId)
        {
            var categoryTemplate = GetCategoryTemplateById(categoryTemplateId);
            if (categoryTemplate == null)
                return;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(categoryTemplate))
                context.CategoryTemplates.Attach(categoryTemplate);
            context.DeleteObject(categoryTemplate);
            context.SaveChanges();
            
            if (TemplateManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(CATEGORYTEMPLATES_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Gets all category templates
        /// </summary>
        /// <returns>Category template collection</returns>
        public static List<CategoryTemplate> GetAllCategoryTemplates()
        {
            string key = string.Format(CATEGORYTEMPLATES_ALL_KEY);
            object obj2 = NopRequestCache.Get(key);
            if (TemplateManager.CacheEnabled && (obj2 != null))
            {
                return (List<CategoryTemplate>)obj2;
            }

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from ct in context.CategoryTemplates
                        orderby ct.DisplayOrder, ct.Name
                        select ct;
            var categoryTemplates = query.ToList();

            if (TemplateManager.CacheEnabled)
            {
                NopRequestCache.Add(key, categoryTemplates);
            }
            return categoryTemplates;
        }

        /// <summary>
        /// Gets a category template
        /// </summary>
        /// <param name="categoryTemplateId">Category template identifier</param>
        /// <returns>A category template</returns>
        public static CategoryTemplate GetCategoryTemplateById(int categoryTemplateId)
        {
            if (categoryTemplateId == 0)
                return null;

            string key = string.Format(CATEGORYTEMPLATES_BY_ID_KEY, categoryTemplateId);
            object obj2 = NopRequestCache.Get(key);
            if (TemplateManager.CacheEnabled && (obj2 != null))
            {
                return (CategoryTemplate)obj2;
            }

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from ct in context.CategoryTemplates
                        where ct.CategoryTemplateId == categoryTemplateId
                        select ct;
            var categoryTemplate = query.SingleOrDefault();

            if (TemplateManager.CacheEnabled)
            {
                NopRequestCache.Add(key, categoryTemplate);
            }
            return categoryTemplate;
        }

        /// <summary>
        /// Inserts a category template
        /// </summary>
        /// <param name="name">The name</param>
        /// <param name="templatePath">The template path</param>
        /// <param name="displayOrder">The display order</param>
        /// <param name="createdOn">The date and time of instance creation</param>
        /// <param name="updatedOn">The date and time of instance update</param>
        /// <returns>A category template</returns>
        public static CategoryTemplate InsertCategoryTemplate(string name,
            string templatePath, int displayOrder, DateTime createdOn, DateTime updatedOn)
        {
            name = CommonHelper.EnsureMaximumLength(name, 100);
            templatePath = CommonHelper.EnsureMaximumLength(templatePath, 200);

            var context = ObjectContextHelper.CurrentObjectContext;

            var categoryTemplate = context.CategoryTemplates.CreateObject();
            categoryTemplate.Name = name;
            categoryTemplate.TemplatePath = templatePath;
            categoryTemplate.DisplayOrder = displayOrder;
            categoryTemplate.CreatedOn = createdOn;
            categoryTemplate.UpdatedOn = updatedOn;

            context.CategoryTemplates.AddObject(categoryTemplate);
            context.SaveChanges();

            if (TemplateManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(CATEGORYTEMPLATES_PATTERN_KEY);
            }

            return categoryTemplate;
        }

        /// <summary>
        /// Updates the category template
        /// </summary>
        /// <param name="categoryTemplateId">Category template identifier</param>
        /// <param name="name">The name</param>
        /// <param name="templatePath">The template path</param>
        /// <param name="displayOrder">The display order</param>
        /// <param name="createdOn">The date and time of instance creation</param>
        /// <param name="updatedOn">The date and time of instance update</param>
        /// <returns>A category template</returns>
        public static CategoryTemplate UpdateCategoryTemplate(int categoryTemplateId,
            string name, string templatePath, int displayOrder,
            DateTime createdOn, DateTime updatedOn)
        {
            name = CommonHelper.EnsureMaximumLength(name, 100);
            templatePath = CommonHelper.EnsureMaximumLength(templatePath, 200);

            var categoryTemplate = GetCategoryTemplateById(categoryTemplateId);
            if (categoryTemplate == null)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(categoryTemplate))
                context.CategoryTemplates.Attach(categoryTemplate);

            categoryTemplate.Name = name;
            categoryTemplate.TemplatePath = templatePath;
            categoryTemplate.DisplayOrder = displayOrder;
            categoryTemplate.CreatedOn = createdOn;
            categoryTemplate.UpdatedOn = updatedOn;
            context.SaveChanges();

            if (TemplateManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(CATEGORYTEMPLATES_PATTERN_KEY);
            }

            return categoryTemplate;
        }
        
        /// <summary>
        /// Deletes a manufacturer template
        /// </summary>
        /// <param name="manufacturerTemplateId">Manufacturer template identifier</param>
        public static void DeleteManufacturerTemplate(int manufacturerTemplateId)
        {
            var manufacturerTemplate = GetManufacturerTemplateById(manufacturerTemplateId);
            if (manufacturerTemplate == null)
                return;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(manufacturerTemplate))
                context.ManufacturerTemplates.Attach(manufacturerTemplate);
            context.DeleteObject(manufacturerTemplate);
            context.SaveChanges();

            if (TemplateManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(MANUFACTURERTEMPLATES_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Gets all manufacturer templates
        /// </summary>
        /// <returns>Manufacturer template collection</returns>
        public static List<ManufacturerTemplate> GetAllManufacturerTemplates()
        {
            string key = string.Format(MANUFACTURERTEMPLATES_ALL_KEY);
            object obj2 = NopRequestCache.Get(key);
            if (TemplateManager.CacheEnabled && (obj2 != null))
            {
                return (List<ManufacturerTemplate>)obj2;
            }

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from mt in context.ManufacturerTemplates
                        orderby mt.DisplayOrder, mt.Name
                        select mt;
            var manufacturerTemplates = query.ToList();

            if (TemplateManager.CacheEnabled)
            {
                NopRequestCache.Add(key, manufacturerTemplates);
            }
            return manufacturerTemplates;
        }

        /// <summary>
        /// Gets a manufacturer template
        /// </summary>
        /// <param name="manufacturerTemplateId">Manufacturer template identifier</param>
        /// <returns>Manufacturer template</returns>
        public static ManufacturerTemplate GetManufacturerTemplateById(int manufacturerTemplateId)
        {
            if (manufacturerTemplateId == 0)
                return null;

            string key = string.Format(MANUFACTURERTEMPLATES_BY_ID_KEY, manufacturerTemplateId);
            object obj2 = NopRequestCache.Get(key);
            if (TemplateManager.CacheEnabled && (obj2 != null))
            {
                return (ManufacturerTemplate)obj2;
            }

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from mt in context.ManufacturerTemplates
                        where mt.ManufacturerTemplateId == manufacturerTemplateId
                        select mt;
            var manufacturerTemplate = query.SingleOrDefault();

            if (TemplateManager.CacheEnabled)
            {
                NopRequestCache.Add(key, manufacturerTemplate);
            }
            return manufacturerTemplate;
        }

        /// <summary>
        /// Inserts a manufacturer template
        /// </summary>
        /// <param name="name">The manufacturer template identifier</param>
        /// <param name="templatePath">The template path</param>
        /// <param name="displayOrder">The display order</param>
        /// <param name="createdOn">The date and time of instance creation</param>
        /// <param name="updatedOn">The date and time of instance update</param>
        /// <returns>Manufacturer template</returns>
        public static ManufacturerTemplate InsertManufacturerTemplate(string name,
            string templatePath, int displayOrder, DateTime createdOn, DateTime updatedOn)
        {
            name = CommonHelper.EnsureMaximumLength(name, 100);
            templatePath = CommonHelper.EnsureMaximumLength(templatePath, 200);

            var context = ObjectContextHelper.CurrentObjectContext;

            var manufacturerTemplate = context.ManufacturerTemplates.CreateObject();
            manufacturerTemplate.Name = name;
            manufacturerTemplate.TemplatePath = templatePath;
            manufacturerTemplate.DisplayOrder = displayOrder;
            manufacturerTemplate.CreatedOn = createdOn;
            manufacturerTemplate.UpdatedOn = updatedOn;

            context.ManufacturerTemplates.AddObject(manufacturerTemplate);
            context.SaveChanges();

            if (TemplateManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(MANUFACTURERTEMPLATES_PATTERN_KEY);
            }
            return manufacturerTemplate;
        }

        /// <summary>
        /// Updates the manufacturer template
        /// </summary>
        /// <param name="manufacturerTemplateId">Manufacturer template identifer</param>
        /// <param name="name">The manufacturer template identifier</param>
        /// <param name="templatePath">The template path</param>
        /// <param name="displayOrder">The display order</param>
        /// <param name="createdOn">The date and time of instance creation</param>
        /// <param name="updatedOn">The date and time of instance update</param>
        /// <returns>Manufacturer template</returns>
        public static ManufacturerTemplate UpdateManufacturerTemplate(int manufacturerTemplateId,
            string name, string templatePath, int displayOrder,
            DateTime createdOn, DateTime updatedOn)
        {
            name = CommonHelper.EnsureMaximumLength(name, 100);
            templatePath = CommonHelper.EnsureMaximumLength(templatePath, 200);

            var manufacturerTemplate = GetManufacturerTemplateById(manufacturerTemplateId);
            if (manufacturerTemplate == null)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(manufacturerTemplate))
                context.ManufacturerTemplates.Attach(manufacturerTemplate);

            manufacturerTemplate.Name = name;
            manufacturerTemplate.TemplatePath = templatePath;
            manufacturerTemplate.DisplayOrder = displayOrder;
            manufacturerTemplate.CreatedOn = createdOn;
            manufacturerTemplate.UpdatedOn = updatedOn;
            context.SaveChanges();


            if (TemplateManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(MANUFACTURERTEMPLATES_PATTERN_KEY);
            }
            return manufacturerTemplate;
        }
        
        /// <summary>
        /// Deletes a product template
        /// </summary>
        /// <param name="productTemplateId">Product template identifier</param>
        public static void DeleteProductTemplate(int productTemplateId)
        {
            var productTemplate = GetProductTemplateById(productTemplateId);
            if (productTemplate == null)
                return;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(productTemplate))
                context.ProductTemplates.Attach(productTemplate);
            context.DeleteObject(productTemplate);
            context.SaveChanges();

            if (TemplateManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(PRODUCTTEMPLATES_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Gets all product templates
        /// </summary>
        /// <returns>Product template collection</returns>
        public static List<ProductTemplate> GetAllProductTemplates()
        {
            string key = string.Format(PRODUCTTEMPLATES_ALL_KEY);
            object obj2 = NopRequestCache.Get(key);
            if (TemplateManager.CacheEnabled && (obj2 != null))
            {
                return (List<ProductTemplate>)obj2;
            }

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from pt in context.ProductTemplates
                        orderby pt.DisplayOrder, pt.Name
                        select pt;
            var productTemplates = query.ToList();

            if (TemplateManager.CacheEnabled)
            {
                NopRequestCache.Add(key, productTemplates);
            }
            return productTemplates;
        }

        /// <summary>
        /// Gets a product template
        /// </summary>
        /// <param name="productTemplateId">Product template identifier</param>
        /// <returns>Product template</returns>
        public static ProductTemplate GetProductTemplateById(int productTemplateId)
        {
            if (productTemplateId == 0)
                return null;

            string key = string.Format(PRODUCTTEMPLATES_BY_ID_KEY, productTemplateId);
            object obj2 = NopRequestCache.Get(key);
            if (TemplateManager.CacheEnabled && (obj2 != null))
            {
                return (ProductTemplate)obj2;
            }

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from pt in context.ProductTemplates
                        where pt.ProductTemplateId == productTemplateId
                        select pt;
            var productTemplate = query.SingleOrDefault();

            if (TemplateManager.CacheEnabled)
            {
                NopRequestCache.Add(key, productTemplate);
            }
            return productTemplate;
        }

        /// <summary>
        /// Inserts a product template
        /// </summary>
        /// <param name="name">The name</param>
        /// <param name="templatePath">The template path</param>
        /// <param name="displayOrder">The display order</param>
        /// <param name="createdOn">The date and time of instance creation</param>
        /// <param name="updatedOn">The date and time of instance update</param>
        /// <returns>Product template</returns>
        public static ProductTemplate InsertProductTemplate(string name, string templatePath,
            int displayOrder, DateTime createdOn, DateTime updatedOn)
        {
            name = CommonHelper.EnsureMaximumLength(name, 100);
            templatePath = CommonHelper.EnsureMaximumLength(templatePath, 200);

            var context = ObjectContextHelper.CurrentObjectContext;

            var productTemplate = context.ProductTemplates.CreateObject();
            productTemplate.Name = name;
            productTemplate.TemplatePath = templatePath;
            productTemplate.DisplayOrder = displayOrder;
            productTemplate.CreatedOn = createdOn;
            productTemplate.UpdatedOn = updatedOn;

            context.ProductTemplates.AddObject(productTemplate);
            context.SaveChanges();

            if (TemplateManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(PRODUCTTEMPLATES_PATTERN_KEY);
            }

            return productTemplate;
        }

        /// <summary>
        /// Updates the product template
        /// </summary>
        /// <param name="productTemplateId">The product template identifier</param>
        /// <param name="name">The name</param>
        /// <param name="templatePath">The template path</param>
        /// <param name="displayOrder">The display order</param>
        /// <param name="createdOn">The date and time of instance creation</param>
        /// <param name="updatedOn">The date and time of instance update</param>
        /// <returns>Product template</returns>
        public static ProductTemplate UpdateProductTemplate(int productTemplateId,
            string name, string templatePath, int displayOrder,
            DateTime createdOn, DateTime updatedOn)
        {
            name = CommonHelper.EnsureMaximumLength(name, 100);
            templatePath = CommonHelper.EnsureMaximumLength(templatePath, 200);

            var productTemplate = GetProductTemplateById(productTemplateId);
            if (productTemplate == null)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(productTemplate))
                context.ProductTemplates.Attach(productTemplate);

            productTemplate.Name = name;
            productTemplate.TemplatePath = templatePath;
            productTemplate.DisplayOrder = displayOrder;
            productTemplate.CreatedOn = createdOn;
            productTemplate.UpdatedOn = updatedOn;
            context.SaveChanges();

            if (TemplateManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(PRODUCTTEMPLATES_PATTERN_KEY);
            }

            return productTemplate;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether cache is enabled
        /// </summary>
        public static bool CacheEnabled
        {
            get
            {
                return SettingManager.GetSettingValueBoolean("Cache.TemplateManager.CacheEnabled");
            }
        }
        #endregion
    }
}