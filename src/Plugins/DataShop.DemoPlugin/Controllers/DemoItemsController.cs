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
    public class DemoItemsController : BasePluginController
    {
        private readonly IDemoService DemoService;

        public DemoItemsController(IDemoService demoService)
        {
            this.DemoService = demoService;
        }

        // GET: DemoItems
        public ActionResult Index()
        {
            var items = DemoService.GetItems();
            return View(items);
        }

        public ActionResult TestView()
        {
            var items = DemoService.GetItems();
            return PartialView(items);
        }

    }
}