using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Admin.Validators;
using Nop.Core.Domain.Catalog;
using Nop.Web.Framework;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc;
using Telerik.Web.Mvc.UI;


namespace Nop.Admin.Models
{
    [Validator(typeof(CategoryValidator))]
    public class CategoryModel : BaseNopEntityModel, ILocalizedModel<CategoryLocalizedModel>
    {
        public CategoryModel()
        {
            if (PageSize < 1)
            {
                PageSize = 5;
            }
            Locales = new List<CategoryLocalizedModel>();
        }

        #region Model

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.Description")]
        [AllowHtml]
        public string Description { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.MetaKeywords")]
        public string MetaKeywords { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.MetaDescription")]
        public string MetaDescription { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.MetaTitle")]
        public string MetaTitle { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.SeName")]
        public string SeName { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.Parent")]
        public int ParentCategoryId { get; set; }

        [UIHint("Picture")]
        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.Picture")]
        public int PictureId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.PageSize")]
        public int PageSize { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.PriceRanges")]
        public string PriceRanges { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.ShowOnHomePage")]
        public bool ShowOnHomePage { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.Published")]
        public bool Published { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.Deleted")]
        public bool Deleted { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        public IList<CategoryLocalizedModel> Locales { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.Breadcrumb")]
        public string Breadcrumb { get; set; }

        #endregion

        #region Nested classes
        
        [Validator(typeof(CategoryProductValidator))]
        public class CategoryProductModel : IEquatable<CategoryProductModel>
        {
            [UIHint("ProductSelector")]
            [NopResourceDisplayName("Admin.Catalog.Categories.Products.Fields.Product")]
            public int ProductId { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Categories.Products.Fields.ProductName")]
            public string ProductName { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Categories.Products.Fields.Category")]
            public int CategoryId { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Categories.Products.Fields.IsFeaturedProduct")]
            public bool IsFeaturedProduct { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Categories.Products.Fields.DisplayOrder")]
            public int DisplayOrder { get; set; }

            public override bool Equals(object obj)
            {
                var productCategory = obj as ProductCategory;
                if (productCategory != null)
                {
                    return Equals(productCategory);
                }
                return false;
            }

            public bool Equals(CategoryProductModel other)
            {
                return other.ProductId.Equals(ProductId);
            }
        }
        
        public class CategoryProductsAttribute : FilterAttribute, IActionFilter
        {
            public void OnActionExecuted(ActionExecutedContext filterContext)
            {

            }

            public void OnActionExecuting(ActionExecutingContext filterContext)
            {
                IStatefulStorage storage = StatefulStorage.PerSession;
                filterContext.ActionParameters["addedCategoryProducts"] = Added();
                filterContext.ActionParameters["removedCategoryProducts"] = Removed();
            }

            public static IList<CategoryModel.CategoryProductModel> Added()
            {
                return StatefulStorage.PerSession.GetOrAdd<IList<CategoryModel.CategoryProductModel>>("category-products-added",
                                                                                        () =>
                                                                                        new List<CategoryModel.CategoryProductModel>());
            }

            public static IList<CategoryModel.CategoryProductModel> Removed()
            {
                return StatefulStorage.PerSession.GetOrAdd<IList<CategoryModel.CategoryProductModel>>("category-products-removed",
                                                                                        () =>
                                                                                        new List<CategoryModel.CategoryProductModel>());
            }

            public static void Add(CategoryModel.CategoryProductModel categoryProduct)
            {
                Removed().Remove(categoryProduct);
                var added = Added();
                var current = added.SingleOrDefault(x => x.ProductId.Equals(categoryProduct.ProductId));
                if (current == null)
                {
                    added.Add(categoryProduct);
                }
                else
                {
                    current.DisplayOrder = categoryProduct.DisplayOrder;
                    current.IsFeaturedProduct = categoryProduct.IsFeaturedProduct;
                }
            }

            public static void Remove(CategoryModel.CategoryProductModel categoryProduct)
            {
                var removed = Removed();
                if (!removed.Contains(categoryProduct)) removed.Add(categoryProduct);
                Added().Remove(categoryProduct);
            }

            public static void Clear()
            {
                Added().Clear();
                Removed().Clear();
            }

            public static List<CategoryModel.CategoryProductModel> MakeStateful(List<CategoryModel.CategoryProductModel> categoryProducts)
            {
                var added = Added();
                var removed = Removed();

                foreach (var categoryProduct in categoryProducts)
                {
                    var productId = categoryProduct.ProductId;
                    //If the products exist in the added list, then it was updated and we should update the existing category product
                    var updated = added.SingleOrDefault(x => x.ProductId.Equals(productId));
                    if (updated != null)
                    {
                        categoryProduct.DisplayOrder = updated.DisplayOrder;
                        categoryProduct.IsFeaturedProduct = updated.IsFeaturedProduct;
                    }
                }
                foreach (var toAdd in added)
                {
                    if (!categoryProducts.Contains(toAdd)) categoryProducts.Add(toAdd);
                }
                foreach (var toRemove in removed)
                {
                    categoryProducts.Remove(toRemove);
                }
                return categoryProducts.OrderBy(x => x.DisplayOrder).ToList();
            }
        }

        #endregion

        public IList<DropDownItem> ParentCategories { get; set; }
    }

    public class CategoryLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.Description")]
        [AllowHtml]
        public string Description {get;set;}

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.MetaKeywords")]
        public string MetaKeywords { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.MetaDescription")]
        public string MetaDescription { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.MetaTitle")]
        public string MetaTitle { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.SeName")]
        public string SeName { get; set; }
    }
}