using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Events;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class ProductReviewController : BaseAdminController
{
    #region Fields

    protected readonly CatalogSettings _catalogSettings;
    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly ICustomerService _customerService;
    protected readonly IEventPublisher _eventPublisher;
    protected readonly ILocalizationService _localizationService;
    protected readonly INotificationService _notificationService;
    protected readonly IPermissionService _permissionService;
    protected readonly IProductReviewModelFactory _productReviewModelFactory;
    protected readonly IProductService _productService;
    protected readonly IWorkContext _workContext;
    protected readonly IWorkflowMessageService _workflowMessageService;

    #endregion Fields

    #region Ctor

    public ProductReviewController(CatalogSettings catalogSettings,
        ICustomerActivityService customerActivityService,
        ICustomerService customerService,
        IEventPublisher eventPublisher,
        ILocalizationService localizationService,
        INotificationService notificationService,
        IPermissionService permissionService,
        IProductReviewModelFactory productReviewModelFactory,
        IProductService productService,
        IWorkContext workContext,
        IWorkflowMessageService workflowMessageService)
    {
        _catalogSettings = catalogSettings;
        _customerActivityService = customerActivityService;
        _customerService = customerService;
        _eventPublisher = eventPublisher;
        _localizationService = localizationService;
        _notificationService = notificationService;
        _permissionService = permissionService;
        _productReviewModelFactory = productReviewModelFactory;
        _productService = productService;
        _workContext = workContext;
        _workflowMessageService = workflowMessageService;
    }

    #endregion

    #region Methods

    public virtual IActionResult Index()
    {
        return RedirectToAction("List");
    }

    [CheckPermission(StandardPermission.Catalog.PRODUCT_REVIEWS_VIEW)]
    public virtual async Task<IActionResult> List()
    {
        //prepare model
        var model = await _productReviewModelFactory.PrepareProductReviewSearchModelAsync(new ProductReviewSearchModel());

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCT_REVIEWS_VIEW)]
    public virtual async Task<IActionResult> List(ProductReviewSearchModel searchModel)
    {
        //prepare model
        var model = await _productReviewModelFactory.PrepareProductReviewListModelAsync(searchModel);

        return Json(model);
    }

    [CheckPermission(StandardPermission.Catalog.PRODUCT_REVIEWS_VIEW)]
    public virtual async Task<IActionResult> Edit(int id)
    {
        //try to get a product review with the specified id
        var productReview = await _productService.GetProductReviewByIdAsync(id);
        if (productReview == null)
            return RedirectToAction("List");

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null && (await _productService.GetProductByIdAsync(productReview.ProductId)).VendorId != currentVendor.Id)
            return RedirectToAction("List");

        //prepare model
        var model = await _productReviewModelFactory.PrepareProductReviewModelAsync(null, productReview);

        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Catalog.PRODUCT_REVIEWS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Edit(ProductReviewModel model, bool continueEditing)
    {
        //try to get a product review with the specified id
        var productReview = await _productService.GetProductReviewByIdAsync(model.Id);
        if (productReview == null)
            return RedirectToAction("List");

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null && (await _productService.GetProductByIdAsync(productReview.ProductId)).VendorId != currentVendor.Id)
            return RedirectToAction("List");

        if (ModelState.IsValid)
        {
            var previousIsApproved = productReview.IsApproved;

            //vendor can edit "Reply text" only
            var isLoggedInAsVendor = currentVendor != null;
            if (!isLoggedInAsVendor)
            {
                productReview.Title = model.Title;
                productReview.ReviewText = model.ReviewText;
                productReview.IsApproved = model.IsApproved;
            }

            productReview.ReplyText = model.ReplyText;

            //notify customer about reply
            if (productReview.IsApproved &&
                !string.IsNullOrEmpty(productReview.ReplyText) &&
                _catalogSettings.NotifyCustomerAboutProductReviewReply && !productReview.CustomerNotifiedOfReply)
            {
                var customer = await _customerService.GetCustomerByIdAsync(productReview.CustomerId);
                var customerLanguageId = customer?.LanguageId ?? 0;

                var queuedEmailIds = await _workflowMessageService.SendProductReviewReplyCustomerNotificationMessageAsync(productReview, customerLanguageId);
                if (queuedEmailIds.Any())
                    productReview.CustomerNotifiedOfReply = true;
            }

            await _productService.UpdateProductReviewAsync(productReview);

            //activity log
            await _customerActivityService.InsertActivityAsync("EditProductReview",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditProductReview"), productReview.Id), productReview);

            //vendor can edit "Reply text" only
            if (!isLoggedInAsVendor)
            {
                var product = await _productService.GetProductByIdAsync(productReview.ProductId);
                //update product totals
                await _productService.UpdateProductReviewTotalsAsync(product);

                //raise event (only if it wasn't approved before and is approved now)
                if (!previousIsApproved && productReview.IsApproved)
                    await _eventPublisher.PublishAsync(new ProductReviewApprovedEvent(productReview));
            }

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.ProductReviews.Updated"));

            return continueEditing ? RedirectToAction("Edit", new { id = productReview.Id }) : RedirectToAction("List");
        }

        //prepare model
        model = await _productReviewModelFactory.PrepareProductReviewModelAsync(model, productReview, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCT_REVIEWS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Delete(int id)
    {
        //try to get a product review with the specified id
        var productReview = await _productService.GetProductReviewByIdAsync(id);
        if (productReview == null)
            return RedirectToAction("List");

        //a vendor does not have access to this functionality
        if (await _workContext.GetCurrentVendorAsync() != null)
            return RedirectToAction("List");

        await _productService.DeleteProductReviewAsync(productReview);

        //activity log
        await _customerActivityService.InsertActivityAsync("DeleteProductReview",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteProductReview"), productReview.Id), productReview);

        var product = await _productService.GetProductByIdAsync(productReview.ProductId);

        //update product totals
        await _productService.UpdateProductReviewTotalsAsync(product);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.ProductReviews.Deleted"));

        return RedirectToAction("List");
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCT_REVIEWS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ApproveSelected(ICollection<int> selectedIds)
    {
        //a vendor does not have access to this functionality
        if (await _workContext.GetCurrentVendorAsync() != null)
            return RedirectToAction("List");

        if (selectedIds == null || !selectedIds.Any())
            return NoContent();

        //filter not approved reviews
        var productReviews = (await _productService.GetProductReviewsByIdsAsync(selectedIds.ToArray())).Where(review => !review.IsApproved);

        foreach (var productReview in productReviews)
        {
            productReview.IsApproved = true;
            await _productService.UpdateProductReviewAsync(productReview);

            var product = await _productService.GetProductByIdAsync(productReview.ProductId);

            //update product totals
            await _productService.UpdateProductReviewTotalsAsync(product);

            //raise event 
            await _eventPublisher.PublishAsync(new ProductReviewApprovedEvent(productReview));
        }

        return Json(new { Result = true });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCT_REVIEWS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> DisapproveSelected(ICollection<int> selectedIds)
    {
        //a vendor does not have access to this functionality
        if (await _workContext.GetCurrentVendorAsync() != null)
            return RedirectToAction("List");

        if (selectedIds == null || !selectedIds.Any())
            return NoContent();

        //filter approved reviews
        var productReviews = (await _productService.GetProductReviewsByIdsAsync(selectedIds.ToArray())).Where(review => review.IsApproved);

        foreach (var productReview in productReviews)
        {
            productReview.IsApproved = false;
            await _productService.UpdateProductReviewAsync(productReview);

            var product = await _productService.GetProductByIdAsync(productReview.ProductId);

            //update product totals
            await _productService.UpdateProductReviewTotalsAsync(product);
        }

        return Json(new { Result = true });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCT_REVIEWS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> DeleteSelected(ICollection<int> selectedIds)
    {
        //a vendor does not have access to this functionality
        if (await _workContext.GetCurrentVendorAsync() != null)
            return RedirectToAction("List");

        if (selectedIds == null || !selectedIds.Any())
            return NoContent();

        var productReviews = await _productService.GetProductReviewsByIdsAsync(selectedIds.ToArray());
        var products = await _productService.GetProductsByIdsAsync(productReviews.Select(p => p.ProductId).Distinct().ToArray());

        await _productService.DeleteProductReviewsAsync(productReviews);

        //activity log
        var activityLogFormat = await _localizationService.GetResourceAsync("ActivityLog.DeleteProductReview");

        foreach (var productReview in productReviews)
            await _customerActivityService.InsertActivityAsync("DeleteProductReview",
                string.Format(activityLogFormat, productReview.Id), productReview);

        //update product totals
        foreach (var product in products)
        {
            await _productService.UpdateProductReviewTotalsAsync(product);
        }

        return Json(new { Result = true });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCT_REVIEWS_VIEW)]
    public virtual async Task<IActionResult> ProductReviewReviewTypeMappingList(ProductReviewReviewTypeMappingSearchModel searchModel)
    {
        var productReview = await _productService.GetProductReviewByIdAsync(searchModel.ProductReviewId)
            ?? throw new ArgumentException("No product review found with the specified id");

        //prepare model
        var model = await _productReviewModelFactory.PrepareProductReviewReviewTypeMappingListModelAsync(searchModel, productReview);

        return Json(model);
    }

    #endregion
}