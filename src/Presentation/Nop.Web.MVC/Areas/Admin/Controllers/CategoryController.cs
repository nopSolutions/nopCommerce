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

        public ActionResult List()
        {
            return View();
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(CategoryModel model)
        {
            return View();
        }

        public ActionResult ExportXml()
        {
            //UNDONE return real Xml file
            string fileName = "categories";
            return File( Encoding.UTF8.GetBytes("some text here"),
                "text/plain",
                string.Format("{0}.txt", fileName));
        }
        
        [HttpPost, ActionName("List")]
        [FormValueRequired("submit.ExportXml")]
        public ActionResult NavigationButtonsPOST()
        {
            return ExportXml();
        }

        [HttpPost]
        public JsonResult TestListGridData(string sidx, string sord, int? page, int? rows)
        {
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows.HasValue ? rows.Value : 10;
            
            var categories = _categoryService.GetAllCategories(pageIndex, pageSize, true);

            int totalRecords = categories.TotalCount;
            int totalPages = categories.TotalPages;

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = (
                    from category in categories
                    select new
                    {
                        i = category.Id,
                        cell = new string[] { category.Id.ToString(), GetCategoryBreadCrumb(category), category.DisplayOrder.ToString() }
                    }).ToArray()
            };

            return Json(jsonData);
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
