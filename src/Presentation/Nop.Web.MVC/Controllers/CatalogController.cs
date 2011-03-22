using System.Web.Mvc;
using Nop.Core;
using Nop.Services.Catalog;
using Nop.Web.MVC.Extensions;
using Nop.Web.MVC.Models;
using Nop.Web.MVC.Models.Catalog;
using System.Linq;

namespace Nop.Web.MVC.Controllers
{
    public class CatalogController : Controller
    {
        private ICategoryService _categoryService;
        private IProductService _productService;
        private IWorkContext _workContext;

        public CatalogController(ICategoryService categoryService, IProductService productService, IWorkContext workContext)
        {
            _workContext = workContext;
            _productService = productService;
            _categoryService = categoryService;
        }

        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        [PagingFilteringCommand]
        public ActionResult Category(int id, PagingFilteringCommand command)
        {
            var category = _categoryService.GetCategoryById(id);
            if(category == null)
            {
                return RedirectToAction("Index", "Home");
            }
            command.PageSize = category.PageSize;
            var model = category.To<CatalogCategoryModel>();
            model.Products = _productService.SearchProducts(id, 0, false,
                                                          command.PriceMin, command.PriceMax, 0, 0, string.Empty,
                                                          false,
                                                          _workContext.WorkingLanguage.Id, command.Specs,
                                                          command.ProductSorting,
                                                          command.PageIndex, command.PageSize)
                                                          .Select(x => x.To<CatalogProductModel>())
                                                          .ToList();

            return View(model);
        }
    }
}
