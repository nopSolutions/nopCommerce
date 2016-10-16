using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Directory;
using Nop.Services.Common;
using Nop.Services.Localization;
using Nop.Services.Directory;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Services.Catalog;
using Nop.Core;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Security;
using DataShop.DemoPlugin.Domain;
using DataShop.DemoPlugin.Services;
using Nop.Core.Domain.Catalog;

namespace DataShop.DemoPlugin.Controllers
{
    public class AdminDemoItemsController : BasePluginController
    {
        private readonly IProductService ProductService;
        private readonly IWorkContext WorkContext;
        private readonly IDemoService DemoService;

        public AdminDemoItemsController(IProductService productService, IWorkContext workContext, IDemoService demoService)
        {
            this.ProductService = productService;
            this.WorkContext = workContext;
            this.DemoService = demoService;
        }

        // GET: AdminDemoItems
        public ActionResult Index()
        {
            var items = DemoService.GetItems();
            return View(items);
        }

        // GET: AdminDemoItem
        public ActionResult GetDemoItem(int Id)
        {
            //Read from the product service
            Product product = ProductService.GetProductById(Id);
            DemoItem item = null;

            //If the product exists we will add it
            if (product != null)
            {
                //Setup the product
                var record = new DemoItem();
                record.ProductId = Id;
                record.ProductName = product.Name;
                record.CustomerId = WorkContext.CurrentCustomer.Id;
                record.IpAddress = WorkContext.CurrentCustomer.LastIpAddress;
                record.IsRegistered = WorkContext.CurrentCustomer.Active;

                item = record;

                ////Map the values we're interested in to our new entity
                //DemoService.AddItem(record);
            }

            return View(item);
        }

    }
}