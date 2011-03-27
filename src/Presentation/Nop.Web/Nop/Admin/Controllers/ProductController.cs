using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Admin.Models;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Web.Framework.Controllers;
using Telerik.Web.Mvc;
using Telerik.Web.Mvc.Extensions;

namespace Nop.Admin.Controllers
{
    [AdminAuthorize]
    public class ProductController : BaseNopController
    {
		#region Fields 

        private IProductService _productService;
        private IWorkContext _workContext;

        #endregion Fields 

		#region Constructors 

        public ProductController(IProductService productService, IWorkContext workContext)
        {
            _workContext = workContext;
            _productService = productService;
        }

        #endregion Constructors 

        #region List

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            //if (!_permissionService.Authorize(CatalogPermissionProvider.ManageCategories))
            //{
            //    //TODO redirect to access denied page
            //}

            var products = _productService.SearchProducts(0, 0, null, null, null, 0, 0, string.Empty, false,
                                                            _workContext.WorkingLanguage.Id, new List<int>(),
                                                            Core.Domain.Catalog.ProductSortingEnum.Position, 0, 10, true);
            var gridModel = new GridModel<ProductModel>
                                {
                                    Data = products.Select(x => x.ToModel()),
                                    Total = products.TotalCount
                                };
            return View(gridModel);
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult List(GridCommand command)
        {
            var model = new GridModel();
            var products = _productService.SearchProducts(0, 0, null, null, null, 0, 0, string.Empty, false,
                                                          _workContext.WorkingLanguage.Id, new List<int>(),
                                                          Core.Domain.Catalog.ProductSortingEnum.Position,
                                                          command.Page - 1, command.PageSize, true);
            model.Data = products.Select(x => x.ToModel());
            model.Total = products.TotalCount;
            return new JsonResult
            {
                Data = model
            };
        }

        #endregion

		#region Methods 

		#region Public Methods 

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

		#endregion Public Methods 

		#endregion Methods 
    }
}
