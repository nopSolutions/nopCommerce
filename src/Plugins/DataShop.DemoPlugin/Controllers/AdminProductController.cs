using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Services.Catalog;

namespace DataShop.DemoPlugin.Controllers
{
    public class AdminProductController : Controller
    {
        private readonly IProductService ProductService;

        public AdminProductController(IProductService productService)
        {
            this.ProductService = productService;
        }

        public ActionResult Edit(int id)
        {
            var product = ProductService.GetProductById(id);
            return View(product);
        }
    }
}