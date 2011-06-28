using System;
using System.Linq;
using System.Web.Mvc;
using Nop.Admin.Models.Catalog;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Telerik.Web.Mvc;
using Nop.Services.Security;

namespace Nop.Admin.Controllers
{
    [AdminAuthorize]
    public class ProductReviewController : BaseNopController
    {
        #region Fields

        private readonly ICustomerContentService _customerContentService;
        private readonly IProductService _productService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;

        #endregion Fields

        #region Constructors

        public ProductReviewController(ICustomerContentService customerContentService,
            IProductService productService, IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService, IPermissionService permissionService)
        {
            this._customerContentService = customerContentService;
            this._productService = productService;
            this._dateTimeHelper = dateTimeHelper;
            this._localizationService = localizationService;
            this._permissionService = permissionService;
        }

        #endregion

        #region Utilities

        [NonAction]
        private void PrepareProductReviewModel(ProductReviewModel model,
            ProductReview productReview, bool excludeProperties, bool formatReviewText)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (productReview == null)
                throw new ArgumentNullException("productReview");

            model.Id = productReview.Id;
            model.ProductId = productReview.ProductId;
            model.ProductName = productReview.Product.Name;
            model.CustomerId = productReview.CustomerId;
            model.IpAddress = productReview.IpAddress;
            model.Rating = productReview.Rating;
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(productReview.CreatedOnUtc, DateTimeKind.Utc);
            if (!excludeProperties)
            {
                model.Title = productReview.Title;
                if (formatReviewText)
                    model.ReviewText = Core.Html.HtmlHelper.FormatText(productReview.ReviewText, false, true, false, false, false, false);
                else
                    model.ReviewText = productReview.ReviewText;
                model.IsApproved = productReview.IsApproved;
            }
        }

        #endregion

        #region Methods

        //list
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var gridModel = new GridModel<ProductReviewModel>();
            return View(gridModel);
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult List(GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var productReviews = _customerContentService.GetAllCustomerContent<ProductReview>(0, null);
            var gridModel = new GridModel<ProductReviewModel>
            {
                Data = productReviews.PagedForCommand(command).Select(x =>
                {
                    var m = new ProductReviewModel();
                    PrepareProductReviewModel(m, x, false, true);
                    return m;
                }),
                Total = productReviews.Count,
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        //edit
        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var productReview = _customerContentService.GetCustomerContentById(id) as ProductReview;
            if (productReview == null)
                throw new ArgumentException("No product review found with the specified id", "id");

            var model = new ProductReviewModel();
            PrepareProductReviewModel(model, productReview, false, false);
            return View(model);
        }

        [HttpPost, FormValueExists("save", "save-continue", "continueEditing")]
        public ActionResult Edit(ProductReviewModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var productReview = _customerContentService.GetCustomerContentById(model.Id) as ProductReview;
            if (productReview == null)
                throw new ArgumentException("No product review found with the specified id");

            if (ModelState.IsValid)
            {
                productReview.Title = model.Title;
                productReview.ReviewText = model.ReviewText;
                productReview.IsApproved = model.IsApproved;
                productReview.UpdatedOnUtc = DateTime.UtcNow;
                _customerContentService.UpdateCustomerContent(productReview);
                
                //update product totals
                _productService.UpdateProductReviewTotals(productReview.Product);

                SuccessNotification(_localizationService.GetResource("Admin.Catalog.ProductReviews.Updated"));
                return continueEditing ? RedirectToAction("Edit", productReview.Id) : RedirectToAction("List");
            }


            //If we got this far, something failed, redisplay form
            PrepareProductReviewModel(model, productReview, true, false);
            return View(model);
        }

        //delete
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var productReview = _customerContentService.GetCustomerContentById(id) as ProductReview;
            if (productReview == null)
                throw new ArgumentException("No product review found with the specified id");

            var product = productReview.Product;
            _customerContentService.DeleteCustomerContent(productReview);
            //update product totals
            _productService.UpdateProductReviewTotals(product);

            SuccessNotification(_localizationService.GetResource("Admin.Catalog.ProductReviews.Deleted"));
            return RedirectToAction("List");
        }

        #endregion
    }
}
