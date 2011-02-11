using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(CategoryModel model)
        {
            return View();
        }

        public ActionResult Edit(int id)
        {
            return View();
        }

        [HttpPost]
        public ActionResult Edit(CategoryModel categoryModel)
        {
            return View();
        }

        public ActionResult List(Nop.Web.Framework.TelerikGridContext gridContext)
        {
            var gridModel = new GridModel<Category>();
            var categories = _categoryService.GetAllCategories(gridContext.PageNumber - 1, gridContext.PageSize, true);
            gridModel.Data = categories;
            gridModel.Total = categories.TotalCount;
            return View(gridModel);
        }

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
