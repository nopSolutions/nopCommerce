using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Services.Catalog;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Web.Framework.Security;
using Nop.Web.Models.Common;
using Nop.Web.Models.MyProductReviews;

namespace Nop.Web.Controllers
{
    [NopHttpsRequirement(SslRequirement.Yes)]
    public partial class MyProductReviewsController : BasePublicController
    {
        #region Fields

        private readonly IProductService _productService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly IDateTimeHelper _dateTimeHelper;

        #endregion

        #region Constructors

        public MyProductReviewsController(IProductService productService,
            ILocalizationService localizationService, IWorkContext workContext,
            IDateTimeHelper dateTimeHelper)
        {
            this._productService = productService;
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._dateTimeHelper = dateTimeHelper;
        }

        #endregion

        #region Utilities

        #endregion

        #region Methods

        public ActionResult Index(int? page)
        {
            if (_workContext.CurrentCustomer.IsGuest())
            {
                return new HttpUnauthorizedResult();
            }
            if (page > 0)
            {
                page -= 1;
            }

            var pageSize = 20;

            var list = _productService
                .GetAllProductReviews(_workContext.CurrentCustomer.Id, null, pageSize: pageSize, pageIndex: page.GetValueOrDefault(0));

            var productReviews = new List<MyProductReviewsModel>();

            foreach (var review in list)
            {
                productReviews.Add(new MyProductReviewsModel()
                {
                    ProductName = review.Product.Name,
                    DateReviewed = _dateTimeHelper.ConvertToUserTime(review.CreatedOnUtc, DateTimeKind.Utc),
                    Id = review.Id,
                    CurrentApprovalStatus = review.IsApproved 
                                                ? _localizationService.GetResource("MyProductReviews.ApprovalStatus.Approved") 
                                                :_localizationService.GetResource("MyProductReviews.ApprovalStatus.Pending")
                });
            }

            var pagerModel = new PagerModel
            {
                PageSize = list.PageSize,
                TotalRecords = list.TotalCount,
                PageIndex = list.PageIndex,
                ShowTotalSummary = false,
                RouteActionName = "MyProductReviewsPaged",
                UseRouteLinks = true,
                RouteValues = new MyProductReviewsRouteValues { page = page ?? 0 }
            };

            var model = new MyProductReviewsListModel
            {
                ProductReviews = productReviews,
                PagerModel = pagerModel
            };
            
            return View(model);
        }

        #endregion
    }
}
