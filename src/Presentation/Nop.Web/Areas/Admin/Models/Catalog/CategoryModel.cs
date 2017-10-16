using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FluentValidation.Attributes;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Areas.Admin.Validators.Catalog;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Web.Areas.Admin.Models.Catalog
{
    [Validator(typeof(CategoryValidator))]
    public partial class CategoryModel : BaseNopEntityModel, ILocalizedModel<CategoryLocalizedModel>
    {
        public CategoryModel()
        {
            if (PageSize < 1)
            {
                PageSize = 5;
            }
            Locales = new List<CategoryLocalizedModel>();
            AvailableCategoryTemplates = new List<SelectListItem>();
            AvailableCategories = new List<SelectListItem>();
            AvailableDiscounts = new List<SelectListItem>();
            SelectedDiscountIds = new List<int>();

            SelectedCustomerRoleIds = new List<int>();
            AvailableCustomerRoles = new List<SelectListItem>();

            SelectedStoreIds = new List<int>();
            AvailableStores = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.Description")]
        public string Description { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.CategoryTemplate")]
        public int CategoryTemplateId { get; set; }
        public IList<SelectListItem> AvailableCategoryTemplates { get; set; }

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

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.AllowCustomersToSelectPageSize")]
        public bool AllowCustomersToSelectPageSize { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.PageSizeOptions")]
        public string PageSizeOptions { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.PriceRanges")]
        public string PriceRanges { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.ShowOnHomePage")]
        public bool ShowOnHomePage { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.IncludeInTopMenu")]
        public bool IncludeInTopMenu { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.Published")]
        public bool Published { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.Deleted")]
        public bool Deleted { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }
        
        public IList<CategoryLocalizedModel> Locales { get; set; }

        public string Breadcrumb { get; set; }

        //ACL (customer roles)
        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.AclCustomerRoles")]
        public IList<int> SelectedCustomerRoleIds { get; set; }
        public IList<SelectListItem> AvailableCustomerRoles { get; set; }
        
        //store mapping
        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.LimitedToStores")]
        public IList<int> SelectedStoreIds { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }

        public IList<SelectListItem> AvailableCategories { get; set; }

        //discounts
        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.Discounts")]
        public IList<int> SelectedDiscountIds { get; set; }
        public IList<SelectListItem> AvailableDiscounts { get; set; }

        #region Nested classes

        public partial class CategoryProductModel : BaseNopEntityModel
        {
            public int CategoryId { get; set; }

            public int ProductId { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Categories.Products.Fields.Product")]
            public string ProductName { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Categories.Products.Fields.IsFeaturedProduct")]
            public bool IsFeaturedProduct { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Categories.Products.Fields.DisplayOrder")]
            public int DisplayOrder { get; set; }
        }

        public partial class AddCategoryProductModel : BaseNopModel
        {
            public AddCategoryProductModel()
            {
                AvailableCategories = new List<SelectListItem>();
                AvailableManufacturers = new List<SelectListItem>();
                AvailableStores = new List<SelectListItem>();
                AvailableVendors = new List<SelectListItem>();
                AvailableProductTypes = new List<SelectListItem>();
            }

            [NopResourceDisplayName("Admin.Catalog.Products.List.SearchProductName")]
            public string SearchProductName { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Products.List.SearchCategory")]
            public int SearchCategoryId { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Products.List.SearchManufacturer")]
            public int SearchManufacturerId { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Products.List.SearchStore")]
            public int SearchStoreId { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Products.List.SearchVendor")]
            public int SearchVendorId { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Products.List.SearchProductType")]
            public int SearchProductTypeId { get; set; }

            public IList<SelectListItem> AvailableCategories { get; set; }
            public IList<SelectListItem> AvailableManufacturers { get; set; }
            public IList<SelectListItem> AvailableStores { get; set; }
            public IList<SelectListItem> AvailableVendors { get; set; }
            public IList<SelectListItem> AvailableProductTypes { get; set; }

            public int CategoryId { get; set; }

            public int[] SelectedProductIds { get; set; }
        }

        #endregion
    }

    public partial class CategoryLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.Description")]
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