using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using FluentValidation.Attributes;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Web.Framework;
using Nop.Web.Framework.Localization;
using Nop.Web.MVC.Areas.Admin.Validators;
using Telerik.Web.Mvc.UI;


namespace Nop.Web.MVC.Areas.Admin.Models
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

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.Parent")]
        public Category ParentCategory {get;set;}

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

        public IList<DropDownItem> ParentCategories
        {
            get
            {
                var parentCategories = new List<DropDownItem> {new DropDownItem {Text = "[None]", Value = "0"}};
                if (ParentCategory != null)
                {
                    parentCategories.Add(new DropDownItem
                                             {Text = ParentCategory.Name, Value = ParentCategory.Id.ToString()});
                }
                return parentCategories;
            }
        }
    }

    public class CategoryLocalizedModel : ILocalizedModelLocal
    {
        public Core.Domain.Localization.Language Language { get; set; }

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