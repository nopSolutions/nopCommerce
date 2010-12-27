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
using Nop.Core.Localization;
using System.Web;

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

        private readonly IWorkingContext _context;
        private readonly ILocalizedEntityService _leService;
        private readonly IRepository<Category> _categoryRespository;
        private readonly IRepository<ProductCategory> _productCategoryRespository;
        private readonly IRepository<Product> _productRespository;
        private readonly ICacheManager _cacheManager;

        #endregion
        
        #region Ctor
        
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="context">Working context</param>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="leService">Localized entity service</param>
        /// <param name="categoryRespository">Category repository</param>
        /// <param name="productCategoryRespository">ProductCategory repository</param>
        /// <param name="productRespository">Product repository</param>
        public CategoryService(IWorkingContext context,
            ICacheManager cacheManager,
            ILocalizedEntityService leService,
            IRepository<Category> categoryRespository,
            IRepository<ProductCategory> productCategoryRespository,
            IRepository<Product> productRespository)
        {
            this._context = context;
            this._cacheManager = cacheManager;
            this._leService = leService;
            this._categoryRespository = categoryRespository;
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
            bool showHidden = _context.IsAdmin;
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

            sortedCategories.ForEach(c => 
                new DefaultPropertyLocalizer<Category, LocalizedCategory>(_leService, c).Localize());
            return sortedCategories;
        }
        
        /// <summary>
        /// Gets all categories by parent category identifier
        /// </summary>
        /// <param name="parentCategoryId">Parent category identifier</param>
        /// <returns>Category collection</returns>
        public List<Category> GetAllCategoriesByParentCategoryId(int parentCategoryId)
        {
            bool showHidden = _context.IsAdmin;
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
            categories.ForEach(c => new DefaultPropertyLocalizer<Category, LocalizedCategory>(_leService, c).Localize());
            return categories;
        }
        
        /// <summary>
        /// Gets all categories displayed on the home page
        /// </summary>
        /// <returns>Category collection</returns>
        public List<Category> GetAllCategoriesDisplayedOnHomePage()
        {
            bool showHidden = _context.IsAdmin;
            
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
            categories.ForEach(c => new DefaultPropertyLocalizer<Category, LocalizedCategory>(_leService, c).Localize());
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
            return _cacheManager.Get(key, () =>
            {
                var category = _categoryRespository.GetById(categoryId);
                //filter by access control list (public store)
                //if (category != null && !showHidden && IsCategoryAccessDenied(category))
                //{
                //    category = null;
                //}
                new DefaultPropertyLocalizer<Category, LocalizedCategory>(_leService, category).Localize();
                return category;
            });
        }

        /// <summary>
        /// Inserts category
        /// </summary>
        /// <param name="category">Category</param>
        public void InsertCategory(Category category)
        {
            if (category == null)
                throw new ArgumentNullException("category");

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

            bool showHidden = _context.IsAdmin;

            string key = string.Format(PRODUCTCATEGORIES_ALLBYCATEGORYID_KEY, showHidden, categoryId);
            return _cacheManager.Get(key, () =>
            {
                var query = from pc in _productCategoryRespository.Table
                            join p in _productRespository.Table on pc.ProductId equals p.Id
                            where pc.CategoryId == categoryId &&
                                  !p.Deleted &&
                                  (showHidden || p.Published)
                            orderby pc.DisplayOrder
                            select pc;
                var productCategories = query.ToList();
                return productCategories;
            });
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

            bool showHidden = _context.IsAdmin;

            string key = string.Format(PRODUCTCATEGORIES_ALLBYPRODUCTID_KEY, showHidden, productId);
            return _cacheManager.Get(key, () =>
            {
                var query = from pc in _productCategoryRespository.Table
                            join c in _categoryRespository.Table on pc.CategoryId equals c.Id
                            where pc.ProductId == productId &&
                                  !c.Deleted &&
                                  (showHidden || c.Published)
                            orderby pc.DisplayOrder
                            select pc;
                var productCategories = query.ToList();
                return productCategories;
            });
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
            return _cacheManager.Get(key, () =>
            {
                return _productCategoryRespository.GetById(productCategoryId);
            });
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
