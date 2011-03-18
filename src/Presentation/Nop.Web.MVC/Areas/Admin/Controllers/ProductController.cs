using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Telerik.Web.Mvc.Extensions;

namespace Nop.Web.MVC.Areas.Admin.Controllers
{
    public class ProductController : BaseNopController
    {
        private IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpPost]
        public ActionResult _AjaxComboBo(string text)
        {
            var products = _productService.GetAllProducts(true).Where(x => x.Name.ToLower().Contains(text.ToLower()));

            return new JsonResult
            {
                Data = new SelectList(products.ToList(), "Id",
                    "Name")
            };
        }


    }
}
