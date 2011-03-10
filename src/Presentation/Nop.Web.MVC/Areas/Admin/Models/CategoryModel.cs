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
    public class CategoryModel : BaseNopEntityModel<Category>, ILocalizedModel<CategoryLocalizedModel>
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

        public CategoryModel(Category category, ICategoryService categoryService)
            :this()
        {
            Id = category.Id;
            Name = category.Name;
            Description = category.Description;
            MetaKeywords = category.MetaKeywords;
            MetaDescription = category.MetaDescription;
            MetaTitle = category.MetaTitle;
            SeName = category.SeName;
            ParentCategoryId = category.ParentCategoryId;
            if (categoryService != null)
            {
                if (ParentCategoryId > 0) ParentCategory = categoryService.GetById(category.ParentCategoryId);
            }
            PictureId = category.PictureId;
            PageSize = category.PageSize;
            PageSize = category.PageSize;
            PriceRanges = category.PriceRanges;
            ShowOnHomePage = category.ShowOnHomePage;
            Published = category.Published;
            Deleted = category.Deleted;
            DisplayOrder = category.DisplayOrder;
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string MetaTitle { get; set; }
        public string SeName { get; set; }
        public int ParentCategoryId { get; set; }
        public Category ParentCategory {get;set;}
        [UIHint("Picture")]
        public int PictureId { get; set; }
        public int PageSize { get; set; }
        public string PriceRanges { get; set; }
        public bool ShowOnHomePage { get; set; }
        public bool Published { get; set; }
        public bool Deleted { get; set; }
        public int DisplayOrder { get; set; }
        public IList<CategoryLocalizedModel> Locales { get; set; }
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
        public string Name { get; set; }
        public string Description {get;set;}
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string MetaTitle { get; set; }
        public string SeName { get; set; }
    }
}