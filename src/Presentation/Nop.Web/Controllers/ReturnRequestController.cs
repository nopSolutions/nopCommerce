using System;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Tax;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Seo;
using Nop.Web.Framework.Security;
using Nop.Web.Models.Order;

namespace Nop.Web.Controllers
{
    public partial class ReturnRequestController : BasePublicController
    {
		#region Fields

        private readonly IReturnRequestService _returnRequestService;
        private readonly IOrderService _orderService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ICurrencyService _currencyService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerService _customerService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IDateTimeHelper _dateTimeHelper;

        private readonly LocalizationSettings _localizationSettings;
        private readonly OrderSettings _orderSettings;

        #endregion

		#region Constructors

        public ReturnRequestController(IReturnRequestService returnRequestService,
            IOrderService orderService, 
            IWorkContext workContext, 
            IStoreContext storeContext,
            ICurrencyService currencyService, 
            IPriceFormatter priceFormatter,
            IOrderProcessingService orderProcessingService,
            ILocalizationService localizationService,
            ICustomerService customerService,
            IWorkflowMessageService workflowMessageService,
            IDateTimeHelper dateTimeHelper,
            LocalizationSettings localizationSettings,
            OrderSettings orderSettings)
        {
            this._returnRequestService = returnRequestService;
            this._orderService = orderService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._currencyService = currencyService;
            this._priceFormatter = priceFormatter;
            this._orderProcessingService = orderProcessingService;
            this._localizationService = localizationService;
            this._customerService = customerService;
            this._workflowMessageService = workflowMessageService;
            this._dateTimeHelper = dateTimeHelper;

            this._localizationSettings = localizationSettings;
            this._orderSettings = orderSettings;
        }

        #endregion

        #region Utilities

        [NonAction]
        protected virtual SubmitReturnRequestModel PrepareReturnRequestModel(SubmitReturnRequestModel model, Order order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            if (model == null)
                throw new ArgumentNullException("model");

            model.OrderId = order.Id;

            //return reasons
            if (_orderSettings.ReturnRequestReasons != null)
                foreach (var rrr in _orderSettings.ReturnRequestReasons)
                {
                    model.AvailableReturnReasons.Add(new SelectListItem
                        {
                            Text = rrr,
                            Value = rrr
                        });
                }

            //return actions
            if (_orderSettings.ReturnRequestActions != null)
                foreach (var rra in _orderSettings.ReturnRequestActions)
                {
                    model.AvailableReturnActions.Add(new SelectListItem
                    {
                        Text = rra,
                        Value = rra
                    });
                }

            //products
            var orderItems = _orderService.GetAllOrderItems(order.Id, null, null, null, null, null, null);
            foreach (var orderItem in orderItems)
            {
                var orderItemModel = new SubmitReturnRequestModel.OrderItemModel
                {
                    Id = orderItem.Id,
                    ProductId = orderItem.Product.Id,
                    ProductName = orderItem.Product.GetLocalized(x => x.Name),
                    ProductSeName = orderItem.Product.GetSeName(),
                    AttributeInfo = orderItem.AttributeDescription,
                    Quantity = orderItem.Quantity
                };
                model.Items.Add(orderItemModel);

                //unit price
                if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
                {
                    //including tax
                    var unitPriceInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.UnitPriceInclTax, order.CurrencyRate);
                    orderItemModel.UnitPrice = _priceFormatter.FormatPrice(unitPriceInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, true);
                }
                else
                {
                    //excluding tax
                    var unitPriceExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.UnitPriceExclTax, order.CurrencyRate);
                    orderItemModel.UnitPrice = _priceFormatter.FormatPrice(unitPriceExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, false);
                }
            }

            return model;
        }

        #endregion
        
        #region Methods

        [NopHttpsRequirement(SslRequirement.Yes)]
        public ActionResult CustomerReturnRequests()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            var model = new CustomerReturnRequestsModel();

            var returnRequests = _returnRequestService.SearchReturnRequests(_storeContext.CurrentStore.Id, 
                _workContext.CurrentCustomer.Id);
            foreach (var returnRequest in returnRequests)
            {
                var orderItem = _orderService.GetOrderItemById(returnRequest.OrderItemId);
                if (orderItem != null)
                {
                    var product = orderItem.Product;

                    var itemModel = new CustomerReturnRequestsModel.ReturnRequestModel
                    {
                        Id = returnRequest.Id,
                        ReturnRequestStatus = returnRequest.ReturnRequestStatus.GetLocalizedEnum(_localizationService, _workContext),
                        ProductId = product.Id,
                        ProductName = product.GetLocalized(x => x.Name),
                        ProductSeName = product.GetSeName(),
                        Quantity = returnRequest.Quantity,
                        ReturnAction = returnRequest.RequestedAction,
                        ReturnReason = returnRequest.ReasonForReturn,
                        Comments = returnRequest.CustomerComments,
                        CreatedOn = _dateTimeHelper.ConvertToUserTime(returnRequest.CreatedOnUtc, DateTimeKind.Utc),
                    };
                    model.Items.Add(itemModel);
                }
            }

            return View(model);
        }

        [NopHttpsRequirement(SslRequirement.Yes)]
        public ActionResult ReturnRequest(int orderId)
        {
            var order = _orderService.GetOrderById(orderId);
            if (order == null || order.Deleted || _workContext.CurrentCustomer.Id != order.CustomerId)
                return new HttpUnauthorizedResult();

            if (!_orderProcessingService.IsReturnRequestAllowed(order))
                return RedirectToRoute("HomePage");

            var model = new SubmitReturnRequestModel();
            model = PrepareReturnRequestModel(model, order);
            return View(model);
        }

        [HttpPost, ActionName("ReturnRequest")]
        [ValidateInput(false)]
        [PublicAntiForgery]
        public ActionResult ReturnRequestSubmit(int orderId, SubmitReturnRequestModel model, FormCollection form)
        {
            var order = _orderService.GetOrderById(orderId);
            if (order == null || order.Deleted || _workContext.CurrentCustomer.Id != order.CustomerId)
                return new HttpUnauthorizedResult();

            if (!_orderProcessingService.IsReturnRequestAllowed(order))
                return RedirectToRoute("HomePage");

            int count = 0;
            foreach (var orderItem in order.OrderItems)
            {
                int quantity = 0; //parse quantity
                foreach (string formKey in form.AllKeys)
                    if (formKey.Equals(string.Format("quantity{0}", orderItem.Id), StringComparison.InvariantCultureIgnoreCase))
                    {
                        int.TryParse(form[formKey], out quantity);
                        break;
                    }
                if (quantity > 0)
                {
                    var rr = new ReturnRequest
                    {
                        StoreId = _storeContext.CurrentStore.Id,
                        OrderItemId = orderItem.Id,
                        Quantity = quantity,
                        CustomerId = _workContext.CurrentCustomer.Id,
                        ReasonForReturn = model.ReturnReason,
                        RequestedAction = model.ReturnAction,
                        CustomerComments = model.Comments,
                        StaffNotes = string.Empty,
                        ReturnRequestStatus = ReturnRequestStatus.Pending,
                        CreatedOnUtc = DateTime.UtcNow,
                        UpdatedOnUtc = DateTime.UtcNow
                    };
                    _workContext.CurrentCustomer.ReturnRequests.Add(rr);
                    _customerService.UpdateCustomer(_workContext.CurrentCustomer);
                    //notify store owner here (email)
                    _workflowMessageService.SendNewReturnRequestStoreOwnerNotification(rr, orderItem, _localizationSettings.DefaultAdminLanguageId);

                    count++;
                }
            }

            model = PrepareReturnRequestModel(model, order);
            if (count > 0)
                model.Result = _localizationService.GetResource("ReturnRequests.Submitted");
            else
                model.Result = _localizationService.GetResource("ReturnRequests.NoItemsSubmitted");

            return View(model);
        }

        #endregion
    }
}
