using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Nop.Admin.Models.Catalog;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Services.Catalog;
using Nop.Services.Events;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;

namespace Nop.Admin.Controllers
{
    public partial class ProductReviewController : BaseAdminController
    {
        #region Fields

        private readonly IProductService _productService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IStoreService _storeService;

        #endregion Fields

        #region Constructors

        public ProductReviewController(IProductService productService, 
            IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService, 
            IPermissionService permissionService,
            IEventPublisher eventPublisher,
            IStoreService storeService)
        {
            this._productService = productService;
            this._dateTimeHelper = dateTimeHelper;
            this._localizationService = localizationService;
            this._permissionService = permissionService;
            this._eventPublisher = eventPublisher;
            _storeService = storeService;
        }

        #endregion

        #region Utilities

        [NonAction]
        protected virtual void PrepareProductReviewModel(ProductReviewModel model,
            ProductReview productReview, bool excludeProperties, bool formatReviewText)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (productReview == null)
                throw new ArgumentNullException("productReview");

            model.Id = productReview.Id;
            model.StoreName = productReview.Store.Name;
            model.ProductId = productReview.ProductId;
            model.ProductName = productReview.Product.Name;
            model.CustomerId = productReview.CustomerId;
            var customer = productReview.Customer;
            model.CustomerInfo = customer.IsRegistered() ? customer.Email : _localizationService.GetResource("Admin.Customers.Guest");
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
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProductReviews))
                return AccessDeniedView();

            var model = new ProductReviewListModel();
            model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var stores = _storeService.GetAllStores().Select(st => new SelectListItem() { Text = st.Name, Value = st.Id.ToString() });
            foreach (var selectListItem in stores)
                model.AvailableStores.Add(selectListItem);
            return View(model);
        }

        [HttpPost]
        public ActionResult List(DataSourceRequest command, ProductReviewListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProductReviews))
                return AccessDeniedView();

            DateTime? createdOnFromValue = (model.CreatedOnFrom == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.CreatedOnFrom.Value, _dateTimeHelper.CurrentTimeZone);

            DateTime? createdToFromValue = (model.CreatedOnTo == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.CreatedOnTo.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            var productReviews = _productService.GetAllProductReviews(0, null, 
                createdOnFromValue, createdToFromValue, model.SearchText, model.SearchStoreId, model.SearchProductId, command.Page - 1, command.PageSize);
            var gridModel = new DataSourceResult
            {
                Data = productReviews.Select(x =>
                {
                    var m = new ProductReviewModel();
                    PrepareProductReviewModel(m, x, false, true);
                    return m;
                }),
                Total = productReviews.TotalCount
            };

            return Json(gridModel);
        }

        //edit
        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProductReviews))
                return AccessDeniedView();

            var productReview = _productService.GetProductReviewById(id);
            if (productReview == null)
                //No product review found with the specified id
                return RedirectToAction("List");

            var model = new ProductReviewModel();
            PrepareProductReviewModel(model, productReview, false, false);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Edit(ProductReviewModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProductReviews))
                return AccessDeniedView();

            var productReview = _productService.GetProductReviewById(model.Id);
            if (productReview == null)
                //No product review found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                productReview.Title = model.Title;
                productReview.ReviewText = model.ReviewText;
                productReview.IsApproved = model.IsApproved;
                _productService.UpdateProduct(productReview.Product);
                
                //update product totals
                _productService.UpdateProductReviewTotals(productReview.Product);

                SuccessNotification(_localizationService.GetResource("Admin.Catalog.ProductReviews.Updated"));
                return continueEditing ? RedirectToAction("Edit", new { id = productReview.Id}) : RedirectToAction("List");
            }


            //If we got this far, something failed, redisplay form
            PrepareProductReviewModel(model, productReview, true, false);
            return View(model);
        }
        
        //delete
        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProductReviews))
                return AccessDeniedView();

            var productReview = _productService.GetProductReviewById(id);
            if (productReview == null)
                //No product review found with the specified id
                return RedirectToAction("List");

            var product = productReview.Product;
            _productService.DeleteProductReview(productReview);
            //update product totals
            _productService.UpdateProductReviewTotals(product);

            SuccessNotification(_localizationService.GetResource("Admin.Catalog.ProductReviews.Deleted"));
            return RedirectToAction("List");
        }

        [HttpPost]
        public ActionResult ApproveSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProductReviews))
                return AccessDeniedView();

            if (selectedIds != null)
            {
                var productReviews = _productService.GetProducReviewsByIds(selectedIds.ToArray());
                foreach (var productReview in productReviews)
                {
                    var previousIsApproved = productReview.IsApproved;
                    productReview.IsApproved = true;
                    _productService.UpdateProduct(productReview.Product);
                    //update product totals
                    _productService.UpdateProductReviewTotals(productReview.Product);


                    //raise event (only if it wasn't approved before)
                    if (!previousIsApproved)
                        _eventPublisher.Publish(new ProductReviewApprovedEvent(productReview));
                }
            }

            return Json(new { Result = true });
        }

        [HttpPost]
        public ActionResult DisapproveSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProductReviews))
                return AccessDeniedView();

            if (selectedIds != null)
            {
                var productReviews = _productService.GetProducReviewsByIds(selectedIds.ToArray());
                foreach (var productReview in productReviews)
                {
                    productReview.IsApproved = false;
                    _productService.UpdateProduct(productReview.Product);
                    //update product totals
                    _productService.UpdateProductReviewTotals(productReview.Product);
                }
            }

            return Json(new { Result = true });
        }

        [HttpPost]
        public ActionResult DeleteSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProductReviews))
                return AccessDeniedView();

            if (selectedIds != null)
            {
                var productReviews = _productService.GetProducReviewsByIds(selectedIds.ToArray());
                var products = _productService.GetProductsByIds(productReviews.Select(p => p.ProductId).Distinct().ToArray());

                _productService.DeleteProductReviews(productReviews);

                //update product totals
                foreach (var product in products)
                {
                    _productService.UpdateProductReviewTotals(product);
                }
            }

            return Json(new { Result = true });
        }

        public ActionResult ProductSearchAutoComplete(string term)
        {
            const int searchTermMinimumLength = 3;
            if (String.IsNullOrWhiteSpace(term) || term.Length < searchTermMinimumLength)
                return Content("");

            //products
            const int productNumber = 15;
            var products = _productService.SearchProducts(
                keywords: term,
                pageSize: productNumber,
                showHidden: true);

            var result = (from p in products
                select new
                {
                    label = p.Name,
                    productid = p.Id
                })
                .ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}
