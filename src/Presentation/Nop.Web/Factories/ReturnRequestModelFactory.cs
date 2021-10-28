using System;
using System.Linq;
using System.Threading.Tasks;
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

namespace Nop.Web.Factories
{
    /// <summary>
    /// Represents the return request model factory
    /// </summary>
    public partial class ReturnRequestModelFactory : IReturnRequestModelFactory
    {
        #region Fields

        protected ICurrencyService CurrencyService { get; }
        protected IDateTimeHelper DateTimeHelper { get; }
        protected IDownloadService DownloadService { get; }
        protected ILocalizationService LocalizationService { get; }
        protected IOrderService OrderService { get; }
        protected IPriceFormatter PriceFormatter { get; }
        protected IProductService ProductService { get; }
        protected IReturnRequestService ReturnRequestService { get; }
        protected IStoreContext StoreContext { get; }
        protected IUrlRecordService UrlRecordService { get; }
        protected IWorkContext WorkContext { get; }
        protected OrderSettings OrderSettings { get; }

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
            CurrencyService = currencyService;
            DateTimeHelper = dateTimeHelper;
            DownloadService = downloadService;
            LocalizationService = localizationService;
            OrderService = orderService;
            PriceFormatter = priceFormatter;
            ProductService = productService;
            ReturnRequestService = returnRequestService;
            StoreContext = storeContext;
            UrlRecordService = urlRecordService;
            WorkContext = workContext;
            OrderSettings = orderSettings;
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
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (model == null)
                throw new ArgumentNullException(nameof(model));

            model.OrderId = order.Id;
            model.AllowFiles = OrderSettings.ReturnRequestsAllowFiles;
            model.CustomOrderNumber = order.CustomOrderNumber;

            //return reasons
            model.AvailableReturnReasons = await (await ReturnRequestService.GetAllReturnRequestReasonsAsync())
                .SelectAwait(async rrr => new SubmitReturnRequestModel.ReturnRequestReasonModel
                {
                    Id = rrr.Id,
                    Name = await LocalizationService.GetLocalizedAsync(rrr, x => x.Name)
                }).ToListAsync();

            //return actions
            model.AvailableReturnActions = await (await ReturnRequestService.GetAllReturnRequestActionsAsync())
                .SelectAwait(async rra => new SubmitReturnRequestModel.ReturnRequestActionModel
                {
                    Id = rra.Id,
                    Name = await LocalizationService.GetLocalizedAsync(rra, x => x.Name)
                })
                .ToListAsync();

            //returnable products
            var orderItems = await OrderService.GetOrderItemsAsync(order.Id, isNotReturnable: false);
            foreach (var orderItem in orderItems)
            {
                var orderItemModel = await PrepareSubmitReturnRequestOrderItemModelAsync(orderItem);
                model.Items.Add(orderItemModel);
            }

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
            var store = await StoreContext.GetCurrentStoreAsync();
            var customer = await WorkContext.GetCurrentCustomerAsync();
            var returnRequests = await ReturnRequestService.SearchReturnRequestsAsync(store.Id, customer.Id);
            
            foreach (var returnRequest in returnRequests)
            {
                var orderItem = await OrderService.GetOrderItemByIdAsync(returnRequest.OrderItemId);
                if (orderItem != null)
                {
                    var product = await ProductService.GetProductByIdAsync(orderItem.ProductId);

                    var download = await DownloadService.GetDownloadByIdAsync(returnRequest.UploadedFileId);

                    var itemModel = new CustomerReturnRequestsModel.ReturnRequestModel
                    {
                        Id = returnRequest.Id,
                        CustomNumber = returnRequest.CustomNumber,
                        ReturnRequestStatus = await LocalizationService.GetLocalizedEnumAsync(returnRequest.ReturnRequestStatus),
                        ProductId = product.Id,
                        ProductName = await LocalizationService.GetLocalizedAsync(product, x => x.Name),
                        ProductSeName = await UrlRecordService.GetSeNameAsync(product),
                        Quantity = returnRequest.Quantity,
                        ReturnAction = returnRequest.RequestedAction,
                        ReturnReason = returnRequest.ReasonForReturn,
                        Comments = returnRequest.CustomerComments,
                        UploadedFileGuid = download?.DownloadGuid ?? Guid.Empty,
                        CreatedOn = await DateTimeHelper.ConvertToUserTimeAsync(returnRequest.CreatedOnUtc, DateTimeKind.Utc),
                    };
                    model.Items.Add(itemModel);
                }
            }

            return model;
        }

        /// <summary>
        /// Prepare the order item model
        /// </summary>
        /// <param name="orderItem">Order item</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the order item model
        /// </returns>
        public virtual async Task<SubmitReturnRequestModel.OrderItemModel> PrepareSubmitReturnRequestOrderItemModelAsync(OrderItem orderItem)
        {
            if (orderItem == null)
                throw new ArgumentNullException(nameof(orderItem));

            var order = await OrderService.GetOrderByIdAsync(orderItem.OrderId);
            var product = await ProductService.GetProductByIdAsync(orderItem.ProductId);

            var model = new SubmitReturnRequestModel.OrderItemModel
            {
                Id = orderItem.Id,
                ProductId = product.Id,
                ProductName = await LocalizationService.GetLocalizedAsync(product, x => x.Name),
                ProductSeName = await UrlRecordService.GetSeNameAsync(product),
                AttributeInfo = orderItem.AttributeDescription,
                Quantity = orderItem.Quantity
            };

            var languageId = (await WorkContext.GetWorkingLanguageAsync()).Id;

            //unit price
            if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
            {
                //including tax
                var unitPriceInclTaxInCustomerCurrency = CurrencyService.ConvertCurrency(orderItem.UnitPriceInclTax, order.CurrencyRate);
                model.UnitPrice = await PriceFormatter.FormatPriceAsync(unitPriceInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, true);
            }
            else
            {
                //excluding tax
                var unitPriceExclTaxInCustomerCurrency = CurrencyService.ConvertCurrency(orderItem.UnitPriceExclTax, order.CurrencyRate);
                model.UnitPrice = await PriceFormatter.FormatPriceAsync(unitPriceExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, false);
            }

            return model;
        }

        #endregion
    }
}