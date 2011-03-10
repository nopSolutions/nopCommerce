using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Core.Infrastructure;
using Nop.Services;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Security.Permissions;
using Nop.Web.Framework.Controllers;
using Nop.Web.MVC.Areas.Admin.Models;
using Telerik.Web.Mvc;
using Telerik.Web.Mvc.UI;

namespace Nop.Web.MVC.Areas.Admin.Controllers
{
    [AdminAuthorizeAttribute]
    public class CategoryController : BaseNopController<Category, CategoryModel>
    {
        #region Fields (4)

        private readonly ICategoryService _categoryService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IPermissionService _permissionService;

        #endregion Fields

        #region Constructors (1)

        public CategoryController(IEntityService<Category> entityService, IPermissionService permissionService, ILanguageService languageService, ILocalizedEntityService localizedEntityService)
            :base(entityService)
        {
            _categoryService = entityService as CategoryService;
            _permissionService = permissionService;
            _languageService = languageService;
            _localizedEntityService = localizedEntityService;
        }

        #endregion Constructors

        #region Methods (4)

        // Public Methods (4) 

        public ActionResult AllCategories(string text, int selectedId)
        {
            var categories = _categoryService.GetAllCategories(true);
            categories.Insert(0, new Category { Name = "[None]", Id = 0 });
            var selectList = new SelectList(categories, "Id", "Name", selectedId);
            return new JsonResult { Data = selectList, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        //public ActionResult EditCategoryProducts(int id)
        //{
        //    var model = _categoryService.GetProductCategoriesByCategoryId(id, true).Select(x => new CategoryProductModel(x)).ToList();

        //    //TODO:Take out test products
        //    var products = EngineContext.Current.Resolve<IProductService>().GetAllProducts(true);
        //    model.Add(new CategoryProductModel { Id = 234, ProductId = products[0].Id });

        //    return View(model);
        //}

        [NonAction]
        private string GetCategoryBreadCrumb(Category category)
        {
            string result = string.Empty;

            while (category != null && !category.Deleted)
            {
                if (String.IsNullOrEmpty(result))
                    result = category.Name;
                else
                    result = category.Name + " >> " + result;

                category = _categoryService.GetById(category.ParentCategoryId);

            }
            return result;
        }

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        #endregion Methods

        #region List

        public ActionResult List()
        {
            if (!_permissionService.Authorize(CatalogPermissionProvider.ManageCategories))
            {
                //TODO redirect to access denied page
            }

            var categories = _categoryService.GetAllCategories(0, 10, true);
            var gridModel = new GridModel<CategoryModel>
                            {
                                Data = categories.Select(x => new CategoryModel(x, null) { Breadcrumb = GetCategoryBreadCrumb(x) }),
                                Total = categories.TotalCount
                            };
            //var gridModel = new GridModel<Category> { Data = categories, Total = categories.TotalCount };
            return View(gridModel);
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult List(GridCommand command)
        {
            var model = new GridModel();
            var categories = _categoryService.GetAllCategories(command.Page - 1, command.PageSize);
            model.Data = categories.Select(x =>
                new { Id = Url.Action("Edit", new { x.Id }), x.Name, x.DisplayOrder, Breadcrumb = GetCategoryBreadCrumb(x), x.Published });
            model.Total = categories.TotalCount;
            return new JsonResult
            {
                Data = model
            };
        }

        #endregion

        #region Tree

        public ActionResult Tree()
        {
            var rootCategories = _categoryService.GetAllCategoriesByParentCategoryId(0, true);
            return View(rootCategories);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TreeLoadChildren(TreeViewItem node)
        {
            var parentId = !string.IsNullOrEmpty(node.Value) ? Convert.ToInt32(node.Value) : 0;

            var children = _categoryService.GetAllCategoriesByParentCategoryId(parentId).Select(x =>
                new TreeViewItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString(),
                    LoadOnDemand = _categoryService.GetAllCategoriesByParentCategoryId(x.Id).Count > 0,
                    Enabled = true,
                    ImageUrl = Url.Content("~/Areas/Admin/Content/images/ico-content.png")
                });

            return new JsonResult { Data = children };
        }

        public ActionResult TreeDrop(int item, int destinationitem, string position)
        {
            var categoryItem = _categoryService.GetById(item);
            var categoryDestinationItem = _categoryService.GetById(destinationitem);

            switch (position)
            {
                case "over":
                    categoryItem.ParentCategoryId = categoryDestinationItem.Id;
                    break;
                case "before":
                    categoryItem.ParentCategoryId = categoryDestinationItem.ParentCategoryId;
                    categoryItem.DisplayOrder = categoryDestinationItem.DisplayOrder - 1;
                    break;
                case "after":
                    categoryItem.ParentCategoryId = categoryDestinationItem.ParentCategoryId;
                    categoryItem.DisplayOrder = categoryDestinationItem.DisplayOrder + 1;
                    break;
            }

            _categoryService.Update(categoryItem);

            return Json(new { success = true });
        }

        #endregion

        public override CategoryModel CreateModel()
        {
            return new CategoryModel();
        }

        public override CategoryModel CreateModel(Category entity)
        {
            var model = new CategoryModel(entity, _categoryService);
            foreach (var language in _languageService.GetAllLanguages())
            {
                var localizedModel = new CategoryLocalizedModel();
                localizedModel.Language = language;
                localizedModel.Description = entity.GetLocalized(x => x.Description, language.Id, false);
                localizedModel.Name = entity.GetLocalized(x => x.Name, language.Id, false);
                localizedModel.MetaKeywords = entity.GetLocalized(x => x.MetaKeywords, language.Id, false);
                localizedModel.MetaDescription = entity.GetLocalized(x => x.MetaDescription, language.Id, false);
                localizedModel.MetaTitle = entity.GetLocalized(x => x.MetaTitle, language.Id, false);
                localizedModel.SeName = entity.GetLocalized(x => x.SeName, language.Id, false);
                model.Locales.Add(localizedModel);
            }
            return model;
        }

        public override void UpdateEntity(Category category, CategoryModel model)
        {
            category.Name = model.Name;
            category.Description = HttpUtility.HtmlDecode(model.Description);
            category.MetaKeywords = model.MetaKeywords;
            category.MetaDescription = model.MetaDescription;
            category.MetaTitle = model.MetaTitle;
            category.SeName = model.SeName;
            category.ParentCategoryId = model.ParentCategoryId;
            category.PictureId = model.PictureId;
            category.PageSize = model.PageSize;
            category.PriceRanges = model.PriceRanges;
            category.ShowOnHomePage = model.ShowOnHomePage;
            category.Published = model.Published;
            category.Deleted = model.Deleted;
            category.DisplayOrder = model.DisplayOrder;
        }

        public override void EntitySavedOrUpdated(Category entity, CategoryModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(entity,
                                                               x => x.Name,
                                                               localized.Name,
                                                               localized.Language.Id);

                _localizedEntityService.SaveLocalizedValue(entity,
                                                           x => x.Description,
                                                           HttpUtility.HtmlDecode(localized.Description),
                                                           localized.Language.Id);

                _localizedEntityService.SaveLocalizedValue(entity,
                                                           x => x.MetaKeywords,
                                                           localized.MetaKeywords,
                                                           localized.Language.Id);

                _localizedEntityService.SaveLocalizedValue(entity,
                                                           x => x.MetaDescription,
                                                           localized.MetaDescription,
                                                           localized.Language.Id);

                _localizedEntityService.SaveLocalizedValue(entity,
                                                           x => x.MetaTitle,
                                                           localized.MetaTitle,
                                                           localized.Language.Id);

                _localizedEntityService.SaveLocalizedValue(entity,
                                                           x => x.SeName,
                                                           localized.SeName,
                                                           localized.Language.Id);
            }
            base.EntitySavedOrUpdated(entity, model);
        }
    }
}
