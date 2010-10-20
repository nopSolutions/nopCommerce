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
using NopSolutions.NopCommerce.BusinessLogic.Security;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.BusinessLogic.Categories
{
    /// <summary>
    /// Category manager
    /// </summary>
    public partial class CategoryManager
    {
        #region Constants
        private const string CATEGORIES_BY_ID_KEY = "Nop.category.id-{0}";
        private const string PRODUCTCATEGORIES_ALLBYCATEGORYID_KEY = "Nop.productcategory.allbycategoryid-{0}-{1}";
        private const string PRODUCTCATEGORIES_ALLBYPRODUCTID_KEY = "Nop.productcategory.allbyproductid-{0}-{1}";
        private const string PRODUCTCATEGORIES_BY_ID_KEY = "Nop.productcategory.id-{0}";
        private const string CATEGORIES_PATTERN_KEY = "Nop.category.";
        private const string PRODUCTCATEGORIES_PATTERN_KEY = "Nop.productcategory.";

        #endregion

        #region Methods

        /// <summary>
        /// Is access denied
        /// </summary>
        /// <param name="category">Category</param>
        /// <returns>true - access is denied; otherwise, false</returns>
        public static bool IsCategoryAccessDenied(Category category)
        {
            var context = ObjectContextHelper.CurrentObjectContext;
            return IsCategoryAccessDenied(category, context);
        }

        /// <summary>
        /// Is access denied
        /// </summary>
        /// <param name="category">Category</param>
        /// <param name="context">context</param>
        /// <returns>true - access is denied; otherwise, false</returns>
        public static bool IsCategoryAccessDenied(Category category, NopObjectContext context)
        {
            if (category == null)
                throw new ArgumentNullException("category");

            return category.IsAccessDenied(context);
        }

        /// <summary>
        /// Marks category as deleted
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        public static void MarkCategoryAsDeleted(int categoryId)
        {
            var category = GetCategoryById(categoryId);
            if (category != null)
            {
                category.Deleted = true;
                UpdateCategory(category);
            }
        }

        /// <summary>
        /// Removes category picture
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        public static void RemoveCategoryPicture(int categoryId)
        {
            var category = GetCategoryById(categoryId);
            if (category != null)
            {
                category.PictureId = 0;
                UpdateCategory(category);
            }
        }

        /// <summary>
        /// Gets all categories
        /// </summary>
        /// <returns>Categories</returns>
        public static List<Category> GetAllCategories()
        {
            bool showHidden = NopContext.Current.IsAdmin;
            return GetAllCategories(showHidden);
        }
        
        /// <summary>
        /// Gets all categories
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Categories</returns>
        public static List<Category> GetAllCategories(bool showHidden)
        {
            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from c in context.Categories
                        orderby c.ParentCategoryId, c.DisplayOrder
                        where (showHidden || c.Published) &&
                        !c.Deleted
                        select c;

            //filter by access control list (public store)
            if (!showHidden)
            {
                query = query.WhereAclPerObjectNotDenied(context);
            }
            var unsortedCategories = query.ToList();

            //sort categories
            //TODO sort categories on database layer
            var sortedCategories = unsortedCategories.SortCategoriesForTree(0);

            return sortedCategories;
        }
        
        /// <summary>
        /// Gets all categories by parent category identifier
        /// </summary>
        /// <param name="parentCategoryId">Parent category identifier</param>
        /// <returns>Category collection</returns>
        public static List<Category> GetAllCategoriesByParentCategoryId(int parentCategoryId)
        {
            bool showHidden = NopContext.Current.IsAdmin;
            return GetAllCategoriesByParentCategoryId(parentCategoryId, showHidden);
        }
        
        /// <summary>
        /// Gets all categories filtered by parent category identifier
        /// </summary>
        /// <param name="parentCategoryId">Parent category identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Category collection</returns>
        public static List<Category> GetAllCategoriesByParentCategoryId(int parentCategoryId,
            bool showHidden)
        {
            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from c in context.Categories
                        orderby c.DisplayOrder
                        where (showHidden || c.Published) && 
                        !c.Deleted && 
                        c.ParentCategoryId == parentCategoryId
                        select c;

            //filter by access control list (public store)
            if (!showHidden)
            {
                query = query.WhereAclPerObjectNotDenied(context);
            }

            var categories = query.ToList();
            return categories;
        }
        
        /// <summary>
        /// Gets all categories displayed on the home page
        /// </summary>
        /// <returns>Category collection</returns>
        public static List<Category> GetAllCategoriesDisplayedOnHomePage()
        {
            bool showHidden = NopContext.Current.IsAdmin;

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from c in context.Categories
                        orderby c.DisplayOrder
                        where (showHidden || c.Published) && !c.Deleted && c.ShowOnHomePage
                        select c;

            //filter by access control list (public store)
            if (!showHidden)
            {
                query = query.WhereAclPerObjectNotDenied(context);
            }

            var categories = query.ToList();
            return categories;
        }
                
        /// <summary>
        /// Gets a category
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <returns>Category</returns>
        public static Category GetCategoryById(int categoryId)
        {
            if (categoryId == 0)
                return null;

            string key = string.Format(CATEGORIES_BY_ID_KEY, categoryId);
            object obj2 = NopRequestCache.Get(key);
            if (CategoryManager.CategoriesCacheEnabled && (obj2 != null))
            {
                return (Category)obj2;
            }
            
            bool showHidden = NopContext.Current.IsAdmin;

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from c in context.Categories
                        where c.CategoryId == categoryId
                        select c;
            var category = query.SingleOrDefault();
            
            //filter by access control list (public store)
            if (!showHidden && IsCategoryAccessDenied(category))
            {
                category = null;
            }
            if (CategoryManager.CategoriesCacheEnabled)
            {
                NopRequestCache.Add(key, category);
            }
            return category;
        }

        /// <summary>
        /// Gets a category breadcrumb
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <returns>Category</returns>
        public static List<Category> GetBreadCrumb(int categoryId)
        {
            var breadCrumb = new List<Category>();

            var category = GetCategoryById(categoryId);

            bool showHidden = NopContext.Current.IsAdmin;

            while (category != null && //category is not null
                !category.Deleted && //category is not deleted
                category.Published && //category is published
                (showHidden || !IsCategoryAccessDenied(category))) //access is allowed (in public store)
            {
                breadCrumb.Add(category);
                category = category.ParentCategory;
            }
            breadCrumb.Reverse();
            return breadCrumb;
        }

        /// <summary>
        /// Inserts category
        /// </summary>
        /// <param name="category">Category</param>
        public static void InsertCategory(Category category)
        {
            if (category == null)
                throw new ArgumentNullException("category");
            
            category.Name = CommonHelper.EnsureNotNull(category.Name);
            category.Name = CommonHelper.EnsureMaximumLength(category.Name, 400);
            category.Description = CommonHelper.EnsureNotNull(category.Description);
            category.MetaKeywords = CommonHelper.EnsureNotNull(category.MetaKeywords);
            category.MetaKeywords = CommonHelper.EnsureMaximumLength(category.MetaKeywords, 400);
            category.MetaDescription = CommonHelper.EnsureNotNull(category.MetaDescription);
            category.MetaDescription = CommonHelper.EnsureMaximumLength(category.MetaDescription, 4000);
            category.MetaTitle = CommonHelper.EnsureNotNull(category.MetaTitle);
            category.MetaTitle = CommonHelper.EnsureMaximumLength(category.MetaTitle, 400);
            category.SEName = CommonHelper.EnsureNotNull(category.SEName);
            category.SEName = CommonHelper.EnsureMaximumLength(category.SEName, 100);
            category.PriceRanges = CommonHelper.EnsureNotNull(category.PriceRanges);
            category.PriceRanges = CommonHelper.EnsureMaximumLength(category.PriceRanges, 400);

            var context = ObjectContextHelper.CurrentObjectContext;

            context.Categories.AddObject(category);
            context.SaveChanges();

            if (CategoryManager.CategoriesCacheEnabled || CategoryManager.MappingsCacheEnabled)
            {
                NopRequestCache.RemoveByPattern(CATEGORIES_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(PRODUCTCATEGORIES_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Updates the category
        /// </summary>
        /// <param name="category">Category</param>
        public static void UpdateCategory(Category category)
        {
            if (category == null)
                throw new ArgumentNullException("category");

            category.Name = CommonHelper.EnsureNotNull(category.Name);
            category.Name = CommonHelper.EnsureMaximumLength(category.Name, 400);
            category.Description = CommonHelper.EnsureNotNull(category.Description);
            category.MetaKeywords = CommonHelper.EnsureNotNull(category.MetaKeywords);
            category.MetaKeywords = CommonHelper.EnsureMaximumLength(category.MetaKeywords, 400);
            category.MetaDescription = CommonHelper.EnsureNotNull(category.MetaDescription);
            category.MetaDescription = CommonHelper.EnsureMaximumLength(category.MetaDescription, 4000);
            category.MetaTitle = CommonHelper.EnsureNotNull(category.MetaTitle);
            category.MetaTitle = CommonHelper.EnsureMaximumLength(category.MetaTitle, 400);
            category.SEName = CommonHelper.EnsureNotNull(category.SEName);
            category.SEName = CommonHelper.EnsureMaximumLength(category.SEName, 100);
            category.PriceRanges = CommonHelper.EnsureNotNull(category.PriceRanges);
            category.PriceRanges = CommonHelper.EnsureMaximumLength(category.PriceRanges, 400);

            //validate category hierarchy
            var parentCategory = GetCategoryById(category.ParentCategoryId);
            while (parentCategory != null)
            {
                if (category.CategoryId == parentCategory.CategoryId)
                {
                    category.ParentCategoryId = 0;
                    break;
                }
                parentCategory = GetCategoryById(parentCategory.ParentCategoryId);
            }

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(category))
                context.Categories.Attach(category);

            context.SaveChanges();

            if (CategoryManager.CategoriesCacheEnabled || CategoryManager.MappingsCacheEnabled)
            {
                NopRequestCache.RemoveByPattern(CATEGORIES_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(PRODUCTCATEGORIES_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Gets localized category by id
        /// </summary>
        /// <param name="categoryLocalizedId">Localized category identifier</param>
        /// <returns>Category content</returns>
        public static CategoryLocalized GetCategoryLocalizedById(int categoryLocalizedId)
        {
            if (categoryLocalizedId == 0)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from cl in context.CategoryLocalized
                        where cl.CategoryLocalizedId == categoryLocalizedId
                        select cl;
            var categoryLocalized = query.SingleOrDefault();
            return categoryLocalized;
        }

        /// <summary>
        /// Gets localized category by category id
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <returns>Category content</returns>
        public static List<CategoryLocalized> GetCategoryLocalizedByCategoryId(int categoryId)
        {
            if (categoryId == 0)
                return new List<CategoryLocalized>();

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from cl in context.CategoryLocalized
                        where cl.CategoryId == categoryId
                        select cl;
            var content = query.ToList();
            return content;
        }

        /// <summary>
        /// Gets localized category by category id and language id
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Category content</returns>
        public static CategoryLocalized GetCategoryLocalizedByCategoryIdAndLanguageId(int categoryId, int languageId)
        {
            if (categoryId == 0 || languageId == 0)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from cl in context.CategoryLocalized
                        orderby cl.CategoryLocalizedId
                        where cl.CategoryId == categoryId &&
                        cl.LanguageId == languageId
                        select cl;
            var categoryLocalized = query.FirstOrDefault();
            return categoryLocalized;
        }

        /// <summary>
        /// Inserts a localized category
        /// </summary>
        /// <param name="categoryLocalized">Localized category</param>
        public static void InsertCategoryLocalized(CategoryLocalized categoryLocalized)
        {
            if (categoryLocalized == null)
                throw new ArgumentNullException("categoryLocalized");
            
            categoryLocalized.Name = CommonHelper.EnsureNotNull(categoryLocalized.Name);
            categoryLocalized.Name = CommonHelper.EnsureMaximumLength(categoryLocalized.Name, 400);
            categoryLocalized.Description = CommonHelper.EnsureNotNull(categoryLocalized.Description);
            categoryLocalized.MetaKeywords = CommonHelper.EnsureNotNull(categoryLocalized.MetaKeywords);
            categoryLocalized.MetaKeywords = CommonHelper.EnsureMaximumLength(categoryLocalized.MetaKeywords, 400);
            categoryLocalized.MetaDescription = CommonHelper.EnsureNotNull(categoryLocalized.MetaDescription);
            categoryLocalized.MetaDescription = CommonHelper.EnsureMaximumLength(categoryLocalized.MetaDescription, 4000);
            categoryLocalized.MetaTitle = CommonHelper.EnsureNotNull(categoryLocalized.MetaTitle);
            categoryLocalized.MetaTitle = CommonHelper.EnsureMaximumLength(categoryLocalized.MetaTitle, 400);
            categoryLocalized.SEName = CommonHelper.EnsureNotNull(categoryLocalized.SEName);
            categoryLocalized.SEName = CommonHelper.EnsureMaximumLength(categoryLocalized.SEName, 100);

            var context = ObjectContextHelper.CurrentObjectContext;
            
            context.CategoryLocalized.AddObject(categoryLocalized);
            context.SaveChanges();

            if (CategoryManager.CategoriesCacheEnabled)
            {
                NopRequestCache.RemoveByPattern(CATEGORIES_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Update a localized category
        /// </summary>
        /// <param name="categoryLocalized">Localized category</param>
        public static void UpdateCategoryLocalized(CategoryLocalized categoryLocalized)
        {
            if (categoryLocalized == null)
                throw new ArgumentNullException("categoryLocalized");

            categoryLocalized.Name = CommonHelper.EnsureNotNull(categoryLocalized.Name);
            categoryLocalized.Name = CommonHelper.EnsureMaximumLength(categoryLocalized.Name, 400);
            categoryLocalized.Description = CommonHelper.EnsureNotNull(categoryLocalized.Description);
            categoryLocalized.MetaKeywords = CommonHelper.EnsureNotNull(categoryLocalized.MetaKeywords);
            categoryLocalized.MetaKeywords = CommonHelper.EnsureMaximumLength(categoryLocalized.MetaKeywords, 400);
            categoryLocalized.MetaDescription = CommonHelper.EnsureNotNull(categoryLocalized.MetaDescription);
            categoryLocalized.MetaDescription = CommonHelper.EnsureMaximumLength(categoryLocalized.MetaDescription, 4000);
            categoryLocalized.MetaTitle = CommonHelper.EnsureNotNull(categoryLocalized.MetaTitle);
            categoryLocalized.MetaTitle = CommonHelper.EnsureMaximumLength(categoryLocalized.MetaTitle, 400);
            categoryLocalized.SEName = CommonHelper.EnsureNotNull(categoryLocalized.SEName);
            categoryLocalized.SEName = CommonHelper.EnsureMaximumLength(categoryLocalized.SEName, 100);

            bool allFieldsAreEmpty = string.IsNullOrEmpty(categoryLocalized.Name) &&
                string.IsNullOrEmpty(categoryLocalized.Description) &&
                string.IsNullOrEmpty(categoryLocalized.MetaKeywords) &&
                string.IsNullOrEmpty(categoryLocalized.MetaDescription) &&
                string.IsNullOrEmpty(categoryLocalized.MetaTitle) &&
                string.IsNullOrEmpty(categoryLocalized.SEName);

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(categoryLocalized))
                context.CategoryLocalized.Attach(categoryLocalized);

            if (allFieldsAreEmpty)
            {
                //delete if all fields are empty
                context.DeleteObject(categoryLocalized);
                context.SaveChanges();
            }
            else
            {
                context.SaveChanges();
            }

            if (CategoryManager.CategoriesCacheEnabled)
            {
                NopRequestCache.RemoveByPattern(CATEGORIES_PATTERN_KEY);
            }
        }
        
        /// <summary>
        /// Deletes a product category mapping
        /// </summary>
        /// <param name="productCategoryId">Product category identifier</param>
        public static void DeleteProductCategory(int productCategoryId)
        {
            if (productCategoryId == 0)
                return;

            var productCategory = GetProductCategoryById(productCategoryId);
            if (productCategory == null)
                return;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(productCategory))
                context.ProductCategories.Attach(productCategory);
            context.DeleteObject(productCategory);
            context.SaveChanges();

            if (CategoryManager.CategoriesCacheEnabled || CategoryManager.MappingsCacheEnabled)
            {
                NopRequestCache.RemoveByPattern(CATEGORIES_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(PRODUCTCATEGORIES_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Gets product category mapping collection
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <returns>Product a category mapping collection</returns>
        public static List<ProductCategory> GetProductCategoriesByCategoryId(int categoryId)
        {
            if (categoryId == 0)
                return new List<ProductCategory>();

            bool showHidden = NopContext.Current.IsAdmin;
            string key = string.Format(PRODUCTCATEGORIES_ALLBYCATEGORYID_KEY, showHidden, categoryId);
            object obj2 = NopRequestCache.Get(key);
            if (CategoryManager.MappingsCacheEnabled && (obj2 != null))
            {
                return (List<ProductCategory>)obj2;
            }

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from pc in context.ProductCategories
                        join p in context.Products on pc.ProductId equals p.ProductId
                        where pc.CategoryId == categoryId &&
                        !p.Deleted &&
                        (showHidden || p.Published)
                        orderby pc.DisplayOrder
                        select pc;
            var productCategories = query.ToList();

            if (CategoryManager.MappingsCacheEnabled)
            {
                NopRequestCache.Add(key, productCategories);
            }
            return productCategories;
        }

        /// <summary>
        /// Gets a product category mapping collection
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <returns>Product category mapping collection</returns>
        public static List<ProductCategory> GetProductCategoriesByProductId(int productId)
        {
            if (productId == 0)
                return new List<ProductCategory>();

            bool showHidden = NopContext.Current.IsAdmin;
            string key = string.Format(PRODUCTCATEGORIES_ALLBYPRODUCTID_KEY, showHidden, productId);
            object obj2 = NopRequestCache.Get(key);
            if (CategoryManager.MappingsCacheEnabled && (obj2 != null))
            {
                return (List<ProductCategory>)obj2;
            }

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from pc in context.ProductCategories
                        join c in context.Categories on pc.CategoryId equals c.CategoryId
                        where pc.ProductId == productId &&
                        !c.Deleted &&
                        (showHidden || c.Published)
                        orderby pc.DisplayOrder
                        select pc;
            var productCategories = query.ToList();

            if (CategoryManager.MappingsCacheEnabled)
            {
                NopRequestCache.Add(key, productCategories);
            }
            return productCategories;
        }

        /// <summary>
        /// Gets a product category mapping 
        /// </summary>
        /// <param name="productCategoryId">Product category mapping identifier</param>
        /// <returns>Product category mapping</returns>
        public static ProductCategory GetProductCategoryById(int productCategoryId)
        {
            if (productCategoryId == 0)
                return null;

            string key = string.Format(PRODUCTCATEGORIES_BY_ID_KEY, productCategoryId);
            object obj2 = NopRequestCache.Get(key);
            if (CategoryManager.MappingsCacheEnabled && (obj2 != null))
            {
                return (ProductCategory)obj2;
            }

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from pc in context.ProductCategories
                        where pc.ProductCategoryId == productCategoryId
                        select pc;
            var productCategory = query.SingleOrDefault();

            if (CategoryManager.MappingsCacheEnabled)
            {
                NopRequestCache.Add(key, productCategory);
            }
            return productCategory;
        }

        /// <summary>
        /// Inserts a product category mapping
        /// </summary>
        /// <param name="productCategory">>Product category mapping</param>
        public static void InsertProductCategory(ProductCategory productCategory)
        {
            if (productCategory == null)
                throw new ArgumentNullException("productCategory");

            var context = ObjectContextHelper.CurrentObjectContext;
            
            context.ProductCategories.AddObject(productCategory);
            context.SaveChanges();

            if (CategoryManager.CategoriesCacheEnabled || CategoryManager.MappingsCacheEnabled)
            {
                NopRequestCache.RemoveByPattern(CATEGORIES_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(PRODUCTCATEGORIES_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Updates the product category mapping 
        /// </summary>
        /// <param name="productCategory">>Product category mapping</param>
        public static void UpdateProductCategory(ProductCategory productCategory)
        {
            if (productCategory == null)
                throw new ArgumentNullException("productCategory");

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(productCategory))
                context.ProductCategories.Attach(productCategory);

            context.SaveChanges();

            if (CategoryManager.CategoriesCacheEnabled || CategoryManager.MappingsCacheEnabled)
            {
                NopRequestCache.RemoveByPattern(CATEGORIES_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(PRODUCTCATEGORIES_PATTERN_KEY);
            }
        }
        #endregion
        
        #region Properties
        /// <summary>
        /// Gets a value indicating whether categories cache is enabled
        /// </summary>
        public static bool CategoriesCacheEnabled
        {
            get
            {
                return SettingManager.GetSettingValueBoolean("Cache.CategoryManager.CategoriesCacheEnabled");
            }
        }

        /// <summary>
        /// Gets a value indicating whether mappings cache is enabled
        /// </summary>
        public static bool MappingsCacheEnabled
        {
            get
            {
                return SettingManager.GetSettingValueBoolean("Cache.CategoryManager.MappingsCacheEnabled");
            }
        }
        #endregion
    }
}
