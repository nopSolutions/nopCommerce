using System;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Core.Infrastructure;
using Nop.Data;
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
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IPermissionService _permissionService;

        public CategoryController(ICategoryService categoryService,
            IPermissionService permissionService)
        {
            _categoryService = categoryService;
            _permissionService = permissionService;
        }

       
        #region Create

        public ActionResult Create()
        {
            return View(new CategoryModel());
        }

        [HttpPost]
        public ActionResult Create(CategoryModel model)
        {
            var category = new Category();
            model.Update(category);
            _categoryService.InsertCategory(category);
            return RedirectToAction("Edit", new { id = category.Id });
        }

        #endregion

        #region Edit

        public ActionResult Edit(int id)
        {
            var category = _categoryService.GetCategoryById(id);
            if (category == null) throw new ArgumentException("No category found with the specified id", "id");
            var model = new CategoryModel(category, _categoryService);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(CategoryModel categoryModel)
        {
            var category = _categoryService.GetCategoryById(categoryModel.Id);
            categoryModel.Update(category);
            _categoryService.UpdateCategory(category);
            return Edit(category.Id);
        }

        #endregion

        #region List

        public ActionResult List()
        {
            if (!_permissionService.Authorize(CatalogPermissionProvider.ManageCategories))
            {
                //TODO redirect to access denied page
            }

            var categories = _categoryService.GetAllCategories(0, 10, true);
            var gridModel = new GridModel<Category> { Data = categories, Total = categories.TotalCount };
            return View(gridModel);
        }

        [HttpPost, GridAction(EnableCustomBinding=true)]
        public ActionResult List(GridCommand command)
        {
            var model = new GridModel();
            var categories = _categoryService.GetAllCategories(command.Page - 1, command.PageSize);
            model.Data = categories.Select(x =>
                new { Id = Url.Action("Edit", new {x.Id }), x.Name, x.DisplayOrder });
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
            var categoryItem = _categoryService.GetCategoryById(item);
            var categoryDestinationItem = _categoryService.GetCategoryById(destinationitem);

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

            _categoryService.UpdateCategory(categoryItem);

            return Json(new { success = true });
        }

        #endregion

        public ActionResult Delete(int id)
        {
            var category = _categoryService.GetCategoryById(id);
            if (category != null)
            {
                _categoryService.DeleteCategory(category);
            }
            return RedirectToAction("List");
        }

        public ActionResult EditCategoryProducts(int id)
        {
            var model = _categoryService.GetProductCategoriesByCategoryId(id, true).Select(x => new CategoryProductModel(x)).ToList();

            //TODO:Take out test products
            var products = EngineContext.Current.Resolve<IProductService>().GetAllProducts(true);
            model.Add(new CategoryProductModel {Id = 234, ProductId = products[0].Id});

            return View(model);
        }
     
        public ActionResult AllCategories(string text, int selectedId)
        {
            Thread.Sleep(1000);
            var categories = _categoryService.GetAllCategories(true);
            categories.Insert(0, new Category {Name = "[None]", Id = 0});
            var selectList = new SelectList(categories, "Id", "Name", selectedId);
            return new JsonResult { Data = selectList, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }
}
