using System.Web.Mvc;
using Nop.Core;
using Nop.Services.Catalog;
using System.Linq;
using Nop.Web.Models;

namespace Nop.Web.Controllers
{
    public class CatalogController : Controller
    {
		#region Fields 

        private ICategoryService _categoryService;
        private IProductService _productService;
        private IWorkContext _workContext;

		#endregion Fields 

		#region Constructors 

        public CatalogController(ICategoryService categoryService, IProductService productService, IWorkContext workContext)
        {
            _workContext = workContext;
            _productService = productService;
            _categoryService = categoryService;
        }

		#endregion Constructors 

		#region Methods 

		#region Public Methods 

        public ActionResult Category(int id, PagingFilteringModel command)
        {
            var category = _categoryService.GetCategoryById(id);

            if (category == null) return RedirectToAction("Index", "Home");

            if (command.PageSize <= 0) command.PageSize = category.PageSize;
            if (command.PageNumber <= 0) command.PageNumber = 1;

            var products = _productService.SearchProducts(id, 0, false,
                                                          command.PriceMin, command.PriceMax, 0, 0, string.Empty,
                                                          false,
                                                          _workContext.WorkingLanguage.Id, command.Specs,
                                                          command.ProductSorting,
                                                          command.PageNumber - 1, command.PageSize);

            var model = category.ToModel();
            model.Products = products.Select(x => x.ToModel()).ToList();
            model.PagingFilteringContext.LoadPagedList(products);

            return View(model);
        }

        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

		#endregion Public Methods 

		#endregion Methods 
    }
}
