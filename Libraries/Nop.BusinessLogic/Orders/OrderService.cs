//------------------------------------------------------------------------------
// The contents of this file are subject to the nopCommerce Public License Version 1.0 ("License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at  http://www.nopCommerce.com/License.aspx. 
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// See the License for the specific language governing rights and limitations under the License.
// 
// The Original Code is nopCommerce.
// The Initial Developer of the Original Code is NopSolutions.
// All Rights Reserved.
// 
// Contributor(s): _______. 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using NopSolutions.NopCommerce.BusinessLogic.Audit;
using NopSolutions.NopCommerce.BusinessLogic.Caching;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Data;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;
using NopSolutions.NopCommerce.BusinessLogic.Localization;
using NopSolutions.NopCommerce.BusinessLogic.Messages;
using NopSolutions.NopCommerce.BusinessLogic.Messages.SMS;
using NopSolutions.NopCommerce.BusinessLogic.Payment;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Products.Attributes;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.BusinessLogic.Promo.Discounts;
using NopSolutions.NopCommerce.BusinessLogic.QuickBooks;
using NopSolutions.NopCommerce.BusinessLogic.Security;
using NopSolutions.NopCommerce.BusinessLogic.Shipping;
using NopSolutions.NopCommerce.BusinessLogic.Tax;
using NopSolutions.NopCommerce.BusinessLogic.Utils;
using NopSolutions.NopCommerce.BusinessLogic.Utils.Html;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Common.Utils.Html;
using System.Data.Objects;

namespace NopSolutions.NopCommerce.BusinessLogic.Orders
{
    /// <summary>
    /// Order service
    /// </summary>
    public partial class OrderService : IOrderService
    {
        #region Fields

        /// <summary>
        /// Object context
        /// </summary>
        protected readonly NopObjectContext _context;

        /// <summary>
        /// Cache service
        /// </summary>
        protected readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="context">Object context</param>
        public OrderService(NopObjectContext context)
        {
            this._context = context;
            this._cacheManager = new NopRequestCache();
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Sets an order status
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="os">New order status</param>
        /// <param name="notifyCustomer">True to notify customer</param>
        protected void SetOrderStatus(Order order,
            OrderStatusEnum os, bool notifyCustomer)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            OrderStatusEnum prevOrderStatus = order.OrderStatus;
            if (prevOrderStatus == os)
                return;

            //set and save new order status
            order.OrderStatusId = (int)os;
            UpdateOrder(order);

            //order notes, notifications
            InsertOrderNote(order.OrderId, string.Format("Order status has been changed to {0}", os.ToString()), false, DateTime.UtcNow);

            if (prevOrderStatus != OrderStatusEnum.Complete &&
                os == OrderStatusEnum.Complete
                && notifyCustomer)
            {
                int orderCompletedCustomerNotificationQueuedEmailId = IoC.Resolve<IMessageService>().SendOrderCompletedCustomerNotification(order, order.CustomerLanguageId);
                if (orderCompletedCustomerNotificationQueuedEmailId > 0)
                {
                    InsertOrderNote(order.OrderId, string.Format("\"Order completed\" email (to customer) has been queued. Queued email identifier: {0}.", orderCompletedCustomerNotificationQueuedEmailId), false, DateTime.UtcNow);
                }
            }

            if (prevOrderStatus != OrderStatusEnum.Cancelled &&
                os == OrderStatusEnum.Cancelled
                && notifyCustomer)
            {
                int orderCancelledCustomerNotificationQueuedEmailId = IoC.Resolve<IMessageService>().SendOrderCancelledCustomerNotification(order, order.CustomerLanguageId);
                if (orderCancelledCustomerNotificationQueuedEmailId > 0)
                {
                    InsertOrderNote(order.OrderId, string.Format("\"Order cancelled\" email (to customer) has been queued. Queued email identifier: {0}.", orderCancelledCustomerNotificationQueuedEmailId), false, DateTime.UtcNow);
                }
            }

            //reward points
            if (this.RewardPointsEnabled)
            {
                if (this.RewardPointsForPurchases_Amount > decimal.Zero)
                {
                    //Ensure that reward points are applied only to registered users
                    if (order.Customer != null && !order.Customer.IsGuest)
                    {
                        int points = (int)Math.Truncate(order.OrderTotal / this.RewardPointsForPurchases_Amount * this.RewardPointsForPurchases_Points);
                        if (points != 0)
                        {
                            if (this.RewardPointsForPurchases_Awarded == order.OrderStatus)
                            {
                                var rph = InsertRewardPointsHistory(order.CustomerId,
                                    0, points, decimal.Zero,
                                    decimal.Zero, string.Empty,
                                    string.Format(IoC.Resolve<ILocalizationManager>().GetLocaleResourceString("RewardPoints.Message.EarnedForOrder"), order.OrderId),
                                    DateTime.UtcNow);
                            }


                            if (this.RewardPointsForPurchases_Canceled == order.OrderStatus)
                            {
                                var rph = InsertRewardPointsHistory(order.CustomerId,
                                    0, -points, decimal.Zero,
                                    decimal.Zero, string.Empty,
                                    string.Format(IoC.Resolve<ILocalizationManager>().GetLocaleResourceString("RewardPoints.Message.ReducedForOrder"), order.OrderId),
                                    DateTime.UtcNow);
                            }
                        }
                    }
                }
            }

            //gift cards activation
            if (this.GiftCards_Activated.HasValue &&
               this.GiftCards_Activated.Value == order.OrderStatus)
            {
                var giftCards = GetAllGiftCards(order.OrderId, null, null, null, null, null, null, false, string.Empty);
                foreach (var gc in giftCards)
                {
                    bool isRecipientNotified = gc.IsRecipientNotified;
                    switch (gc.PurchasedOrderProductVariant.ProductVariant.GiftCardType)
                    {
                        case (int)GiftCardTypeEnum.Virtual:
                            {
                                //send email for virtual gift card
                                if (!String.IsNullOrEmpty(gc.RecipientEmail) &&
                                    !String.IsNullOrEmpty(gc.SenderEmail))
                                {
                                    Language customerLang = IoC.Resolve<ILanguageService>().GetLanguageById(order.CustomerLanguageId);
                                    if (customerLang == null)
                                        customerLang = NopContext.Current.WorkingLanguage;
                                    int queuedEmailId = IoC.Resolve<IMessageService>().SendGiftCardNotification(gc, customerLang.LanguageId);
                                    if (queuedEmailId > 0)
                                    {
                                        isRecipientNotified = true;
                                    }
                                }
                            }
                            break;
                        case (int)GiftCardTypeEnum.Physical:
                            {
                            }
                            break;
                        default:
                            break;
                    }

                    gc.IsGiftCardActivated = true;
                    gc.IsRecipientNotified = isRecipientNotified;
                    this.UpdateGiftCard(gc);
                }
            }

            //gift cards deactivation
            if (this.GiftCards_Deactivated.HasValue &&
               this.GiftCards_Deactivated.Value == order.OrderStatus)
            {
                var giftCards = GetAllGiftCards(order.OrderId,
                    null, null, null, null, null, null, true, string.Empty);
                foreach (var gc in giftCards)
                {
                    gc.IsGiftCardActivated = false;
                    this.UpdateGiftCard(gc);
                }
            }
        }

        /// <summary>
        /// Checks order status
        /// </summary>
        /// <param name="orderId">Order identifier</param>
        /// <returns>Validated order</returns>
        protected Order CheckOrderStatus(int orderId)
        {
            var order = GetOrderById(orderId);
            if (order == null)
                return null;

            if (order.OrderStatus == OrderStatusEnum.Pending)
            {
                if (order.PaymentStatus == PaymentStatusEnum.Authorized || 
                    order.PaymentStatus == PaymentStatusEnum.Paid)
                {
                    SetOrderStatus(order, OrderStatusEnum.Processing, false);
                }
            }

            if (order.OrderStatus == OrderStatusEnum.Pending)
            {
                if (order.ShippingStatus == ShippingStatusEnum.Shipped ||
                    order.ShippingStatus == ShippingStatusEnum.Delivered)
                {
                    SetOrderStatus(order, OrderStatusEnum.Processing, false);
                }
            }

            if (order.OrderStatus != OrderStatusEnum.Cancelled && 
                order.OrderStatus != OrderStatusEnum.Complete)
            {
                if (order.PaymentStatus == PaymentStatusEnum.Paid)
                {
                    if (!CanShip(order) && !CanDeliver(order))
                    {
                        SetOrderStatus(order, OrderStatusEnum.Complete, true);
                    }
                }
            }

            if (order.PaymentStatus == PaymentStatusEnum.Paid && !order.PaidDate.HasValue)
            {
                //ensure that paid date is set
                order.PaidDate = DateTime.UtcNow;;
                UpdateOrder(order);
            }

            return order;
        }

        #endregion

        #region Methods

        #region Repository methods

        #region Orders

        /// <summary>
        /// Gets an order
        /// </summary>
        /// <param name="orderId">The order identifier</param>
        /// <returns>Order</returns>
        public Order GetOrderById(int orderId)
        {
            if (orderId == 0)
                return null;

            
            var query = from o in _context.Orders
                        where o.OrderId == orderId
                        select o;
            var order = query.SingleOrDefault();
            return order;
        }

        /// <summary>
        /// Gets an order
        /// </summary>
        /// <param name="orderGuid">The order identifier</param>
        /// <returns>Order</returns>
        public Order GetOrderByGuid(Guid orderGuid)
        {
            if (orderGuid == Guid.Empty)
                return null;

            
            var query = from o in _context.Orders
                        where o.OrderGuid == orderGuid
                        select o;
            var order = query.FirstOrDefault();
            return order;
        }

        /// <summary>
        /// Marks an order as deleted
        /// </summary>
        /// <param name="orderId">The order identifier</param>
        public void MarkOrderAsDeleted(int orderId)
        {
            var order = GetOrderById(orderId);
            if (order != null)
            {
                order.Deleted = true;
                UpdateOrder(order);
            }
        }

        /// <summary>
        /// Search orders
        /// </summary>
        /// <param name="startTime">Order start time; null to load all orders</param>
        /// <param name="endTime">Order end time; null to load all orders</param>
        /// <param name="customerEmail">Customer email</param>
        /// <param name="os">Order status; null to load all orders</param>
        /// <param name="ps">Order payment status; null to load all orders</param>
        /// <param name="ss">Order shippment status; null to load all orders</param>
        /// <returns>Order collection</returns>
        public List<Order> SearchOrders(DateTime? startTime, DateTime? endTime,
            string customerEmail, OrderStatusEnum? os, PaymentStatusEnum? ps, 
            ShippingStatusEnum? ss)
        {
            return SearchOrders(startTime, endTime,
             customerEmail, os, ps, ss, string.Empty);
        }

        /// <summary>
        /// Search orders
        /// </summary>
        /// <param name="startTime">Order start time; null to load all orders</param>
        /// <param name="endTime">Order end time; null to load all orders</param>
        /// <param name="customerEmail">Customer email</param>
        /// <param name="os">Order status; null to load all orders</param>
        /// <param name="ps">Order payment status; null to load all orders</param>
        /// <param name="ss">Order shippment status; null to load all orders</param>
        /// <param name="orderGuid">Search by order GUID (Global unique identifier) or part of GUID. Leave empty to load all orders.</param>
        /// <returns>Order collection</returns>
        public List<Order> SearchOrders(DateTime? startTime, DateTime? endTime,
            string customerEmail, OrderStatusEnum? os, PaymentStatusEnum? ps,
            ShippingStatusEnum? ss, string orderGuid)
        {
            int? orderStatusId = null;
            if (os.HasValue)
                orderStatusId = (int)os.Value;

            int? paymentStatusId = null;
            if (ps.HasValue)
                paymentStatusId = (int)ps.Value;

            int? shippingStatusId = null;
            if (ss.HasValue)
                shippingStatusId = (int)ss.Value;

            

            var query = from o in _context.Orders
                        join c in _context.Customers on o.CustomerId equals c.CustomerId
                        where (String.IsNullOrEmpty(customerEmail) || c.Email.Contains(customerEmail)) &&
                        (!startTime.HasValue || startTime.Value <= o.CreatedOn) &&
                        (!endTime.HasValue || endTime.Value >= o.CreatedOn) &&
                        (!orderStatusId.HasValue || orderStatusId == o.OrderStatusId) &&
                        (!paymentStatusId.HasValue || paymentStatusId.Value == o.PaymentStatusId) &&
                        (!shippingStatusId.HasValue || shippingStatusId.Value == o.ShippingStatusId) &&
                        !o.Deleted
                        orderby o.CreatedOn descending
                        select o;

            var orders = query.ToList();
            
            //filter by GUID. Filter in BLL because EF doesn't support casting of GUID to string
            if (!String.IsNullOrEmpty(orderGuid))
            {
                orders = orders.FindAll(o => o.OrderGuid.ToString().ToLowerInvariant().Contains(orderGuid.ToLowerInvariant()));
            }

            return orders;
        }

        /// <summary>
        /// Load all orders
        /// </summary>
        /// <returns>Order collection</returns>
        public List<Order> LoadAllOrders()
        {
            return SearchOrders(null, null, string.Empty, null, null, null);
        }

        /// <summary>
        /// Gets all orders by customer identifier
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <returns>Order collection</returns>
        public List<Order> GetOrdersByCustomerId(int customerId)
        {
            
            var query = from o in _context.Orders
                        orderby o.CreatedOn descending
                        where !o.Deleted && o.CustomerId == customerId
                        select o;
            var orders = query.ToList();
            return orders;
        }

        /// <summary>
        /// Gets an order by authorization transaction identifier
        /// </summary>
        /// <param name="authorizationTransactionId">Authorization transaction identifier</param>
        /// <param name="paymentMethodId">Payment method identifier</param>
        /// <returns>Order</returns>
        public Order GetOrderByAuthorizationTransactionIdAndPaymentMethodId(string authorizationTransactionId, 
            int paymentMethodId)
        {
            
            var query = from o in _context.Orders
                        orderby o.CreatedOn descending
                        where o.AuthorizationTransactionId == authorizationTransactionId && 
                        o.PaymentMethodId == paymentMethodId
                        select o;
            var order = query.FirstOrDefault();
            return order;
        }

        /// <summary>
        /// Gets all orders by affiliate identifier
        /// </summary>
        /// <param name="affiliateId">Affiliate identifier</param>
        /// <returns>Order collection</returns>
        public List<Order> GetOrdersByAffiliateId(int affiliateId)
        {
            
            var query = from o in _context.Orders
                        orderby o.CreatedOn descending
                        where !o.Deleted && o.AffiliateId == affiliateId
                        select o;
            var orders = query.ToList();
            return orders;
        }

        /// <summary>
        /// Inserts an order
        /// </summary>
        /// <param name="order">Order</param>
        public void InsertOrder(Order order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            order.TaxRates = CommonHelper.EnsureNotNull(order.TaxRates);
            order.TaxRatesInCustomerCurrency = CommonHelper.EnsureNotNull(order.TaxRatesInCustomerCurrency);
            order.CustomerIP = CommonHelper.EnsureNotNull(order.CustomerIP);
            order.CheckoutAttributeDescription = CommonHelper.EnsureNotNull(order.CheckoutAttributeDescription);
            order.CheckoutAttributesXml = CommonHelper.EnsureNotNull(order.CheckoutAttributesXml);
            order.CardType = CommonHelper.EnsureNotNull(order.CardType);
            order.CardName = CommonHelper.EnsureNotNull(order.CardName);
            order.CardNumber = CommonHelper.EnsureNotNull(order.CardNumber);
            order.MaskedCreditCardNumber = CommonHelper.EnsureNotNull(order.MaskedCreditCardNumber);
            order.CardCvv2 = CommonHelper.EnsureNotNull(order.CardCvv2);
            order.CardExpirationMonth = CommonHelper.EnsureNotNull(order.CardExpirationMonth);
            order.CardExpirationYear = CommonHelper.EnsureNotNull(order.CardExpirationYear);
            order.PaymentMethodName = CommonHelper.EnsureNotNull(order.PaymentMethodName);
            order.AuthorizationTransactionId = CommonHelper.EnsureNotNull(order.AuthorizationTransactionId);
            order.AuthorizationTransactionCode = CommonHelper.EnsureNotNull(order.AuthorizationTransactionCode);
            order.AuthorizationTransactionResult = CommonHelper.EnsureNotNull(order.AuthorizationTransactionResult);
            order.CaptureTransactionId = CommonHelper.EnsureNotNull(order.CaptureTransactionId);
            order.CaptureTransactionResult = CommonHelper.EnsureNotNull(order.CaptureTransactionResult);
            order.SubscriptionTransactionId = CommonHelper.EnsureNotNull(order.SubscriptionTransactionId);
            order.PurchaseOrderNumber = CommonHelper.EnsureNotNull(order.PurchaseOrderNumber);
            order.BillingFirstName = CommonHelper.EnsureNotNull(order.BillingFirstName);
            order.BillingLastName = CommonHelper.EnsureNotNull(order.BillingLastName);
            order.BillingPhoneNumber = CommonHelper.EnsureNotNull(order.BillingPhoneNumber);
            order.BillingEmail = CommonHelper.EnsureNotNull(order.BillingEmail);
            order.BillingFaxNumber = CommonHelper.EnsureNotNull(order.BillingFaxNumber);
            order.BillingCompany = CommonHelper.EnsureNotNull(order.BillingCompany);
            order.BillingAddress1 = CommonHelper.EnsureNotNull(order.BillingAddress1);
            order.BillingAddress2 = CommonHelper.EnsureNotNull(order.BillingAddress2);
            order.BillingCity = CommonHelper.EnsureNotNull(order.BillingCity);
            order.BillingStateProvince = CommonHelper.EnsureNotNull(order.BillingStateProvince);
            order.BillingZipPostalCode = CommonHelper.EnsureNotNull(order.BillingZipPostalCode);
            order.BillingCountry = CommonHelper.EnsureNotNull(order.BillingCountry);
            order.ShippingFirstName = CommonHelper.EnsureNotNull(order.ShippingFirstName);
            order.ShippingLastName = CommonHelper.EnsureNotNull(order.ShippingLastName);
            order.ShippingPhoneNumber = CommonHelper.EnsureNotNull(order.ShippingPhoneNumber);
            order.ShippingEmail = CommonHelper.EnsureNotNull(order.ShippingEmail);
            order.ShippingFaxNumber = CommonHelper.EnsureNotNull(order.ShippingFaxNumber);
            order.ShippingCompany = CommonHelper.EnsureNotNull(order.ShippingCompany);
            order.ShippingAddress1 = CommonHelper.EnsureNotNull(order.ShippingAddress1);
            order.ShippingAddress2 = CommonHelper.EnsureNotNull(order.ShippingAddress2);
            order.ShippingCity = CommonHelper.EnsureNotNull(order.ShippingCity);
            order.ShippingStateProvince = CommonHelper.EnsureNotNull(order.ShippingStateProvince);
            order.ShippingZipPostalCode = CommonHelper.EnsureNotNull(order.ShippingZipPostalCode);
            order.ShippingCountry = CommonHelper.EnsureNotNull(order.ShippingCountry);
            order.ShippingMethod = CommonHelper.EnsureNotNull(order.ShippingMethod);
            order.TrackingNumber = CommonHelper.EnsureNotNull(order.TrackingNumber);
            order.VatNumber = CommonHelper.EnsureNotNull(order.VatNumber);
            
            order.BillingEmail = order.BillingEmail.Trim();
            order.ShippingEmail = order.ShippingEmail.Trim();
            
            order.TaxRates = CommonHelper.EnsureMaximumLength(order.TaxRates, 4000);
            order.TaxRatesInCustomerCurrency = CommonHelper.EnsureMaximumLength(order.TaxRatesInCustomerCurrency, 4000);
            order.CustomerIP = CommonHelper.EnsureMaximumLength(order.CustomerIP, 50);
            order.CardType = CommonHelper.EnsureMaximumLength(order.CardType, 100);
            order.CardName = CommonHelper.EnsureMaximumLength(order.CardName, 1000);
            order.CardNumber = CommonHelper.EnsureMaximumLength(order.CardNumber, 100);
            order.MaskedCreditCardNumber = CommonHelper.EnsureMaximumLength(order.MaskedCreditCardNumber, 100);
            order.CardCvv2 = CommonHelper.EnsureMaximumLength(order.CardCvv2, 100);
            order.CardExpirationMonth = CommonHelper.EnsureMaximumLength(order.CardExpirationMonth, 100);
            order.CardExpirationYear = CommonHelper.EnsureMaximumLength(order.CardExpirationYear, 100);
            order.PaymentMethodName = CommonHelper.EnsureMaximumLength(order.PaymentMethodName, 100);
            order.AuthorizationTransactionId = CommonHelper.EnsureMaximumLength(order.AuthorizationTransactionId, 4000);
            order.AuthorizationTransactionCode = CommonHelper.EnsureMaximumLength(order.AuthorizationTransactionCode, 4000);
            order.AuthorizationTransactionResult = CommonHelper.EnsureMaximumLength(order.AuthorizationTransactionResult, 4000);
            order.CaptureTransactionId = CommonHelper.EnsureMaximumLength(order.CaptureTransactionId, 4000);
            order.CaptureTransactionResult = CommonHelper.EnsureMaximumLength(order.CaptureTransactionResult, 4000);
            order.SubscriptionTransactionId = CommonHelper.EnsureMaximumLength(order.SubscriptionTransactionId, 4000);
            order.PurchaseOrderNumber = CommonHelper.EnsureMaximumLength(order.PurchaseOrderNumber, 100);
            order.BillingFirstName = CommonHelper.EnsureMaximumLength(order.BillingFirstName, 100);
            order.BillingLastName = CommonHelper.EnsureMaximumLength(order.BillingLastName, 100);
            order.BillingPhoneNumber = CommonHelper.EnsureMaximumLength(order.BillingPhoneNumber, 50);
            order.BillingEmail = CommonHelper.EnsureMaximumLength(order.BillingEmail, 255);
            order.BillingFaxNumber = CommonHelper.EnsureMaximumLength(order.BillingFaxNumber, 50);
            order.BillingCompany = CommonHelper.EnsureMaximumLength(order.BillingCompany, 100);
            order.BillingAddress1 = CommonHelper.EnsureMaximumLength(order.BillingAddress1, 100);
            order.BillingAddress2 = CommonHelper.EnsureMaximumLength(order.BillingAddress2, 100);
            order.BillingCity = CommonHelper.EnsureMaximumLength(order.BillingCity, 100);
            order.BillingStateProvince = CommonHelper.EnsureMaximumLength(order.BillingStateProvince, 100);
            order.BillingZipPostalCode = CommonHelper.EnsureMaximumLength(order.BillingZipPostalCode, 30);
            order.BillingCountry = CommonHelper.EnsureMaximumLength(order.BillingCountry, 100);
            order.ShippingFirstName = CommonHelper.EnsureMaximumLength(order.ShippingFirstName, 100);
            order.ShippingLastName = CommonHelper.EnsureMaximumLength(order.ShippingLastName, 100);
            order.ShippingPhoneNumber = CommonHelper.EnsureMaximumLength(order.ShippingPhoneNumber, 50);
            order.ShippingEmail = CommonHelper.EnsureMaximumLength(order.ShippingEmail, 255);
            order.ShippingFaxNumber = CommonHelper.EnsureMaximumLength(order.ShippingFaxNumber, 50);
            order.ShippingCompany = CommonHelper.EnsureMaximumLength(order.ShippingCompany, 100);
            order.ShippingAddress1 = CommonHelper.EnsureMaximumLength(order.ShippingAddress1, 100);
            order.ShippingAddress2 = CommonHelper.EnsureMaximumLength(order.ShippingAddress2, 100);
            order.ShippingCity = CommonHelper.EnsureMaximumLength(order.ShippingCity, 100);
            order.ShippingStateProvince = CommonHelper.EnsureMaximumLength(order.ShippingStateProvince, 100);
            order.ShippingZipPostalCode = CommonHelper.EnsureMaximumLength(order.ShippingZipPostalCode, 30);
            order.ShippingCountry = CommonHelper.EnsureMaximumLength(order.ShippingCountry, 100);
            order.ShippingMethod = CommonHelper.EnsureMaximumLength(order.ShippingMethod, 100);
            order.TrackingNumber = CommonHelper.EnsureMaximumLength(order.TrackingNumber, 100);
            order.VatNumber = CommonHelper.EnsureMaximumLength(order.VatNumber, 100);

            

            _context.Orders.AddObject(order);
            _context.SaveChanges();

            //quickbooks
            if (IoC.Resolve<IQBService>().QBIsEnabled)
            {
                IoC.Resolve<IQBService>().RequestSynchronization(order);
            }

            //raise event             
            EventContext.Current.OnOrderCreated(null,
                new OrderEventArgs() { Order = order });
        }

        /// <summary>
        /// Updates the order
        /// </summary>
        /// <param name="order">The order</param>
        public void UpdateOrder(Order order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            order.TaxRates = CommonHelper.EnsureNotNull(order.TaxRates);
            order.TaxRatesInCustomerCurrency = CommonHelper.EnsureNotNull(order.TaxRatesInCustomerCurrency);
            order.CustomerIP = CommonHelper.EnsureNotNull(order.CustomerIP);
            order.CheckoutAttributeDescription = CommonHelper.EnsureNotNull(order.CheckoutAttributeDescription);
            order.CheckoutAttributesXml = CommonHelper.EnsureNotNull(order.CheckoutAttributesXml);
            order.CardType = CommonHelper.EnsureNotNull(order.CardType);
            order.CardName = CommonHelper.EnsureNotNull(order.CardName);
            order.CardNumber = CommonHelper.EnsureNotNull(order.CardNumber);
            order.MaskedCreditCardNumber = CommonHelper.EnsureNotNull(order.MaskedCreditCardNumber);
            order.CardCvv2 = CommonHelper.EnsureNotNull(order.CardCvv2);
            order.CardExpirationMonth = CommonHelper.EnsureNotNull(order.CardExpirationMonth);
            order.CardExpirationYear = CommonHelper.EnsureNotNull(order.CardExpirationYear);
            order.PaymentMethodName = CommonHelper.EnsureNotNull(order.PaymentMethodName);
            order.AuthorizationTransactionId = CommonHelper.EnsureNotNull(order.AuthorizationTransactionId);
            order.AuthorizationTransactionCode = CommonHelper.EnsureNotNull(order.AuthorizationTransactionCode);
            order.AuthorizationTransactionResult = CommonHelper.EnsureNotNull(order.AuthorizationTransactionResult);
            order.CaptureTransactionId = CommonHelper.EnsureNotNull(order.CaptureTransactionId);
            order.CaptureTransactionResult = CommonHelper.EnsureNotNull(order.CaptureTransactionResult);
            order.SubscriptionTransactionId = CommonHelper.EnsureNotNull(order.SubscriptionTransactionId);
            order.PurchaseOrderNumber = CommonHelper.EnsureNotNull(order.PurchaseOrderNumber);
            order.BillingFirstName = CommonHelper.EnsureNotNull(order.BillingFirstName);
            order.BillingLastName = CommonHelper.EnsureNotNull(order.BillingLastName);
            order.BillingPhoneNumber = CommonHelper.EnsureNotNull(order.BillingPhoneNumber);
            order.BillingEmail = CommonHelper.EnsureNotNull(order.BillingEmail);
            order.BillingFaxNumber = CommonHelper.EnsureNotNull(order.BillingFaxNumber);
            order.BillingCompany = CommonHelper.EnsureNotNull(order.BillingCompany);
            order.BillingAddress1 = CommonHelper.EnsureNotNull(order.BillingAddress1);
            order.BillingAddress2 = CommonHelper.EnsureNotNull(order.BillingAddress2);
            order.BillingCity = CommonHelper.EnsureNotNull(order.BillingCity);
            order.BillingStateProvince = CommonHelper.EnsureNotNull(order.BillingStateProvince);
            order.BillingZipPostalCode = CommonHelper.EnsureNotNull(order.BillingZipPostalCode);
            order.BillingCountry = CommonHelper.EnsureNotNull(order.BillingCountry);
            order.ShippingFirstName = CommonHelper.EnsureNotNull(order.ShippingFirstName);
            order.ShippingLastName = CommonHelper.EnsureNotNull(order.ShippingLastName);
            order.ShippingPhoneNumber = CommonHelper.EnsureNotNull(order.ShippingPhoneNumber);
            order.ShippingEmail = CommonHelper.EnsureNotNull(order.ShippingEmail);
            order.ShippingFaxNumber = CommonHelper.EnsureNotNull(order.ShippingFaxNumber);
            order.ShippingCompany = CommonHelper.EnsureNotNull(order.ShippingCompany);
            order.ShippingAddress1 = CommonHelper.EnsureNotNull(order.ShippingAddress1);
            order.ShippingAddress2 = CommonHelper.EnsureNotNull(order.ShippingAddress2);
            order.ShippingCity = CommonHelper.EnsureNotNull(order.ShippingCity);
            order.ShippingStateProvince = CommonHelper.EnsureNotNull(order.ShippingStateProvince);
            order.ShippingZipPostalCode = CommonHelper.EnsureNotNull(order.ShippingZipPostalCode);
            order.ShippingCountry = CommonHelper.EnsureNotNull(order.ShippingCountry);
            order.ShippingMethod = CommonHelper.EnsureNotNull(order.ShippingMethod);
            order.TrackingNumber = CommonHelper.EnsureNotNull(order.TrackingNumber);
            order.VatNumber = CommonHelper.EnsureNotNull(order.VatNumber);

            order.BillingEmail = order.BillingEmail.Trim();
            order.ShippingEmail = order.ShippingEmail.Trim();

            order.TaxRates = CommonHelper.EnsureMaximumLength(order.TaxRates, 4000);
            order.TaxRatesInCustomerCurrency = CommonHelper.EnsureMaximumLength(order.TaxRatesInCustomerCurrency, 4000);
            order.CustomerIP = CommonHelper.EnsureMaximumLength(order.CustomerIP, 50);
            order.CardType = CommonHelper.EnsureMaximumLength(order.CardType, 100);
            order.CardName = CommonHelper.EnsureMaximumLength(order.CardName, 1000);
            order.CardNumber = CommonHelper.EnsureMaximumLength(order.CardNumber, 100);
            order.MaskedCreditCardNumber = CommonHelper.EnsureMaximumLength(order.MaskedCreditCardNumber, 100);
            order.CardCvv2 = CommonHelper.EnsureMaximumLength(order.CardCvv2, 100);
            order.CardExpirationMonth = CommonHelper.EnsureMaximumLength(order.CardExpirationMonth, 100);
            order.CardExpirationYear = CommonHelper.EnsureMaximumLength(order.CardExpirationYear, 100);
            order.PaymentMethodName = CommonHelper.EnsureMaximumLength(order.PaymentMethodName, 100);
            order.AuthorizationTransactionId = CommonHelper.EnsureMaximumLength(order.AuthorizationTransactionId, 4000);
            order.AuthorizationTransactionCode = CommonHelper.EnsureMaximumLength(order.AuthorizationTransactionCode, 4000);
            order.AuthorizationTransactionResult = CommonHelper.EnsureMaximumLength(order.AuthorizationTransactionResult, 4000);
            order.CaptureTransactionId = CommonHelper.EnsureMaximumLength(order.CaptureTransactionId, 4000);
            order.CaptureTransactionResult = CommonHelper.EnsureMaximumLength(order.CaptureTransactionResult, 4000);
            order.SubscriptionTransactionId = CommonHelper.EnsureMaximumLength(order.SubscriptionTransactionId, 4000);
            order.PurchaseOrderNumber = CommonHelper.EnsureMaximumLength(order.PurchaseOrderNumber, 100);
            order.BillingFirstName = CommonHelper.EnsureMaximumLength(order.BillingFirstName, 100);
            order.BillingLastName = CommonHelper.EnsureMaximumLength(order.BillingLastName, 100);
            order.BillingPhoneNumber = CommonHelper.EnsureMaximumLength(order.BillingPhoneNumber, 50);
            order.BillingEmail = CommonHelper.EnsureMaximumLength(order.BillingEmail, 255);
            order.BillingFaxNumber = CommonHelper.EnsureMaximumLength(order.BillingFaxNumber, 50);
            order.BillingCompany = CommonHelper.EnsureMaximumLength(order.BillingCompany, 100);
            order.BillingAddress1 = CommonHelper.EnsureMaximumLength(order.BillingAddress1, 100);
            order.BillingAddress2 = CommonHelper.EnsureMaximumLength(order.BillingAddress2, 100);
            order.BillingCity = CommonHelper.EnsureMaximumLength(order.BillingCity, 100);
            order.BillingStateProvince = CommonHelper.EnsureMaximumLength(order.BillingStateProvince, 100);
            order.BillingZipPostalCode = CommonHelper.EnsureMaximumLength(order.BillingZipPostalCode, 30);
            order.BillingCountry = CommonHelper.EnsureMaximumLength(order.BillingCountry, 100);
            order.ShippingFirstName = CommonHelper.EnsureMaximumLength(order.ShippingFirstName, 100);
            order.ShippingLastName = CommonHelper.EnsureMaximumLength(order.ShippingLastName, 100);
            order.ShippingPhoneNumber = CommonHelper.EnsureMaximumLength(order.ShippingPhoneNumber, 50);
            order.ShippingEmail = CommonHelper.EnsureMaximumLength(order.ShippingEmail, 255);
            order.ShippingFaxNumber = CommonHelper.EnsureMaximumLength(order.ShippingFaxNumber, 50);
            order.ShippingCompany = CommonHelper.EnsureMaximumLength(order.ShippingCompany, 100);
            order.ShippingAddress1 = CommonHelper.EnsureMaximumLength(order.ShippingAddress1, 100);
            order.ShippingAddress2 = CommonHelper.EnsureMaximumLength(order.ShippingAddress2, 100);
            order.ShippingCity = CommonHelper.EnsureMaximumLength(order.ShippingCity, 100);
            order.ShippingStateProvince = CommonHelper.EnsureMaximumLength(order.ShippingStateProvince, 100);
            order.ShippingZipPostalCode = CommonHelper.EnsureMaximumLength(order.ShippingZipPostalCode, 30);
            order.ShippingCountry = CommonHelper.EnsureMaximumLength(order.ShippingCountry, 100);
            order.ShippingMethod = CommonHelper.EnsureMaximumLength(order.ShippingMethod, 100);
            order.TrackingNumber = CommonHelper.EnsureMaximumLength(order.TrackingNumber, 100);
            order.VatNumber = CommonHelper.EnsureMaximumLength(order.VatNumber, 100);

            
            if (!_context.IsAttached(order))
                _context.Orders.Attach(order);

            _context.SaveChanges();

            //quickbooks
            if (IoC.Resolve<IQBService>().QBIsEnabled)
            {
                IoC.Resolve<IQBService>().RequestSynchronization(order);
            }


            //raise event             
            EventContext.Current.OnOrderUpdated(null,
                new OrderEventArgs() { Order = order });
        }

        #endregion
        
        #region Orders product variants

        /// <summary>
        /// Gets an order product variant
        /// </summary>
        /// <param name="orderProductVariantId">Order product variant identifier</param>
        /// <returns>Order product variant</returns>
        public OrderProductVariant GetOrderProductVariantById(int orderProductVariantId)
        {
            if (orderProductVariantId == 0)
                return null;

            
            var query = from opv in _context.OrderProductVariants
                        where opv.OrderProductVariantId == orderProductVariantId
                        select opv;
            var orderProductVariant = query.SingleOrDefault();

            return orderProductVariant;
        }

        /// <summary>
        /// Delete an order product variant
        /// </summary>
        /// <param name="orderProductVariantId">Order product variant identifier</param>
        public void DeleteOrderProductVariant(int orderProductVariantId)
        {
            if (orderProductVariantId == 0)
                return;

            var orderProductVariant = GetOrderProductVariantById(orderProductVariantId);
            if (orderProductVariant == null)
                return;

            
            if (!_context.IsAttached(orderProductVariant))
                _context.OrderProductVariants.Attach(orderProductVariant);
            _context.DeleteObject(orderProductVariant);
            _context.SaveChanges();
        }

        /// <summary>
        /// Gets an order product variant
        /// </summary>
        /// <param name="orderProductVariantGuid">Order product variant identifier</param>
        /// <returns>Order product variant</returns>
        public OrderProductVariant GetOrderProductVariantByGuid(Guid orderProductVariantGuid)
        {
            if (orderProductVariantGuid == Guid.Empty)
                return null;

            
            var query = from opv in _context.OrderProductVariants
                        where opv.OrderProductVariantGuid == orderProductVariantGuid
                        select opv;
            var orderProductVariant = query.FirstOrDefault();

            return orderProductVariant;
        }

        /// <summary>
        /// Gets all order product variants
        /// </summary>
        /// <param name="orderId">Order identifier; null to load all records</param>
        /// <param name="customerId">Customer identifier; null to load all records</param>
        /// <param name="startTime">Order start time; null to load all records</param>
        /// <param name="endTime">Order end time; null to load all records</param>
        /// <param name="os">Order status; null to load all records</param>
        /// <param name="ps">Order payment status; null to load all records</param>
        /// <param name="ss">Order shippment status; null to load all records</param>
        /// <returns>Order collection</returns>
        public List<OrderProductVariant> GetAllOrderProductVariants(int? orderId,
            int? customerId, DateTime? startTime, DateTime? endTime,
            OrderStatusEnum? os, PaymentStatusEnum? ps, ShippingStatusEnum? ss)
        {
            return GetAllOrderProductVariants(orderId, customerId, startTime, 
                endTime, os, ps, ss, false);
        }

        /// <summary>
        /// Gets all order product variants
        /// </summary>
        /// <param name="orderId">Order identifier; null to load all records</param>
        /// <param name="customerId">Customer identifier; null to load all records</param>
        /// <param name="startTime">Order start time; null to load all records</param>
        /// <param name="endTime">Order end time; null to load all records</param>
        /// <param name="os">Order status; null to load all records</param>
        /// <param name="ps">Order payment status; null to load all records</param>
        /// <param name="ss">Order shippment status; null to load all records</param>
        /// <param name="loadDownloableProductsOnly">Value indicating whether to load downloadable products only</param>
        /// <returns>Order collection</returns>
        public List<OrderProductVariant> GetAllOrderProductVariants(int? orderId,
            int? customerId, DateTime? startTime, DateTime? endTime,
            OrderStatusEnum? os, PaymentStatusEnum? ps, ShippingStatusEnum? ss,
            bool loadDownloableProductsOnly)
        {
            int? orderStatusId = null;
            if (os.HasValue)
                orderStatusId = (int)os.Value;

            int? paymentStatusId = null;
            if (ps.HasValue)
                paymentStatusId = (int)ps.Value;

            int? shippingStatusId = null;
            if (ss.HasValue)
                shippingStatusId = (int)ss.Value;

            

            var query = from opv in _context.OrderProductVariants
                        join o in _context.Orders on opv.OrderId equals o.OrderId
                        join pv in _context.ProductVariants on opv.ProductVariantId equals pv.ProductVariantId
                        where (!orderId.HasValue || orderId.Value == 0 || orderId == o.OrderId) &&
                        (!customerId.HasValue || customerId.Value == 0 || customerId == o.CustomerId) &&
                        (!startTime.HasValue || startTime.Value <= o.CreatedOn) &&
                        (!endTime.HasValue || endTime.Value >= o.CreatedOn) &&
                        (!orderStatusId.HasValue || orderStatusId == o.OrderStatusId) &&
                        (!paymentStatusId.HasValue || paymentStatusId.Value == o.PaymentStatusId) &&
                        (!shippingStatusId.HasValue || shippingStatusId.Value == o.ShippingStatusId) &&
                        (!loadDownloableProductsOnly || pv.IsDownload) &&
                        !o.Deleted
                        orderby o.CreatedOn descending, opv.OrderProductVariantId
                        select opv;

            var orderProductVariants = query.ToList();
            return orderProductVariants;
        }

        /// <summary>
        /// Gets an order product variants by the order identifier
        /// </summary>
        /// <param name="orderId">The order identifier</param>
        /// <returns>Order product variant collection</returns>
        public List<OrderProductVariant> GetOrderProductVariantsByOrderId(int orderId)
        {
            return GetAllOrderProductVariants(orderId, null, null, null, null, null, null);
        }

        /// <summary>
        /// Inserts a order product variant
        /// </summary>
        /// <param name="opv">Order product variant</param>
        public void InsertOrderProductVariant(OrderProductVariant opv)
        {
            if (opv == null)
                throw new ArgumentNullException("opv");

            opv.AttributeDescription = CommonHelper.EnsureNotNull(opv.AttributeDescription);
            opv.AttributeDescription = CommonHelper.EnsureMaximumLength(opv.AttributeDescription, 4000);

            
            
            _context.OrderProductVariants.AddObject(opv);
            _context.SaveChanges();
        }

        /// <summary>
        /// Updates the order product variant
        /// </summary>
        /// <param name="opv">Order product variant</param>
        public void UpdateOrderProductVariant(OrderProductVariant opv)
        {
            if (opv == null)
                throw new ArgumentNullException("opv");

            opv.AttributeDescription = CommonHelper.EnsureNotNull(opv.AttributeDescription);
            opv.AttributeDescription = CommonHelper.EnsureMaximumLength(opv.AttributeDescription, 4000);

            
            if (!_context.IsAttached(opv))
                _context.OrderProductVariants.Attach(opv);

            _context.SaveChanges();
        }

        #endregion

        #region Order notes

        /// <summary>
        /// Gets an order note
        /// </summary>
        /// <param name="orderNoteId">Order note identifier</param>
        /// <returns>Order note</returns>
        public OrderNote GetOrderNoteById(int orderNoteId)
        {
            if (orderNoteId == 0)
                return null;

            
            var query = from onote in _context.OrderNotes
                        where onote.OrderNoteId == orderNoteId
                        select onote;
            var orderNote = query.SingleOrDefault();
            return orderNote;
        }

        /// <summary>
        /// Gets an order notes by order identifier
        /// </summary>
        /// <param name="orderId">Order identifier</param>
        /// <returns>Order note collection</returns>
        public List<OrderNote> GetOrderNoteByOrderId(int orderId)
        {
            return GetOrderNoteByOrderId(orderId, NopContext.Current.IsAdmin);
        }

        /// <summary>
        /// Gets an order notes by order identifier
        /// </summary>
        /// <param name="orderId">Order identifier</param>
        /// <param name="showHidden">A value indicating whether all orders should be loaded</param>
        /// <returns>Order note collection</returns>
        public List<OrderNote> GetOrderNoteByOrderId(int orderId, bool showHidden)
        {
            
            var query = from onote in _context.OrderNotes
                        orderby onote.CreatedOn descending, onote.OrderNoteId descending
                        where (showHidden || onote.DisplayToCustomer) &&
                        onote.OrderId == orderId
                        select onote;
            var orderNotes = query.ToList();
            return orderNotes;
        }

        /// <summary>
        /// Deletes an order note
        /// </summary>
        /// <param name="orderNoteId">Order note identifier</param>
        public void DeleteOrderNote(int orderNoteId)
        {
            var orderNote = GetOrderNoteById(orderNoteId);
            if (orderNote == null)
                return;

            
            if (!_context.IsAttached(orderNote))
                _context.OrderNotes.Attach(orderNote);
            _context.DeleteObject(orderNote);
            _context.SaveChanges();
        }

        /// <summary>
        /// Inserts an order note
        /// </summary>
        /// <param name="orderId">The order identifier</param>
        /// <param name="note">The note</param>
        /// <param name="createdOn">The date and time of order note creation</param>
        /// <returns>Order note</returns>
        public OrderNote InsertOrderNote(int orderId, string note, DateTime createdOn)
        {
            return InsertOrderNote(orderId, note, false, createdOn);
        }

        /// <summary>
        /// Inserts an order note
        /// </summary>
        /// <param name="orderId">The order identifier</param>
        /// <param name="note">The note</param>
        /// <param name="displayToCustomer">A value indicating whether the customer can see a note</param>
        /// <param name="createdOn">The date and time of order note creation</param>
        /// <returns>Order note</returns>
        public OrderNote InsertOrderNote(int orderId, string note, 
            bool displayToCustomer, DateTime createdOn)
        {
            note = CommonHelper.EnsureNotNull(note);
            note = CommonHelper.EnsureMaximumLength(note, 4000);

            

            var orderNote = _context.OrderNotes.CreateObject();
            orderNote.OrderId = orderId;
            orderNote.Note = note;
            orderNote.DisplayToCustomer = displayToCustomer;
            orderNote.CreatedOn = createdOn;

            _context.OrderNotes.AddObject(orderNote);
            _context.SaveChanges();
            return orderNote;
        }

        /// <summary>
        /// Updates the order note
        /// </summary>
        /// <param name="orderNote">Order note</param>
        public void UpdateOrderNote(OrderNote orderNote)
        {
            if (orderNote == null)
                throw new ArgumentNullException("orderNote");

            orderNote.Note = CommonHelper.EnsureNotNull(orderNote.Note);
            orderNote.Note = CommonHelper.EnsureMaximumLength(orderNote.Note, 4000);

            
            if (!_context.IsAttached(orderNote))
                _context.OrderNotes.Attach(orderNote);

            _context.SaveChanges();
        }

        #endregion

        #region Reports

        /// <summary>
        /// Get order product variant sales report
        /// </summary>
        /// <param name="startTime">Order start time; null to load all</param>
        /// <param name="endTime">Order end time; null to load all</param>
        /// <param name="os">Order status; null to load all records</param>
        /// <param name="ps">Order payment status; null to load all records</param>
        /// <param name="billingCountryId">Billing country identifier; null to load all records</param>
        /// <returns>Result</returns>
        public List<OrderProductVariantReportLine> OrderProductVariantReport(DateTime? startTime,
            DateTime? endTime, OrderStatusEnum? os, PaymentStatusEnum? ps,
            int? billingCountryId)
        {
            int? orderStatusId = null;
            if (os.HasValue)
                orderStatusId = (int)os.Value;

            int? paymentStatusId = null;
            if (ps.HasValue)
                paymentStatusId = (int)ps.Value;

            
            var report = _context.Sp_OrderProductVariantReport(startTime,
                endTime, orderStatusId, paymentStatusId, billingCountryId).ToList();
            return report;
        }

        /// <summary>
        /// Get the bests sellers report
        /// </summary>
        /// <param name="lastDays">Last number of days</param>
        /// <param name="recordsToReturn">Number of products to return</param>
        /// <param name="orderBy">1 - order by total count, 2 - Order by total amount</param>
        /// <returns>Result</returns>
        public List<BestSellersReportLine> BestSellersReport(int lastDays, 
            int recordsToReturn, int orderBy)
        {
            
            var report = _context.Sp_SalesBestSellersReport(lastDays,
                recordsToReturn, orderBy).ToList();
            return report;
        }

        /// <summary>
        /// Get order average report
        /// </summary>
        /// <param name="os">Order status;</param>
        /// <param name="startTime">Start date</param>
        /// <param name="endTime">End date</param>
        /// <returns>Result</returns>
        public OrderAverageReportLine GetOrderAverageReportLine(OrderStatusEnum os,
            DateTime? startTime, DateTime? endTime)
        {
            int orderStatusId = (int)os;

            var query = from o in _context.Orders
                        where (!o.Deleted) &&
                        (o.OrderStatusId == orderStatusId) &&
                        (!startTime.HasValue || startTime.Value <= o.CreatedOn) &&
                        (!endTime.HasValue || endTime.Value >= o.CreatedOn)
                        select o;

            var item = new OrderAverageReportLine();
            item.SumOrders = Convert.ToDecimal(query.Sum(o => (decimal?)o.OrderTotal));
            item.CountOrders = query.Count();
            return item;
        }
        
        /// <summary>
        /// Get order average report
        /// </summary>
        /// <param name="os">Order status</param>
        /// <returns>Result</returns>
        public OrderAverageReportLineSummary OrderAverageReport(OrderStatusEnum os)
        {
            var item = new OrderAverageReportLineSummary();
            item.OrderStatus = os;

            DateTime nowDT = DateTimeHelper.ConvertToUserTime(DateTime.Now);            
            TimeZoneInfo timeZone = DateTimeHelper.CurrentTimeZone;

            //today
            DateTime t1 = new DateTime(nowDT.Year, nowDT.Month, nowDT.Day);
            if (!timeZone.IsInvalidTime(t1))
            {
                DateTime? startTime1 = DateTimeHelper.ConvertToUtcTime(t1, timeZone);
                DateTime? endTime1 = null;
                var todayResult = GetOrderAverageReportLine(os, startTime1, endTime1);
                item.SumTodayOrders = todayResult.SumOrders;
                item.CountTodayOrders = todayResult.CountOrders;
            }
            //week
            DayOfWeek fdow = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
            DateTime today = new DateTime(nowDT.Year, nowDT.Month, nowDT.Day);
            DateTime t2 = today.AddDays(-(today.DayOfWeek - fdow));
            if (!timeZone.IsInvalidTime(t2))
            {
                DateTime? startTime2 = DateTimeHelper.ConvertToUtcTime(t2, timeZone);
                DateTime? endTime2 = null;
                var weekResult = GetOrderAverageReportLine(os, startTime2, endTime2);
                item.SumThisWeekOrders = weekResult.SumOrders;
                item.CountThisWeekOrders = weekResult.CountOrders;
            }
            //month
            DateTime t3 = new DateTime(nowDT.Year, nowDT.Month, 1);
            if (!timeZone.IsInvalidTime(t3))
            {
                DateTime? startTime3 = DateTimeHelper.ConvertToUtcTime(t3, timeZone);
                DateTime? endTime3 = null;
                var monthResult = GetOrderAverageReportLine(os, startTime3, endTime3);
                item.SumThisMonthOrders = monthResult.SumOrders;
                item.CountThisMonthOrders = monthResult.CountOrders;
            }
            //year
            DateTime t4 = new DateTime(nowDT.Year, 1, 1);
            if (!timeZone.IsInvalidTime(t4))
            {
                DateTime? startTime4 = DateTimeHelper.ConvertToUtcTime(t4, timeZone);
                DateTime? endTime4 = null;
                var yearResult = GetOrderAverageReportLine(os, startTime4, endTime4);
                item.SumThisYearOrders = yearResult.SumOrders;
                item.CountThisYearOrders = yearResult.CountOrders;
            }
            //all time
            DateTime? startTime5 = null;
            DateTime? endTime5 = null;
            var allTimeResult = GetOrderAverageReportLine(os, startTime5, endTime5);
            item.SumAllTimeOrders = allTimeResult.SumOrders;
            item.CountAllTimeOrders = allTimeResult.CountOrders;

            return item;
        }

        /// <summary>
        /// Gets an order report
        /// </summary>
        /// <param name="os">Order status; null to load all orders</param>
        /// <param name="ps">Order payment status; null to load all orders</param>
        /// <param name="ss">Order shippment status; null to load all orders</param>
        /// <returns>IdataReader</returns>
        public OrderIncompleteReportLine GetOrderReport(OrderStatusEnum? os,
            PaymentStatusEnum? ps, ShippingStatusEnum? ss)
        {
            int? orderStatusId = null;
            if (os.HasValue)
                orderStatusId = (int)os.Value;

            int? paymentStatusId = null;
            if (ps.HasValue)
                paymentStatusId = (int)ps.Value;

            int? shippingStatusId = null;
            if (ss.HasValue)
                shippingStatusId = (int)ss.Value;

            var query = from o in _context.Orders
                        where (!o.Deleted) &&
                        (!orderStatusId.HasValue || orderStatusId.Value == 0 || o.OrderStatusId == orderStatusId.Value) &&
                        (!paymentStatusId.HasValue || paymentStatusId.Value == 0 || o.PaymentStatusId == paymentStatusId.Value) &&
                        (!shippingStatusId.HasValue || shippingStatusId.Value == 0 || o.ShippingStatusId == shippingStatusId.Value)
                        select o;

            var item = new OrderIncompleteReportLine();
            item.Total = Convert.ToDecimal(query.Sum(o => (decimal?)o.OrderTotal));
            item.Count = query.Count();
            return item;
        }

        #endregion

        #region Recurring payments

        /// <summary>
        /// Gets a recurring payment
        /// </summary>
        /// <param name="recurringPaymentId">The recurring payment identifier</param>
        /// <returns>Recurring payment</returns>
        public RecurringPayment GetRecurringPaymentById(int recurringPaymentId)
        {
            if (recurringPaymentId == 0)
                return null;

            
            var query = from rp in _context.RecurringPayments
                        where rp.RecurringPaymentId == recurringPaymentId
                        select rp;
            var recurringPayment = query.SingleOrDefault();
            return recurringPayment;
        }

        /// <summary>
        /// Deletes a recurring payment
        /// </summary>
        /// <param name="recurringPaymentId">Recurring payment identifier</param>
        public void DeleteRecurringPayment(int recurringPaymentId)
        {
            var recurringPayment = GetRecurringPaymentById(recurringPaymentId);
            if (recurringPayment != null)
            {
                recurringPayment.Deleted = true;
                UpdateRecurringPayment(recurringPayment);
            }
        }

        /// <summary>
        /// Inserts a recurring payment
        /// </summary>
        /// <param name="recurringPayment">Recurring payment</param>
        public void InsertRecurringPayment(RecurringPayment recurringPayment)
        {
            if (recurringPayment == null)
                throw new ArgumentNullException("recurringPayment");

            

            _context.RecurringPayments.AddObject(recurringPayment);
            _context.SaveChanges();
        }

        /// <summary>
        /// Updates the recurring payment
        /// </summary>
        /// <param name="recurringPayment">Recurring payment</param>
        public void UpdateRecurringPayment(RecurringPayment recurringPayment)
        {
            if (recurringPayment == null)
                throw new ArgumentNullException("recurringPayment");

            
            if (!_context.IsAttached(recurringPayment))
                _context.RecurringPayments.Attach(recurringPayment);

            _context.SaveChanges();
        }

        /// <summary>
        /// Search recurring payments
        /// </summary>
        /// <param name="customerId">The customer identifier; 0 to load all records</param>
        /// <param name="initialOrderId">The initial order identifier; 0 to load all records</param>
        /// <param name="initialOrderStatus">Initial order status identifier; null to load all records</param>
        /// <returns>Recurring payment collection</returns>
        public List<RecurringPayment> SearchRecurringPayments(int customerId, 
            int initialOrderId, OrderStatusEnum? initialOrderStatus)
        {
            bool showHidden = NopContext.Current.IsAdmin;
            return SearchRecurringPayments(showHidden, 
                customerId, initialOrderId, initialOrderStatus);
        }

        /// <summary>
        /// Search recurring payments
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <param name="customerId">The customer identifier; 0 to load all records</param>
        /// <param name="initialOrderId">The initial order identifier; 0 to load all records</param>
        /// <param name="initialOrderStatus">Initial order status identifier; null to load all records</param>
        /// <returns>Recurring payment collection</returns>
        public List<RecurringPayment> SearchRecurringPayments(bool showHidden,
            int customerId, int initialOrderId, OrderStatusEnum? initialOrderStatus)
        {
            int? initialOrderStatusId = null;
            if (initialOrderStatus.HasValue)
                initialOrderStatusId = (int)initialOrderStatus.Value;

            var query1 = from rp in _context.RecurringPayments
                         join o in _context.Orders on rp.InitialOrderId equals o.OrderId
                         join c in _context.Customers on o.CustomerId equals c.CustomerId
                         where
                         (!rp.Deleted && !o.Deleted && !c.Deleted) &&
                         (showHidden || rp.IsActive) &&
                         (customerId == 0 || o.CustomerId == customerId) &&
                         (initialOrderId == 0 || rp.InitialOrderId == initialOrderId) &&
                         (!initialOrderStatusId.HasValue || initialOrderStatusId.Value == 0 || o.OrderStatusId == initialOrderStatusId.Value)
                         select rp.RecurringPaymentId;

            var query2 = from rp in _context.RecurringPayments
                         where query1.Contains(rp.RecurringPaymentId)
                         orderby rp.StartDate, rp.RecurringPaymentId
                         select rp;


            var recurringPayments = query2.ToList();
            return recurringPayments;
        }

        /// <summary>
        /// Deletes a recurring payment history
        /// </summary>
        /// <param name="recurringPaymentHistoryId">Recurring payment history identifier</param>
        public void DeleteRecurringPaymentHistory(int recurringPaymentHistoryId)
        {
            var recurringPaymentHistory = GetRecurringPaymentHistoryById(recurringPaymentHistoryId);
            if (recurringPaymentHistory == null)
                return;

            
            if (!_context.IsAttached(recurringPaymentHistory))
                _context.RecurringPaymentHistory.Attach(recurringPaymentHistory);
            _context.DeleteObject(recurringPaymentHistory);
            _context.SaveChanges();
        }

        /// <summary>
        /// Gets a recurring payment history
        /// </summary>
        /// <param name="recurringPaymentHistoryId">The recurring payment history identifier</param>
        /// <returns>Recurring payment history</returns>
        public RecurringPaymentHistory GetRecurringPaymentHistoryById(int recurringPaymentHistoryId)
        {
            if (recurringPaymentHistoryId == 0)
                return null;

            
            var query = from rph in _context.RecurringPaymentHistory
                        where rph.RecurringPaymentHistoryId == recurringPaymentHistoryId
                        select rph;
            var recurringPaymentHistory = query.SingleOrDefault();
            return recurringPaymentHistory;
        }

        /// <summary>
        /// Inserts a recurring payment history
        /// </summary>
        /// <param name="recurringPaymentHistory">Recurring payment history</param>
        public void InsertRecurringPaymentHistory(RecurringPaymentHistory recurringPaymentHistory)
        {
            if (recurringPaymentHistory == null)
                throw new ArgumentNullException("recurringPaymentHistory");

            
            
            _context.RecurringPaymentHistory.AddObject(recurringPaymentHistory);
            _context.SaveChanges();
        }

        /// <summary>
        /// Updates the recurring payment history
        /// </summary>
        /// <param name="recurringPaymentHistory">Recurring payment history</param>
        public void UpdateRecurringPaymentHistory(RecurringPaymentHistory recurringPaymentHistory)
        {
            if (recurringPaymentHistory == null)
                throw new ArgumentNullException("recurringPaymentHistory");

            
            if (!_context.IsAttached(recurringPaymentHistory))
                _context.RecurringPaymentHistory.Attach(recurringPaymentHistory);

            _context.SaveChanges();
        }

        /// <summary>
        /// Search recurring payment history
        /// </summary>
        /// <param name="recurringPaymentId">The recurring payment identifier; 0 to load all records</param>
        /// <param name="orderId">The order identifier; 0 to load all records</param>
        /// <returns>Recurring payment history collection</returns>
        public List<RecurringPaymentHistory> SearchRecurringPaymentHistory(int recurringPaymentId, 
            int orderId)
        {
            var query1 = from rph in _context.RecurringPaymentHistory
                         from rp in _context.RecurringPayments
                         .Where(rp => rp.RecurringPaymentId == rph.RecurringPaymentId)
                         .DefaultIfEmpty()
                         from o in _context.Orders
                         .Where(o => o.OrderId == rph.OrderId)
                         .DefaultIfEmpty()
                         where
                         (!rp.Deleted && !o.Deleted) &&
                         (recurringPaymentId == 0 || rph.RecurringPaymentId == recurringPaymentId) &&
                         (orderId == 0 || rph.OrderId == orderId)
                         select rph.RecurringPaymentHistoryId;

            var query2 = from rph in _context.RecurringPaymentHistory
                         where query1.Contains(rph.RecurringPaymentHistoryId)
                         orderby rph.CreatedOn, rph.RecurringPaymentHistoryId
                         select rph;

            var recurringPaymentHistory = query2.ToList();
            return recurringPaymentHistory;
        }

        #endregion

        #region Gift Cards

        /// <summary>
        /// Deletes a gift card
        /// </summary>
        /// <param name="giftCardId">Gift card identifier</param>
        public void DeleteGiftCard(int giftCardId)
        {
            var giftCard = GetGiftCardById(giftCardId);
            if (giftCard == null)
                return;

            
            if (!_context.IsAttached(giftCard))
                _context.GiftCards.Attach(giftCard);
            _context.DeleteObject(giftCard);
            _context.SaveChanges();
        }

        /// <summary>
        /// Gets a gift card
        /// </summary>
        /// <param name="giftCardId">Gift card identifier</param>
        /// <returns>Gift card entry</returns>
        public GiftCard GetGiftCardById(int giftCardId)
        {
            if (giftCardId == 0)
                return null;

            
            var query = from gc in _context.GiftCards
                        where gc.GiftCardId == giftCardId
                        select gc;
            var giftCard = query.SingleOrDefault();
            return giftCard;
        }

        /// <summary>
        /// Gets all gift cards
        /// </summary>
        /// <param name="orderId">Order identifier; null to load all records</param>
        /// <param name="customerId">Customer identifier; null to load all records</param>
        /// <param name="startTime">Order start time; null to load all records</param>
        /// <param name="endTime">Order end time; null to load all records</param>
        /// <param name="os">Order status; null to load all records</param>
        /// <param name="ps">Order payment status; null to load all records</param>
        /// <param name="ss">Order shippment status; null to load all records</param>
        /// <param name="isGiftCardActivated">Value indicating whether gift card is activated; null to load all records</param>
        /// <param name="giftCardCouponCode">Gift card coupon code; null or string.empty to load all records</param>
        /// <returns>Gift cards</returns>
        public List<GiftCard> GetAllGiftCards(int? orderId,
            int? customerId, DateTime? startTime, DateTime? endTime,
            OrderStatusEnum? os, PaymentStatusEnum? ps, ShippingStatusEnum? ss,
            bool? isGiftCardActivated, string giftCardCouponCode)
        {
            int? orderStatusId = null;
            if (os.HasValue)
                orderStatusId = (int)os.Value;

            int? paymentStatusId = null;
            if (ps.HasValue)
                paymentStatusId = (int)ps.Value;

            int? shippingStatusId = null;
            if (ss.HasValue)
                shippingStatusId = (int)ss.Value;

            if (giftCardCouponCode != null)
                giftCardCouponCode = giftCardCouponCode.Trim();

            giftCardCouponCode = CommonHelper.EnsureNotNull(giftCardCouponCode);

            var gcQuery = from gc in _context.GiftCards
                          join opv in _context.OrderProductVariants on gc.PurchasedOrderProductVariantId equals opv.OrderProductVariantId
                          join o in _context.Orders on opv.OrderId equals o.OrderId
                          where
                          (!orderId.HasValue || orderId.Value == 0 || o.OrderId == orderId.Value) &&
                          (!customerId.HasValue || customerId.Value == 0 || o.CustomerId == customerId.Value) &&
                          (!startTime.HasValue || startTime.Value <= gc.CreatedOn) &&
                          (!endTime.HasValue || endTime.Value >= gc.CreatedOn) &&
                          (!orderStatusId.HasValue || orderStatusId.Value == 0 || o.OrderStatusId == orderStatusId.Value) &&
                          (!paymentStatusId.HasValue || paymentStatusId.Value == 0 || o.PaymentStatusId == paymentStatusId.Value) &&
                          (!shippingStatusId.HasValue || shippingStatusId.Value == 0 || o.ShippingStatusId == shippingStatusId.Value) &&
                          (!isGiftCardActivated.HasValue || gc.IsGiftCardActivated == isGiftCardActivated.Value) &&
                          (string.IsNullOrEmpty(giftCardCouponCode) || gc.GiftCardCouponCode == giftCardCouponCode)
                          select gc.GiftCardId;

            var query = from gc in _context.GiftCards
                        where gcQuery.Contains(gc.GiftCardId)
                        orderby gc.CreatedOn descending, gc.GiftCardId
                        select gc;

            var giftCards = query.ToList();
            return giftCards;
        }

        /// <summary>
        /// Inserts a gift card
        /// </summary>
        /// <param name="giftCard">Gift card</param>
        public void InsertGiftCard(GiftCard giftCard)
        {
            if (giftCard == null)
                throw new ArgumentNullException("giftCard");

            giftCard.GiftCardCouponCode = CommonHelper.EnsureNotNull(giftCard.GiftCardCouponCode);
            giftCard.GiftCardCouponCode = CommonHelper.EnsureMaximumLength(giftCard.GiftCardCouponCode, 100);
            giftCard.RecipientName = CommonHelper.EnsureNotNull(giftCard.RecipientName);
            giftCard.RecipientName = CommonHelper.EnsureMaximumLength(giftCard.RecipientName, 100);
            giftCard.RecipientEmail = CommonHelper.EnsureNotNull(giftCard.RecipientEmail);
            giftCard.RecipientEmail = CommonHelper.EnsureMaximumLength(giftCard.RecipientEmail, 100);
            giftCard.SenderName = CommonHelper.EnsureNotNull(giftCard.SenderName);
            giftCard.SenderName = CommonHelper.EnsureMaximumLength(giftCard.SenderName, 100);
            giftCard.SenderEmail = CommonHelper.EnsureNotNull(giftCard.SenderEmail);
            giftCard.SenderEmail = CommonHelper.EnsureMaximumLength(giftCard.SenderEmail, 100);
            giftCard.Message = CommonHelper.EnsureNotNull(giftCard.Message);
            giftCard.Message = CommonHelper.EnsureMaximumLength(giftCard.Message, 4000);

            

            _context.GiftCards.AddObject(giftCard);
            _context.SaveChanges();
        }

        /// <summary>
        /// Updates the gift card
        /// </summary>
        /// <param name="giftCard">Gift card</param>
        public void UpdateGiftCard(GiftCard giftCard)
        {
            if (giftCard == null)
                throw new ArgumentNullException("giftCard");
            giftCard.GiftCardCouponCode = CommonHelper.EnsureNotNull(giftCard.GiftCardCouponCode);
            giftCard.GiftCardCouponCode = CommonHelper.EnsureMaximumLength(giftCard.GiftCardCouponCode, 100);
            giftCard.RecipientName = CommonHelper.EnsureNotNull(giftCard.RecipientName);
            giftCard.RecipientName = CommonHelper.EnsureMaximumLength(giftCard.RecipientName, 100);
            giftCard.RecipientEmail = CommonHelper.EnsureNotNull(giftCard.RecipientEmail);
            giftCard.RecipientEmail = CommonHelper.EnsureMaximumLength(giftCard.RecipientEmail, 100);
            giftCard.SenderName = CommonHelper.EnsureNotNull(giftCard.SenderName);
            giftCard.SenderName = CommonHelper.EnsureMaximumLength(giftCard.SenderName, 100);
            giftCard.SenderEmail = CommonHelper.EnsureNotNull(giftCard.SenderEmail);
            giftCard.SenderEmail = CommonHelper.EnsureMaximumLength(giftCard.SenderEmail, 100);
            giftCard.Message = CommonHelper.EnsureNotNull(giftCard.Message);
            giftCard.Message = CommonHelper.EnsureMaximumLength(giftCard.Message, 4000);

            
            if (!_context.IsAttached(giftCard))
                _context.GiftCards.Attach(giftCard);

            _context.SaveChanges();
        }

        /// <summary>
        /// Deletes a gift card usage history entry
        /// </summary>
        /// <param name="giftCardUsageHistoryId">Gift card usage history entry identifier</param>
        public void DeleteGiftCardUsageHistory(int giftCardUsageHistoryId)
        {
            var giftCardUsageHistory = GetGiftCardUsageHistoryById(giftCardUsageHistoryId);
            if (giftCardUsageHistory == null)
                return;

            
            if (!_context.IsAttached(giftCardUsageHistory))
                _context.GiftCardUsageHistory.Attach(giftCardUsageHistory);
            _context.DeleteObject(giftCardUsageHistory);
            _context.SaveChanges();
        }

        /// <summary>
        /// Gets a gift card usage history entry
        /// </summary>
        /// <param name="giftCardUsageHistoryId">Gift card usage history entry identifier</param>
        /// <returns>Gift card usage history entry</returns>
        public GiftCardUsageHistory GetGiftCardUsageHistoryById(int giftCardUsageHistoryId)
        {
            if (giftCardUsageHistoryId == 0)
                return null;

            
            var query = from gcuh in _context.GiftCardUsageHistory
                        where gcuh.GiftCardUsageHistoryId == giftCardUsageHistoryId
                        select gcuh;
            var giftCardUsageHistory = query.SingleOrDefault();
            return giftCardUsageHistory;
        }

        /// <summary>
        /// Gets all gift card usage history entries
        /// </summary>
        /// <param name="giftCardId">Gift card identifier; null to load all records</param>
        /// <param name="customerId">Customer identifier; null to load all records</param>
        /// <param name="orderId">Order identifier; null to load all records</param>
        /// <returns>Gift card usage history entries</returns>
        public List<GiftCardUsageHistory> GetAllGiftCardUsageHistoryEntries(int? giftCardId,
            int? customerId, int? orderId)
        {
            var query1 = from gcuh in _context.GiftCardUsageHistory
                         from gc in _context.GiftCards
                         .Where(gc => gc.GiftCardId == gcuh.GiftCardId)
                         .DefaultIfEmpty()
                         from opv in _context.OrderProductVariants
                         .Where(opv => opv.OrderProductVariantId == gc.PurchasedOrderProductVariantId)
                         .DefaultIfEmpty()
                         from o in _context.Orders
                         .Where(o => o.OrderId == gcuh.OrderId)
                         .DefaultIfEmpty()
                         where
                         (!o.Deleted) &&
                         (!giftCardId.HasValue || giftCardId.Value == 0 || gcuh.GiftCardId == giftCardId.Value) &&
                         (!customerId.HasValue || customerId.Value == 0 || gcuh.CustomerId == customerId.Value) &&
                         (!orderId.HasValue || orderId.Value == 0 || gcuh.OrderId == orderId.Value)
                         select gcuh.GiftCardUsageHistoryId;

            var query2 = from gcuh in _context.GiftCardUsageHistory
                         where query1.Contains(gcuh.GiftCardUsageHistoryId)
                         select gcuh;

            var giftCardUsageHistoryEntries = query2.ToList();
            return giftCardUsageHistoryEntries;
        }

        /// <summary>
        /// Inserts a gift card usage history entry
        /// </summary>
        /// <param name="giftCardUsageHistory">Gift card usage history entry</param>
        public void InsertGiftCardUsageHistory(GiftCardUsageHistory giftCardUsageHistory)
        {
            if (giftCardUsageHistory == null)
                throw new ArgumentNullException("giftCardUsageHistory");

            

            _context.GiftCardUsageHistory.AddObject(giftCardUsageHistory);
            _context.SaveChanges();
        }

        /// <summary>
        /// Updates the gift card usage history entry
        /// </summary>
        /// <param name="giftCardUsageHistory">Gift card usage history entry</param>
        public void UpdateGiftCardUsageHistory(GiftCardUsageHistory giftCardUsageHistory)
        {
            if (giftCardUsageHistory == null)
                throw new ArgumentNullException("giftCardUsageHistory");

            
            if (!_context.IsAttached(giftCardUsageHistory))
                _context.GiftCardUsageHistory.Attach(giftCardUsageHistory);

            _context.SaveChanges();
        }

        #endregion

        #region Reward points

        /// <summary>
        /// Deletes a reward point history entry
        /// </summary>
        /// <param name="rewardPointsHistoryId">Reward point history entry identifier</param>
        public void DeleteRewardPointsHistory(int rewardPointsHistoryId)
        {
            var rewardPointsHistory = GetRewardPointsHistoryById(rewardPointsHistoryId);
            if (rewardPointsHistory == null)
                return;

            
            if (!_context.IsAttached(rewardPointsHistory))
                _context.RewardPointsHistory.Attach(rewardPointsHistory);
            _context.DeleteObject(rewardPointsHistory);
            _context.SaveChanges();
        }

        /// <summary>
        /// Gets a reward point history entry
        /// </summary>
        /// <param name="rewardPointsHistoryId">Reward point history entry identifier</param>
        /// <returns>Reward point history entry</returns>
        public RewardPointsHistory GetRewardPointsHistoryById(int rewardPointsHistoryId)
        {
            if (rewardPointsHistoryId == 0)
                return null;

            
            var query = from rph in _context.RewardPointsHistory
                        where rph.RewardPointsHistoryId == rewardPointsHistoryId
                        select rph;
            var rewardPointsHistory = query.SingleOrDefault();
            return rewardPointsHistory;
        }

        /// <summary>
        /// Gets all reward point history entries
        /// </summary>
        /// <param name="customerId">Customer identifier; null to load all records</param>
        /// <param name="orderId">Order identifier; null to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Reward point history entries</returns>
        public PagedList<RewardPointsHistory> GetAllRewardPointsHistoryEntries(int? customerId,
            int? orderId, int pageIndex, int pageSize)
        {
            var query = from rph in _context.RewardPointsHistory
                        where
                        (!customerId.HasValue || customerId.Value == 0 || customerId.Value == rph.CustomerId) &&
                        (!orderId.HasValue || orderId.Value == 0 || orderId.Value == rph.OrderId)
                        orderby rph.CreatedOn descending
                        select rph;
            var rewardPointsHistoryEntries = new PagedList<RewardPointsHistory>(query, pageIndex, pageSize);

            return rewardPointsHistoryEntries;
        }

        /// <summary>
        /// Inserts a reward point history entry
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="orderId">Order identifier</param>
        /// <param name="points">Points redeemed/added</param>
        /// <param name="usedAmount">Used amount</param>
        /// <param name="usedAmountInCustomerCurrency">Used amount (customer currency)</param>
        /// <param name="customerCurrencyCode">Customer currency code</param>
        /// <param name="message">Customer currency code</param>
        /// <param name="createdOn">A date and time of instance creation</param>
        /// <returns>Reward point history entry</returns>
        public RewardPointsHistory InsertRewardPointsHistory(int customerId,
            int orderId, int points, decimal usedAmount,
            decimal usedAmountInCustomerCurrency, string customerCurrencyCode,
            string message, DateTime createdOn)
        {
            customerCurrencyCode = CommonHelper.EnsureNotNull(customerCurrencyCode);
            customerCurrencyCode = CommonHelper.EnsureMaximumLength(customerCurrencyCode, 5);
            message = CommonHelper.EnsureNotNull(message);
            message = CommonHelper.EnsureMaximumLength(message, 1000);

            Customer customer = IoC.Resolve<ICustomerService>().GetCustomerById(customerId);
            if (customer == null)
                throw new NopException("Customer not found. ID=" + customerId);

            int newPointsBalance = customer.RewardPointsBalance + points;

            

            var rewardPointsHistory = _context.RewardPointsHistory.CreateObject();
            rewardPointsHistory.CustomerId = customerId;
            rewardPointsHistory.OrderId = orderId;
            rewardPointsHistory.Points = points;
            rewardPointsHistory.PointsBalance = newPointsBalance;
            rewardPointsHistory.UsedAmount = usedAmount;
            rewardPointsHistory.UsedAmountInCustomerCurrency = usedAmountInCustomerCurrency;
            rewardPointsHistory.CustomerCurrencyCode = customerCurrencyCode;
            rewardPointsHistory.Message = message;
            rewardPointsHistory.CreatedOn = createdOn;

            _context.RewardPointsHistory.AddObject(rewardPointsHistory);
            _context.SaveChanges();

            customer.ResetCachedValues();

            return rewardPointsHistory;
        }

        /// <summary>
        /// Updates a reward point history entry
        /// </summary>
        /// <param name="rewardPointsHistory">Reward point history entry</param>
        public void UpdateRewardPointsHistory(RewardPointsHistory rewardPointsHistory)
        {
            if (rewardPointsHistory == null)
                throw new ArgumentNullException("rewardPointsHistory");

            rewardPointsHistory.CustomerCurrencyCode = CommonHelper.EnsureNotNull(rewardPointsHistory.CustomerCurrencyCode);
            rewardPointsHistory.CustomerCurrencyCode = CommonHelper.EnsureMaximumLength(rewardPointsHistory.CustomerCurrencyCode, 5);
            rewardPointsHistory.Message = CommonHelper.EnsureNotNull(rewardPointsHistory.Message);
            rewardPointsHistory.Message = CommonHelper.EnsureMaximumLength(rewardPointsHistory.Message, 1000);

            
            if (!_context.IsAttached(rewardPointsHistory))
                _context.RewardPointsHistory.Attach(rewardPointsHistory);

            _context.SaveChanges();
        }

        #endregion

        #region Return requests (RMA)

        /// <summary>
        /// Gets a return request status name
        /// </summary>
        /// <param name="rs">Return status</param>
        /// <returns>Return request status name</returns>
        public string GetReturnRequestStatusName(ReturnStatusEnum rs)
        {
            string name = string.Empty;
            switch (rs)
            {
                case ReturnStatusEnum.Pending:
                    name = IoC.Resolve<ILocalizationManager>().GetLocaleResourceString("ReturnStatus.Pending");
                    break;
                case ReturnStatusEnum.Received:
                    name = IoC.Resolve<ILocalizationManager>().GetLocaleResourceString("ReturnStatus.Received");
                    break;
                case ReturnStatusEnum.ReturnAuthorized:
                    name = IoC.Resolve<ILocalizationManager>().GetLocaleResourceString("ReturnStatus.ReturnAuthorized");
                    break;
                case ReturnStatusEnum.ItemsRepaired:
                    name = IoC.Resolve<ILocalizationManager>().GetLocaleResourceString("ReturnStatus.ItemsRepaired");
                    break;
                case ReturnStatusEnum.ItemsRefunded:
                    name = IoC.Resolve<ILocalizationManager>().GetLocaleResourceString("ReturnStatus.ItemsRefunded");
                    break;
                case ReturnStatusEnum.RequestRejected:
                    name = IoC.Resolve<ILocalizationManager>().GetLocaleResourceString("ReturnStatus.RequestRejected");
                    break;
                case ReturnStatusEnum.Cancelled:
                    name = IoC.Resolve<ILocalizationManager>().GetLocaleResourceString("ReturnStatus.Cancelled");
                    break;
                default:
                    name = CommonHelper.ConvertEnum(rs.ToString());
                    break;
            }
            return name;
        }

        /// <summary>
        /// Check whether return request is allowed
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>Result</returns>
        public bool IsReturnRequestAllowed(Order order)
        {
            if (!IoC.Resolve<ISettingManager>().GetSettingValueBoolean("ReturnRequests.Enable"))
                return false;

            if (order == null || order.Deleted)
                return false;

            return order.OrderStatus == OrderStatusEnum.Complete;
        }

        /// <summary>
        /// Gets a return request
        /// </summary>
        /// <param name="returnRequestId">Return request identifier</param>
        /// <returns>Return request</returns>
        public ReturnRequest GetReturnRequestById(int returnRequestId)
        {
            if (returnRequestId == 0)
                return null;

            
            var query = from rr in _context.ReturnRequests
                        where rr.ReturnRequestId == returnRequestId
                        select rr;
            var returnRequest = query.SingleOrDefault();
            return returnRequest;
        }
        
        /// <summary>
        /// Deletes a return request
        /// </summary>
        /// <param name="returnRequestId">Return request identifier</param>
        public void DeleteReturnRequest(int returnRequestId)
        {
            var returnRequest = GetReturnRequestById(returnRequestId);
            if (returnRequest == null)
                return;

            
            if (!_context.IsAttached(returnRequest))
                _context.ReturnRequests.Attach(returnRequest);
            _context.DeleteObject(returnRequest);
            _context.SaveChanges();
        }

        /// <summary>
        /// Search return requests
        /// </summary>
        /// <param name="customerId">Customer identifier; null to load all entries</param>
        /// <param name="orderProductVariantId">Order product variant identifier; null to load all entries</param>
        /// <param name="rs">Return status; null to load all entries</param>
        /// <returns>Return requests</returns>
        public List<ReturnRequest> SearchReturnRequests(int customerId, 
            int orderProductVariantId, ReturnStatusEnum? rs)
        {
            int? returnStatusId = null;
            if (rs.HasValue)
                returnStatusId = (int)rs.Value;

            

            var query = from rr in _context.ReturnRequests
                        where (!returnStatusId.HasValue || returnStatusId == rr.ReturnStatusId) &&
                        (customerId == 0 || customerId == rr.CustomerId) &&
                        (orderProductVariantId == 0 || orderProductVariantId == rr.OrderProductVariantId)
                        orderby rr.CreatedOn descending, rr.ReturnRequestId descending
                        select rr;

            var returnRequests = query.ToList();
            return returnRequests;
        }

        /// <summary>
        /// Inserts a return request
        /// </summary>
        /// <param name="returnRequest">Return request</param>
        /// <param name="notifyStoreOwner">A value indicating whether to notify about new return request</param>
        public void InsertReturnRequest(ReturnRequest returnRequest, bool notifyStoreOwner)
        {
            if (returnRequest == null)
                throw new ArgumentNullException("returnRequest");

            returnRequest.ReasonForReturn = CommonHelper.EnsureNotNull(returnRequest.ReasonForReturn);
            returnRequest.ReasonForReturn = CommonHelper.EnsureMaximumLength(returnRequest.ReasonForReturn, 400);
            returnRequest.RequestedAction = CommonHelper.EnsureNotNull(returnRequest.RequestedAction);
            returnRequest.RequestedAction = CommonHelper.EnsureMaximumLength(returnRequest.RequestedAction, 400);
            returnRequest.CustomerComments = CommonHelper.EnsureNotNull(returnRequest.CustomerComments);
            returnRequest.StaffNotes = CommonHelper.EnsureNotNull(returnRequest.StaffNotes);

            

            _context.ReturnRequests.AddObject(returnRequest);
            _context.SaveChanges();

            if (notifyStoreOwner)
            {
                IoC.Resolve<IMessageService>().SendNewReturnRequestStoreOwnerNotification(returnRequest, IoC.Resolve<ILocalizationManager>().DefaultAdminLanguage.LanguageId);
            }
        }
        
        /// <summary>
        /// Updates the return request
        /// </summary>
        /// <param name="returnRequest">Return request</param>
        public void UpdateReturnRequest(ReturnRequest returnRequest)
        {
            if (returnRequest == null)
                throw new ArgumentNullException("returnRequest");

            returnRequest.ReasonForReturn = CommonHelper.EnsureNotNull(returnRequest.ReasonForReturn);
            returnRequest.ReasonForReturn = CommonHelper.EnsureMaximumLength(returnRequest.ReasonForReturn, 400);
            returnRequest.RequestedAction = CommonHelper.EnsureNotNull(returnRequest.RequestedAction);
            returnRequest.RequestedAction = CommonHelper.EnsureMaximumLength(returnRequest.RequestedAction, 400);
            returnRequest.CustomerComments = CommonHelper.EnsureNotNull(returnRequest.CustomerComments);
            returnRequest.StaffNotes = CommonHelper.EnsureNotNull(returnRequest.StaffNotes);

            
            if (!_context.IsAttached(returnRequest))
                _context.ReturnRequests.Attach(returnRequest);

            _context.SaveChanges();
        }

        /// <summary>
        /// Formats the comments text of a return request
        /// </summary>
        /// <param name="text">Text</param>
        /// <returns>Formatted text</returns>
        public string FormatReturnRequestCommentsText(string text)
        {
            if (String.IsNullOrEmpty(text))
                return string.Empty;

            text = HtmlHelper.FormatText(text, false, true, false, false, false, false);
            return text;
        }

        #endregion

        #endregion

        #region Etc
        /// <summary>
        /// Gets a value indicating whether download is allowed
        /// </summary>
        /// <param name="orderProductVariant">Order produvt variant to check</param>
        /// <returns>True if download is allowed; otherwise, false.</returns>
        public bool IsDownloadAllowed(OrderProductVariant orderProductVariant)
        {
            if (orderProductVariant == null)
                return false;

            var order = orderProductVariant.Order;
            if (order == null || order.Deleted)
                return false;

            //order status
            if (order.OrderStatus == OrderStatusEnum.Cancelled)
                return false;

            var productVariant = orderProductVariant.ProductVariant;
            if (productVariant == null || !productVariant.IsDownload)
                return false;

            //payment status
            switch (productVariant.DownloadActivationType)
            {
                case (int)DownloadActivationTypeEnum.WhenOrderIsPaid:
                    {
                        if (order.PaymentStatus == PaymentStatusEnum.Paid && order.PaidDate.HasValue)
                        {
                            //expiration date
                            if (productVariant.DownloadExpirationDays.HasValue)
                            {
                                if (order.PaidDate.Value.AddDays(productVariant.DownloadExpirationDays.Value) > DateTime.UtcNow)
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                return true;
                            }
                        }
                    }
                    break;
                case (int)DownloadActivationTypeEnum.Manually:
                    {
                        if (orderProductVariant.IsDownloadActivated)
                        {
                            //expiration date
                            if (productVariant.DownloadExpirationDays.HasValue)
                            {
                                if (order.CreatedOn.AddDays(productVariant.DownloadExpirationDays.Value) > DateTime.UtcNow)
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                return true;
                            }
                        }
                    }
                    break;
                default:
                    break;
            }

            return false;
        }

        /// <summary>
        /// Gets a value indicating whether license download is allowed
        /// </summary>
        /// <param name="orderProductVariant">Order produvt variant to check</param>
        /// <returns>True if license download is allowed; otherwise, false.</returns>
        public bool IsLicenseDownloadAllowed(OrderProductVariant orderProductVariant)
        {
            if (orderProductVariant == null)
                return false;

            return IsDownloadAllowed(orderProductVariant) && orderProductVariant.LicenseDownloadId > 0;
        }
        
        /// <summary>
        /// Places an order
        /// </summary>
        /// <param name="paymentInfo">Payment info</param>
        /// <param name="customer">Customer</param>
        /// <param name="orderId">Order identifier</param>
        /// <returns>The error status, or String.Empty if no errors</returns>
        public string PlaceOrder(PaymentInfo paymentInfo, Customer customer, 
            out int orderId)
        {
            var orderGuid = Guid.NewGuid();
            return PlaceOrder(paymentInfo, customer, orderGuid, out orderId);
        }

        /// <summary>
        /// Places an order
        /// </summary>
        /// <param name="paymentInfo">Payment info</param>
        /// <param name="customer">Customer</param>
        /// <param name="orderGuid">Order GUID to use</param>
        /// <param name="orderId">Order identifier</param>
        /// <returns>The error status, or String.Empty if no errors</returns>
        public string PlaceOrder(PaymentInfo paymentInfo, Customer customer, 
            Guid orderGuid, out int orderId)
        {
            orderId = 0;
            var processPaymentResult = new ProcessPaymentResult();

            var customerService = IoC.Resolve<ICustomerService>();
            var shoppingCartService = IoC.Resolve<IShoppingCartService>();
            var taxService = IoC.Resolve<ITaxService>();
            var currencyService = IoC.Resolve<ICurrencyService>();
            var shippingService= IoC.Resolve<IShippingService>();
            var paymentService = IoC.Resolve<IPaymentService>();
            var productService = IoC.Resolve<IProductService>();
            var discountService = IoC.Resolve<IDiscountService>();
            var localizationManager = IoC.Resolve<ILocalizationManager>();
            var messageService = IoC.Resolve<IMessageService>();
            var customerActivityService = IoC.Resolve<ICustomerActivityService>();
            var smsService = IoC.Resolve<ISMSService>();
            var logService = IoC.Resolve<ILogService>();

            try
            {
                if (customer == null)
                    throw new ArgumentNullException("customer");

                if (customer.IsGuest && !customerService.AnonymousCheckoutAllowed)
                    throw new NopException("Anonymous checkout is not allowed");

                if (!CommonHelper.IsValidEmail(customer.Email))
                {
                    throw new NopException("Email is not valid");
                }

                if (paymentInfo == null)
                    throw new ArgumentNullException("paymentInfo");

                Order initialOrder = null;
                if (paymentInfo.IsRecurringPayment)
                {
                    initialOrder = GetOrderById(paymentInfo.InitialOrderId);
                    if (initialOrder == null)
                        throw new NopException("Initial order could not be loaded");
                }

                if (!paymentInfo.IsRecurringPayment)
                {
                    if (paymentInfo.BillingAddress == null)
                        throw new NopException("Billing address not provided");

                    if (!CommonHelper.IsValidEmail(paymentInfo.BillingAddress.Email))
                    {
                        throw new NopException("Email is not valid");
                    }
                }

                if (paymentInfo.IsRecurringPayment)
                {
                    paymentInfo.PaymentMethodId = initialOrder.PaymentMethodId;
                }

                paymentInfo.CreditCardType = CommonHelper.EnsureNotNull(paymentInfo.CreditCardType);
                paymentInfo.CreditCardName = CommonHelper.EnsureNotNull(paymentInfo.CreditCardName);
                paymentInfo.CreditCardName = CommonHelper.EnsureMaximumLength(paymentInfo.CreditCardName, 100);
                paymentInfo.CreditCardName = paymentInfo.CreditCardName.Trim();
                paymentInfo.CreditCardNumber = CommonHelper.EnsureNotNull(paymentInfo.CreditCardNumber);
                paymentInfo.CreditCardNumber = paymentInfo.CreditCardNumber.Trim();
                paymentInfo.CreditCardCvv2 = CommonHelper.EnsureNotNull(paymentInfo.CreditCardCvv2);
                paymentInfo.CreditCardCvv2 = paymentInfo.CreditCardCvv2.Trim();
                paymentInfo.PaypalToken = CommonHelper.EnsureNotNull(paymentInfo.PaypalToken);
                paymentInfo.PaypalPayerId = CommonHelper.EnsureNotNull(paymentInfo.PaypalPayerId);
                paymentInfo.GoogleOrderNumber = CommonHelper.EnsureNotNull(paymentInfo.GoogleOrderNumber);                
                paymentInfo.PurchaseOrderNumber = CommonHelper.EnsureNotNull(paymentInfo.PurchaseOrderNumber);

                ShoppingCart cart = null;
                if (!paymentInfo.IsRecurringPayment)
                {
                    cart = shoppingCartService.GetCustomerShoppingCart(customer.CustomerId, ShoppingCartTypeEnum.ShoppingCart);

                    //validate cart
                    var warnings = shoppingCartService.GetShoppingCartWarnings(cart, customer.CheckoutAttributes, true);
                    if (warnings.Count > 0)
                    {
                        StringBuilder warningsSb = new StringBuilder();
                        foreach (string warning in warnings)
                        {
                            warningsSb.Append(warning);
                            warningsSb.Append(";");
                        }
                        throw new NopException(warningsSb.ToString());
                    }

                    //validate individual cart items
                    foreach (var sci in cart)
                    {
                        var sciWarnings = shoppingCartService.GetShoppingCartItemWarnings(sci.ShoppingCartType,
                            sci.ProductVariantId, sci.AttributesXml, 
                            sci.CustomerEnteredPrice, sci.Quantity);

                        if (sciWarnings.Count > 0)
                        {
                            var warningsSb = new StringBuilder();
                            foreach (string warning in sciWarnings)
                            {
                                warningsSb.Append(warning);
                                warningsSb.Append(";");
                            }
                            throw new NopException(warningsSb.ToString());
                        }
                    }
                }

                //tax type
                var customerTaxDisplayType = TaxDisplayTypeEnum.IncludingTax;
                if (!paymentInfo.IsRecurringPayment)
                {
                    if (taxService.AllowCustomersToSelectTaxDisplayType)
                        customerTaxDisplayType = customer.TaxDisplayType;
                    else
                        customerTaxDisplayType = taxService.TaxDisplayType;
                }
                else
                {
                    customerTaxDisplayType = initialOrder.CustomerTaxDisplayType;
                }

                //discount usage history
                var appliedDiscounts = new List<Discount>();

                //checkout attributes
                string checkoutAttributeDescription = string.Empty;
                string checkoutAttributesXml = string.Empty;
                if (!paymentInfo.IsRecurringPayment)
                {
                    checkoutAttributeDescription = CheckoutAttributeHelper.FormatAttributes(customer.CheckoutAttributes, customer, "<br />");
                    checkoutAttributesXml = customer.CheckoutAttributes;
                }
                else
                {
                    checkoutAttributeDescription = initialOrder.CheckoutAttributeDescription;
                    checkoutAttributesXml = initialOrder.CheckoutAttributesXml;
                }

                //sub total
                decimal orderSubTotalInclTax = decimal.Zero;
                decimal orderSubTotalExclTax = decimal.Zero;
                decimal orderSubtotalInclTaxInCustomerCurrency = decimal.Zero;
                decimal orderSubtotalExclTaxInCustomerCurrency = decimal.Zero;
                decimal orderSubTotalDiscountInclTax = decimal.Zero;
                decimal orderSubTotalDiscountExclTax = decimal.Zero;
                decimal orderSubTotalDiscountInclTaxInCustomerCurrency = decimal.Zero;
                decimal orderSubTotalDiscountExclTaxInCustomerCurrency = decimal.Zero;
                if (!paymentInfo.IsRecurringPayment)
                {
                    //sub total (incl tax)
                    decimal orderSubTotalDiscountAmount1 = decimal.Zero;
                    Discount orderSubTotalAppliedDiscount1 = null;
                    decimal subTotalWithoutDiscountBase1 = decimal.Zero;
                    decimal subTotalWithDiscountBase1 = decimal.Zero;
                    string subTotalError1 = shoppingCartService.GetShoppingCartSubTotal(cart, customer,
                        true, out orderSubTotalDiscountAmount1, out orderSubTotalAppliedDiscount1,
                        out subTotalWithoutDiscountBase1, out subTotalWithDiscountBase1);
                    orderSubTotalInclTax = subTotalWithoutDiscountBase1;
                    orderSubTotalDiscountInclTax =orderSubTotalDiscountAmount1;
                    
                    //discount history
                    if (orderSubTotalAppliedDiscount1 != null && !appliedDiscounts.ContainsDiscount(orderSubTotalAppliedDiscount1.Name))
                        appliedDiscounts.Add(orderSubTotalAppliedDiscount1);
                        
                    //sub total (excl tax)
                    decimal orderSubTotalDiscountAmount2 = decimal.Zero;
                    Discount orderSubTotalAppliedDiscount2 = null;
                    decimal subTotalWithoutDiscountBase2 = decimal.Zero;
                    decimal subTotalWithDiscountBase2 = decimal.Zero;
                    string subTotalError2 = shoppingCartService.GetShoppingCartSubTotal(cart, customer,
                        false, out orderSubTotalDiscountAmount2, out orderSubTotalAppliedDiscount2,
                        out subTotalWithoutDiscountBase2, out subTotalWithDiscountBase2);
                    orderSubTotalExclTax = subTotalWithoutDiscountBase2;
                    orderSubTotalDiscountExclTax = orderSubTotalDiscountAmount2;
                    
                    if (!String.IsNullOrEmpty(subTotalError1) || !String.IsNullOrEmpty(subTotalError2))
                        throw new NopException("Sub total couldn't be calculated");
                    
                    //in customer currency
                    orderSubtotalInclTaxInCustomerCurrency = currencyService.ConvertCurrency(orderSubTotalInclTax, currencyService.PrimaryStoreCurrency, paymentInfo.CustomerCurrency);
                    orderSubtotalExclTaxInCustomerCurrency = currencyService.ConvertCurrency(orderSubTotalExclTax, currencyService.PrimaryStoreCurrency, paymentInfo.CustomerCurrency);
                    orderSubTotalDiscountInclTaxInCustomerCurrency = currencyService.ConvertCurrency(orderSubTotalDiscountInclTax, currencyService.PrimaryStoreCurrency, paymentInfo.CustomerCurrency);
                    orderSubTotalDiscountExclTaxInCustomerCurrency = currencyService.ConvertCurrency(orderSubTotalDiscountExclTax, currencyService.PrimaryStoreCurrency, paymentInfo.CustomerCurrency);
                }
                else
                {
                    orderSubTotalInclTax = initialOrder.OrderSubtotalInclTax;
                    orderSubTotalExclTax = initialOrder.OrderSubtotalExclTax;

                    //in customer currency
                    orderSubtotalInclTaxInCustomerCurrency = initialOrder.OrderSubtotalInclTaxInCustomerCurrency;
                    orderSubtotalExclTaxInCustomerCurrency = initialOrder.OrderSubtotalExclTaxInCustomerCurrency;
                }

                //shipping info
                decimal orderWeight = decimal.Zero;
                bool shoppingCartRequiresShipping = false;
                if (!paymentInfo.IsRecurringPayment)
                {
                    orderWeight = shippingService.GetShoppingCartTotalWeight(cart, customer);
                    shoppingCartRequiresShipping = shippingService.ShoppingCartRequiresShipping(cart);
                    if (shoppingCartRequiresShipping)
                    {
                        if (paymentInfo.ShippingAddress == null)
                            throw new NopException("Shipping address is not provided");

                        if (!CommonHelper.IsValidEmail(paymentInfo.ShippingAddress.Email))
                        {
                            throw new NopException("Email is not valid");
                        }
                    }
                }
                else
                {
                    orderWeight = initialOrder.OrderWeight;
                    if (initialOrder.ShippingStatus != ShippingStatusEnum.ShippingNotRequired)
                        shoppingCartRequiresShipping = true;
                }


                //shipping total
                decimal? orderShippingTotalInclTax = null;
                decimal? orderShippingTotalExclTax = null;
                decimal orderShippingInclTaxInCustomerCurrency = decimal.Zero;
                decimal orderShippingExclTaxInCustomerCurrency = decimal.Zero;
                if (!paymentInfo.IsRecurringPayment)
                {
                    decimal taxRate = decimal.Zero;
                    string shippingTotalError1 = string.Empty;
                    string shippingTotalError2 = string.Empty;
                    Discount shippingTotalDiscount = null;
                    orderShippingTotalInclTax = shippingService.GetShoppingCartShippingTotal(cart, customer, true, out taxRate, out shippingTotalDiscount, ref shippingTotalError1);
                    orderShippingTotalExclTax = shippingService.GetShoppingCartShippingTotal(cart, customer, false, ref shippingTotalError2);
                    if (!orderShippingTotalInclTax.HasValue || !orderShippingTotalExclTax.HasValue)
                        throw new NopException("Shipping total couldn't be calculated");
                    if (shippingTotalDiscount != null && !appliedDiscounts.ContainsDiscount(shippingTotalDiscount.Name))
                        appliedDiscounts.Add(shippingTotalDiscount);

                    //in customer currency
                    orderShippingInclTaxInCustomerCurrency = currencyService.ConvertCurrency(orderShippingTotalInclTax.Value, currencyService.PrimaryStoreCurrency, paymentInfo.CustomerCurrency);
                    orderShippingExclTaxInCustomerCurrency = currencyService.ConvertCurrency(orderShippingTotalExclTax.Value, currencyService.PrimaryStoreCurrency, paymentInfo.CustomerCurrency);

                }
                else
                {
                    orderShippingTotalInclTax = initialOrder.OrderShippingInclTax;
                    orderShippingTotalExclTax = initialOrder.OrderShippingExclTax;
                    orderShippingInclTaxInCustomerCurrency = initialOrder.OrderShippingInclTaxInCustomerCurrency;
                    orderShippingExclTaxInCustomerCurrency = initialOrder.OrderShippingExclTaxInCustomerCurrency;
                }


                //payment total
                decimal paymentAdditionalFeeInclTax = decimal.Zero;
                decimal paymentAdditionalFeeExclTax = decimal.Zero;
                decimal paymentAdditionalFeeInclTaxInCustomerCurrency = decimal.Zero;
                decimal paymentAdditionalFeeExclTaxInCustomerCurrency = decimal.Zero;
                if (!paymentInfo.IsRecurringPayment)
                {
                    string paymentAdditionalFeeError1 = string.Empty;
                    string paymentAdditionalFeeError2 = string.Empty;
                    decimal paymentAdditionalFee = paymentService.GetAdditionalHandlingFee(paymentInfo.PaymentMethodId);
                    paymentAdditionalFeeInclTax = taxService.GetPaymentMethodAdditionalFee(paymentAdditionalFee, true, customer, ref paymentAdditionalFeeError1);
                    paymentAdditionalFeeExclTax = taxService.GetPaymentMethodAdditionalFee(paymentAdditionalFee, false, customer, ref paymentAdditionalFeeError2);
                    if (!String.IsNullOrEmpty(paymentAdditionalFeeError1))
                        throw new NopException("Payment method fee couldn't be calculated");
                    if (!String.IsNullOrEmpty(paymentAdditionalFeeError2))
                        throw new NopException("Payment method fee couldn't be calculated");

                    //in customer currency
                    paymentAdditionalFeeInclTaxInCustomerCurrency = currencyService.ConvertCurrency(paymentAdditionalFeeInclTax, currencyService.PrimaryStoreCurrency, paymentInfo.CustomerCurrency);
                    paymentAdditionalFeeExclTaxInCustomerCurrency = currencyService.ConvertCurrency(paymentAdditionalFeeExclTax, currencyService.PrimaryStoreCurrency, paymentInfo.CustomerCurrency);
                }
                else
                {
                    paymentAdditionalFeeInclTax = initialOrder.PaymentMethodAdditionalFeeInclTax;
                    paymentAdditionalFeeExclTax = initialOrder.PaymentMethodAdditionalFeeExclTax;
                    paymentAdditionalFeeInclTaxInCustomerCurrency = initialOrder.PaymentMethodAdditionalFeeInclTaxInCustomerCurrency;
                    paymentAdditionalFeeExclTaxInCustomerCurrency = initialOrder.PaymentMethodAdditionalFeeExclTaxInCustomerCurrency;
                }


                //tax total
                decimal orderTaxTotal = decimal.Zero;
                decimal orderTaxInCustomerCurrency = decimal.Zero;
                string vatNumber = string.Empty;
                string taxRates = string.Empty;
                string taxRatesInCustomerCurrency = string.Empty;
                if (!paymentInfo.IsRecurringPayment)
                {
                    //tax amount
                    SortedDictionary<decimal, decimal> taxRatesDictionary = null;
                    string taxError = string.Empty;
                    orderTaxTotal = taxService.GetTaxTotal(cart,
                        paymentInfo.PaymentMethodId, customer, out taxRatesDictionary, ref taxError);
                    if (!String.IsNullOrEmpty(taxError))
                        throw new NopException("Tax total couldn't be calculated");

                    //in customer currency
                    orderTaxInCustomerCurrency = currencyService.ConvertCurrency(orderTaxTotal, currencyService.PrimaryStoreCurrency, paymentInfo.CustomerCurrency);

                    //VAT number
                    if (taxService.EUVatEnabled && customer.VatNumberStatus == VatNumberStatusEnum.Valid)
                    {
                        vatNumber = customer.VatNumber;
                    }

                    //tax rates
                    foreach (var kvp in taxRatesDictionary)
                    {
                        var taxRate = kvp.Key;
                        var taxValue = kvp.Value;

                        var taxValueInCustomerCurrency = currencyService.ConvertCurrency(taxValue, currencyService.PrimaryStoreCurrency, paymentInfo.CustomerCurrency);

                        taxRates += string.Format("{0}:{1};   ", taxRate.ToString(new CultureInfo("en-US")), taxValue.ToString(new CultureInfo("en-US")));
                        taxRatesInCustomerCurrency += string.Format("{0}:{1};   ", taxRate.ToString(new CultureInfo("en-US")), taxValueInCustomerCurrency.ToString(new CultureInfo("en-US")));
                    }
                }
                else
                {
                    orderTaxTotal = initialOrder.OrderTax;
                    orderTaxInCustomerCurrency = initialOrder.OrderTaxInCustomerCurrency;

                    //VAT number
                    //TODO: Possible BUG: VAT number status may have changed since original order was placed, probably best to recalculate tax or do some checks?
                    vatNumber = initialOrder.VatNumber;
                }


                //order total (and applied discounts, gift cards, reward points)
                decimal? orderTotal = null;
                decimal orderTotalInCustomerCurrency = decimal.Zero;
                decimal orderDiscountAmount = decimal.Zero;
                decimal orderDiscountInCustomerCurrency = decimal.Zero;
                List<AppliedGiftCard> appliedGiftCards = null;
                int redeemedRewardPoints = 0;
                decimal redeemedRewardPointsAmount = decimal.Zero;
                if (!paymentInfo.IsRecurringPayment)
                {
                    Discount orderAppliedDiscount = null;

                    bool useRewardPoints = customer.UseRewardPointsDuringCheckout;

                    orderTotal = shoppingCartService.GetShoppingCartTotal(cart,
                        paymentInfo.PaymentMethodId, customer,
                        out orderDiscountAmount, out orderAppliedDiscount, 
                        out appliedGiftCards, useRewardPoints,
                        out redeemedRewardPoints, out redeemedRewardPointsAmount);
                    if (!orderTotal.HasValue)
                        throw new NopException("Order total couldn't be calculated");

                    //discount history
                    if (orderAppliedDiscount != null && !appliedDiscounts.ContainsDiscount(orderAppliedDiscount.Name))
                        appliedDiscounts.Add(orderAppliedDiscount);

                    //in customer currency
                    orderDiscountInCustomerCurrency = currencyService.ConvertCurrency(orderDiscountAmount, currencyService.PrimaryStoreCurrency, paymentInfo.CustomerCurrency);
                    orderTotalInCustomerCurrency = currencyService.ConvertCurrency(orderTotal.Value, currencyService.PrimaryStoreCurrency, paymentInfo.CustomerCurrency);
                }
                else
                {
                    orderDiscountAmount = initialOrder.OrderDiscount;
                    orderTotal = initialOrder.OrderTotal;

                    orderDiscountInCustomerCurrency = initialOrder.OrderDiscountInCustomerCurrency;
                    orderTotalInCustomerCurrency = initialOrder.OrderTotalInCustomerCurrency;
                }
                paymentInfo.OrderTotal = orderTotal.Value;

                //skip payment workflow if order total equals zero
                bool skipPaymentWorkflow = false;
                if (orderTotal.Value == decimal.Zero)
                {
                    skipPaymentWorkflow = true;
                }
                PaymentMethod paymentMethod = null;
                string paymentMethodName = string.Empty;
                if (!skipPaymentWorkflow)
                {
                    paymentMethod = paymentService.GetPaymentMethodById(paymentInfo.PaymentMethodId);
                    if (paymentMethod == null)
                        throw new NopException("Payment method couldn't be loaded");

                    if (!paymentMethod.IsActive)
                        throw new NopException("Payment method is not active");

                    paymentMethodName = paymentMethod.Name;
                }
                else
                {
                    paymentInfo.PaymentMethodId = 0;
                }

                string customerCurrencyCode = string.Empty;
                if (!paymentInfo.IsRecurringPayment)
                {
                    customerCurrencyCode = paymentInfo.CustomerCurrency.CurrencyCode;
                }
                else
                {
                    customerCurrencyCode = initialOrder.CustomerCurrencyCode;
                }

                //billing info
                string billingFirstName = string.Empty;
                string billingLastName = string.Empty;
                string billingPhoneNumber = string.Empty;
                string billingEmail = string.Empty;
                string billingFaxNumber = string.Empty;
                string billingCompany = string.Empty;
                string billingAddress1 = string.Empty;
                string billingAddress2 = string.Empty;
                string billingCity = string.Empty;
                string billingStateProvince = string.Empty;
                int billingStateProvinceId = 0;
                string billingZipPostalCode = string.Empty;
                string billingCountry = string.Empty;
                int billingCountryId = 0;
                if (!paymentInfo.IsRecurringPayment)
                {
                    var billingAddress = paymentInfo.BillingAddress;
                    billingFirstName = billingAddress.FirstName;
                    billingLastName = billingAddress.LastName;
                    billingPhoneNumber = billingAddress.PhoneNumber;
                    billingEmail = billingAddress.Email;
                    billingFaxNumber = billingAddress.FaxNumber;
                    billingCompany = billingAddress.Company;
                    billingAddress1 = billingAddress.Address1;
                    billingAddress2 = billingAddress.Address2;
                    billingCity = billingAddress.City;
                    if (billingAddress.StateProvince != null)
                    {
                        billingStateProvince = billingAddress.StateProvince.Name;
                        billingStateProvinceId = billingAddress.StateProvince.StateProvinceId;
                    }
                    billingZipPostalCode = billingAddress.ZipPostalCode;
                    if (billingAddress.Country != null)
                    {
                        billingCountry = billingAddress.Country.Name;
                        billingCountryId = billingAddress.Country.CountryId;

                        if (!billingAddress.Country.AllowsBilling)
                        {
                            throw new NopException(string.Format("{0} is not allowed for billing", billingCountry));
                        }
                    }
                }
                else
                {
                    billingFirstName = initialOrder.BillingFirstName;
                    billingLastName = initialOrder.BillingLastName;
                    billingPhoneNumber = initialOrder.BillingPhoneNumber;
                    billingEmail = initialOrder.BillingEmail;
                    billingFaxNumber = initialOrder.BillingFaxNumber;
                    billingCompany = initialOrder.BillingCompany;
                    billingAddress1 = initialOrder.BillingAddress1;
                    billingAddress2 = initialOrder.BillingAddress2;
                    billingCity = initialOrder.BillingCity;
                    billingStateProvince = initialOrder.BillingStateProvince;
                    billingStateProvinceId = initialOrder.BillingStateProvinceId;
                    billingZipPostalCode = initialOrder.BillingZipPostalCode;
                    billingCountry = initialOrder.BillingCountry;
                    billingCountryId = initialOrder.BillingCountryId;
                }

                //shipping info
                string shippingFirstName = string.Empty;
                string shippingLastName = string.Empty;
                string shippingPhoneNumber = string.Empty;
                string shippingEmail = string.Empty;
                string shippingFaxNumber = string.Empty;
                string shippingCompany = string.Empty;
                string shippingAddress1 = string.Empty;
                string shippingAddress2 = string.Empty;
                string shippingCity = string.Empty;
                string shippingStateProvince = string.Empty;
                int shippingStateProvinceId = 0;
                string shippingZipPostalCode = string.Empty;
                string shippingCountry = string.Empty;
                int shippingCountryId = 0;
                string shippingMethodName = string.Empty;
                int shippingRateComputationMethodId = 0;
                if (shoppingCartRequiresShipping)
                {
                    if (!paymentInfo.IsRecurringPayment)
                    {
                        var shippingAddress = paymentInfo.ShippingAddress;
                        if (shippingAddress != null)
                        {
                            shippingFirstName = shippingAddress.FirstName;
                            shippingLastName = shippingAddress.LastName;
                            shippingPhoneNumber = shippingAddress.PhoneNumber;
                            shippingEmail = shippingAddress.Email;
                            shippingFaxNumber = shippingAddress.FaxNumber;
                            shippingCompany = shippingAddress.Company;
                            shippingAddress1 = shippingAddress.Address1;
                            shippingAddress2 = shippingAddress.Address2;
                            shippingCity = shippingAddress.City;
                            if (shippingAddress.StateProvince != null)
                            {
                                shippingStateProvince = shippingAddress.StateProvince.Name;
                                shippingStateProvinceId = shippingAddress.StateProvince.StateProvinceId;
                            }
                            shippingZipPostalCode = shippingAddress.ZipPostalCode;
                            if (shippingAddress.Country != null)
                            {
                                shippingCountry = shippingAddress.Country.Name;
                                shippingCountryId = shippingAddress.Country.CountryId;

                                if (!shippingAddress.Country.AllowsShipping)
                                {
                                    throw new NopException(string.Format("{0} is not allowed for shipping", shippingCountry));
                                }
                            }
                            shippingMethodName = string.Empty;
                            var shippingOption = customer.LastShippingOption;
                            if (shippingOption != null)
                            {
                                shippingMethodName = shippingOption.Name;
                                shippingRateComputationMethodId = shippingOption.ShippingRateComputationMethodId;
                            }
                        }
                    }
                    else
                    {
                        shippingFirstName = initialOrder.ShippingFirstName;
                        shippingLastName = initialOrder.ShippingLastName;
                        shippingPhoneNumber = initialOrder.ShippingPhoneNumber;
                        shippingEmail = initialOrder.ShippingEmail;
                        shippingFaxNumber = initialOrder.ShippingFaxNumber;
                        shippingCompany = initialOrder.ShippingCompany;
                        shippingAddress1 = initialOrder.ShippingAddress1;
                        shippingAddress2 = initialOrder.ShippingAddress2;
                        shippingCity = initialOrder.ShippingCity;
                        shippingStateProvince = initialOrder.ShippingStateProvince;
                        shippingStateProvinceId = initialOrder.ShippingStateProvinceId;
                        shippingZipPostalCode = initialOrder.ShippingZipPostalCode;
                        shippingCountry = initialOrder.ShippingCountry;
                        shippingCountryId = initialOrder.ShippingCountryId;
                        shippingMethodName = initialOrder.ShippingMethod;
                        shippingRateComputationMethodId = initialOrder.ShippingRateComputationMethodId;
                    }
                }

                //customer language
                int customerLanguageId = 0;
                if (!paymentInfo.IsRecurringPayment)
                {
                    customerLanguageId = paymentInfo.CustomerLanguage.LanguageId;
                }
                else
                {
                    customerLanguageId = initialOrder.CustomerLanguageId;
                }

                //recurring or standard shopping cart
                bool isRecurringShoppingCart = false;
                int recurringCycleLength = 0;
                int recurringCyclePeriod = 0;
                int recurringTotalCycles = 0;
                if (!paymentInfo.IsRecurringPayment)
                {
                    isRecurringShoppingCart = cart.IsRecurring;
                    if (isRecurringShoppingCart)
                    {
                        string recurringCyclesError = shoppingCartService.GetReccuringCycleInfo(cart, out recurringCycleLength, out recurringCyclePeriod, out recurringTotalCycles);
                        if (!string.IsNullOrEmpty(recurringCyclesError))
                        {
                            throw new NopException(recurringCyclesError);
                        }
                        paymentInfo.RecurringCycleLength = recurringCycleLength;
                        paymentInfo.RecurringCyclePeriod = recurringCyclePeriod;
                        paymentInfo.RecurringTotalCycles = recurringTotalCycles;
                    }
                }
                else
                {
                    isRecurringShoppingCart = true;
                }

                if (!skipPaymentWorkflow)
                {
                    //process payment
                    if (!paymentInfo.IsRecurringPayment)
                    {
                        if (isRecurringShoppingCart)
                        {
                            //recurring cart
                            var recurringPaymentType = paymentService.SupportRecurringPayments(paymentInfo.PaymentMethodId);
                            switch (recurringPaymentType)
                            {
                                case RecurringPaymentTypeEnum.NotSupported:
                                    throw new NopException("Recurring payments are not supported by selected payment method");
                                case RecurringPaymentTypeEnum.Manual:
                                case RecurringPaymentTypeEnum.Automatic:
                                    paymentService.ProcessRecurringPayment(paymentInfo, customer, orderGuid, ref processPaymentResult);
                                    break;
                                default:
                                    throw new NopException("Not supported recurring payment type");
                            }
                        }
                        else
                        {
                            //standard cart
                            paymentService.ProcessPayment(paymentInfo, customer, orderGuid, ref processPaymentResult);
                        }
                    }
                    else
                    {
                        if (isRecurringShoppingCart)
                        {
                            var recurringPaymentType = paymentService.SupportRecurringPayments(paymentInfo.PaymentMethodId);
                            switch (recurringPaymentType)
                            {
                                case RecurringPaymentTypeEnum.NotSupported:
                                    throw new NopException("Recurring payments are not supported by selected payment method");
                                case RecurringPaymentTypeEnum.Manual:
                                    paymentService.ProcessRecurringPayment(paymentInfo, customer, orderGuid, ref processPaymentResult);
                                    break;
                                case RecurringPaymentTypeEnum.Automatic:
                                    //payment is processed on payment gateway site
                                    break;
                                default:
                                    throw new NopException("Not supported recurring payment type");
                            }
                        }
                        else
                        {
                            throw new NopException("No recurring products");
                        }
                    }
                }
                else
                {
                    processPaymentResult.PaymentStatus = PaymentStatusEnum.Paid;
                }

                //process order
                if (String.IsNullOrEmpty(processPaymentResult.Error))
                {
                    var shippingStatusEnum = ShippingStatusEnum.NotYetShipped;
                    if (!shoppingCartRequiresShipping)
                        shippingStatusEnum = ShippingStatusEnum.ShippingNotRequired;

                    //save order in data storage
                    //uncomment this line to support transactions
                    //using (var scope = new System.Transactions.TransactionScope())
                    {
                        var order = new Order()
                        {
                            OrderGuid = orderGuid,
                            CustomerId = customer.CustomerId,
                            CustomerLanguageId = customerLanguageId,
                            CustomerTaxDisplayTypeId = (int)customerTaxDisplayType,
                            CustomerIP = NopContext.Current.UserHostAddress,
                            OrderSubtotalInclTax = orderSubTotalInclTax,
                            OrderSubtotalExclTax = orderSubTotalExclTax,
                            OrderSubTotalDiscountInclTax = orderSubTotalDiscountInclTax,
                            OrderSubTotalDiscountExclTax = orderSubTotalDiscountExclTax,
                            OrderShippingInclTax = orderShippingTotalInclTax.Value,
                            OrderShippingExclTax = orderShippingTotalExclTax.Value,
                            PaymentMethodAdditionalFeeInclTax = paymentAdditionalFeeInclTax,
                            PaymentMethodAdditionalFeeExclTax = paymentAdditionalFeeExclTax,
                            TaxRates = taxRates,
                            OrderTax = orderTaxTotal,
                            OrderTotal = orderTotal.Value,
                            RefundedAmount = decimal.Zero,
                            OrderDiscount = orderDiscountAmount,
                            OrderSubtotalInclTaxInCustomerCurrency = orderSubtotalInclTaxInCustomerCurrency,
                            OrderSubtotalExclTaxInCustomerCurrency = orderSubtotalExclTaxInCustomerCurrency,
                            OrderSubTotalDiscountInclTaxInCustomerCurrency = orderSubTotalDiscountInclTaxInCustomerCurrency,
                            OrderSubTotalDiscountExclTaxInCustomerCurrency = orderSubTotalDiscountExclTaxInCustomerCurrency,
                            OrderShippingInclTaxInCustomerCurrency = orderShippingInclTaxInCustomerCurrency,
                            OrderShippingExclTaxInCustomerCurrency = orderShippingExclTaxInCustomerCurrency,
                            PaymentMethodAdditionalFeeInclTaxInCustomerCurrency = paymentAdditionalFeeInclTaxInCustomerCurrency,
                            PaymentMethodAdditionalFeeExclTaxInCustomerCurrency = paymentAdditionalFeeExclTaxInCustomerCurrency,
                            TaxRatesInCustomerCurrency = taxRatesInCustomerCurrency,
                            OrderTaxInCustomerCurrency = orderTaxInCustomerCurrency,
                            OrderTotalInCustomerCurrency = orderTotalInCustomerCurrency,
                            OrderDiscountInCustomerCurrency = orderDiscountInCustomerCurrency,
                            CheckoutAttributeDescription = checkoutAttributeDescription,
                            CheckoutAttributesXml = checkoutAttributesXml,
                            CustomerCurrencyCode = customerCurrencyCode,
                            OrderWeight = orderWeight,
                            AffiliateId = customer.AffiliateId,
                            OrderStatusId = (int)OrderStatusEnum.Pending,
                            AllowStoringCreditCardNumber = processPaymentResult.AllowStoringCreditCardNumber,
                            CardType = processPaymentResult.AllowStoringCreditCardNumber ? SecurityHelper.Encrypt(paymentInfo.CreditCardType) : string.Empty,
                            CardName = processPaymentResult.AllowStoringCreditCardNumber ? SecurityHelper.Encrypt(paymentInfo.CreditCardName) : string.Empty,
                            CardNumber = processPaymentResult.AllowStoringCreditCardNumber ? SecurityHelper.Encrypt(paymentInfo.CreditCardNumber) : string.Empty,
                            MaskedCreditCardNumber = SecurityHelper.Encrypt(paymentService.GetMaskedCreditCardNumber(paymentInfo.CreditCardNumber)),
                            CardCvv2 = processPaymentResult.AllowStoringCreditCardNumber ? SecurityHelper.Encrypt(paymentInfo.CreditCardCvv2) : string.Empty,
                            CardExpirationMonth = processPaymentResult.AllowStoringCreditCardNumber ? SecurityHelper.Encrypt(paymentInfo.CreditCardExpireMonth.ToString()) : string.Empty,
                            CardExpirationYear = processPaymentResult.AllowStoringCreditCardNumber ? SecurityHelper.Encrypt(paymentInfo.CreditCardExpireYear.ToString()) : string.Empty,
                            PaymentMethodId = paymentInfo.PaymentMethodId,
                            PaymentMethodName = paymentMethodName,
                            AuthorizationTransactionId = processPaymentResult.AuthorizationTransactionId,
                            AuthorizationTransactionCode = processPaymentResult.AuthorizationTransactionCode,
                            AuthorizationTransactionResult = processPaymentResult.AuthorizationTransactionResult,
                            CaptureTransactionId = processPaymentResult.CaptureTransactionId,
                            CaptureTransactionResult = processPaymentResult.CaptureTransactionResult,
                            SubscriptionTransactionId = processPaymentResult.SubscriptionTransactionId,
                            PurchaseOrderNumber = paymentInfo.PurchaseOrderNumber,
                            PaymentStatusId = (int)processPaymentResult.PaymentStatus,
                            PaidDate = null,
                            BillingFirstName = billingFirstName,
                            BillingLastName = billingLastName,
                            BillingPhoneNumber = billingPhoneNumber,
                            BillingEmail = billingEmail,
                            BillingFaxNumber = billingFaxNumber,
                            BillingCompany = billingCompany,
                            BillingAddress1 = billingAddress1,
                            BillingAddress2 = billingAddress2,
                            BillingCity = billingCity,
                            BillingStateProvince = billingStateProvince,
                            BillingStateProvinceId = billingStateProvinceId,
                            BillingZipPostalCode = billingZipPostalCode,
                            BillingCountry = billingCountry,
                            BillingCountryId = billingCountryId,
                            ShippingStatusId = (int)shippingStatusEnum,
                            ShippingFirstName = shippingFirstName,
                            ShippingLastName = shippingLastName,
                            ShippingPhoneNumber = shippingPhoneNumber,
                            ShippingEmail = shippingEmail,
                            ShippingFaxNumber = shippingFaxNumber,
                            ShippingCompany = shippingCompany,
                            ShippingAddress1 = shippingAddress1,
                            ShippingAddress2 = shippingAddress2,
                            ShippingCity = shippingCity,
                            ShippingStateProvince = shippingStateProvince,
                            ShippingStateProvinceId = shippingStateProvinceId,
                            ShippingZipPostalCode = shippingZipPostalCode,
                            ShippingCountry = shippingCountry,
                            ShippingCountryId = shippingCountryId,
                            ShippingMethod = shippingMethodName,
                            ShippingRateComputationMethodId = shippingRateComputationMethodId,
                            ShippedDate = null,
                            DeliveryDate = null,
                            TrackingNumber = string.Empty,
                            VatNumber = vatNumber,
                            Deleted = false,
                            CreatedOn = DateTime.UtcNow
                        };
                        InsertOrder(order);

                        orderId = order.OrderId;

                        if (!paymentInfo.IsRecurringPayment)
                        {
                            //move shopping cart items to order product variants
                            foreach (var sc in cart)
                            {
                                //prices
                                decimal taxRate = decimal.Zero;
                                decimal scUnitPrice = PriceHelper.GetUnitPrice(sc, customer, true);
                                decimal scSubTotal = PriceHelper.GetSubTotal(sc, customer, true);
                                decimal scUnitPriceInclTax = taxService.GetPrice(sc.ProductVariant, scUnitPrice, true, customer, out taxRate);
                                decimal scUnitPriceExclTax = taxService.GetPrice(sc.ProductVariant, scUnitPrice, false, customer, out taxRate);
                                decimal scSubTotalInclTax = taxService.GetPrice(sc.ProductVariant, scSubTotal, true, customer, out taxRate);
                                decimal scSubTotalExclTax = taxService.GetPrice(sc.ProductVariant, scSubTotal, false, customer, out taxRate);
                                decimal scUnitPriceInclTaxInCustomerCurrency = currencyService.ConvertCurrency(scUnitPriceInclTax, currencyService.PrimaryStoreCurrency, paymentInfo.CustomerCurrency);
                                decimal scUnitPriceExclTaxInCustomerCurrency = currencyService.ConvertCurrency(scUnitPriceExclTax, currencyService.PrimaryStoreCurrency, paymentInfo.CustomerCurrency);
                                decimal scSubTotalInclTaxInCustomerCurrency = currencyService.ConvertCurrency(scSubTotalInclTax, currencyService.PrimaryStoreCurrency, paymentInfo.CustomerCurrency);
                                decimal scSubTotalExclTaxInCustomerCurrency = currencyService.ConvertCurrency(scSubTotalExclTax, currencyService.PrimaryStoreCurrency, paymentInfo.CustomerCurrency);

                                //discounts
                                Discount scDiscount = null;
                                decimal discountAmount = PriceHelper.GetDiscountAmount(sc, customer, out scDiscount);
                                decimal discountAmountInclTax = taxService.GetPrice(sc.ProductVariant, discountAmount, true, customer, out taxRate);
                                decimal discountAmountExclTax = taxService.GetPrice(sc.ProductVariant, discountAmount, false, customer, out taxRate);
                                if (scDiscount != null && !appliedDiscounts.ContainsDiscount(scDiscount.Name))
                                    appliedDiscounts.Add(scDiscount);

                                //attributes
                                string attributeDescription = ProductAttributeHelper.FormatAttributes(sc.ProductVariant, sc.AttributesXml, customer, "<br />");

                                //save item
                                var opv = new OrderProductVariant()
                                {
                                    OrderProductVariantGuid = Guid.NewGuid(),
                                    OrderId = order.OrderId,
                                    ProductVariantId = sc.ProductVariantId,
                                    UnitPriceInclTax = scUnitPriceInclTax,
                                    UnitPriceExclTax = scUnitPriceExclTax,
                                    PriceInclTax = scSubTotalInclTax,
                                    PriceExclTax = scSubTotalExclTax,
                                    UnitPriceInclTaxInCustomerCurrency = scUnitPriceInclTaxInCustomerCurrency,
                                    UnitPriceExclTaxInCustomerCurrency = scUnitPriceExclTaxInCustomerCurrency,
                                    PriceInclTaxInCustomerCurrency = scSubTotalInclTaxInCustomerCurrency,
                                    PriceExclTaxInCustomerCurrency = scSubTotalExclTaxInCustomerCurrency,
                                    AttributeDescription = attributeDescription,
                                    AttributesXml = sc.AttributesXml,
                                    Quantity = sc.Quantity,
                                    DiscountAmountInclTax = discountAmountInclTax,
                                    DiscountAmountExclTax = discountAmountExclTax,
                                    DownloadCount = 0,
                                    IsDownloadActivated = false,
                                    LicenseDownloadId = 0
                                };
                                InsertOrderProductVariant(opv);

                                //gift cards
                                if (sc.ProductVariant.IsGiftCard)
                                {
                                    string giftCardRecipientName = string.Empty;
                                    string giftCardRecipientEmail = string.Empty;
                                    string giftCardSenderName = string.Empty;
                                    string giftCardSenderEmail = string.Empty;
                                    string giftCardMessage = string.Empty;
                                    ProductAttributeHelper.GetGiftCardAttribute(sc.AttributesXml,
                                        out giftCardRecipientName, out giftCardRecipientEmail,
                                        out giftCardSenderName, out giftCardSenderEmail, out giftCardMessage);

                                    for (int i = 0; i < sc.Quantity; i++)
                                    {
                                        var gc = new GiftCard()
                                        {
                                            PurchasedOrderProductVariantId = opv.OrderProductVariantId,
                                            Amount = scUnitPriceExclTax,
                                            IsGiftCardActivated = false,
                                            GiftCardCouponCode = GiftCardHelper.GenerateGiftCardCode(),
                                            RecipientName = giftCardRecipientName,
                                            RecipientEmail = giftCardRecipientEmail,
                                            SenderName = giftCardSenderName,
                                            SenderEmail = giftCardSenderEmail,
                                            Message = giftCardMessage,
                                            IsRecipientNotified = false,
                                            CreatedOn = DateTime.UtcNow
                                        };
                                        InsertGiftCard(gc);
                                    }
                                }

                                //inventory
                                productService.AdjustInventory(sc.ProductVariantId, true, sc.Quantity, sc.AttributesXml);
                            }

                            //clear shopping cart
                            foreach (var sc in cart)
                            {
                                shoppingCartService.DeleteShoppingCartItem(sc.ShoppingCartItemId, false);
                            }
                        }
                        else
                        {
                            var initialOrderProductVariants = initialOrder.OrderProductVariants;
                            foreach (var opv in initialOrderProductVariants)
                            {
                                //save item
                                var newOpv = new OrderProductVariant()
                                {
                                    OrderProductVariantGuid = Guid.NewGuid(),
                                    OrderId = order.OrderId,
                                    ProductVariantId = opv.ProductVariantId,
                                    UnitPriceInclTax = opv.UnitPriceInclTax,
                                    UnitPriceExclTax = opv.UnitPriceExclTax,
                                    PriceInclTax = opv.PriceInclTax,
                                    PriceExclTax = opv.PriceExclTax,
                                    UnitPriceInclTaxInCustomerCurrency = opv.UnitPriceInclTaxInCustomerCurrency,
                                    UnitPriceExclTaxInCustomerCurrency = opv.UnitPriceExclTaxInCustomerCurrency,
                                    PriceInclTaxInCustomerCurrency =  opv.PriceInclTaxInCustomerCurrency,
                                    PriceExclTaxInCustomerCurrency = opv.PriceExclTaxInCustomerCurrency,
                                    AttributeDescription = opv.AttributeDescription,
                                    AttributesXml = opv.AttributesXml,
                                    Quantity = opv.Quantity,
                                    DiscountAmountInclTax = opv.DiscountAmountInclTax,
                                    DiscountAmountExclTax =  opv.DiscountAmountExclTax,
                                    DownloadCount = 0,
                                    IsDownloadActivated = false,
                                    LicenseDownloadId = 0
                                };
                                
                                InsertOrderProductVariant(newOpv);

                                //gift cards
                                if (opv.ProductVariant.IsGiftCard)
                                {
                                    string giftCardRecipientName = string.Empty;
                                    string giftCardRecipientEmail = string.Empty;
                                    string giftCardSenderName = string.Empty;
                                    string giftCardSenderEmail = string.Empty;
                                    string giftCardMessage = string.Empty;
                                    ProductAttributeHelper.GetGiftCardAttribute(opv.AttributesXml,
                                        out giftCardRecipientName, out giftCardRecipientEmail,
                                        out giftCardSenderName, out giftCardSenderEmail, out giftCardMessage);

                                    for (int i = 0; i < opv.Quantity; i++)
                                    {
                                        var gc = new GiftCard()
                                        {
                                            PurchasedOrderProductVariantId = newOpv.OrderProductVariantId,
                                            Amount = opv.UnitPriceExclTax,
                                            IsGiftCardActivated = false,
                                            GiftCardCouponCode = GiftCardHelper.GenerateGiftCardCode(),
                                            RecipientName = giftCardRecipientName,
                                            RecipientEmail = giftCardRecipientEmail,
                                            SenderName = giftCardSenderName,
                                            SenderEmail = giftCardSenderEmail,
                                            Message = giftCardMessage,
                                            IsRecipientNotified = false,
                                            CreatedOn = DateTime.UtcNow
                                        };
                                        InsertGiftCard(gc);
                                    }
                                }

                                //inventory
                                productService.AdjustInventory(opv.ProductVariantId, true, opv.Quantity, opv.AttributesXml);
                            }
                        }

                        //discount usage history
                        if (!paymentInfo.IsRecurringPayment)
                        {
                            foreach (var discount in appliedDiscounts)
                            {
                                var duh = new DiscountUsageHistory()
                                {
                                    DiscountId = discount.DiscountId,
                                    CustomerId = customer.CustomerId,
                                    OrderId = order.OrderId,
                                    CreatedOn = DateTime.UtcNow
                                };
                                discountService.InsertDiscountUsageHistory(duh);
                            }
                        }

                        //gift card usage history
                        if (!paymentInfo.IsRecurringPayment)
                        {
                            if (appliedGiftCards != null)
                            {
                                foreach (var agc in appliedGiftCards)
                                {
                                    decimal amountUsed = agc.AmountCanBeUsed;
                                    decimal amountUsedInCustomerCurrency = currencyService.ConvertCurrency(amountUsed, currencyService.PrimaryStoreCurrency, paymentInfo.CustomerCurrency);
                                    var gcuh = new GiftCardUsageHistory()
                                    {
                                        GiftCardId = agc.GiftCardId,
                                        CustomerId = customer.CustomerId,
                                        OrderId = order.OrderId,
                                        UsedValue = amountUsed,
                                        UsedValueInCustomerCurrency = amountUsedInCustomerCurrency,
                                        CreatedOn = DateTime.UtcNow
                                    };
                                    InsertGiftCardUsageHistory(gcuh);
                                }
                            }
                        }

                        //reward points history
                        if (redeemedRewardPointsAmount > decimal.Zero)
                        {
                            decimal redeemedRewardPointsAmountInCustomerCurrency = currencyService.ConvertCurrency(redeemedRewardPointsAmount, currencyService.PrimaryStoreCurrency, paymentInfo.CustomerCurrency);
                            string message = string.Format(localizationManager.GetLocaleResourceString("RewardPoints.Message.RedeemedForOrder", order.CustomerLanguageId), order.OrderId);

                            RewardPointsHistory rph = this.InsertRewardPointsHistory(customer.CustomerId,
                                order.OrderId, -redeemedRewardPoints,
                                redeemedRewardPointsAmount,
                                redeemedRewardPointsAmountInCustomerCurrency,
                                customerCurrencyCode,
                                message,
                                DateTime.UtcNow);
                        }

                        //recurring orders
                        if (!paymentInfo.IsRecurringPayment)
                        {
                            if (isRecurringShoppingCart)
                            {
                                //create recurring payment
                                var rp = new RecurringPayment()
                                {
                                    InitialOrderId = order.OrderId,
                                    CycleLength = paymentInfo.RecurringCycleLength,
                                    CyclePeriod = paymentInfo.RecurringCyclePeriod,
                                    TotalCycles = paymentInfo.RecurringTotalCycles,
                                    StartDate = DateTime.UtcNow,
                                    IsActive = true,
                                    Deleted = false,
                                    CreatedOn = DateTime.UtcNow
                                };
                                InsertRecurringPayment(rp);


                                var recurringPaymentType = paymentService.SupportRecurringPayments(paymentInfo.PaymentMethodId);
                                switch (recurringPaymentType)
                                {
                                    case RecurringPaymentTypeEnum.NotSupported:
                                        {
                                            //not supported
                                        }
                                        break;
                                    case RecurringPaymentTypeEnum.Manual:
                                        {
                                            //first payment
                                            var rph = new RecurringPaymentHistory()
                                            {
                                                RecurringPaymentId = rp.RecurringPaymentId,
                                                OrderId = order.OrderId,
                                                CreatedOn = DateTime.UtcNow
                                            };
                                            InsertRecurringPaymentHistory(rph);
                                        }
                                        break;
                                    case RecurringPaymentTypeEnum.Automatic:
                                        {
                                            //will be created later (process is automated)
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }


                        //notes, messages
                        InsertOrderNote(order.OrderId, string.Format("Order placed"), false, DateTime.UtcNow);

                        int orderPlacedStoreOwnerNotificationQueuedEmailId = messageService.SendOrderPlacedStoreOwnerNotification(order, localizationManager.DefaultAdminLanguage.LanguageId);
                        if (orderPlacedStoreOwnerNotificationQueuedEmailId > 0)
                        {
                            InsertOrderNote(order.OrderId, string.Format("\"Order placed\" email (to store owner) has been queued. Queued email identifier: {0}.", orderPlacedStoreOwnerNotificationQueuedEmailId), false, DateTime.UtcNow);
                        }

                        int orderPlacedCustomerNotificationQueuedEmailId = messageService.SendOrderPlacedCustomerNotification(order, order.CustomerLanguageId);
                        if (orderPlacedCustomerNotificationQueuedEmailId > 0)
                        {
                            InsertOrderNote(order.OrderId, string.Format("\"Order placed\" email (to customer) has been queued. Queued email identifier: {0}.", orderPlacedCustomerNotificationQueuedEmailId), false, DateTime.UtcNow);
                        }

                        smsService.SendOrderPlacedNotification(order);

                        //order status
                        order = CheckOrderStatus(order.OrderId);

                        //reset checkout data
                        if (!paymentInfo.IsRecurringPayment)
                        {
                            customerService.ResetCheckoutData(customer.CustomerId, true);
                        }

                        //log
                        if (!paymentInfo.IsRecurringPayment)
                        {
                            customerActivityService.InsertActivity(
                                "PlaceOrder",
                                localizationManager.GetLocaleResourceString("ActivityLog.PlaceOrder"),
                                order.OrderId);
                        }

                        //uncomment this line to support transactions
                        //scope.Complete();


                        //raise event             
                        EventContext.Current.OnOrderPlaced(null,
                            new OrderEventArgs() { Order = order });

                        //raise event         
                        if (order.PaymentStatus == PaymentStatusEnum.Paid)
                        {
                            EventContext.Current.OnOrderPaid(null,
                                new OrderEventArgs() { Order = order });
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                processPaymentResult.Error = exc.Message;
                processPaymentResult.FullError = exc.ToString();
            }

            if (!String.IsNullOrEmpty(processPaymentResult.Error))
            {
                logService.InsertLog(LogTypeEnum.OrderError, string.Format("Error while placing order. {0}", processPaymentResult.Error), processPaymentResult.FullError);
            }
            return processPaymentResult.Error;
        }

        /// <summary>
        /// Place order items in current user shopping cart.
        /// </summary>
        /// <param name="orderId">The order identifier</param>
        public void ReOrder(int orderId)
        {
            var order = GetOrderById(orderId);
            if(order != null)
            {
                foreach (var orderProductVariant in order.OrderProductVariants)
                {
                    IoC.Resolve<IShoppingCartService>().AddToCart(ShoppingCartTypeEnum.ShoppingCart, 
                        orderProductVariant.ProductVariantId, 
                        orderProductVariant.AttributesXml, 
                        orderProductVariant.UnitPriceExclTax,
                        orderProductVariant.Quantity);
                }
            }
        }

        /// <summary>
        /// Process next recurring psayment
        /// </summary>
        /// <param name="recurringPaymentId">Recurring payment identifier</param>
        public void ProcessNextRecurringPayment(int recurringPaymentId)
        {
            try
            {
                var rp = GetRecurringPaymentById(recurringPaymentId);
                if (rp == null)
                    throw new NopException("Recurring payment could not be loaded");

                if (!rp.IsActive)
                    throw new NopException("Recurring payment is not active");

                var initialOrder = rp.InitialOrder;
                if (initialOrder == null)
                    throw new NopException("Initial order could not be loaded");

                var customer = initialOrder.Customer;
                if (customer == null)
                    throw new NopException("Customer could not be loaded");

                var nextPaymentDate = rp.NextPaymentDate;
                if (!nextPaymentDate.HasValue)
                    throw new NopException("Next payment date could not be calculated");

                //payment info
                var paymentInfo = new PaymentInfo();
                paymentInfo.IsRecurringPayment = true;
                paymentInfo.InitialOrderId = initialOrder.OrderId;
                paymentInfo.RecurringCycleLength = rp.CycleLength;
                paymentInfo.RecurringCyclePeriod = rp.CyclePeriod;
                paymentInfo.RecurringTotalCycles = rp.TotalCycles;

                //place new order
                int newOrderId = 0;
                string result = this.PlaceOrder(paymentInfo, customer,
                    Guid.NewGuid(), out newOrderId);
                if (!String.IsNullOrEmpty(result))
                {
                    throw new NopException(result);
                }
                else
                {
                    var rph = new RecurringPaymentHistory()
                    {
                        RecurringPaymentId = rp.RecurringPaymentId,
                        OrderId = newOrderId,
                        CreatedOn = DateTime.UtcNow
                    };
                    InsertRecurringPaymentHistory(rph);
                }
            }
            catch (Exception exc)
            {
                IoC.Resolve<ILogService>().InsertLog(LogTypeEnum.OrderError, string.Format("Error while processing recurring order. {0}", exc.Message), exc);
                throw;
            }
        }

        /// <summary>
        /// Cancels a recurring payment
        /// </summary>
        /// <param name="recurringPaymentId">Recurring payment identifier</param>
        public RecurringPayment CancelRecurringPayment(int recurringPaymentId)
        {
            return CancelRecurringPayment(recurringPaymentId, true);
        }

        /// <summary>
        /// Cancels a recurring payment
        /// </summary>
        /// <param name="recurringPaymentId">Recurring payment identifier</param>
        /// <param name="throwException">A value indicating whether to throw the exception after an error has occupied.</param>
        public RecurringPayment CancelRecurringPayment(int recurringPaymentId,
            bool throwException)
        {
            var recurringPayment = GetRecurringPaymentById(recurringPaymentId);

            if (recurringPayment != null)
            {
                var initialOrder = recurringPayment.InitialOrder;
                if (initialOrder == null)
                    return recurringPayment;

                try
                {
                    //old info from placing order
                    var cancelPaymentResult = new CancelPaymentResult();
                    cancelPaymentResult.AuthorizationTransactionId = initialOrder.AuthorizationTransactionId;
                    cancelPaymentResult.CaptureTransactionId = initialOrder.CaptureTransactionId;
                    cancelPaymentResult.SubscriptionTransactionId = initialOrder.SubscriptionTransactionId;
                    cancelPaymentResult.Amount = initialOrder.OrderTotal;
                    IoC.Resolve<IPaymentService>().CancelRecurringPayment(initialOrder, ref cancelPaymentResult);

                    if (String.IsNullOrEmpty(cancelPaymentResult.Error))
                    {
                        //update recurring payment
                        recurringPayment.IsActive = false;
                        UpdateRecurringPayment(recurringPayment);

                        //order note
                        InsertOrderNote(initialOrder.OrderId, string.Format("Recurring payment has been cancelled"), false, DateTime.UtcNow);
                    }
                    else
                    {
                        //order note
                        InsertOrderNote(initialOrder.OrderId, string.Format("Error cancelling recurring payment. Error: {0}", cancelPaymentResult.Error), false, DateTime.UtcNow);
                    }

                }
                catch (Exception exc)
                {
                    IoC.Resolve<ILogService>().InsertLog(LogTypeEnum.OrderError, "Error cancelling recurring payment", exc);
                    if (throwException)
                        throw;
                }
            }
            return recurringPayment;
        }

        /// <summary>
        /// Gets a value indicating whether a customer can cancel recurring payment
        /// </summary>
        /// <param name="customerToValidate">Customer</param>
        /// <param name="recurringPayment">Recurring Payment</param>
        /// <returns>value indicating whether a customer can cancel recurring payment</returns>
        public bool CanCancelRecurringPayment(Customer customerToValidate, RecurringPayment recurringPayment)
        {
            if (recurringPayment == null)
                return false;

            if (customerToValidate == null)
                return false;

            var initialOrder = recurringPayment.InitialOrder;
            if (initialOrder == null)
                return false;

            var customer = recurringPayment.Customer;
            if (customer == null)
                return false;

            if (initialOrder.OrderStatus == OrderStatusEnum.Cancelled)
                return false;

            if (!customerToValidate.IsAdmin)
            {
                if (customer.CustomerId != customerToValidate.CustomerId)
                    return false;
            }

            if (!recurringPayment.NextPaymentDate.HasValue)
                return false;

            return true;
        }

        /// <summary>
        /// Gets a value indicating whether shipping is allowed
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A value indicating whether shipping is allowed</returns>
        public bool CanShip(Order order)
        {
            if (order == null)
                return false;

            if (order.OrderStatus == OrderStatusEnum.Cancelled)
                return false;

            if (order.ShippingStatus == ShippingStatusEnum.NotYetShipped)
                return true;

            return false;
        }

        /// <summary>
        /// Ships order
        /// </summary>
        /// <param name="orderId">Order identifier</param>
        /// <param name="notifyCustomer">True to notify customer</param>
        /// <returns>Updated order</returns>
        public Order Ship(int orderId, bool notifyCustomer)
        {
            var order = GetOrderById(orderId);
            if (order == null)
                return order;

            if (!CanShip(order))
                throw new NopException("Can not do shipment for order.");

            order.ShippedDate = DateTime.UtcNow;
            order.ShippingStatusId = (int)ShippingStatusEnum.Shipped;
            UpdateOrder(order);

            InsertOrderNote(order.OrderId, string.Format("Order has been shipped"), false, DateTime.UtcNow);

            if (notifyCustomer)
            {
                int orderShippedCustomerNotificationQueuedEmailId = IoC.Resolve<IMessageService>().SendOrderShippedCustomerNotification(order, order.CustomerLanguageId);
                if (orderShippedCustomerNotificationQueuedEmailId > 0)
                {
                    InsertOrderNote(order.OrderId, string.Format("\"Shipped\" email (to customer) has been queued. Queued email identifier: {0}.", orderShippedCustomerNotificationQueuedEmailId), false, DateTime.UtcNow);
                }
            }

            order = CheckOrderStatus(order.OrderId);

            return order;
        }

        /// <summary>
        /// Gets a value indicating whether order is delivered
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A value indicating whether shipping is delivered</returns>
        public bool CanDeliver(Order order)
        {
            if (order == null)
                return false;

            if (order.OrderStatus == OrderStatusEnum.Cancelled)
                return false;

            if (order.ShippingStatus == ShippingStatusEnum.Shipped)
                return true;

            return false;
        }

        /// <summary>
        /// Marks order status as delivered
        /// </summary>
        /// <param name="orderId">Order identifier</param>
        /// <param name="notifyCustomer">True to notify customer</param>
        /// <returns>Updated order</returns>
        public Order Deliver(int orderId, bool notifyCustomer)
        {
            var order = GetOrderById(orderId);
            if (order == null)
                return order;

            if (!CanDeliver(order))
                throw new NopException("Can not do delivery for order.");

            order.DeliveryDate = DateTime.UtcNow;
            order.ShippingStatusId = (int)ShippingStatusEnum.Delivered;
            UpdateOrder(order);

            InsertOrderNote(order.OrderId, string.Format("Order has been delivered"), false, DateTime.UtcNow);

            if (notifyCustomer)
            {
                int orderDeliveredCustomerNotificationQueuedEmailId = IoC.Resolve<IMessageService>().SendOrderDeliveredCustomerNotification(order, order.CustomerLanguageId);
                if (orderDeliveredCustomerNotificationQueuedEmailId > 0)
                {
                    InsertOrderNote(order.OrderId, string.Format("\"Delivered\" email (to customer) has been queued. Queued email identifier: {0}.", orderDeliveredCustomerNotificationQueuedEmailId), false, DateTime.UtcNow);
                }
            }

            order = CheckOrderStatus(order.OrderId);

            return order;
        }

        /// <summary>
        /// Gets a value indicating whether cancel is allowed
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A value indicating whether cancel is allowed</returns>
        public bool CanCancelOrder(Order order)
        {
            if (order == null)
                return false;

            if (order.OrderStatus == OrderStatusEnum.Cancelled)
                return false;

            return true;
        }

        /// <summary>
        /// Cancels order
        /// </summary>
        /// <param name="orderId">Order identifier</param>
        /// <param name="notifyCustomer">True to notify customer</param>
        /// <returns>Cancelled order</returns>
        public Order CancelOrder(int orderId, bool notifyCustomer)
        {
            var order = GetOrderById(orderId);
            if (order == null)
                return order;

            if (!CanCancelOrder(order))
                throw new NopException("Can not do cancel for order.");
            
            //Cancel order
            SetOrderStatus(order, OrderStatusEnum.Cancelled, notifyCustomer);

            InsertOrderNote(order.OrderId, string.Format("Order has been cancelled"), false, DateTime.UtcNow);
            
            //cancel recurring payments
            var recurringPayments = SearchRecurringPayments(0, order.OrderId, null);
            foreach (var rp in recurringPayments)
            {
                CancelRecurringPayment(rp.RecurringPaymentId, false);
            }
                
            //Adjust inventory
            foreach (var opv in order.OrderProductVariants)
                IoC.Resolve<IProductService>().AdjustInventory(opv.ProductVariantId, false, opv.Quantity, opv.AttributesXml);

            return order;
        }

        /// <summary>
        /// Gets a value indicating whether order can be marked as authorized
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A value indicating whether order can be marked as authorized</returns>
        public bool CanMarkOrderAsAuthorized(Order order)
        {
            if (order == null)
                return false;

            if (order.OrderStatus == OrderStatusEnum.Cancelled)
                return false;

            if (order.PaymentStatus == PaymentStatusEnum.Pending)
                return true;

            return false;
        }

        /// <summary>
        /// Marks order as authorized
        /// </summary>
        /// <param name="orderId">Order identifier</param>
        /// <returns>Authorized order</returns>
        public Order MarkAsAuthorized(int orderId)
        {
            var order = GetOrderById(orderId);
            if (order == null)
                return order;

            order.PaymentStatusId = (int)PaymentStatusEnum.Authorized;
            UpdateOrder(order);

            InsertOrderNote(order.OrderId, string.Format("Order has been marked as authorized"), false, DateTime.UtcNow);

            order = CheckOrderStatus(order.OrderId);

            return order;
        }

        /// <summary>
        /// Gets a value indicating whether capture from admin panel is allowed
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A value indicating whether capture from admin panel is allowed</returns>
        public bool CanCapture(Order order)
        {
            if (order == null)
                return false;

            if (order.OrderStatus == OrderStatusEnum.Cancelled ||
                order.OrderStatus == OrderStatusEnum.Pending)
                return false;

            if (order.PaymentStatus == PaymentStatusEnum.Authorized &&
                IoC.Resolve<IPaymentService>().CanCapture(order.PaymentMethodId))
                return true;

            return false;
        }

        /// <summary>
        /// Captures order (from admin panel)
        /// </summary>
        /// <param name="orderId">Order identifier</param>
        /// <param name="error">Error</param>
        /// <returns>Captured order</returns>
        public Order Capture(int orderId, ref string error)
        {
            var order = GetOrderById(orderId);
            if (order == null)
                return order;

            if (!CanCapture(order))
                throw new NopException("Can not do capture for order.");

            var processPaymentResult = new ProcessPaymentResult();
            try
            {
                //old info from placing order
                processPaymentResult.AuthorizationTransactionId = order.AuthorizationTransactionId;
                processPaymentResult.AuthorizationTransactionCode = order.AuthorizationTransactionCode;
                processPaymentResult.AuthorizationTransactionResult = order.AuthorizationTransactionResult;
                processPaymentResult.CaptureTransactionId = order.CaptureTransactionId;
                processPaymentResult.CaptureTransactionResult = order.CaptureTransactionResult;
                processPaymentResult.SubscriptionTransactionId = order.SubscriptionTransactionId;
                processPaymentResult.PaymentStatus = order.PaymentStatus;

                IoC.Resolve<IPaymentService>().Capture(order, ref processPaymentResult);

                if (String.IsNullOrEmpty(processPaymentResult.Error))
                {
                    var paidDate = order.PaidDate;
                    var paymentStatus = processPaymentResult.PaymentStatus;
                    if (paymentStatus == PaymentStatusEnum.Paid)
                        paidDate = DateTime.UtcNow;

                    order.AuthorizationTransactionId = processPaymentResult.AuthorizationTransactionId;
                    order.AuthorizationTransactionCode = processPaymentResult.AuthorizationTransactionCode;
                    order.AuthorizationTransactionResult = processPaymentResult.AuthorizationTransactionResult;
                    order.CaptureTransactionId = processPaymentResult.CaptureTransactionId;
                    order.CaptureTransactionResult = processPaymentResult.CaptureTransactionResult;
                    order.SubscriptionTransactionId = processPaymentResult.SubscriptionTransactionId;
                    order.PaymentStatusId = (int)paymentStatus;
                    order.PaidDate = paidDate;

                    UpdateOrder(order);

                    InsertOrderNote(order.OrderId, string.Format("Order has been captured"), false, DateTime.UtcNow);
                }
                else
                {
                    InsertOrderNote(order.OrderId, string.Format("Unable to capture order. Error: {0}", processPaymentResult.Error), false, DateTime.UtcNow);

                }
                order = CheckOrderStatus(order.OrderId);
                
                //raise event         
                if (order.PaymentStatus == PaymentStatusEnum.Paid)
                {
                    EventContext.Current.OnOrderPaid(null,
                        new OrderEventArgs() { Order = order });
                }
            }
            catch (Exception exc)
            {
                processPaymentResult.Error = exc.Message;
                processPaymentResult.FullError = exc.ToString();
            }

            if (!String.IsNullOrEmpty(processPaymentResult.Error))
            {
                error = processPaymentResult.Error;
                IoC.Resolve<ILogService>().InsertLog(LogTypeEnum.OrderError, string.Format("Error capturing order. {0}", processPaymentResult.Error), processPaymentResult.FullError);
            }
            return order;
        }

        /// <summary>
        /// Gets a value indicating whether order can be marked as paid
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A value indicating whether order can be marked as paid</returns>
        public bool CanMarkOrderAsPaid(Order order)
        {
            if (order == null)
                return false;

            if (order.OrderStatus == OrderStatusEnum.Cancelled)
                return false;

            if (order.PaymentStatus == PaymentStatusEnum.Paid || 
                order.PaymentStatus == PaymentStatusEnum.Refunded || 
                order.PaymentStatus == PaymentStatusEnum.Voided)
                return false;

            return true;
        }

        /// <summary>
        /// Marks order as paid
        /// </summary>
        /// <param name="orderId">Order identifier</param>
        /// <returns>Updated order</returns>
        public Order MarkOrderAsPaid(int orderId)
        {
            var order = GetOrderById(orderId);
            if (order == null)
                return order;

            if (!CanMarkOrderAsPaid(order))
                throw new NopException("You can't mark this order as paid");

            order.PaymentStatusId = (int)PaymentStatusEnum.Paid;
            order.PaidDate = DateTime.UtcNow;
            UpdateOrder(order);

            InsertOrderNote(order.OrderId, string.Format("Order has been marked as paid"), false, DateTime.UtcNow);

            order = CheckOrderStatus(order.OrderId);

            //raise event         
            if (order.PaymentStatus == PaymentStatusEnum.Paid)
            {
                EventContext.Current.OnOrderPaid(null,
                    new OrderEventArgs() { Order = order });
            }

            return order;
        }

        /// <summary>
        /// Gets a value indicating whether refund from admin panel is allowed
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A value indicating whether refund from admin panel is allowed</returns>
        public bool CanRefund(Order order)
        {
            if (order == null)
                return false;

            if (order.OrderTotal == decimal.Zero)
                return false;

            if (order.OrderStatus == OrderStatusEnum.Cancelled)
                return false;

            if (order.PaymentStatus == PaymentStatusEnum.Paid &&
                IoC.Resolve<IPaymentService>().CanRefund(order.PaymentMethodId))
                return true;

            return false;
        }

        /// <summary>
        /// Refunds an order (from admin panel)
        /// </summary>
        /// <param name="orderId">Order identifier</param>
        /// <param name="error">Error</param>
        /// <returns>Refunded order</returns>
        public Order Refund(int orderId, ref string error)
        {
            var order = GetOrderById(orderId);
            if (order == null)
                return order;
            
            if (!CanRefund(order))
                throw new NopException("Can not do refund for order.");

            var cancelPaymentResult = new CancelPaymentResult();
            try
            {
                //amout to refund
                decimal amountToRefund = order.OrderTotal;

                //old info from placing order
                cancelPaymentResult.AuthorizationTransactionId = order.AuthorizationTransactionId;
                cancelPaymentResult.CaptureTransactionId = order.CaptureTransactionId;
                cancelPaymentResult.SubscriptionTransactionId = order.SubscriptionTransactionId;
                cancelPaymentResult.Amount = amountToRefund;
                cancelPaymentResult.IsPartialRefund = false;
                cancelPaymentResult.PaymentStatus = order.PaymentStatus;

                IoC.Resolve<IPaymentService>().Refund(order, ref cancelPaymentResult);

                if (String.IsNullOrEmpty(cancelPaymentResult.Error))
                {
                    //total amount refunded
                    decimal totalAmountRefunded = order.RefundedAmount + amountToRefund;

                    //update order info
                    order.RefundedAmount = totalAmountRefunded;
                    order.AuthorizationTransactionId = cancelPaymentResult.AuthorizationTransactionId;
                    order.CaptureTransactionId = cancelPaymentResult.CaptureTransactionId;
                    order.PaymentStatusId = (int)cancelPaymentResult.PaymentStatus;
                    UpdateOrder(order);

                    InsertOrderNote(order.OrderId, string.Format("Order has been refunded. Amount = {0}", PriceHelper.FormatPrice(amountToRefund, true, false)), false, DateTime.UtcNow);
                }
                else
                {
                    InsertOrderNote(order.OrderId, string.Format("Unable to refund order. Error: {0}", cancelPaymentResult.Error), false, DateTime.UtcNow);
                }

                //check orer status
                order = CheckOrderStatus(order.OrderId);
            }
            catch (Exception exc)
            {
                cancelPaymentResult.Error = exc.Message;
                cancelPaymentResult.FullError = exc.ToString();
            }

            if (!String.IsNullOrEmpty(cancelPaymentResult.Error))
            {
                error = cancelPaymentResult.Error;
                IoC.Resolve<ILogService>().InsertLog(LogTypeEnum.OrderError, string.Format("Error refunding order. {0}", cancelPaymentResult.Error), cancelPaymentResult.FullError);
            }
            return order;
        }

        /// <summary>
        /// Gets a value indicating whether order can be marked as refunded
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A value indicating whether order can be marked as refunded</returns>
        public bool CanRefundOffline(Order order)
        {
            if (order == null)
                return false;

            if (order.OrderTotal == decimal.Zero)
                return false;

            if (order.OrderStatus == OrderStatusEnum.Cancelled)
                return false;

            if (order.PaymentStatus == PaymentStatusEnum.Paid)
                return true;

            return false;
        }

        /// <summary>
        /// Refunds an order (offline)
        /// </summary>
        /// <param name="orderId">Order identifier</param>
        /// <returns>Updated order</returns>
        public Order RefundOffline(int orderId)
        {
            var order = GetOrderById(orderId);
            if (order == null)
                return order;

            if (!CanRefundOffline(order))
                throw new NopException("You can't refund this order");

            //amout to refund
            decimal amountToRefund = order.OrderTotal;

            //total amount refunded
            decimal totalAmountRefunded = order.RefundedAmount + amountToRefund;

            //update order info
            order.RefundedAmount = totalAmountRefunded;
            order.PaymentStatusId = (int)PaymentStatusEnum.Refunded;

            UpdateOrder(order);

            InsertOrderNote(order.OrderId, string.Format("Order has been marked as refunded. Amount = {0}", PriceHelper.FormatPrice(amountToRefund, true, false)), false, DateTime.UtcNow);

            order = CheckOrderStatus(order.OrderId);

            return order;
        }

        /// <summary>
        /// Gets a value indicating whether partial refund from admin panel is allowed
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="amountToRefund">Amount to refund</param>
        /// <returns>A value indicating whether refund from admin panel is allowed</returns>
        public bool CanPartiallyRefund(Order order, decimal amountToRefund)
        {
            if (order == null)
                return false;

            if (order.OrderTotal == decimal.Zero)
                return false;

            if (order.OrderStatus == OrderStatusEnum.Cancelled)
                return false;

            decimal canBeRefunded = order.OrderTotal - order.RefundedAmount;
            if (canBeRefunded <= decimal.Zero)
                return false;

            if (amountToRefund > canBeRefunded)
                return false;

            if ((order.PaymentStatus == PaymentStatusEnum.Paid || 
                order.PaymentStatus == PaymentStatusEnum.PartiallyRefunded) &&
                IoC.Resolve<IPaymentService>().CanPartiallyRefund(order.PaymentMethodId))
                return true;

            return false;
        }

        /// <summary>
        /// Partially refunds an order (from admin panel)
        /// </summary>
        /// <param name="orderId">Order identifier</param>
        /// <param name="amountToRefund">Amount to refund</param>
        /// <param name="error">Error</param>
        /// <returns>Refunded order</returns>
        public Order PartiallyRefund(int orderId, decimal amountToRefund, ref string error)
        {
            var order = GetOrderById(orderId);
            if (order == null)
                return order;

            amountToRefund = Math.Round(amountToRefund, 2);

            if (!CanPartiallyRefund(order, amountToRefund))
                throw new NopException("Can not do partial refund for order.");

            var cancelPaymentResult = new CancelPaymentResult();
            try
            {
                //old info from placing order
                cancelPaymentResult.AuthorizationTransactionId = order.AuthorizationTransactionId;
                cancelPaymentResult.CaptureTransactionId = order.CaptureTransactionId;
                cancelPaymentResult.SubscriptionTransactionId = order.SubscriptionTransactionId;
                cancelPaymentResult.Amount = amountToRefund;
                cancelPaymentResult.IsPartialRefund = true;
                cancelPaymentResult.PaymentStatus = order.PaymentStatus;

                IoC.Resolve<IPaymentService>().Refund(order, ref cancelPaymentResult);

                if (String.IsNullOrEmpty(cancelPaymentResult.Error))
                {
                    //total amount refunded
                    decimal totalAmountRefunded = order.RefundedAmount + amountToRefund;

                    //update order info
                    order.RefundedAmount = totalAmountRefunded;
                    order.AuthorizationTransactionId = cancelPaymentResult.AuthorizationTransactionId;
                    order.CaptureTransactionId = cancelPaymentResult.CaptureTransactionId;
                    order.PaymentStatusId = (int)cancelPaymentResult.PaymentStatus;
                    UpdateOrder(order);

                    InsertOrderNote(order.OrderId, string.Format("Order has been partially refunded. Amount = {0}", PriceHelper.FormatPrice(amountToRefund, true, false)), false, DateTime.UtcNow);
                }
                else
                {
                    InsertOrderNote(order.OrderId, string.Format("Unable to partially refund order. Error: {0}", cancelPaymentResult.Error), false, DateTime.UtcNow);
                }

                //check orer status
                order = CheckOrderStatus(order.OrderId);
            }
            catch (Exception exc)
            {
                cancelPaymentResult.Error = exc.Message;
                cancelPaymentResult.FullError = exc.ToString();
            }

            if (!String.IsNullOrEmpty(cancelPaymentResult.Error))
            {
                error = cancelPaymentResult.Error;
                IoC.Resolve<ILogService>().InsertLog(LogTypeEnum.OrderError, string.Format("Error refunding order. {0}", cancelPaymentResult.Error), cancelPaymentResult.FullError);
            }
            return order;
        }

        /// <summary>
        /// Gets a value indicating whether order can be marked as partially refunded
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="amountToRefund">Amount to refund</param>
        /// <returns>A value indicating whether order can be marked as partially refunded</returns>
        public bool CanPartiallyRefundOffline(Order order, decimal amountToRefund)
        {
            if (order == null)
                return false;

            if (order.OrderTotal == decimal.Zero)
                return false;

            if (order.OrderStatus == OrderStatusEnum.Cancelled)
                return false;

            decimal canBeRefunded = order.OrderTotal - order.RefundedAmount;
            if (canBeRefunded <= decimal.Zero)
                return false;

            if (amountToRefund > canBeRefunded)
                return false;

            if (order.PaymentStatus == PaymentStatusEnum.Paid ||
                order.PaymentStatus == PaymentStatusEnum.PartiallyRefunded)
                return true;

            return false;
        }

        /// <summary>
        /// Partially refunds an order (offline)
        /// </summary>
        /// <param name="orderId">Order identifier</param>
        /// <returns>Updated order</returns>
        public Order PartiallyRefundOffline(int orderId, decimal amountToRefund)
        {
            var order = GetOrderById(orderId);
            if (order == null)
                return order;

            amountToRefund = Math.Round(amountToRefund, 2);

            if (!CanPartiallyRefundOffline(order, amountToRefund))
                throw new NopException("You can't partially refund (offline) this order");

            //total amount refunded
            decimal totalAmountRefunded = order.RefundedAmount + amountToRefund;

            //update order info
            order.RefundedAmount = totalAmountRefunded;
            order.PaymentStatusId = (int)PaymentStatusEnum.PartiallyRefunded;

            UpdateOrder(order);

            InsertOrderNote(order.OrderId, string.Format("Order has been marked as partially refunded. Amount = {0}", PriceHelper.FormatPrice(amountToRefund, true, false)), false, DateTime.UtcNow);

            order = CheckOrderStatus(order.OrderId);

            return order;
        }

        /// <summary>
        /// Gets a value indicating whether void from admin panel is allowed
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A value indicating whether void from admin panel is allowed</returns>
        public bool CanVoid(Order order)
        {
            if (order == null)
                return false;

            if (order.OrderTotal == decimal.Zero)
                return false;

            if (order.OrderStatus == OrderStatusEnum.Cancelled)
                return false;

            if (order.PaymentStatus == PaymentStatusEnum.Authorized &&
                IoC.Resolve<IPaymentService>().CanVoid(order.PaymentMethodId))
                return true;

            return false;
        }

        /// <summary>
        /// Voids order (from admin panel)
        /// </summary>
        /// <param name="orderId">Order identifier</param>
        /// <param name="error">Error</param>
        /// <returns>Voided order</returns>
        public Order Void(int orderId, ref string error)
        {
            var order = GetOrderById(orderId);
            if (order == null)
                return order;

            if (!CanVoid(order))
                throw new NopException("Can not do void for order.");

            var cancelPaymentResult = new CancelPaymentResult();
            try
            {
                //old info from placing order
                cancelPaymentResult.AuthorizationTransactionId = order.AuthorizationTransactionId;
                cancelPaymentResult.CaptureTransactionId = order.CaptureTransactionId;
                cancelPaymentResult.Amount = order.OrderTotal;
                cancelPaymentResult.PaymentStatus = order.PaymentStatus;

                IoC.Resolve<IPaymentService>().Void(order, ref cancelPaymentResult);

                if (String.IsNullOrEmpty(cancelPaymentResult.Error))
                {
                    order.AuthorizationTransactionId = cancelPaymentResult.AuthorizationTransactionId;
                    order.CaptureTransactionId = cancelPaymentResult.CaptureTransactionId;
                    order.PaymentStatusId = (int)cancelPaymentResult.PaymentStatus;
                    UpdateOrder(order);

                    InsertOrderNote(order.OrderId, string.Format("Order has been voided"), false, DateTime.UtcNow);
                }
                else
                {
                    InsertOrderNote(order.OrderId, string.Format("Unable to void order. Error: {0}", cancelPaymentResult.Error), false, DateTime.UtcNow);

                }
                order = CheckOrderStatus(order.OrderId);
            }
            catch (Exception exc)
            {
                cancelPaymentResult.Error = exc.Message;
                cancelPaymentResult.FullError = exc.ToString();
            }

            if (!String.IsNullOrEmpty(cancelPaymentResult.Error))
            {
                error = cancelPaymentResult.Error;
                IoC.Resolve<ILogService>().InsertLog(LogTypeEnum.OrderError, string.Format("Error voiding order. {0}", cancelPaymentResult.Error), cancelPaymentResult.FullError);
            }
            return order;
        }

        /// <summary>
        /// Gets a value indicating whether order can be marked as voided
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A value indicating whether order can be marked as voided</returns>
        public bool CanVoidOffline(Order order)
        {
            if (order == null)
                return false;

            if (order.OrderTotal == decimal.Zero)
                return false;

            if (order.OrderStatus == OrderStatusEnum.Cancelled)
                return false;

            if (order.PaymentStatus == PaymentStatusEnum.Authorized)
                return true;

            return false;
        }

        /// <summary>
        /// Voids order (offline)
        /// </summary>
        /// <param name="orderId">Order identifier</param>
        /// <returns>Updated order</returns>
        public Order VoidOffline(int orderId)
        {
            var order = GetOrderById(orderId);
            if (order == null)
                return order;

            if (!CanVoidOffline(order))
                throw new NopException("You can't void this order");

            order.PaymentStatusId = (int)PaymentStatusEnum.Voided;
            UpdateOrder(order);

            InsertOrderNote(order.OrderId, string.Format("Order has been marked as voided"), false, DateTime.UtcNow);

            order = CheckOrderStatus(order.OrderId);

            return order;
        }

        /// <summary>
        /// Converts reward points to amount primary store currency
        /// </summary>
        /// <param name="rewardPoints">Reward points</param>
        /// <returns>Converted value</returns>
        public decimal ConvertRewardPointsToAmount(int rewardPoints)
        {
            decimal result = decimal.Zero;
            if (rewardPoints <= 0)
                return decimal.Zero;

            result = rewardPoints * this.RewardPointsExchangeRate;            
            result = Math.Round(result, 2);
            return result;
        }

        /// <summary>
        /// Converts an amount in primary store currency to reward points
        /// </summary>
        /// <param name="amount">Amount</param>
        /// <returns>Converted value</returns>
        public int ConvertAmountToRewardPoints(decimal amount)
        {
            int result = 0;
            if (amount <= 0)
                return 0;

            if (this.RewardPointsExchangeRate > 0)
            {
                result = (int)Math.Ceiling(amount / this.RewardPointsExchangeRate);
            }
            return result;
        }

        /// <summary>
        /// Valdiate minimum order sub-total amount
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <param name="customer">Customer</param>
        /// <returns>true - OK; false - minimum order sub-total amount is not reached</returns>
        public bool ValidateMinOrderSubtotalAmount(ShoppingCart cart, Customer customer)
        {
            bool result = true;
            //min order amount sub-total validation
            if (cart.Count > 0 &&
                this.MinOrderSubtotalAmount > decimal.Zero)
            {
                //subtotal
                decimal subtotalBase = decimal.Zero;
                decimal orderSubTotalDiscountAmountBase = decimal.Zero;
                Discount orderSubTotalAppliedDiscount = null;
                decimal subTotalWithoutDiscountBase = decimal.Zero;
                decimal subTotalWithDiscountBase = decimal.Zero;
                string SubTotalError = IoC.Resolve<IShoppingCartService>().GetShoppingCartSubTotal(cart,
                    customer, out orderSubTotalDiscountAmountBase, out orderSubTotalAppliedDiscount,
                out subTotalWithoutDiscountBase, out subTotalWithDiscountBase);
                subtotalBase = subTotalWithoutDiscountBase;
                if (String.IsNullOrEmpty(SubTotalError))
                {
                    if (subtotalBase < this.MinOrderSubtotalAmount)
                    {
                        result = false;
                    }
                }
                else
                {
                    //do nothing (subtotal could not be calculated)
                    //result = false;
                }
            }

            return result;
        }

        /// <summary>
        /// Valdiate minimum order total amount
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <param name="customer">Customer</param>
        /// <returns>true - OK; false - minimum order total amount is not reached</returns>
        public bool ValidateMinOrderTotalAmount(ShoppingCart cart, Customer customer)
        {
            bool result = true;
            //min order amount validation
            if (cart.Count > 0 && this.MinOrderTotalAmount > decimal.Zero)
            {
                int paymentMethodId = 0;
                if (customer != null)
                    paymentMethodId = customer.LastPaymentMethodId;

                decimal discountAmountBase = decimal.Zero;
                Discount appliedDiscount = null;
                List<AppliedGiftCard> appliedGiftCards = null;
                int redeemedRewardPoints = 0;
                decimal redeemedRewardPointsAmount = decimal.Zero;
                bool useRewardPoints = false;
                if (customer != null)
                    useRewardPoints = customer.UseRewardPointsDuringCheckout;
                decimal? shoppingCartTotalBase = IoC.Resolve<IShoppingCartService>().GetShoppingCartTotal(cart,
                    paymentMethodId, customer,
                    out discountAmountBase, out appliedDiscount,
                    out appliedGiftCards, useRewardPoints,
                    out redeemedRewardPoints, out redeemedRewardPointsAmount);
                if (shoppingCartTotalBase.HasValue)
                {
                    if (shoppingCartTotalBase.Value < this.MinOrderTotalAmount)
                    {
                        result = false;
                    }
                }
            }

            return result;
        }
        #endregion

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether cache is enabled
        /// </summary>
        public bool CacheEnabled
        {
            get
            {
                return IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Cache.OrderManager.CacheEnabled");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether customer can make re-order
        /// </summary>
        public bool IsReOrderAllowed
        {
            get
            {
                return IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Order.IsReOrderAllowed", true);
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("Order.IsReOrderAllowed", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Reward Points Program is enabled
        /// </summary>
        public bool RewardPointsEnabled
        {
            get
            {
                return IoC.Resolve<ISettingManager>().GetSettingValueBoolean("RewardPoints.Enabled", false);
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("RewardPoints.Enabled", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Reward Points exchange rate
        /// </summary>
        public decimal RewardPointsExchangeRate
        {
            get
            {
                return IoC.Resolve<ISettingManager>().GetSettingValueDecimalNative("RewardPoints.Rate", 1.00M);
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParamNative("RewardPoints.Rate", value);
            }
        }

        /// <summary>
        /// Gets or sets a number of points awarded for registration
        /// </summary>
        public int RewardPointsForRegistration
        {
            get
            {
                return IoC.Resolve<ISettingManager>().GetSettingValueInteger("RewardPoints.Earning.ForRegistration", 0);
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("RewardPoints.Earning.ForRegistration", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a number of points awarded for purchases (amount in primary store currency)
        /// </summary>
        public decimal RewardPointsForPurchases_Amount
        {
            get
            {
                return IoC.Resolve<ISettingManager>().GetSettingValueDecimalNative("RewardPoints.Earning.RewardPointsForPurchases.Amount", 10.00M);
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParamNative("RewardPoints.Earning.RewardPointsForPurchases.Amount", value);
            }
        }

        /// <summary>
        /// Gets or sets a number of points awarded for purchases
        /// </summary>
        public int RewardPointsForPurchases_Points
        {
            get
            {
                return IoC.Resolve<ISettingManager>().GetSettingValueInteger("RewardPoints.Earning.RewardPointsForPurchases.Points", 1);
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("RewardPoints.Earning.RewardPointsForPurchases.Points", value.ToString());
            }
        }

        /// <summary>
        /// Points are awarded when the order status is
        /// </summary>
        public OrderStatusEnum RewardPointsForPurchases_Awarded
        {
            get
            {
                return (OrderStatusEnum)IoC.Resolve<ISettingManager>().GetSettingValueInteger("RewardPoints.Earning.RewardPointsForPurchases.AwardedOS", (int)OrderStatusEnum.Complete);
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("RewardPoints.Earning.RewardPointsForPurchases.AwardedOS", ((int)value).ToString());
            }
        }

        /// <summary>
        /// Points are canceled when the order is
        /// </summary>
        public OrderStatusEnum RewardPointsForPurchases_Canceled
        {
            get
            {
                return (OrderStatusEnum)IoC.Resolve<ISettingManager>().GetSettingValueInteger("RewardPoints.Earning.RewardPointsForPurchases.CanceledOS", (int)OrderStatusEnum.Cancelled);
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("RewardPoints.Earning.RewardPointsForPurchases.CanceledOS", ((int)value).ToString());
            }
        }

        /// <summary>
        /// Gift cards are activated when the order status is
        /// </summary>
        public OrderStatusEnum? GiftCards_Activated
        {
            get
            {
                int os = IoC.Resolve<ISettingManager>().GetSettingValueInteger("GiftCards.Activation.ActivatedOS");
                if (os > 0)
                {
                    return (OrderStatusEnum)os;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value.HasValue)
                {
                    IoC.Resolve<ISettingManager>().SetParam("GiftCards.Activation.ActivatedOS", ((int)value).ToString());
                }
                else
                {
                    IoC.Resolve<ISettingManager>().SetParam("GiftCards.Activation.ActivatedOS", "0");
                }
            }
        }

        /// <summary>
        /// Gift cards are deactivated when the order status is
        /// </summary>
        public OrderStatusEnum? GiftCards_Deactivated
        {
            get
            {
                int os = IoC.Resolve<ISettingManager>().GetSettingValueInteger("GiftCards.Activation.DeactivatedOS");
                if (os > 0)
                {
                    return (OrderStatusEnum)os;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value.HasValue)
                {
                    IoC.Resolve<ISettingManager>().SetParam("GiftCards.Activation.DeactivatedOS", ((int)value).ToString());
                }
                else
                {
                    IoC.Resolve<ISettingManager>().SetParam("GiftCards.Activation.DeactivatedOS", "0");
                }
            }
        }

        /// <summary>
        /// Gets or sets a minimum order subtotal amount
        /// </summary>
        public decimal MinOrderSubtotalAmount
        {
            get
            {
                return IoC.Resolve<ISettingManager>().GetSettingValueDecimalNative("Order.MinOrderSubtotalAmount", decimal.Zero);
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParamNative("Order.MinOrderSubtotalAmount", value);
            }
        }

        /// <summary>
        /// Gets or sets a minimum order total amount
        /// </summary>
        public decimal MinOrderTotalAmount
        {
            get
            {
                return IoC.Resolve<ISettingManager>().GetSettingValueDecimalNative("Order.MinOrderAmount", decimal.Zero);
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParamNative("Order.MinOrderAmount", value);
            }
        }

        #endregion
    }
}
