using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Tax;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Seo;
using Nop.Web.Models.Order;

namespace Nop.Web.Factories;

/// <summary>
/// Represents the return request model factory
/// </summary>
public partial class ReturnRequestModelFactory : IReturnRequestModelFactory
{
    #region Fields

    protected readonly ICurrencyService _currencyService;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly IDownloadService _downloadService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IOrderService _orderService;
    protected readonly IPriceFormatter _priceFormatter;
    protected readonly IProductService _productService;
    protected readonly IReturnRequestService _returnRequestService;
    protected readonly IStoreContext _storeContext;
    protected readonly IUrlRecordService _urlRecordService;
    protected readonly IWorkContext _workContext;
    protected readonly OrderSettings _orderSettings;

    #endregion

    #region Ctor

    public ReturnRequestModelFactory(ICurrencyService currencyService,
        IDateTimeHelper dateTimeHelper,
        IDownloadService downloadService,
        ILocalizationService localizationService,
        IOrderService orderService,
        IPriceFormatter priceFormatter,
        IProductService productService,
        IReturnRequestService returnRequestService,
        IStoreContext storeContext,
        IUrlRecordService urlRecordService,
        IWorkContext workContext,
        OrderSettings orderSettings)
    {
        _currencyService = currencyService;
        _dateTimeHelper = dateTimeHelper;
        _downloadService = downloadService;
        _localizationService = localizationService;
        _orderService = orderService;
        _priceFormatter = priceFormatter;
        _productService = productService;
        _returnRequestService = returnRequestService;
        _storeContext = storeContext;
        _urlRecordService = urlRecordService;
        _workContext = workContext;
        _orderSettings = orderSettings;
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
        model.AllowFiles = _orderSettings.ReturnRequestsAllowFiles;
        model.CustomOrderNumber = order.CustomOrderNumber;

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
        var model = new CustomerReturnRequestsModel();
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
                    ReturnAction = returnRequest.RequestedAction,
                    ReturnReason = returnRequest.ReasonForReturn,
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