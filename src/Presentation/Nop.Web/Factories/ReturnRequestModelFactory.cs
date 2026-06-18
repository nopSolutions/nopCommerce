using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Tax;
using Nop.Core.Http;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Seo;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Models.Order;

namespace Nop.Web.Factories;

/// <summary>
/// Represents the return request model factory
/// </summary>
public partial class ReturnRequestModelFactory : IReturnRequestModelFactory
{
    #region Fields

    protected readonly CaptchaSettings _captchaSettings;
    protected readonly ICurrencyService _currencyService;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly IDownloadService _downloadService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IOrderService _orderService;
    protected readonly IPriceFormatter _priceFormatter;
    protected readonly IProductService _productService;
    protected readonly IReturnRequestService _returnRequestService;
    protected readonly IStoreContext _storeContext;
    protected readonly INopUrlHelper _nopUrlHelper;
    protected readonly IUrlRecordService _urlRecordService;
    protected readonly IWorkContext _workContext;
    protected readonly ReturnRequestSettings _returnRequestSettings;

    #endregion

    #region Ctor

    public ReturnRequestModelFactory(CaptchaSettings captchaSettings,
        ICurrencyService currencyService,
        IDateTimeHelper dateTimeHelper,
        IDownloadService downloadService,
        ILocalizationService localizationService,
        IOrderService orderService,
        IPriceFormatter priceFormatter,
        IProductService productService,
        IReturnRequestService returnRequestService,
        IStoreContext storeContext,
        INopUrlHelper nopUrlHelper,
        IUrlRecordService urlRecordService,
        IWorkContext workContext,
        ReturnRequestSettings returnRequestSettings)
    {
        _captchaSettings = captchaSettings;
        _currencyService = currencyService;
        _dateTimeHelper = dateTimeHelper;
        _downloadService = downloadService;
        _localizationService = localizationService;
        _orderService = orderService;
        _priceFormatter = priceFormatter;
        _productService = productService;
        _returnRequestService = returnRequestService;
        _storeContext = storeContext;
        _nopUrlHelper = nopUrlHelper;
        _urlRecordService = urlRecordService;
        _workContext = workContext;
        _returnRequestSettings = returnRequestSettings;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare the submit return request model
    /// </summary>
    /// <param name="model">Submit return request model</param>
    /// <param name="order">Order</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the submit return request model
    /// </returns>
    public virtual async Task<SubmitReturnRequestModel> PrepareSubmitReturnRequestModelAsync(SubmitReturnRequestModel model,
        Order order)
    {
        ArgumentNullException.ThrowIfNull(order);

        ArgumentNullException.ThrowIfNull(model);

        model.OrderId = order.Id;
        model.AllowFiles = _returnRequestSettings.ReturnRequestsAllowFiles;
        model.CustomOrderNumber = order.CustomOrderNumber;
        model.ReturnActionsEnabled = _returnRequestSettings.ReturnActionsEnabled;
        model.ReturnReasonsEnabled = _returnRequestSettings.ReturnReasonsEnabled;

        model.ReturnRequestPageTitle = _returnRequestSettings.UseEuWithdrawalLocales ?
            await _localizationService.GetResourceAsync("PageTitle.ReturnItems.Withdrawal") :
            await _localizationService.GetResourceAsync("PageTitle.ReturnItems");

        var titlePattern = _returnRequestSettings.UseEuWithdrawalLocales ?
            await _localizationService.GetResourceAsync("ReturnRequests.Withdrawal.Title") :
            await _localizationService.GetResourceAsync("ReturnRequests.Title");

        model.ReturnRequestTitle = string.Format(titlePattern,
            _nopUrlHelper.RouteUrl(NopRouteNames.Standard.ORDER_DETAILS, new { orderId = order.Id }),
            order.CustomOrderNumber);

        model.ReturnRequestSubmitText = _returnRequestSettings.UseEuWithdrawalLocales ?
            await _localizationService.GetResourceAsync("ReturnRequests.Withdrawal.Submit") :
            await _localizationService.GetResourceAsync("ReturnRequests.Submit");

        //return reasons
        model.AvailableReturnReasons = await (await _returnRequestService.GetAllReturnRequestReasonsAsync())
            .SelectAwait(async rrr => new SubmitReturnRequestModel.ReturnRequestReasonModel
            {
                Id = rrr.Id,
                Name = await _localizationService.GetLocalizedAsync(rrr, x => x.Name)
            }).ToListAsync();

        //return actions
        model.AvailableReturnActions = await (await _returnRequestService.GetAllReturnRequestActionsAsync())
            .SelectAwait(async rra => new SubmitReturnRequestModel.ReturnRequestActionModel
            {
                Id = rra.Id,
                Name = await _localizationService.GetLocalizedAsync(rra, x => x.Name)
            })
            .ToListAsync();

        //returnable products
        model.Items = await PrepareSubmitReturnRequestOrderItemModelsAsync(order);

        return model;
    }

    /// <summary>
    /// Prepare the customer return requests model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer return requests model
    /// </returns>
    public virtual async Task<CustomerReturnRequestsModel> PrepareCustomerReturnRequestsModelAsync()
    {
        var model = new CustomerReturnRequestsModel
        {
            ReturnRequestsTitle = _returnRequestSettings.UseEuWithdrawalLocales ?
                await _localizationService.GetResourceAsync("Account.CustomerReturnRequests.Withdrawals") :
                await _localizationService.GetResourceAsync("Account.CustomerReturnRequests"),
            ReturnRequestTitlePattern = _returnRequestSettings.UseEuWithdrawalLocales ?
                await _localizationService.GetResourceAsync("Account.CustomerReturnRequests.Withdrawal.Title") :
                await _localizationService.GetResourceAsync("Account.CustomerReturnRequests.Title"),
        };

        var store = await _storeContext.GetCurrentStoreAsync();
        var customer = await _workContext.GetCurrentCustomerAsync();
        var returnRequests = await _returnRequestService.SearchReturnRequestsAsync(store.Id, customer.Id);

        foreach (var returnRequest in returnRequests)
        {
            var orderItem = await _orderService.GetOrderItemByIdAsync(returnRequest.OrderItemId);
            if (orderItem != null)
            {
                var product = await _productService.GetProductByIdAsync(orderItem.ProductId);

                var download = await _downloadService.GetDownloadByIdAsync(returnRequest.UploadedFileId);

                var itemModel = new CustomerReturnRequestsModel.ReturnRequestModel
                {
                    Id = returnRequest.Id,
                    CustomNumber = returnRequest.CustomNumber,
                    ReturnRequestStatus = await _localizationService.GetLocalizedEnumAsync(returnRequest.ReturnRequestStatus),
                    ProductId = product.Id,
                    ProductName = await _localizationService.GetLocalizedAsync(product, x => x.Name),
                    ProductSeName = await _urlRecordService.GetSeNameAsync(product),
                    Quantity = returnRequest.Quantity,
                    ReturnAction = _returnRequestSettings.ReturnActionsEnabled ? returnRequest.RequestedAction : "not available",
                    ReturnReason = _returnRequestSettings.ReturnReasonsEnabled ? returnRequest.ReasonForReturn : "not available",
                    Comments = returnRequest.CustomerComments,
                    UploadedFileGuid = download?.DownloadGuid ?? Guid.Empty,
                    CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(returnRequest.CreatedOnUtc, DateTimeKind.Utc),
                };
                model.Items.Add(itemModel);
            }
        }

        return model;
    }

    /// <summary>
    /// Prepare the withdrawal form model
    /// </summary>
    /// <returns>
    /// <param name="model">Withdrawal form model</param>
    /// A task that represents the asynchronous operation
    /// The task result contains the withdrawal form model
    /// </returns>
    public virtual Task<WithdrawalFormModel> PrepareWithdrawalFormModelAsync(WithdrawalFormModel model)
    {
        model ??= new WithdrawalFormModel();

        model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnWithdrawalForm;

        return Task.FromResult(model);
    }

    /// <summary>
    /// Prepares the order item models for return request by specified order.
    /// </summary>
    /// <param name="order">Order</param>
    /// <returns>
    /// The <see cref="Task"/> containing the <see cref="IList{SubmitReturnRequestModel.OrderItemModel}"/>
    /// </returns>
    protected virtual async Task<IList<SubmitReturnRequestModel.OrderItemModel>> PrepareSubmitReturnRequestOrderItemModelsAsync(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);

        var models = new List<SubmitReturnRequestModel.OrderItemModel>();

        var returnRequestAvailability = await _returnRequestService.GetReturnRequestAvailabilityAsync(order.Id);
        if (returnRequestAvailability?.IsAllowed == true)
        {
            foreach (var returnableOrderItem in returnRequestAvailability.ReturnableOrderItems)
            {
                if (returnableOrderItem.AvailableQuantityForReturn == 0)
                    continue;

                var orderItem = returnableOrderItem.OrderItem;
                var product = await _productService.GetProductByIdAsync(orderItem.ProductId);

                var model = new SubmitReturnRequestModel.OrderItemModel
                {
                    Id = orderItem.Id,
                    ProductId = product.Id,
                    ProductName = await _localizationService.GetLocalizedAsync(product, x => x.Name),
                    ProductSeName = await _urlRecordService.GetSeNameAsync(product),
                    AttributeInfo = orderItem.AttributeDescription,
                    Quantity = returnableOrderItem.AvailableQuantityForReturn
                };

                var languageId = (await _workContext.GetWorkingLanguageAsync()).Id;

                //unit price
                if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
                {
                    //including tax
                    var unitPriceInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.UnitPriceInclTax, order.CurrencyRate);
                    model.UnitPrice = await _priceFormatter.FormatPriceAsync(unitPriceInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, true);
                }
                else
                {
                    //excluding tax
                    var unitPriceExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.UnitPriceExclTax, order.CurrencyRate);
                    model.UnitPrice = await _priceFormatter.FormatPriceAsync(unitPriceExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, false);
                }

                models.Add(model);
            }
        }

        return models;
    }

    #endregion
}