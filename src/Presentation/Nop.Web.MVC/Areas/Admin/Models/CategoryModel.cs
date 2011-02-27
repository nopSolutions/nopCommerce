using System.Collections.Generic;
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
    public class CategoryModel : BaseNopModel
    {
        public CategoryModel()
        {
            if (PageSize < 1)
            {
                PageSize = 5;
            }
            Localized = new LocalizedModels<CategoryLocalizedModel>();
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
                if (ParentCategoryId > 0) ParentCategory = categoryService.GetCategoryById(category.ParentCategoryId);
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

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string MetaTitle { get; set; }
        public string SeName { get; set; }
        public int ParentCategoryId { get; set; }
        public Category ParentCategory {get;set;}
        public int PictureId { get; set; }
        public int PageSize { get; set; }
        public string PriceRanges { get; set; }
        public bool ShowOnHomePage { get; set; }
        public bool Published { get; set; }
        public bool Deleted { get; set; }
        public int DisplayOrder { get; set; }
        public LocalizedModels<CategoryLocalizedModel> Localized { get; set; }
        public string Breadcrumb { get; set; }

        #endregion

        public override void BindModel(System.Web.Mvc.ControllerContext controllerContext, System.Web.Mvc.ModelBindingContext bindingContext)
        {
            foreach (var item in controllerContext.RequestContext.HttpContext.Request.Form.AllKeys)
            {
            }
        }

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

    public class CategoryLocalizedModel : ILocalizedModel
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