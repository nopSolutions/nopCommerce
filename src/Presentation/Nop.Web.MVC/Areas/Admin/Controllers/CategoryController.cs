using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Activation;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Nop.Services.Catalog;
using Nop.Core.Domain.Catalog;
using Nop.Web.Framework.Controllers;
using Nop.Web.MVC.Areas.Admin.Models;
using Telerik.Web.Mvc;
using Telerik.Web.Mvc.Infrastructure;
using Telerik.Web.Mvc.UI;

namespace Nop.Web.MVC.Areas.Admin.Controllers
{
    [AdminAuthorizeAttribute]
    public class CategoryController : Controller
    {
        private ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        #region CRUD

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
            return RedirectToAction("Edit", new {id = category.Id});
        }

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

        public ActionResult List(TelerikGridContextModel gridContext)
        {
            var gridModel = new GridModel<Category>();
            var categories = _categoryService.GetAllCategories(gridContext.PageNumber - 1, gridContext.PageSize, true);
            gridModel.Data = categories;
            gridModel.Total = categories.TotalCount;
            return View(gridModel);
        }

        public ActionResult Delete(int id)
        {
            var category = _categoryService.GetCategoryById(id);
            if (category != null)
            {
                _categoryService.DeleteCategory(category);
            }
            return RedirectToAction("List");
        }

        #endregion

        public ActionResult EditCategoryProducts(int id)
        {
            var model = _categoryService.GetProductCategoriesByCategoryId(id, true).Select(x => new CategoryProductModel(x)).ToList();

            //TODO:Take out test products
            var products = DependencyResolver.Current.GetService<IProductService>().GetAllProducts(true);
            model.Add(new CategoryProductModel {Id = 234, ProductId = products[0].Id});
            return View(model);
        }

        #region Json

        public ActionResult AllCategories(string text, int selectedId)
        {
            Thread.Sleep(1000);
            var categories = _categoryService.GetAllCategories(true);
            categories.Insert(0, new Category {Name = "[None]", Id = 0});
            var selectList = new SelectList(categories, "Id", "Name", selectedId);
            return new JsonResult { Data = selectList, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        #endregion

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

                category = _categoryService.GetCategoryById(category.ParentCategoryId);

            }
            return result;
        }

    }
}
