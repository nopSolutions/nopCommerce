using System;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Nop.Services.Blogs;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Forums;
using Nop.Services.Localization;
using Nop.Services.News;
using Nop.Services.Seo;
using Nop.Services.Topics;

namespace Nop.Web.Controllers
{
    public partial class BackwardCompatibility2XController : BaseNopController
    {
		#region Fields

        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IManufacturerService _manufacturerService;
        #endregion

		#region Constructors

        public BackwardCompatibility2XController(IProductService productService,
            ICategoryService categoryService, IManufacturerService manufacturerService)
        {
            this._productService = productService;
            this._categoryService = categoryService;
            this._manufacturerService = manufacturerService;
        }

		#endregion
        
        #region Methods

        //in versions 2.00-2.65 we had typo in producttag URLs ("productag" instead of "producttag")
        public ActionResult RedirectProductsByTag(int productTagId, string seName)
        {
            return RedirectToRoutePermanent("ProductsByTag", new { productTagId = productTagId, SeName = seName });
        }
        //in versions 2.00-2.65 we had typo in producttag URLs ("productag" instead of "producttag")
        public ActionResult RedirectProductTagsAll()
        {
            return RedirectToRoutePermanent("ProductTagsAll");
        }


        //in versions 2.00-2.65 we had ID in product URLs
        public ActionResult RedirectProductById(int productId)
        {
            var product = _productService.GetProductById(productId);
            if (product == null)
                return RedirectToRoutePermanent("HomePage");

            return RedirectToRoutePermanent("Product", new { SeName = product.GetSeName() });
        }
        //in versions 2.00-2.65 we had ID in category URLs
        public ActionResult RedirectCategoryById(int categoryId)
        {
            var category = _categoryService.GetCategoryById(categoryId);
            if (category == null)
                return RedirectToRoutePermanent("HomePage");

            return RedirectToRoutePermanent("Category", new { SeName = category.GetSeName() });
        }
        //in versions 2.00-2.65 we had ID in product URLs
        public ActionResult RedirectManufacturerById(int manufacturerId)
        {
            var manufacturer = _manufacturerService.GetManufacturerById(manufacturerId);
            if (manufacturer == null)
                return RedirectToRoutePermanent("HomePage");

            return RedirectToRoutePermanent("Manufacturer", new { SeName = manufacturer.GetSeName() });
        }

        #endregion
    }
}
