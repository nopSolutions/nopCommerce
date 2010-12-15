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
using Nop.Core.Domain;
using Nop.Core.Caching;
using Nop.Data;
using Nop.Core;

namespace Nop.Services
{
    /// <summary>
    /// Category service
    /// </summary>
    public partial class CategoryService : ICategoryService
    {
        #region Constants
        private const string CATEGORIES_BY_ID_KEY = "Nop.category.id-{0}";
        private const string PRODUCTCATEGORIES_ALLBYCATEGORYID_KEY = "Nop.productcategory.allbycategoryid-{0}-{1}";
        private const string PRODUCTCATEGORIES_ALLBYPRODUCTID_KEY = "Nop.productcategory.allbyproductid-{0}-{1}";
        private const string PRODUCTCATEGORIES_BY_ID_KEY = "Nop.productcategory.id-{0}";
        private const string CATEGORIES_PATTERN_KEY = "Nop.category.";
        private const string PRODUCTCATEGORIES_PATTERN_KEY = "Nop.productcategory.";

        #endregion

        #region Fields

        private readonly IRepository<Category> _categoryRespository;
        private readonly IRepository<LocalizedCategory> _localizedCategoryRespository;
        private readonly IRepository<ProductCategory> _productCategoryRespository;
        private readonly IRepository<Product> _productRespository;
        private readonly ICacheManager _cacheManager;

        #endregion
        
        #region Ctor
        
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="categoryRespository">Category repository</param>
        /// <param name="localizedCategoryRespository">Localized category repository</param>
        /// <param name="productCategoryRespository">ProductCategory repository</param>
        /// <param name="productRespository">Productrepository</param>
        public CategoryService(ICacheManager cacheManager,
            IRepository<Category> categoryRespository,
            IRepository<LocalizedCategory> localizedCategoryRespository,
            IRepository<ProductCategory> productCategoryRespository,
            IRepository<Product> productRespository)
        {
            this._cacheManager = cacheManager;
            this._categoryRespository = categoryRespository;
            this._localizedCategoryRespository = localizedCategoryRespository;
            this._productCategoryRespository = productCategoryRespository;
            this._productRespository = productRespository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Marks category as deleted
        /// </summary>
        /// <param name="category">Category</param>
        public void DeleteCategory(Category category)
        {
            if (category == null)
                return;

            category.Deleted = true;
            UpdateCategory(category);
        }

        /// <summary>
        /// Gets all categories
        /// </summary>
        /// <returns>Categories</returns>
        public List<Category> GetAllCategories()
        {
            //TODO: use bool showHidden = NopContext.Current.IsAdmin;
            bool showHidden = true;
            return GetAllCategories(showHidden);
        }
        
        /// <summary>
        /// Gets all categories
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Categories</returns>
        public List<Category> GetAllCategories(bool showHidden)
        {
            
            var query = from c in _categoryRespository.Table
                        orderby c.ParentCategoryId, c.DisplayOrder
                        where (showHidden || c.Published) &&
                        !c.Deleted
                        select c;

            //filter by access control list (public store)
            //if (!showHidden)
            //{
            //    query = query.WhereAclPerObjectNotDenied(_categoryRespository);
            //}
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
        public List<Category> GetAllCategoriesByParentCategoryId(int parentCategoryId)
        {
            //TODO: use bool showHidden = NopContext.Current.IsAdmin;
            bool showHidden = true;
            return GetAllCategoriesByParentCategoryId(parentCategoryId, showHidden);
        }
        
        /// <summary>
        /// Gets all categories filtered by parent category identifier
        /// </summary>
        /// <param name="parentCategoryId">Parent category identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Category collection</returns>
        public List<Category> GetAllCategoriesByParentCategoryId(int parentCategoryId,
            bool showHidden)
        {
            
            var query = from c in _categoryRespository.Table
                        orderby c.DisplayOrder
                        where (showHidden || c.Published) && 
                        !c.Deleted && 
                        c.ParentCategoryId == parentCategoryId
                        select c;

            //filter by access control list (public store)
            //if (!showHidden)
            //{
            //    query = query.WhereAclPerObjectNotDenied(_categoryRespository);
            //}

            var categories = query.ToList();
            return categories;
        }
        
        /// <summary>
        /// Gets all categories displayed on the home page
        /// </summary>
        /// <returns>Category collection</returns>
        public List<Category> GetAllCategoriesDisplayedOnHomePage()
        {
            //TODO: use bool showHidden = NopContext.Current.IsAdmin;
            bool showHidden = true;
            
            var query = from c in _categoryRespository.Table
                        orderby c.DisplayOrder
                        where (showHidden || c.Published) && !c.Deleted && c.ShowOnHomePage
                        select c;

            //filter by access control list (public store)
            //if (!showHidden)
            //{
            //    query = query.WhereAclPerObjectNotDenied(_context);
            //}

            var categories = query.ToList();
            return categories;
        }
                
        /// <summary>
        /// Gets a category
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <returns>Category</returns>
        public Category GetCategoryById(int categoryId)
        {
            if (categoryId == 0)
                return null;

            string key = string.Format(CATEGORIES_BY_ID_KEY, categoryId);
            object obj2 = _cacheManager.Get(key);
            if (obj2 != null)
            {
                return (Category) obj2;
            }

            var category = _categoryRespository.GetById(categoryId);

            //filter by access control list (public store)
            //if (category != null && !showHidden && IsCategoryAccessDenied(category))
            //{
            //    category = null;
            //}

            //cache
            _cacheManager.Add(key, category);

            return category;
        }

        /// <summary>
        /// Inserts category
        /// </summary>
        /// <param name="category">Category</param>
        public void InsertCategory(Category category)
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
            category.SeName = CommonHelper.EnsureNotNull(category.SeName);
            category.SeName = CommonHelper.EnsureMaximumLength(category.SeName, 100);
            category.PriceRanges = CommonHelper.EnsureNotNull(category.PriceRanges);
            category.PriceRanges = CommonHelper.EnsureMaximumLength(category.PriceRanges, 400);

            _categoryRespository.Insert(category);

            //cache
            _cacheManager.RemoveByPattern(CATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTCATEGORIES_PATTERN_KEY);
        }

        /// <summary>
        /// Updates the category
        /// </summary>
        /// <param name="category">Category</param>
        public void UpdateCategory(Category category)
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
            category.SeName = CommonHelper.EnsureNotNull(category.SeName);
            category.SeName = CommonHelper.EnsureMaximumLength(category.SeName, 100);
            category.PriceRanges = CommonHelper.EnsureNotNull(category.PriceRanges);
            category.PriceRanges = CommonHelper.EnsureMaximumLength(category.PriceRanges, 400);

            //validate category hierarchy
            var parentCategory = GetCategoryById(category.ParentCategoryId);
            while (parentCategory != null)
            {
                if (category.Id == parentCategory.Id)
                {
                    category.ParentCategoryId = 0;
                    break;
                }
                parentCategory = GetCategoryById(parentCategory.ParentCategoryId);
            }

            _categoryRespository.Update(category);

            //cache
            _cacheManager.RemoveByPattern(CATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTCATEGORIES_PATTERN_KEY);
        }

        /// <summary>
        /// Gets localized category by id
        /// </summary>
        /// <param name="localizedCategoryId">Localized category identifier</param>
        /// <returns>Category content</returns>
        public LocalizedCategory GetLocalizedCategoryById(int localizedCategoryId)
        {
            if (localizedCategoryId == 0)
                return null;
            
            var categoryLocalized = _localizedCategoryRespository.GetById(localizedCategoryId);
            return categoryLocalized;
        }

        /// <summary>
        /// Gets localized category by category id
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <returns>Category content</returns>
        public List<LocalizedCategory> GetLocalizedCategoriesByCategoryId(int categoryId)
        {
            if (categoryId == 0)
                return new List<LocalizedCategory>();
            
            var query = from cl in _localizedCategoryRespository.Table
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
        public LocalizedCategory GetCategoryLocalizedByCategoryIdAndLanguageId(int categoryId, int languageId)
        {
            if (categoryId == 0 || languageId == 0)
                return null;
            
            var query = from cl in _localizedCategoryRespository.Table
                        orderby cl.Id
                        where cl.CategoryId == categoryId &&
                        cl.LanguageId == languageId
                        select cl;
            var categoryLocalized = query.FirstOrDefault();
            return categoryLocalized;
        }

        /// <summary>
        /// Inserts a localized category
        /// </summary>
        /// <param name="localizedCategory">Localized category</param>
        public void InsertLocalizedCategory(LocalizedCategory localizedCategory)
        {
            if (localizedCategory == null)
                throw new ArgumentNullException("localizedCategory");

            localizedCategory.Name = CommonHelper.EnsureNotNull(localizedCategory.Name);
            localizedCategory.Name = CommonHelper.EnsureMaximumLength(localizedCategory.Name, 400);
            localizedCategory.Description = CommonHelper.EnsureNotNull(localizedCategory.Description);
            localizedCategory.MetaKeywords = CommonHelper.EnsureNotNull(localizedCategory.MetaKeywords);
            localizedCategory.MetaKeywords = CommonHelper.EnsureMaximumLength(localizedCategory.MetaKeywords, 400);
            localizedCategory.MetaDescription = CommonHelper.EnsureNotNull(localizedCategory.MetaDescription);
            localizedCategory.MetaDescription = CommonHelper.EnsureMaximumLength(localizedCategory.MetaDescription, 4000);
            localizedCategory.MetaTitle = CommonHelper.EnsureNotNull(localizedCategory.MetaTitle);
            localizedCategory.MetaTitle = CommonHelper.EnsureMaximumLength(localizedCategory.MetaTitle, 400);
            localizedCategory.SeName = CommonHelper.EnsureNotNull(localizedCategory.SeName);
            localizedCategory.SeName = CommonHelper.EnsureMaximumLength(localizedCategory.SeName, 100);

            _localizedCategoryRespository.Insert(localizedCategory);

            //cache
            _cacheManager.RemoveByPattern(CATEGORIES_PATTERN_KEY);
        }

        /// <summary>
        /// Update a localized category
        /// </summary>
        /// <param name="localizedCategory">Localized category</param>
        public void UpdateLocalizedCategory(LocalizedCategory localizedCategory)
        {
            if (localizedCategory == null)
                throw new ArgumentNullException("localizedCategory");

            localizedCategory.Name = CommonHelper.EnsureNotNull(localizedCategory.Name);
            localizedCategory.Name = CommonHelper.EnsureMaximumLength(localizedCategory.Name, 400);
            localizedCategory.Description = CommonHelper.EnsureNotNull(localizedCategory.Description);
            localizedCategory.MetaKeywords = CommonHelper.EnsureNotNull(localizedCategory.MetaKeywords);
            localizedCategory.MetaKeywords = CommonHelper.EnsureMaximumLength(localizedCategory.MetaKeywords, 400);
            localizedCategory.MetaDescription = CommonHelper.EnsureNotNull(localizedCategory.MetaDescription);
            localizedCategory.MetaDescription = CommonHelper.EnsureMaximumLength(localizedCategory.MetaDescription, 4000);
            localizedCategory.MetaTitle = CommonHelper.EnsureNotNull(localizedCategory.MetaTitle);
            localizedCategory.MetaTitle = CommonHelper.EnsureMaximumLength(localizedCategory.MetaTitle, 400);
            localizedCategory.SeName = CommonHelper.EnsureNotNull(localizedCategory.SeName);
            localizedCategory.SeName = CommonHelper.EnsureMaximumLength(localizedCategory.SeName, 100);

            bool allFieldsAreEmpty = string.IsNullOrEmpty(localizedCategory.Name) &&
                                     string.IsNullOrEmpty(localizedCategory.Description) &&
                                     string.IsNullOrEmpty(localizedCategory.MetaKeywords) &&
                                     string.IsNullOrEmpty(localizedCategory.MetaDescription) &&
                                     string.IsNullOrEmpty(localizedCategory.MetaTitle) &&
                                     string.IsNullOrEmpty(localizedCategory.SeName);

            if (allFieldsAreEmpty)
            {
                //delete if all fields are empty
                _localizedCategoryRespository.Delete(localizedCategory);
            }
            else
            {
                _localizedCategoryRespository.Update(localizedCategory);
            }

            //cache
            _cacheManager.RemoveByPattern(CATEGORIES_PATTERN_KEY);
        }

        /// <summary>
        /// Deletes a product category mapping
        /// </summary>
        /// <param name="productCategory">Product category</param>
        public void DeleteProductCategory(ProductCategory productCategory)
        {
            if (productCategory == null)
                return;

            _productCategoryRespository.Delete(productCategory);

            //cache
            _cacheManager.RemoveByPattern(CATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTCATEGORIES_PATTERN_KEY);
        }

        /// <summary>
        /// Gets product category mapping collection
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <returns>Product a category mapping collection</returns>
        public List<ProductCategory> GetProductCategoriesByCategoryId(int categoryId)
        {
            if (categoryId == 0)
                return new List<ProductCategory>();

            //TODO: use bool showHidden = NopContext.Current.IsAdmin;
            bool showHidden = true;

            string key = string.Format(PRODUCTCATEGORIES_ALLBYCATEGORYID_KEY, showHidden, categoryId);
            object obj2 = _cacheManager.Get(key);
            if (obj2 != null)
            {
                return (List<ProductCategory>) obj2;
            }


            var query = from pc in _productCategoryRespository.Table
                        join p in _productRespository.Table on pc.ProductId equals p.Id
                        where pc.CategoryId == categoryId &&
                              !p.Deleted &&
                              (showHidden || p.Published)
                        orderby pc.DisplayOrder
                        select pc;
            var productCategories = query.ToList();

            //cache
            _cacheManager.Add(key, productCategories);

            return productCategories;
        }

        /// <summary>
        /// Gets a product category mapping collection
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <returns>Product category mapping collection</returns>
        public List<ProductCategory> GetProductCategoriesByProductId(int productId)
        {
            if (productId == 0)
                return new List<ProductCategory>();

            //TODO: use bool showHidden = NopContext.Current.IsAdmin;
            bool showHidden = true;

            string key = string.Format(PRODUCTCATEGORIES_ALLBYPRODUCTID_KEY, showHidden, productId);
            object obj2 = _cacheManager.Get(key);
            if (obj2 != null)
            {
                return (List<ProductCategory>) obj2;
            }


            var query = from pc in _productCategoryRespository.Table
                        join c in _categoryRespository.Table on pc.CategoryId equals c.Id
                        where pc.ProductId == productId &&
                              !c.Deleted &&
                              (showHidden || c.Published)
                        orderby pc.DisplayOrder
                        select pc;
            var productCategories = query.ToList();

            //cahe
            _cacheManager.Add(key, productCategories);

            return productCategories;
        }

        /// <summary>
        /// Gets a product category mapping 
        /// </summary>
        /// <param name="productCategoryId">Product category mapping identifier</param>
        /// <returns>Product category mapping</returns>
        public ProductCategory GetProductCategoryById(int productCategoryId)
        {
            if (productCategoryId == 0)
                return null;

            string key = string.Format(PRODUCTCATEGORIES_BY_ID_KEY, productCategoryId);
            object obj2 = _cacheManager.Get(key);
            if (obj2 != null)
            {
                return (ProductCategory)obj2;
            }

            var productCategory = _productCategoryRespository.GetById(productCategoryId);

           //cache
                _cacheManager.Add(key, productCategory);
            
            return productCategory;
        }

        /// <summary>
        /// Inserts a product category mapping
        /// </summary>
        /// <param name="productCategory">>Product category mapping</param>
        public void InsertProductCategory(ProductCategory productCategory)
        {
            if (productCategory == null)
                throw new ArgumentNullException("productCategory");
            
            _productCategoryRespository.Insert(productCategory);

            //cache
            _cacheManager.RemoveByPattern(CATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTCATEGORIES_PATTERN_KEY);
        }

        /// <summary>
        /// Updates the product category mapping 
        /// </summary>
        /// <param name="productCategory">>Product category mapping</param>
        public void UpdateProductCategory(ProductCategory productCategory)
        {
            if (productCategory == null)
                throw new ArgumentNullException("productCategory");

            _productCategoryRespository.Update(productCategory);

            //cache
            _cacheManager.RemoveByPattern(CATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTCATEGORIES_PATTERN_KEY);
        }

        #endregion
    }
}
