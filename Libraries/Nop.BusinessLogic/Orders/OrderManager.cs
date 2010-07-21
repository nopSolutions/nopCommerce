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

namespace NopSolutions.NopCommerce.BusinessLogic.Orders
{
    /// <summary>
    /// Order manager
    /// </summary>
    public partial class OrderManager
    {
        #region Constants
        private const string ORDERSTATUSES_ALL_KEY = "Nop.orderstatus.all";
        private const string ORDERSTATUSES_BY_ID_KEY = "Nop.orderstatus.id-{0}";
        private const string ORDERSTATUSES_PATTERN_KEY = "Nop.orderstatus.";
        #endregion

        #region Utilities

        /// <summary>
        /// Sets an order status
        /// </summary>
        /// <param name="orderId">Order identifier</param>
        /// <param name="os">New order status</param>
        /// <param name="notifyCustomer">True to notify customer</param>
        /// <returns>Order</returns>
        protected static Order SetOrderStatus(int orderId, 
            OrderStatusEnum os, bool notifyCustomer)
        {
            var order = GetOrderById(orderId);
            if (order != null)
            {
                if (order.OrderStatus == os)
                    return order;

                OrderStatusEnum prevOrderStatus = order.OrderStatus;

                var updatedOrder = UpdateOrder(order.OrderId, 
                    order.OrderGuid, 
                    order.CustomerId, 
                    order.CustomerLanguageId,
                    order.CustomerTaxDisplayType, 
                    order.CustomerIP,
                    order.OrderSubtotalInclTax, 
                    order.OrderSubtotalExclTax, 
                    order.OrderShippingInclTax,
                    order.OrderShippingExclTax, 
                    order.PaymentMethodAdditionalFeeInclTax,
                    order.PaymentMethodAdditionalFeeExclTax,
                    order.OrderTax, order.OrderTotal, order.OrderDiscount,
                    order.OrderSubtotalInclTaxInCustomerCurrency, 
                    order.OrderSubtotalExclTaxInCustomerCurrency,
                    order.OrderShippingInclTaxInCustomerCurrency, 
                    order.OrderShippingExclTaxInCustomerCurrency,
                    order.PaymentMethodAdditionalFeeInclTaxInCustomerCurrency, 
                    order.PaymentMethodAdditionalFeeExclTaxInCustomerCurrency,
                    order.OrderTaxInCustomerCurrency, 
                    order.OrderTotalInCustomerCurrency,
                    order.OrderDiscountInCustomerCurrency,
                    order.CheckoutAttributeDescription, 
                    order.CheckoutAttributesXml, 
                    order.CustomerCurrencyCode,
                    order.OrderWeight,
                    order.AffiliateId, 
                    os,
                    order.AllowStoringCreditCardNumber,
                    order.CardType,
                    order.CardName, order.CardNumber, 
                    order.MaskedCreditCardNumber,
                    order.CardCvv2, order.CardExpirationMonth, 
                    order.CardExpirationYear,
                    order.PaymentMethodId, 
                    order.PaymentMethodName,
                    order.AuthorizationTransactionId,
                    order.AuthorizationTransactionCode, 
                    order.AuthorizationTransactionResult,
                    order.CaptureTransactionId, 
                    order.CaptureTransactionResult,
                    order.SubscriptionTransactionId, 
                    order.PurchaseOrderNumber, 
                    order.PaymentStatus, order.PaidDate,
                    order.BillingFirstName, 
                    order.BillingLastName, 
                    order.BillingPhoneNumber,
                    order.BillingEmail, 
                    order.BillingFaxNumber, 
                    order.BillingCompany, 
                    order.BillingAddress1,
                    order.BillingAddress2, 
                    order.BillingCity,
                    order.BillingStateProvince, 
                    order.BillingStateProvinceId,
                    order.BillingZipPostalCode,
                    order.BillingCountry, 
                    order.BillingCountryId, 
                    order.ShippingStatus,
                    order.ShippingFirstName, 
                    order.ShippingLastName,
                    order.ShippingPhoneNumber,
                    order.ShippingEmail, 
                    order.ShippingFaxNumber,
                    order.ShippingCompany,
                    order.ShippingAddress1,
                    order.ShippingAddress2, 
                    order.ShippingCity,
                    order.ShippingStateProvince, 
                    order.ShippingStateProvinceId,
                    order.ShippingZipPostalCode,
                    order.ShippingCountry,
                    order.ShippingCountryId,
                    order.ShippingMethod,
                    order.ShippingRateComputationMethodId, 
                    order.ShippedDate,
                    order.DeliveryDate,
                    order.TrackingNumber,
                    order.VatNumber,
                    order.Deleted,
                    order.CreatedOn);

                //order notes, notifications
                InsertOrderNote(orderId, string.Format("Order status has been changed to {0}", os.ToString()), false, DateTime.UtcNow);

                if (prevOrderStatus != OrderStatusEnum.Complete &&
                    os == OrderStatusEnum.Complete
                    && notifyCustomer)
                {
                    int orderCompletedCustomerNotificationQueuedEmailId = MessageManager.SendOrderCompletedCustomerNotification(updatedOrder, updatedOrder.CustomerLanguageId);
                    if (orderCompletedCustomerNotificationQueuedEmailId > 0)
                    {
                        InsertOrderNote(orderId, string.Format("\"Order completed\" email (to customer) has been queued. Queued email identifier: {0}.", orderCompletedCustomerNotificationQueuedEmailId), false, DateTime.UtcNow);
                    }
                }

                if (prevOrderStatus != OrderStatusEnum.Cancelled &&
                    os == OrderStatusEnum.Cancelled
                    && notifyCustomer)
                {
                    int orderCancelledCustomerNotificationQueuedEmailId = MessageManager.SendOrderCancelledCustomerNotification(updatedOrder, updatedOrder.CustomerLanguageId);
                    if (orderCancelledCustomerNotificationQueuedEmailId > 0)
                    {
                        InsertOrderNote(orderId, string.Format("\"Order cancelled\" email (to customer) has been queued. Queued email identifier: {0}.", orderCancelledCustomerNotificationQueuedEmailId), false, DateTime.UtcNow);
                    }
                }

                //reward points
                if (OrderManager.RewardPointsEnabled)
                {
                    if (OrderManager.RewardPointsForPurchases_Amount > decimal.Zero)
                    {
                        int points = (int)Math.Truncate(updatedOrder.OrderTotal / OrderManager.RewardPointsForPurchases_Amount * OrderManager.RewardPointsForPurchases_Points);
                        if (points != 0)
                        {
                            if (OrderManager.RewardPointsForPurchases_Awarded == updatedOrder.OrderStatus)
                            {
                                var rph = InsertRewardPointsHistory(order.CustomerId,
                                    0, points, decimal.Zero,
                                    decimal.Zero, string.Empty,
                                    string.Format(LocalizationManager.GetLocaleResourceString("RewardPoints.Message.EarnedForOrder"), order.OrderId),
                                    DateTime.UtcNow);
                            }


                            if (OrderManager.RewardPointsForPurchases_Canceled == updatedOrder.OrderStatus)
                            {
                                var rph = InsertRewardPointsHistory(order.CustomerId,
                                    0, -points, decimal.Zero,
                                    decimal.Zero, string.Empty,
                                    string.Format(LocalizationManager.GetLocaleResourceString("RewardPoints.Message.ReducedForOrder"), order.OrderId),
                                    DateTime.UtcNow);
                            }
                        }
                    }
                }

                //gift cards activation
                if (OrderManager.GiftCards_Activated.HasValue &&
                   OrderManager.GiftCards_Activated.Value == updatedOrder.OrderStatus)
                {
                    var giftCards = GetAllGiftCards(order.OrderId,
                        null, null,null,null,null,null,false, string.Empty);
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
                                        Language customerLang = LanguageManager.GetLanguageById(updatedOrder.CustomerLanguageId);
                                        if (customerLang == null)
                                            customerLang = NopContext.Current.WorkingLanguage;
                                        int queuedEmailId = MessageManager.SendGiftCardNotification(gc, customerLang.LanguageId);
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

                        OrderManager.UpdateGiftCard(gc.GiftCardId,
                            gc.PurchasedOrderProductVariantId, gc.Amount, true,
                            gc.GiftCardCouponCode, gc.RecipientName, gc.RecipientEmail,
                            gc.SenderName, gc.SenderEmail, gc.Message,
                            isRecipientNotified, gc.CreatedOn);
                    }
                }

                //gift cards deactivation
                if (OrderManager.GiftCards_Deactivated.HasValue &&
                   OrderManager.GiftCards_Deactivated.Value == updatedOrder.OrderStatus)
                {
                    var giftCards = GetAllGiftCards(order.OrderId,
                        null, null, null, null, null, null, true, string.Empty);
                    foreach (var gc in giftCards)
                    {
                        OrderManager.UpdateGiftCard(gc.GiftCardId,
                            gc.PurchasedOrderProductVariantId, gc.Amount, false,
                            gc.GiftCardCouponCode, gc.RecipientName, gc.RecipientEmail,
                            gc.SenderName, gc.SenderEmail, gc.Message,
                            gc.IsRecipientNotified, gc.CreatedOn);
                    }
                }

                return updatedOrder;
            }
            return null;
        }

        /// <summary>
        /// Checks order status
        /// </summary>
        /// <param name="orderId">Order identifier</param>
        /// <returns>Validated order</returns>
        protected static Order CheckOrderStatus(int orderId)
        {
            var order = GetOrderById(orderId);
            if (order == null)
                return null;

            if (order.OrderStatus == OrderStatusEnum.Pending)
            {
                if (order.PaymentStatus == PaymentStatusEnum.Authorized || 
                    order.PaymentStatus == PaymentStatusEnum.Paid)
                {
                    order = SetOrderStatus(orderId, OrderStatusEnum.Processing, false);
                }
            }

            if (order.OrderStatus == OrderStatusEnum.Pending)
            {
                if (order.ShippingStatus == ShippingStatusEnum.Shipped ||
                    order.ShippingStatus == ShippingStatusEnum.Delivered)
                {
                    order = SetOrderStatus(orderId, OrderStatusEnum.Processing, false);
                }
            }

            if (order.OrderStatus != OrderStatusEnum.Cancelled && 
                order.OrderStatus != OrderStatusEnum.Complete)
            {
                if (order.PaymentStatus == PaymentStatusEnum.Paid)
                {
                    if (!CanShip(order) && !CanDeliver(order))
                    {
                        order = SetOrderStatus(orderId, OrderStatusEnum.Complete, true);
                    }
                }
            }

            if (order.PaymentStatus == PaymentStatusEnum.Paid && !order.PaidDate.HasValue)
            {
                //ensure that paid date is set
                DateTime paidDate = DateTime.UtcNow;
                order = UpdateOrder(order.OrderId, order.OrderGuid, order.CustomerId, order.CustomerLanguageId,
                    order.CustomerTaxDisplayType, order.CustomerIP, order.OrderSubtotalInclTax, order.OrderSubtotalExclTax, order.OrderShippingInclTax,
                    order.OrderShippingExclTax, order.PaymentMethodAdditionalFeeInclTax, order.PaymentMethodAdditionalFeeExclTax,
                    order.OrderTax, order.OrderTotal, order.OrderDiscount,
                    order.OrderSubtotalInclTaxInCustomerCurrency, order.OrderSubtotalExclTaxInCustomerCurrency,
                    order.OrderShippingInclTaxInCustomerCurrency, order.OrderShippingExclTaxInCustomerCurrency,
                    order.PaymentMethodAdditionalFeeInclTaxInCustomerCurrency, order.PaymentMethodAdditionalFeeExclTaxInCustomerCurrency,
                    order.OrderTaxInCustomerCurrency, order.OrderTotalInCustomerCurrency,
                    order.OrderDiscountInCustomerCurrency,
                    order.CheckoutAttributeDescription, order.CheckoutAttributesXml, 
                    order.CustomerCurrencyCode, order.OrderWeight,
                    order.AffiliateId, order.OrderStatus, order.AllowStoringCreditCardNumber, order.CardType,
                    order.CardName, order.CardNumber, order.MaskedCreditCardNumber,
                    order.CardCvv2, order.CardExpirationMonth, order.CardExpirationYear,
                    order.PaymentMethodId, order.PaymentMethodName,
                    order.AuthorizationTransactionId,
                    order.AuthorizationTransactionCode, order.AuthorizationTransactionResult,
                    order.CaptureTransactionId, order.CaptureTransactionResult,
                    order.SubscriptionTransactionId, order.PurchaseOrderNumber, order.PaymentStatus, paidDate,
                    order.BillingFirstName, order.BillingLastName, order.BillingPhoneNumber,
                    order.BillingEmail, order.BillingFaxNumber, order.BillingCompany, order.BillingAddress1,
                    order.BillingAddress2, order.BillingCity,
                    order.BillingStateProvince, order.BillingStateProvinceId, order.BillingZipPostalCode,
                    order.BillingCountry, order.BillingCountryId, order.ShippingStatus,
                    order.ShippingFirstName, order.ShippingLastName, order.ShippingPhoneNumber,
                    order.ShippingEmail, order.ShippingFaxNumber, order.ShippingCompany,
                    order.ShippingAddress1, order.ShippingAddress2, order.ShippingCity,
                    order.ShippingStateProvince, order.ShippingStateProvinceId, order.ShippingZipPostalCode,
                    order.ShippingCountry, order.ShippingCountryId,
                    order.ShippingMethod, order.ShippingRateComputationMethodId,
                    order.ShippedDate, order.DeliveryDate,
                    order.TrackingNumber, order.VatNumber, order.Deleted, order.CreatedOn);
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
        public static Order GetOrderById(int orderId)
        {
            if (orderId == 0)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from o in context.Orders
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
        public static Order GetOrderByGuid(Guid orderGuid)
        {
            if (orderGuid == Guid.Empty)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from o in context.Orders
                        where o.OrderGuid == orderGuid
                        select o;
            var order = query.FirstOrDefault();
            return order;
        }

        /// <summary>
        /// Marks an order as deleted
        /// </summary>
        /// <param name="orderId">The order identifier</param>
        public static void MarkOrderAsDeleted(int orderId)
        {
            var order = GetOrderById(orderId);
            if (order != null)
            {
                UpdateOrder(order.OrderId, order.OrderGuid, order.CustomerId, order.CustomerLanguageId,
                    order.CustomerTaxDisplayType, order.CustomerIP, order.OrderSubtotalInclTax, order.OrderSubtotalExclTax, order.OrderShippingInclTax,
                   order.OrderShippingExclTax, order.PaymentMethodAdditionalFeeInclTax, order.PaymentMethodAdditionalFeeExclTax,
                   order.OrderTax, order.OrderTotal, order.OrderDiscount,
                   order.OrderSubtotalInclTaxInCustomerCurrency, order.OrderSubtotalExclTaxInCustomerCurrency,
                   order.OrderShippingInclTaxInCustomerCurrency, order.OrderShippingExclTaxInCustomerCurrency,
                   order.PaymentMethodAdditionalFeeInclTaxInCustomerCurrency, order.PaymentMethodAdditionalFeeExclTaxInCustomerCurrency,
                   order.OrderTaxInCustomerCurrency, order.OrderTotalInCustomerCurrency,
                   order.OrderDiscountInCustomerCurrency,
                   order.CheckoutAttributeDescription, order.CheckoutAttributesXml,
                   order.CustomerCurrencyCode, order.OrderWeight,
                   order.AffiliateId, order.OrderStatus, order.AllowStoringCreditCardNumber, order.CardType,
                   order.CardName, order.CardNumber, order.MaskedCreditCardNumber,
                    order.CardCvv2, order.CardExpirationMonth, order.CardExpirationYear,
                    order.PaymentMethodId, order.PaymentMethodName, order.AuthorizationTransactionId,
                    order.AuthorizationTransactionCode, order.AuthorizationTransactionResult,
                    order.CaptureTransactionId, order.CaptureTransactionResult,
                    order.SubscriptionTransactionId, order.PurchaseOrderNumber, order.PaymentStatus, order.PaidDate,
                    order.BillingFirstName, order.BillingLastName, order.BillingPhoneNumber,
                    order.BillingEmail, order.BillingFaxNumber, order.BillingCompany, order.BillingAddress1,
                    order.BillingAddress2, order.BillingCity, order.BillingStateProvince,
                    order.BillingStateProvinceId, order.BillingZipPostalCode, order.BillingCountry,
                    order.BillingCountryId, order.ShippingStatus,
                    order.ShippingFirstName, order.ShippingLastName, order.ShippingPhoneNumber,
                    order.ShippingEmail, order.ShippingFaxNumber, order.ShippingCompany,
                    order.ShippingAddress1, order.ShippingAddress2, order.ShippingCity,
                    order.ShippingStateProvince, order.ShippingStateProvinceId, order.ShippingZipPostalCode,
                    order.ShippingCountry, order.ShippingCountryId,
                    order.ShippingMethod, order.ShippingRateComputationMethodId,
                    order.ShippedDate, order.DeliveryDate, order.TrackingNumber, 
                    order.VatNumber, true, order.CreatedOn);
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
        public static List<Order> SearchOrders(DateTime? startTime, DateTime? endTime,
            string customerEmail, OrderStatusEnum? os, PaymentStatusEnum? ps, 
            ShippingStatusEnum? ss)
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

            var context = ObjectContextHelper.CurrentObjectContext;

            var query = from o in context.Orders
                        join c in context.Customers on o.CustomerId equals c.CustomerId
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
            return orders;
        }

        /// <summary>
        /// Load all orders
        /// </summary>
        /// <returns>Order collection</returns>
        public static List<Order> LoadAllOrders()
        {
            return SearchOrders(null, null, string.Empty, null, null, null);
        }

        /// <summary>
        /// Gets all orders by customer identifier
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <returns>Order collection</returns>
        public static List<Order> GetOrdersByCustomerId(int customerId)
        {
            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from o in context.Orders
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
        public static Order GetOrderByAuthorizationTransactionIdAndPaymentMethodId(string authorizationTransactionId, 
            int paymentMethodId)
        {
            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from o in context.Orders
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
        public static List<Order> GetOrdersByAffiliateId(int affiliateId)
        {
            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from o in context.Orders
                        orderby o.CreatedOn descending
                        where !o.Deleted && o.AffiliateId == affiliateId
                        select o;
            var orders = query.ToList();
            return orders;
        }

        /// <summary>
        /// Inserts an order
        /// </summary>
        /// <param name="orderGuid">The order identifier</param>
        /// <param name="customerId">The customer identifier</param>
        /// <param name="customerLanguageId">The customer language identifier</param>
        /// <param name="customerTaxDisplayType">The customer tax display type</param>
        /// <param name="customerIP">The customer IP address</param>
        /// <param name="orderSubtotalInclTax">The order subtotal (incl tax)</param>
        /// <param name="orderSubtotalExclTax">The order subtotal (excl tax)</param>
        /// <param name="orderShippingInclTax">The order shipping (incl tax)</param>
        /// <param name="orderShippingExclTax">The order shipping (excl tax)</param>
        /// <param name="paymentMethodAdditionalFeeInclTax">The payment method additional fee (incl tax)</param>
        /// <param name="paymentMethodAdditionalFeeExclTax">The payment method additional fee (excl tax)</param>
        /// <param name="orderTax">The order tax</param>
        /// <param name="orderTotal">The order total</param>
        /// <param name="orderDiscount">The order discount</param>
        /// <param name="orderSubtotalInclTaxInCustomerCurrency">The order subtotal incl tax (customer currency)</param>
        /// <param name="orderSubtotalExclTaxInCustomerCurrency">The order subtotal excl tax (customer currency)</param>
        /// <param name="orderShippingInclTaxInCustomerCurrency">The order shipping incl tax (customer currency)</param>
        /// <param name="orderShippingExclTaxInCustomerCurrency">The order shipping excl tax (customer currency)</param>
        /// <param name="paymentMethodAdditionalFeeInclTaxInCustomerCurrency">The payment method additional fee incl tax (customer currency)</param>
        /// <param name="paymentMethodAdditionalFeeExclTaxInCustomerCurrency">The payment method additional fee excl tax (customer currency)</param>
        /// <param name="orderTaxInCustomerCurrency">The order tax (customer currency)</param>
        /// <param name="orderTotalInCustomerCurrency">The order total (customer currency)</param>
        /// <param name="orderDiscountInCustomerCurrency">The order discount (customer currency)</param>
        /// <param name="checkoutAttributeDescription">The checkout attribute description</param>
        /// <param name="checkoutAttributesXml">The checkout attributes in XML format</param>
        /// <param name="customerCurrencyCode">The customer currency code</param>
        /// <param name="orderWeight">The order weight</param>
        /// <param name="affiliateId">The affiliate identifier</param>
        /// <param name="orderStatus">The order status</param>
        /// <param name="allowStoringCreditCardNumber">The value indicating whether storing of credit card number is allowed</param>
        /// <param name="cardType">The card type</param>
        /// <param name="cardName">The card name</param>
        /// <param name="cardNumber">The card number</param>
        /// <param name="maskedCreditCardNumber">The masked credit card number</param>
        /// <param name="cardCvv2">The card CVV2</param>
        /// <param name="cardExpirationMonth">The card expiration month</param>
        /// <param name="cardExpirationYear">The card expiration year</param>
        /// <param name="paymentMethodId">The payment method identifier</param>
        /// <param name="paymentMethodName">The payment method name</param>
        /// <param name="authorizationTransactionId">The authorization transaction identifier</param>
        /// <param name="authorizationTransactionCode">The authorization transaction code</param>
        /// <param name="authorizationTransactionResult">The authorization transaction result</param>
        /// <param name="captureTransactionId">The capture transaction identifier</param>
        /// <param name="captureTransactionResult">The capture transaction result</param>
        /// <param name="subscriptionTransactionId">The subscription transaction identifier</param>
        /// <param name="purchaseOrderNumber">The purchase order number</param>
        /// <param name="paymentStatus">The payment status</param>
        /// <param name="paidDate">The paid date and time</param>
        /// <param name="billingFirstName">The billing first name</param>
        /// <param name="billingLastName">The billing last name</param>
        /// <param name="billingPhoneNumber">he billing phone number</param>
        /// <param name="billingEmail">The billing email</param>
        /// <param name="billingFaxNumber">The billing fax number</param>
        /// <param name="billingCompany">The billing company</param>
        /// <param name="billingAddress1">The billing address 1</param>
        /// <param name="billingAddress2">The billing address 2</param>
        /// <param name="billingCity">The billing city</param>
        /// <param name="billingStateProvince">The billing state/province</param>
        /// <param name="billingStateProvinceId">The billing state/province identifier</param>
        /// <param name="billingZipPostalCode">The billing zip/postal code</param>
        /// <param name="billingCountry">The billing country</param>
        /// <param name="billingCountryId">The billing country identifier</param>
        /// <param name="shippingStatus">The shipping status</param>
        /// <param name="shippingFirstName">The shipping first name</param>
        /// <param name="shippingLastName">The shipping last name</param>
        /// <param name="shippingPhoneNumber">The shipping phone number</param>
        /// <param name="shippingEmail">The shipping email</param>
        /// <param name="shippingFaxNumber">The shipping fax number</param>
        /// <param name="shippingCompany">The shipping  company</param>
        /// <param name="shippingAddress1">The shipping address 1</param>
        /// <param name="shippingAddress2">The shipping address 2</param>
        /// <param name="shippingCity">The shipping city</param>
        /// <param name="shippingStateProvince">The shipping state/province</param>
        /// <param name="shippingStateProvinceId">The shipping state/province identifier</param>
        /// <param name="shippingZipPostalCode">The shipping zip/postal code</param>
        /// <param name="shippingCountry">The shipping country</param>
        /// <param name="shippingCountryId">The shipping country identifier</param>
        /// <param name="shippingMethod">The shipping method</param>
        /// <param name="shippingRateComputationMethodId">The shipping rate computation method identifier</param>
        /// <param name="shippedDate">The shipped date and time</param>
        /// <param name="deliveryDate">The delivery date and time</param>
        /// <param name="trackingNumber">The tracking number of order</param>
        /// <param name="vatNumber">The VAT number (the European Union Value Added Tax)</param>
        /// <param name="deleted">A value indicating whether the entity has been deleted</param>
        /// <param name="createdOn">The date and time of order creation</param>
        /// <returns>Order</returns>
        public static Order InsertOrder(Guid orderGuid,
            int customerId,
            int customerLanguageId,
            TaxDisplayTypeEnum customerTaxDisplayType,
            string customerIP,
            decimal orderSubtotalInclTax,
            decimal orderSubtotalExclTax,
            decimal orderShippingInclTax,
            decimal orderShippingExclTax,
            decimal paymentMethodAdditionalFeeInclTax,
            decimal paymentMethodAdditionalFeeExclTax,
            decimal orderTax,
            decimal orderTotal,
            decimal orderDiscount,
            decimal orderSubtotalInclTaxInCustomerCurrency,
            decimal orderSubtotalExclTaxInCustomerCurrency,
            decimal orderShippingInclTaxInCustomerCurrency,
            decimal orderShippingExclTaxInCustomerCurrency,
            decimal paymentMethodAdditionalFeeInclTaxInCustomerCurrency,
            decimal paymentMethodAdditionalFeeExclTaxInCustomerCurrency,
            decimal orderTaxInCustomerCurrency,
            decimal orderTotalInCustomerCurrency,
            decimal orderDiscountInCustomerCurrency,
            string checkoutAttributeDescription,
            string checkoutAttributesXml,
            string customerCurrencyCode,
            decimal orderWeight,
            int affiliateId,
            OrderStatusEnum orderStatus,
            bool allowStoringCreditCardNumber,
            string cardType,
            string cardName,
            string cardNumber,
            string maskedCreditCardNumber,
            string cardCvv2,
            string cardExpirationMonth,
            string cardExpirationYear,
            int paymentMethodId,
            string paymentMethodName,
            string authorizationTransactionId,
            string authorizationTransactionCode,
            string authorizationTransactionResult,
            string captureTransactionId,
            string captureTransactionResult,
            string subscriptionTransactionId,
            string purchaseOrderNumber,
            PaymentStatusEnum paymentStatus,
            DateTime? paidDate,
            string billingFirstName,
            string billingLastName,
            string billingPhoneNumber,
            string billingEmail,
            string billingFaxNumber,
            string billingCompany,
            string billingAddress1,
            string billingAddress2,
            string billingCity,
            string billingStateProvince,
            int billingStateProvinceId,
            string billingZipPostalCode,
            string billingCountry,
            int billingCountryId,
            ShippingStatusEnum shippingStatus,
            string shippingFirstName,
            string shippingLastName,
            string shippingPhoneNumber,
            string shippingEmail,
            string shippingFaxNumber,
            string shippingCompany,
            string shippingAddress1,
            string shippingAddress2,
            string shippingCity,
            string shippingStateProvince,
            int shippingStateProvinceId,
            string shippingZipPostalCode,
            string shippingCountry,
            int shippingCountryId,
            string shippingMethod,
            int shippingRateComputationMethodId,
            DateTime? shippedDate,
            DateTime? deliveryDate,
            string trackingNumber,
            string vatNumber,
            bool deleted,
            DateTime createdOn)
        {
            if (customerIP == null)
                customerIP = string.Empty;
            if (checkoutAttributeDescription == null)
                checkoutAttributeDescription = string.Empty;
            if (checkoutAttributesXml == null)
                checkoutAttributesXml = string.Empty;
            if (cardType == null)
                cardType = string.Empty;
            if (cardName == null)
                cardName = string.Empty;
            if (cardNumber == null)
                cardNumber = string.Empty;
            if (cardCvv2 == null)
                cardCvv2 = string.Empty;
            if (cardExpirationMonth == null)
                cardExpirationMonth = string.Empty;
            if (cardExpirationYear == null)
                cardExpirationYear = string.Empty;
            if (paymentMethodName == null)
                paymentMethodName = string.Empty;
            if (authorizationTransactionId == null)
                authorizationTransactionId = string.Empty;
            if (authorizationTransactionCode == null)
                authorizationTransactionCode = string.Empty;
            if (authorizationTransactionResult == null)
                authorizationTransactionResult = string.Empty;
            if (captureTransactionId == null)
                captureTransactionId = string.Empty;
            if (captureTransactionResult == null)
                captureTransactionResult = string.Empty;
            if (subscriptionTransactionId == null)
                subscriptionTransactionId = string.Empty;
            if (purchaseOrderNumber == null)
                purchaseOrderNumber = string.Empty;
            if (billingFirstName == null)
                billingFirstName = string.Empty;
            if (billingLastName == null)
                billingLastName = string.Empty;
            if (billingPhoneNumber == null)
                billingPhoneNumber = string.Empty;
            if (billingEmail == null)
                billingEmail = string.Empty;
            if (billingFaxNumber == null)
                billingFaxNumber = string.Empty;
            if (billingCompany == null)
                billingCompany = string.Empty;
            if (billingZipPostalCode == null)
                billingZipPostalCode = string.Empty;
            if (billingCountry == null)
                billingCountry = string.Empty;
            if (shippingLastName == null)
                shippingLastName = string.Empty;
            if (shippingPhoneNumber == null)
                shippingPhoneNumber = string.Empty;
            if (shippingEmail == null)
                shippingEmail = string.Empty;
            if (shippingFaxNumber == null)
                shippingFaxNumber = string.Empty;
            if (shippingCompany == null)
                shippingCompany = string.Empty;
            if (shippingAddress1 == null)
                shippingAddress1 = string.Empty;
            if (shippingAddress2 == null)
                shippingAddress2 = string.Empty;
            if (shippingCity == null)
                shippingCity = string.Empty;
            if (shippingStateProvince == null)
                shippingStateProvince = string.Empty;
            if (shippingZipPostalCode == null)
                shippingZipPostalCode = string.Empty;
            if (shippingCountry == null)
                shippingCountry = string.Empty;
            if (shippingMethod == null)
                shippingMethod = string.Empty;
            if (trackingNumber == null)
                trackingNumber = string.Empty;
            if (vatNumber == null)
                vatNumber = string.Empty;

            customerIP = CommonHelper.EnsureMaximumLength(customerIP, 50);
            cardType = CommonHelper.EnsureMaximumLength(cardType, 100);
            cardName = CommonHelper.EnsureMaximumLength(cardName, 100);
            cardNumber = CommonHelper.EnsureMaximumLength(cardNumber, 100);
            maskedCreditCardNumber = CommonHelper.EnsureMaximumLength(maskedCreditCardNumber, 100);
            cardCvv2 = CommonHelper.EnsureMaximumLength(cardCvv2, 100);
            cardExpirationMonth = CommonHelper.EnsureMaximumLength(cardExpirationMonth, 100);
            cardExpirationYear = CommonHelper.EnsureMaximumLength(cardExpirationYear, 100);
            paymentMethodName = CommonHelper.EnsureMaximumLength(paymentMethodName, 100);
            authorizationTransactionId = CommonHelper.EnsureMaximumLength(authorizationTransactionId, 4000);
            authorizationTransactionCode = CommonHelper.EnsureMaximumLength(authorizationTransactionCode, 4000);
            authorizationTransactionResult = CommonHelper.EnsureMaximumLength(authorizationTransactionResult, 4000);
            captureTransactionId = CommonHelper.EnsureMaximumLength(captureTransactionId, 4000);
            captureTransactionResult = CommonHelper.EnsureMaximumLength(captureTransactionResult, 4000);
            subscriptionTransactionId = CommonHelper.EnsureMaximumLength(subscriptionTransactionId, 4000);
            purchaseOrderNumber = CommonHelper.EnsureMaximumLength(purchaseOrderNumber, 100);
            billingFirstName = CommonHelper.EnsureMaximumLength(billingFirstName, 100);
            billingLastName = CommonHelper.EnsureMaximumLength(billingLastName, 100);
            billingPhoneNumber = CommonHelper.EnsureMaximumLength(billingPhoneNumber, 50);
            billingEmail = CommonHelper.EnsureMaximumLength(billingEmail, 255);
            billingFaxNumber = CommonHelper.EnsureMaximumLength(billingFaxNumber, 50);
            billingCompany = CommonHelper.EnsureMaximumLength(billingCompany, 100);
            billingAddress1 = CommonHelper.EnsureMaximumLength(billingAddress1, 100);
            billingAddress2 = CommonHelper.EnsureMaximumLength(billingAddress2, 100);
            billingCity = CommonHelper.EnsureMaximumLength(billingCity, 100);
            billingStateProvince = CommonHelper.EnsureMaximumLength(billingStateProvince, 100);
            billingZipPostalCode = CommonHelper.EnsureMaximumLength(billingZipPostalCode, 10);
            billingCountry = CommonHelper.EnsureMaximumLength(billingCountry, 100);
            shippingFirstName = CommonHelper.EnsureMaximumLength(shippingFirstName, 100);
            shippingLastName = CommonHelper.EnsureMaximumLength(shippingLastName, 100);
            shippingPhoneNumber = CommonHelper.EnsureMaximumLength(shippingPhoneNumber, 50);
            shippingEmail = CommonHelper.EnsureMaximumLength(shippingEmail, 255);
            shippingFaxNumber = CommonHelper.EnsureMaximumLength(shippingFaxNumber, 50);
            shippingCompany = CommonHelper.EnsureMaximumLength(shippingCompany, 100);
            shippingAddress1 = CommonHelper.EnsureMaximumLength(shippingAddress1, 100);
            shippingAddress2 = CommonHelper.EnsureMaximumLength(shippingAddress2, 100);
            shippingCity = CommonHelper.EnsureMaximumLength(shippingCity, 100);
            shippingStateProvince = CommonHelper.EnsureMaximumLength(shippingStateProvince, 100);
            shippingZipPostalCode = CommonHelper.EnsureMaximumLength(shippingZipPostalCode, 10);
            shippingCountry = CommonHelper.EnsureMaximumLength(shippingCountry, 100);
            shippingMethod = CommonHelper.EnsureMaximumLength(shippingMethod, 100);
            trackingNumber = CommonHelper.EnsureMaximumLength(trackingNumber, 100);
            vatNumber = CommonHelper.EnsureMaximumLength(vatNumber, 100);

            var context = ObjectContextHelper.CurrentObjectContext;

            var order = context.Orders.CreateObject();
            order.OrderGuid = orderGuid;
            order.CustomerId = customerId;
            order.CustomerLanguageId = customerLanguageId;
            order.CustomerTaxDisplayTypeId = (int)customerTaxDisplayType;
            order.CustomerIP = customerIP;
            order.OrderSubtotalInclTax = orderSubtotalInclTax;
            order.OrderSubtotalExclTax = orderSubtotalExclTax;
            order.OrderShippingInclTax = orderShippingInclTax;
            order.OrderShippingExclTax = orderShippingExclTax;
            order.PaymentMethodAdditionalFeeInclTax = paymentMethodAdditionalFeeInclTax;
            order.PaymentMethodAdditionalFeeExclTax = paymentMethodAdditionalFeeExclTax;
            order.OrderTax = orderTax;
            order.OrderTotal = orderTotal;
            order.OrderDiscount = orderDiscount;
            order.OrderSubtotalInclTaxInCustomerCurrency = orderSubtotalInclTaxInCustomerCurrency;
            order.OrderSubtotalExclTaxInCustomerCurrency = orderSubtotalExclTaxInCustomerCurrency;
            order.OrderShippingInclTaxInCustomerCurrency = orderShippingInclTaxInCustomerCurrency;
            order.OrderShippingExclTaxInCustomerCurrency = orderShippingExclTaxInCustomerCurrency;
            order.PaymentMethodAdditionalFeeInclTaxInCustomerCurrency = paymentMethodAdditionalFeeInclTaxInCustomerCurrency;
            order.PaymentMethodAdditionalFeeExclTaxInCustomerCurrency = paymentMethodAdditionalFeeExclTaxInCustomerCurrency;
            order.OrderTaxInCustomerCurrency = orderTaxInCustomerCurrency;
            order.OrderTotalInCustomerCurrency = orderTotalInCustomerCurrency;
            order.OrderDiscountInCustomerCurrency = orderDiscountInCustomerCurrency;
            order.CheckoutAttributeDescription = checkoutAttributeDescription;
            order.CheckoutAttributesXml = checkoutAttributesXml;
            order.CustomerCurrencyCode = customerCurrencyCode;
            order.OrderWeight = orderWeight;
            order.AffiliateId = affiliateId;
            order.OrderStatusId = (int)orderStatus;
            order.AllowStoringCreditCardNumber = allowStoringCreditCardNumber;
            order.CardType = cardType;
            order.CardName = cardName;
            order.CardNumber = cardNumber;
            order.MaskedCreditCardNumber = maskedCreditCardNumber;
            order.CardCvv2 = cardCvv2;
            order.CardExpirationMonth = cardExpirationMonth;
            order.CardExpirationYear = cardExpirationYear;
            order.PaymentMethodId = paymentMethodId;
            order.PaymentMethodName = paymentMethodName;
            order.AuthorizationTransactionId = authorizationTransactionId;
            order.AuthorizationTransactionCode = authorizationTransactionCode;
            order.AuthorizationTransactionResult = authorizationTransactionResult;
            order.CaptureTransactionId = captureTransactionId;
            order.CaptureTransactionResult = captureTransactionResult;
            order.SubscriptionTransactionId = subscriptionTransactionId;
            order.PurchaseOrderNumber = purchaseOrderNumber;
            order.PaymentStatusId = (int)paymentStatus;
            order.PaidDate = paidDate;
            order.BillingFirstName = billingFirstName;
            order.BillingLastName = billingLastName;
            order.BillingPhoneNumber = billingPhoneNumber;
            order.BillingEmail = billingEmail;
            order.BillingFaxNumber = billingFaxNumber;
            order.BillingCompany = billingCompany;
            order.BillingAddress1 = billingAddress1;
            order.BillingAddress2 = billingAddress2;
            order.BillingCity = billingCity;
            order.BillingStateProvince = billingStateProvince;
            order.BillingStateProvinceId = billingStateProvinceId;
            order.BillingZipPostalCode = billingZipPostalCode;
            order.BillingCountry = billingCountry;
            order.BillingCountryId = billingCountryId;
            order.ShippingStatusId = (int)shippingStatus;
            order.ShippingFirstName = shippingFirstName;
            order.ShippingLastName = shippingLastName;
            order.ShippingPhoneNumber = shippingPhoneNumber;
            order.ShippingEmail = shippingEmail;
            order.ShippingFaxNumber = shippingFaxNumber;
            order.ShippingCompany = shippingCompany;
            order.ShippingAddress1 = shippingAddress1;
            order.ShippingAddress2 = shippingAddress2;
            order.ShippingCity = shippingCity;
            order.ShippingStateProvince = shippingStateProvince;
            order.ShippingStateProvinceId = shippingStateProvinceId;
            order.ShippingZipPostalCode = shippingZipPostalCode;
            order.ShippingCountry = shippingCountry;
            order.ShippingCountryId = shippingCountryId;
            order.ShippingMethod = shippingMethod;
            order.ShippingRateComputationMethodId = shippingRateComputationMethodId;
            order.ShippedDate = shippedDate;
            order.DeliveryDate = deliveryDate;
            order.TrackingNumber = trackingNumber;
            order.VatNumber = vatNumber;
            order.Deleted = deleted;
            order.CreatedOn = createdOn;

            context.Orders.AddObject(order);
            context.SaveChanges();

            //quickbooks
            if (QBManager.QBIsEnabled)
            {
                QBManager.RequestSynchronization(order);
            }

            //raise event             
            EventContext.Current.OnOrderCreated(null,
                new OrderEventArgs() { Order = order });
            
            return order;
        }

        /// <summary>
        /// Updates the order
        /// </summary>
        /// <param name="orderId">The order identifier</param>
        /// <param name="orderGuid">The order identifier</param>
        /// <param name="customerId">The customer identifier</param>
        /// <param name="customerLanguageId">The customer language identifier</param>
        /// <param name="customerTaxDisplayType">The customer tax display type</param>
        /// <param name="customerIP">The customer IP address</param>
        /// <param name="orderSubtotalInclTax">The order subtotal (incl tax)</param>
        /// <param name="orderSubtotalExclTax">The order subtotal (excl tax)</param>
        /// <param name="orderShippingInclTax">The order shipping (incl tax)</param>
        /// <param name="orderShippingExclTax">The order shipping (excl tax)</param>
        /// <param name="paymentMethodAdditionalFeeInclTax">The payment method additional fee (incl tax)</param>
        /// <param name="paymentMethodAdditionalFeeExclTax">The payment method additional fee (excl tax)</param>
        /// <param name="orderTax">The order tax</param>
        /// <param name="orderTotal">The order total</param>
        /// <param name="orderDiscount">The order discount</param>
        /// <param name="orderSubtotalInclTaxInCustomerCurrency">The order subtotal incl tax (customer currency)</param>
        /// <param name="orderSubtotalExclTaxInCustomerCurrency">The order subtotal excl tax (customer currency)</param>
        /// <param name="orderShippingInclTaxInCustomerCurrency">The order shipping incl tax (customer currency)</param>
        /// <param name="orderShippingExclTaxInCustomerCurrency">The order shipping excl tax (customer currency)</param>
        /// <param name="paymentMethodAdditionalFeeInclTaxInCustomerCurrency">The payment method additional fee incl tax (customer currency)</param>
        /// <param name="paymentMethodAdditionalFeeExclTaxInCustomerCurrency">The payment method additional fee excl tax (customer currency)</param>
        /// <param name="orderTaxInCustomerCurrency">The order tax (customer currency)</param>
        /// <param name="orderTotalInCustomerCurrency">The order total (customer currency)</param>
        /// <param name="orderDiscountInCustomerCurrency">The order discount (customer currency)</param>
        /// <param name="checkoutAttributeDescription">The checkout attribute description</param>
        /// <param name="checkoutAttributesXml">The checkout attributes in XML format</param>
        /// <param name="customerCurrencyCode">The customer currency code</param>
        /// <param name="orderWeight">The order weight</param>
        /// <param name="affiliateId">The affiliate identifier</param>
        /// <param name="orderStatus">The order status</param>
        /// <param name="allowStoringCreditCardNumber">The value indicating whether storing of credit card number is allowed</param>
        /// <param name="cardType">The card type</param>
        /// <param name="cardName">The card name</param>
        /// <param name="cardNumber">The card number</param>
        /// <param name="maskedCreditCardNumber">The masked credit card number</param>
        /// <param name="cardCvv2">The card CVV2</param>
        /// <param name="cardExpirationMonth">The card expiration month</param>
        /// <param name="cardExpirationYear">The card expiration year</param>
        /// <param name="paymentMethodId">The payment method identifier</param>
        /// <param name="paymentMethodName">The payment method name</param>
        /// <param name="authorizationTransactionId">The authorization transaction identifier</param>
        /// <param name="authorizationTransactionCode">The authorization transaction code</param>
        /// <param name="authorizationTransactionResult">The authorization transaction result</param>
        /// <param name="captureTransactionId">The capture transaction identifier</param>
        /// <param name="captureTransactionResult">The capture transaction result</param>
        /// <param name="subscriptionTransactionId">The subscription transaction identifier</param>
        /// <param name="purchaseOrderNumber">The purchase order number</param>
        /// <param name="paymentStatus">The payment status</param>
        /// <param name="paidDate">The paid date and time</param>
        /// <param name="billingFirstName">The billing first name</param>
        /// <param name="billingLastName">The billing last name</param>
        /// <param name="billingPhoneNumber">he billing phone number</param>
        /// <param name="billingEmail">The billing email</param>
        /// <param name="billingFaxNumber">The billing fax number</param>
        /// <param name="billingCompany">The billing company</param>
        /// <param name="billingAddress1">The billing address 1</param>
        /// <param name="billingAddress2">The billing address 2</param>
        /// <param name="billingCity">The billing city</param>
        /// <param name="billingStateProvince">The billing state/province</param>
        /// <param name="billingStateProvinceId">The billing state/province identifier</param>
        /// <param name="billingZipPostalCode">The billing zip/postal code</param>
        /// <param name="billingCountry">The billing country</param>
        /// <param name="billingCountryId">The billing country identifier</param>
        /// <param name="shippingStatus">The shipping status</param>
        /// <param name="shippingFirstName">The shipping first name</param>
        /// <param name="shippingLastName">The shipping last name</param>
        /// <param name="shippingPhoneNumber">The shipping phone number</param>
        /// <param name="shippingEmail">The shipping email</param>
        /// <param name="shippingFaxNumber">The shipping fax number</param>
        /// <param name="shippingCompany">The shipping  company</param>
        /// <param name="shippingAddress1">The shipping address 1</param>
        /// <param name="shippingAddress2">The shipping address 2</param>
        /// <param name="shippingCity">The shipping city</param>
        /// <param name="shippingStateProvince">The shipping state/province</param>
        /// <param name="shippingStateProvinceId">The shipping state/province identifier</param>
        /// <param name="shippingZipPostalCode">The shipping zip/postal code</param>
        /// <param name="shippingCountry">The shipping country</param>
        /// <param name="shippingCountryId">The shipping country identifier</param>
        /// <param name="shippingMethod">The shipping method</param>
        /// <param name="shippingRateComputationMethodId">The shipping rate computation method identifier</param>
        /// <param name="shippedDate">The shipped date and time</param>
        /// <param name="deliveryDate">The delivery date and time</param>
        /// <param name="trackingNumber">The tracking number of order</param>
        /// <param name="vatNumber">The VAT number (the European Union Value Added Tax)</param>
        /// <param name="deleted">A value indicating whether the entity has been deleted</param>
        /// <param name="createdOn">The date and time of order creation</param>
        /// <returns>Order</returns>
        public static Order UpdateOrder(int orderId,
            Guid orderGuid,
            int customerId,
            int customerLanguageId,
            TaxDisplayTypeEnum customerTaxDisplayType,
            string customerIP,
            decimal orderSubtotalInclTax,
            decimal orderSubtotalExclTax,
            decimal orderShippingInclTax,
            decimal orderShippingExclTax,
            decimal paymentMethodAdditionalFeeInclTax,
            decimal paymentMethodAdditionalFeeExclTax,
            decimal orderTax,
            decimal orderTotal,
            decimal orderDiscount,
            decimal orderSubtotalInclTaxInCustomerCurrency,
            decimal orderSubtotalExclTaxInCustomerCurrency,
            decimal orderShippingInclTaxInCustomerCurrency,
            decimal orderShippingExclTaxInCustomerCurrency,
            decimal paymentMethodAdditionalFeeInclTaxInCustomerCurrency,
            decimal paymentMethodAdditionalFeeExclTaxInCustomerCurrency,
            decimal orderTaxInCustomerCurrency,
            decimal orderTotalInCustomerCurrency,
            decimal orderDiscountInCustomerCurrency,
            string checkoutAttributeDescription,
            string checkoutAttributesXml,
            string customerCurrencyCode,
            decimal orderWeight,
            int affiliateId,
            OrderStatusEnum orderStatus,
            bool allowStoringCreditCardNumber,
            string cardType,
            string cardName,
            string cardNumber,
            string maskedCreditCardNumber,
            string cardCvv2,
            string cardExpirationMonth,
            string cardExpirationYear,
            int paymentMethodId,
            string paymentMethodName,
            string authorizationTransactionId,
            string authorizationTransactionCode,
            string authorizationTransactionResult,
            string captureTransactionId,
            string captureTransactionResult,
            string subscriptionTransactionId,
            string purchaseOrderNumber,
            PaymentStatusEnum paymentStatus,
            DateTime? paidDate,
            string billingFirstName,
            string billingLastName,
            string billingPhoneNumber,
            string billingEmail,
            string billingFaxNumber,
            string billingCompany,
            string billingAddress1,
            string billingAddress2,
            string billingCity,
            string billingStateProvince,
            int billingStateProvinceId,
            string billingZipPostalCode,
            string billingCountry,
            int billingCountryId,
            ShippingStatusEnum shippingStatus,
            string shippingFirstName,
            string shippingLastName,
            string shippingPhoneNumber,
            string shippingEmail,
            string shippingFaxNumber,
            string shippingCompany,
            string shippingAddress1,
            string shippingAddress2,
            string shippingCity,
            string shippingStateProvince,
            int shippingStateProvinceId,
            string shippingZipPostalCode,
            string shippingCountry,
            int shippingCountryId,
            string shippingMethod,
            int shippingRateComputationMethodId,
            DateTime? shippedDate,
            DateTime? deliveryDate,
            string trackingNumber,
            string vatNumber,
            bool deleted,
            DateTime createdOn)
        {
            if (trackingNumber == null)
                trackingNumber = string.Empty;

            customerIP = CommonHelper.EnsureMaximumLength(customerIP, 50);
            cardType = CommonHelper.EnsureMaximumLength(cardType, 100);
            cardName = CommonHelper.EnsureMaximumLength(cardName, 100);
            cardNumber = CommonHelper.EnsureMaximumLength(cardNumber, 100);
            maskedCreditCardNumber = CommonHelper.EnsureMaximumLength(maskedCreditCardNumber, 100);
            cardCvv2 = CommonHelper.EnsureMaximumLength(cardCvv2, 100);
            cardExpirationMonth = CommonHelper.EnsureMaximumLength(cardExpirationMonth, 100);
            cardExpirationYear = CommonHelper.EnsureMaximumLength(cardExpirationYear, 100);
            paymentMethodName = CommonHelper.EnsureMaximumLength(paymentMethodName, 100);
            authorizationTransactionId = CommonHelper.EnsureMaximumLength(authorizationTransactionId, 4000);
            authorizationTransactionCode = CommonHelper.EnsureMaximumLength(authorizationTransactionCode, 4000);
            authorizationTransactionResult = CommonHelper.EnsureMaximumLength(authorizationTransactionResult, 4000);
            captureTransactionId = CommonHelper.EnsureMaximumLength(captureTransactionId, 4000);
            captureTransactionResult = CommonHelper.EnsureMaximumLength(captureTransactionResult, 4000);
            subscriptionTransactionId = CommonHelper.EnsureMaximumLength(subscriptionTransactionId, 4000);
            purchaseOrderNumber = CommonHelper.EnsureMaximumLength(purchaseOrderNumber, 100);
            billingFirstName = CommonHelper.EnsureMaximumLength(billingFirstName, 100);
            billingLastName = CommonHelper.EnsureMaximumLength(billingLastName, 100);
            billingPhoneNumber = CommonHelper.EnsureMaximumLength(billingPhoneNumber, 50);
            billingEmail = CommonHelper.EnsureMaximumLength(billingEmail, 255);
            billingFaxNumber = CommonHelper.EnsureMaximumLength(billingFaxNumber, 50);
            billingCompany = CommonHelper.EnsureMaximumLength(billingCompany, 100);
            billingAddress1 = CommonHelper.EnsureMaximumLength(billingAddress1, 100);
            billingAddress2 = CommonHelper.EnsureMaximumLength(billingAddress2, 100);
            billingCity = CommonHelper.EnsureMaximumLength(billingCity, 100);
            billingStateProvince = CommonHelper.EnsureMaximumLength(billingStateProvince, 100);
            billingZipPostalCode = CommonHelper.EnsureMaximumLength(billingZipPostalCode, 10);
            billingCountry = CommonHelper.EnsureMaximumLength(billingCountry, 100);
            shippingFirstName = CommonHelper.EnsureMaximumLength(shippingFirstName, 100);
            shippingLastName = CommonHelper.EnsureMaximumLength(shippingLastName, 100);
            shippingPhoneNumber = CommonHelper.EnsureMaximumLength(shippingPhoneNumber, 50);
            shippingEmail = CommonHelper.EnsureMaximumLength(shippingEmail, 255);
            shippingFaxNumber = CommonHelper.EnsureMaximumLength(shippingFaxNumber, 50);
            shippingCompany = CommonHelper.EnsureMaximumLength(shippingCompany, 100);
            shippingAddress1 = CommonHelper.EnsureMaximumLength(shippingAddress1, 100);
            shippingAddress2 = CommonHelper.EnsureMaximumLength(shippingAddress2, 100);
            shippingCity = CommonHelper.EnsureMaximumLength(shippingCity, 100);
            shippingStateProvince = CommonHelper.EnsureMaximumLength(shippingStateProvince, 100);
            shippingZipPostalCode = CommonHelper.EnsureMaximumLength(shippingZipPostalCode, 10);
            shippingCountry = CommonHelper.EnsureMaximumLength(shippingCountry, 100);
            shippingMethod = CommonHelper.EnsureMaximumLength(shippingMethod, 100);
            trackingNumber = CommonHelper.EnsureMaximumLength(trackingNumber, 100);
            vatNumber = CommonHelper.EnsureMaximumLength(vatNumber, 100);

            var order = GetOrderById(orderId);
            if (order == null)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(order))
                context.Orders.Attach(order);

            order.OrderGuid = orderGuid;
            order.CustomerId = customerId;
            order.CustomerLanguageId = customerLanguageId;
            order.CustomerTaxDisplayTypeId = (int)customerTaxDisplayType;
            order.CustomerIP = customerIP;
            order.OrderSubtotalInclTax = orderSubtotalInclTax;
            order.OrderSubtotalExclTax = orderSubtotalExclTax;
            order.OrderShippingInclTax = orderShippingInclTax;
            order.OrderShippingExclTax = orderShippingExclTax;
            order.PaymentMethodAdditionalFeeInclTax = paymentMethodAdditionalFeeInclTax;
            order.PaymentMethodAdditionalFeeExclTax = paymentMethodAdditionalFeeExclTax;
            order.OrderTax = orderTax;
            order.OrderTotal = orderTotal;
            order.OrderDiscount = orderDiscount;
            order.OrderSubtotalInclTaxInCustomerCurrency = orderSubtotalInclTaxInCustomerCurrency;
            order.OrderSubtotalExclTaxInCustomerCurrency = orderSubtotalExclTaxInCustomerCurrency;
            order.OrderShippingInclTaxInCustomerCurrency = orderShippingInclTaxInCustomerCurrency;
            order.OrderShippingExclTaxInCustomerCurrency = orderShippingExclTaxInCustomerCurrency;
            order.PaymentMethodAdditionalFeeInclTaxInCustomerCurrency = paymentMethodAdditionalFeeInclTaxInCustomerCurrency;
            order.PaymentMethodAdditionalFeeExclTaxInCustomerCurrency = paymentMethodAdditionalFeeExclTaxInCustomerCurrency;
            order.OrderTaxInCustomerCurrency = orderTaxInCustomerCurrency;
            order.OrderTotalInCustomerCurrency = orderTotalInCustomerCurrency;
            order.OrderDiscountInCustomerCurrency = orderDiscountInCustomerCurrency;
            order.CheckoutAttributeDescription = checkoutAttributeDescription;
            order.CheckoutAttributesXml = checkoutAttributesXml;
            order.CustomerCurrencyCode = customerCurrencyCode;
            order.OrderWeight = orderWeight;
            order.AffiliateId = affiliateId;
            order.OrderStatusId = (int)orderStatus;
            order.AllowStoringCreditCardNumber = allowStoringCreditCardNumber;
            order.CardType = cardType;
            order.CardName = cardName;
            order.CardNumber = cardNumber;
            order.MaskedCreditCardNumber = maskedCreditCardNumber;
            order.CardCvv2 = cardCvv2;
            order.CardExpirationMonth = cardExpirationMonth;
            order.CardExpirationYear = cardExpirationYear;
            order.PaymentMethodId = paymentMethodId;
            order.PaymentMethodName = paymentMethodName;
            order.AuthorizationTransactionId = authorizationTransactionId;
            order.AuthorizationTransactionCode = authorizationTransactionCode;
            order.AuthorizationTransactionResult = authorizationTransactionResult;
            order.CaptureTransactionId = captureTransactionId;
            order.CaptureTransactionResult = captureTransactionResult;
            order.SubscriptionTransactionId = subscriptionTransactionId;
            order.PurchaseOrderNumber = purchaseOrderNumber;
            order.PaymentStatusId = (int)paymentStatus;
            order.PaidDate = paidDate;
            order.BillingFirstName = billingFirstName;
            order.BillingLastName = billingLastName;
            order.BillingPhoneNumber = billingPhoneNumber;
            order.BillingEmail = billingEmail;
            order.BillingFaxNumber = billingFaxNumber;
            order.BillingCompany = billingCompany;
            order.BillingAddress1 = billingAddress1;
            order.BillingAddress2 = billingAddress2;
            order.BillingCity = billingCity;
            order.BillingStateProvince = billingStateProvince;
            order.BillingStateProvinceId = billingStateProvinceId;
            order.BillingZipPostalCode = billingZipPostalCode;
            order.BillingCountry = billingCountry;
            order.BillingCountryId = billingCountryId;
            order.ShippingStatusId = (int)shippingStatus;
            order.ShippingFirstName = shippingFirstName;
            order.ShippingLastName = shippingLastName;
            order.ShippingPhoneNumber = shippingPhoneNumber;
            order.ShippingEmail = shippingEmail;
            order.ShippingFaxNumber = shippingFaxNumber;
            order.ShippingCompany = shippingCompany;
            order.ShippingAddress1 = shippingAddress1;
            order.ShippingAddress2 = shippingAddress2;
            order.ShippingCity = shippingCity;
            order.ShippingStateProvince = shippingStateProvince;
            order.ShippingStateProvinceId = shippingStateProvinceId;
            order.ShippingZipPostalCode = shippingZipPostalCode;
            order.ShippingCountry = shippingCountry;
            order.ShippingCountryId = shippingCountryId;
            order.ShippingMethod = shippingMethod;
            order.ShippingRateComputationMethodId = shippingRateComputationMethodId;
            order.ShippedDate = shippedDate;
            order.DeliveryDate = deliveryDate;
            order.TrackingNumber = trackingNumber;
            order.VatNumber = vatNumber;
            order.Deleted = deleted;
            order.CreatedOn = createdOn;
            context.SaveChanges();

            //quickbooks
            if (QBManager.QBIsEnabled)
            {
                QBManager.RequestSynchronization(order);
            }


            //raise event             
            EventContext.Current.OnOrderUpdated(null,
                new OrderEventArgs() { Order = order });
            
            return order;
        }

        /// <summary>
        /// Set tracking number of order
        /// </summary>
        /// <param name="orderId">Order note identifier</param>
        /// <param name="trackingNumber">The tracking number of order</param>
        public static void SetOrderTrackingNumber(int orderId, string trackingNumber)
        {
            var order = GetOrderById(orderId);
            if (order != null)
            {
                UpdateOrder(
                   order.OrderId, order.OrderGuid, order.CustomerId, order.CustomerLanguageId,
                   order.CustomerTaxDisplayType, order.CustomerIP, order.OrderSubtotalInclTax, order.OrderSubtotalExclTax, order.OrderShippingInclTax,
                   order.OrderShippingExclTax, order.PaymentMethodAdditionalFeeInclTax, order.PaymentMethodAdditionalFeeExclTax,
                   order.OrderTax, order.OrderTotal, order.OrderDiscount,
                   order.OrderSubtotalInclTaxInCustomerCurrency, order.OrderSubtotalExclTaxInCustomerCurrency,
                   order.OrderShippingInclTaxInCustomerCurrency, order.OrderShippingExclTaxInCustomerCurrency,
                   order.PaymentMethodAdditionalFeeInclTaxInCustomerCurrency, order.PaymentMethodAdditionalFeeExclTaxInCustomerCurrency,
                   order.OrderTaxInCustomerCurrency, order.OrderTotalInCustomerCurrency,
                   order.OrderDiscountInCustomerCurrency,
                   order.CheckoutAttributeDescription, order.CheckoutAttributesXml, 
                   order.CustomerCurrencyCode, order.OrderWeight,
                   order.AffiliateId, order.OrderStatus, order.AllowStoringCreditCardNumber, order.CardType,
                   order.CardName, order.CardNumber, order.MaskedCreditCardNumber,
                   order.CardCvv2, order.CardExpirationMonth, order.CardExpirationYear,
                   order.PaymentMethodId, order.PaymentMethodName, order.AuthorizationTransactionId,
                   order.AuthorizationTransactionCode, order.AuthorizationTransactionResult,
                   order.CaptureTransactionId, order.CaptureTransactionResult,
                   order.SubscriptionTransactionId, order.PurchaseOrderNumber, order.PaymentStatus, order.PaidDate,
                   order.BillingFirstName, order.BillingLastName, order.BillingPhoneNumber,
                   order.BillingEmail, order.BillingFaxNumber, order.BillingCompany, order.BillingAddress1,
                   order.BillingAddress2, order.BillingCity, order.BillingStateProvince,
                   order.BillingStateProvinceId, order.BillingZipPostalCode, order.BillingCountry,
                   order.BillingCountryId, order.ShippingStatus,
                   order.ShippingFirstName, order.ShippingLastName, order.ShippingPhoneNumber,
                   order.ShippingEmail, order.ShippingFaxNumber, order.ShippingCompany,
                   order.ShippingAddress1, order.ShippingAddress2, order.ShippingCity,
                   order.ShippingStateProvince, order.ShippingStateProvinceId, order.ShippingZipPostalCode,
                   order.ShippingCountry, order.ShippingCountryId,
                   order.ShippingMethod, order.ShippingRateComputationMethodId,
                   order.ShippedDate, order.DeliveryDate,
                   trackingNumber, order.VatNumber, order.Deleted, order.CreatedOn);
            }
        }

        #endregion
        
        #region Orders product variants

        /// <summary>
        /// Gets an order product variant
        /// </summary>
        /// <param name="orderProductVariantId">Order product variant identifier</param>
        /// <returns>Order product variant</returns>
        public static OrderProductVariant GetOrderProductVariantById(int orderProductVariantId)
        {
            if (orderProductVariantId == 0)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from opv in context.OrderProductVariants
                        where opv.OrderProductVariantId == orderProductVariantId
                        select opv;
            var orderProductVariant = query.SingleOrDefault();

            return orderProductVariant;
        }

        /// <summary>
        /// Delete an order product variant
        /// </summary>
        /// <param name="orderProductVariantId">Order product variant identifier</param>
        public static void DeleteOrderProductVariant(int orderProductVariantId)
        {
            if (orderProductVariantId == 0)
                return;

            var orderProductVariant = GetOrderProductVariantById(orderProductVariantId);
            if (orderProductVariant == null)
                return;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(orderProductVariant))
                context.OrderProductVariants.Attach(orderProductVariant);
            context.DeleteObject(orderProductVariant);
            context.SaveChanges();
        }

        /// <summary>
        /// Gets an order product variant
        /// </summary>
        /// <param name="orderProductVariantGuid">Order product variant identifier</param>
        /// <returns>Order product variant</returns>
        public static OrderProductVariant GetOrderProductVariantByGuid(Guid orderProductVariantGuid)
        {
            if (orderProductVariantGuid == Guid.Empty)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from opv in context.OrderProductVariants
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
        public static List<OrderProductVariant> GetAllOrderProductVariants(int? orderId,
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
        public static List<OrderProductVariant> GetAllOrderProductVariants(int? orderId,
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

            var context = ObjectContextHelper.CurrentObjectContext;

            var query = from opv in context.OrderProductVariants
                        join o in context.Orders on opv.OrderId equals o.OrderId
                        join pv in context.ProductVariants on opv.ProductVariantId equals pv.ProductVariantId
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
        public static List<OrderProductVariant> GetOrderProductVariantsByOrderId(int orderId)
        {
            return GetAllOrderProductVariants(orderId, null, null, null, null, null, null);
        }

        /// <summary>
        /// Inserts a order product variant
        /// </summary>
        /// <param name="orderProductVariantGuid">The order product variant identifier</param>
        /// <param name="orderId">The order identifier</param>
        /// <param name="productVariantId">The product variant identifier</param>
        /// <param name="unitPriceInclTax">The unit price in primary store currency (incl tax)</param>
        /// <param name="unitPriceExclTax">The unit price in primary store currency (excl tax)</param>
        /// <param name="priceInclTax">The price in primary store currency (incl tax)</param>
        /// <param name="priceExclTax">The price in primary store currency (excl tax)</param>
        /// <param name="unitPriceInclTaxInCustomerCurrency">The unit price in primary store currency (incl tax)</param>
        /// <param name="unitPriceExclTaxInCustomerCurrency">The unit price in customer currency (excl tax)</param>
        /// <param name="priceInclTaxInCustomerCurrency">The price in primary store currency (incl tax)</param>
        /// <param name="priceExclTaxInCustomerCurrency">The price in customer currency (excl tax)</param>
        /// <param name="attributeDescription">The attribute description</param>
        /// <param name="attributesXml">The attribute description in XML format</param>
        /// <param name="quantity">The quantity</param>
        /// <param name="discountAmountInclTax">The discount amount (incl tax)</param>
        /// <param name="discountAmountExclTax">The discount amount (excl tax)</param>
        /// <param name="downloadCount">The download count</param>
        /// <param name="isDownloadActivated">The value indicating whether download is activated</param>
        /// <param name="licenseDownloadId">A license download identifier (in case this is a downloadable product)</param>
        /// <returns>Order product variant</returns>
        public static OrderProductVariant InsertOrderProductVariant(Guid orderProductVariantGuid,
            int orderId,
            int productVariantId,
            decimal unitPriceInclTax,
            decimal unitPriceExclTax,
            decimal priceInclTax,
            decimal priceExclTax,
            decimal unitPriceInclTaxInCustomerCurrency,
            decimal unitPriceExclTaxInCustomerCurrency,
            decimal priceInclTaxInCustomerCurrency,
            decimal priceExclTaxInCustomerCurrency,
            string attributeDescription,
            string attributesXml,
            int quantity,
            decimal discountAmountInclTax,
            decimal discountAmountExclTax,
            int downloadCount,
            bool isDownloadActivated,
            int licenseDownloadId)
        {
            if (attributeDescription == null)
                attributeDescription = string.Empty;

            attributeDescription = CommonHelper.EnsureMaximumLength(attributeDescription, 4000);

            var context = ObjectContextHelper.CurrentObjectContext;

            var opv = context.OrderProductVariants.CreateObject();
            opv.OrderProductVariantGuid = orderProductVariantGuid;
            opv.OrderId = orderId;
            opv.ProductVariantId = productVariantId;
            opv.UnitPriceInclTax = unitPriceInclTax;
            opv.UnitPriceExclTax = unitPriceExclTax;
            opv.PriceInclTax = priceInclTax;
            opv.PriceExclTax = priceExclTax;
            opv.UnitPriceInclTaxInCustomerCurrency = unitPriceInclTaxInCustomerCurrency;
            opv.UnitPriceExclTaxInCustomerCurrency = unitPriceExclTaxInCustomerCurrency;
            opv.PriceInclTaxInCustomerCurrency = priceInclTaxInCustomerCurrency;
            opv.PriceExclTaxInCustomerCurrency = priceExclTaxInCustomerCurrency;
            opv.AttributeDescription = attributeDescription;
            opv.AttributesXml = attributesXml;
            opv.Quantity = quantity;
            opv.DiscountAmountInclTax = discountAmountInclTax;
            opv.DiscountAmountExclTax = discountAmountExclTax;
            opv.DownloadCount = downloadCount;
            opv.IsDownloadActivated = isDownloadActivated;
            opv.LicenseDownloadId = licenseDownloadId;

            context.OrderProductVariants.AddObject(opv);
            context.SaveChanges();

            return opv;
        }

        /// <summary>
        /// Updates the order product variant
        /// </summary>
        /// <param name="orderProductVariantId">The order product variant identifier</param>
        /// <param name="orderProductVariantGuid">The order product variant identifier</param>
        /// <param name="orderId">The order identifier</param>
        /// <param name="productVariantId">The product variant identifier</param>
        /// <param name="unitPriceInclTax">The unit price in primary store currency (incl tax)</param>
        /// <param name="unitPriceExclTax">The unit price in primary store currency (excl tax)</param>
        /// <param name="priceInclTax">The price in primary store currency (incl tax)</param>
        /// <param name="priceExclTax">The price in primary store currency (excl tax)</param>
        /// <param name="unitPriceInclTaxInCustomerCurrency">The unit price in primary store currency (incl tax)</param>
        /// <param name="unitPriceExclTaxInCustomerCurrency">The unit price in customer currency (excl tax)</param>
        /// <param name="priceInclTaxInCustomerCurrency">The price in primary store currency (incl tax)</param>
        /// <param name="priceExclTaxInCustomerCurrency">The price in customer currency (excl tax)</param>
        /// <param name="attributeDescription">The attribute description</param>
        /// <param name="attributesXml">The attribute description in XML format</param>
        /// <param name="quantity">The quantity</param>
        /// <param name="discountAmountInclTax">The discount amount (incl tax)</param>
        /// <param name="discountAmountExclTax">The discount amount (excl tax)</param>
        /// <param name="downloadCount">The download count</param>
        /// <param name="isDownloadActivated">The value indicating whether download is activated</param>
        /// <param name="licenseDownloadId">A license download identifier (in case this is a downloadable product)</param>
        /// <returns>Order product variant</returns>
        public static OrderProductVariant UpdateOrderProductVariant(int orderProductVariantId,
            Guid orderProductVariantGuid,
            int orderId,
            int productVariantId,
            decimal unitPriceInclTax,
            decimal unitPriceExclTax,
            decimal priceInclTax,
            decimal priceExclTax,
            decimal unitPriceInclTaxInCustomerCurrency,
            decimal unitPriceExclTaxInCustomerCurrency,
            decimal priceInclTaxInCustomerCurrency,
            decimal priceExclTaxInCustomerCurrency,
            string attributeDescription,
            string attributesXml,
            int quantity,
            decimal discountAmountInclTax,
            decimal discountAmountExclTax,
            int downloadCount,
            bool isDownloadActivated,
            int licenseDownloadId)
        {
            if (attributeDescription == null)
                attributeDescription = string.Empty;

            attributeDescription = CommonHelper.EnsureMaximumLength(attributeDescription, 4000);

            var opv = GetOrderProductVariantById(orderProductVariantId);
            if (opv == null)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(opv))
                context.OrderProductVariants.Attach(opv);

            opv.OrderProductVariantGuid = orderProductVariantGuid;
            opv.OrderId = orderId;
            opv.ProductVariantId = productVariantId;
            opv.UnitPriceInclTax = unitPriceInclTax;
            opv.UnitPriceExclTax = unitPriceExclTax;
            opv.PriceInclTax = priceInclTax;
            opv.PriceExclTax = priceExclTax;
            opv.UnitPriceInclTaxInCustomerCurrency = unitPriceInclTaxInCustomerCurrency;
            opv.UnitPriceExclTaxInCustomerCurrency = unitPriceExclTaxInCustomerCurrency;
            opv.PriceInclTaxInCustomerCurrency = priceInclTaxInCustomerCurrency;
            opv.PriceExclTaxInCustomerCurrency = priceExclTaxInCustomerCurrency;
            opv.AttributeDescription = attributeDescription;
            opv.AttributesXml = attributesXml;
            opv.Quantity = quantity;
            opv.DiscountAmountInclTax = discountAmountInclTax;
            opv.DiscountAmountExclTax = discountAmountExclTax;
            opv.DownloadCount = downloadCount;
            opv.IsDownloadActivated = isDownloadActivated;
            opv.LicenseDownloadId = licenseDownloadId;
            context.SaveChanges();

            return opv;
        }

        /// <summary>
        /// Increase an order product variant download count
        /// </summary>
        /// <param name="orderProductVariantId">Order product variant identifier</param>
        /// <returns>Order product variant</returns>
        public static OrderProductVariant IncreaseOrderProductDownloadCount(int orderProductVariantId)
        {
            var orderProductVariant = GetOrderProductVariantById(orderProductVariantId);
            if (orderProductVariant == null)
                throw new NopException("Order product variant could not be loaded");

            int newDownloadCount = orderProductVariant.DownloadCount + 1;

            orderProductVariant = UpdateOrderProductVariant(orderProductVariant.OrderProductVariantId,
                orderProductVariant.OrderProductVariantGuid, orderProductVariant.OrderId,
                orderProductVariant.ProductVariantId,
                orderProductVariant.UnitPriceInclTax, orderProductVariant.UnitPriceExclTax,
                orderProductVariant.PriceInclTax, orderProductVariant.PriceExclTax,
                orderProductVariant.UnitPriceInclTaxInCustomerCurrency, orderProductVariant.UnitPriceExclTaxInCustomerCurrency,
                orderProductVariant.PriceInclTaxInCustomerCurrency, orderProductVariant.PriceExclTaxInCustomerCurrency,
                orderProductVariant.AttributeDescription, orderProductVariant.AttributesXml,
                orderProductVariant.Quantity,
                orderProductVariant.DiscountAmountInclTax, orderProductVariant.DiscountAmountExclTax,
                newDownloadCount, orderProductVariant.IsDownloadActivated,
                orderProductVariant.LicenseDownloadId);

            return orderProductVariant;
        }

        #endregion

        #region Order notes

        /// <summary>
        /// Gets an order note
        /// </summary>
        /// <param name="orderNoteId">Order note identifier</param>
        /// <returns>Order note</returns>
        public static OrderNote GetOrderNoteById(int orderNoteId)
        {
            if (orderNoteId == 0)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from onote in context.OrderNotes
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
        public static List<OrderNote> GetOrderNoteByOrderId(int orderId)
        {
            return GetOrderNoteByOrderId(orderId, NopContext.Current.IsAdmin);
        }

        /// <summary>
        /// Gets an order notes by order identifier
        /// </summary>
        /// <param name="orderId">Order identifier</param>
        /// <param name="showHidden">A value indicating whether all orders should be loaded</param>
        /// <returns>Order note collection</returns>
        public static List<OrderNote> GetOrderNoteByOrderId(int orderId, bool showHidden)
        {
            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from onote in context.OrderNotes
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
        public static void DeleteOrderNote(int orderNoteId)
        {
            var orderNote = GetOrderNoteById(orderNoteId);
            if (orderNote == null)
                return;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(orderNote))
                context.OrderNotes.Attach(orderNote);
            context.DeleteObject(orderNote);
            context.SaveChanges();
        }

        /// <summary>
        /// Inserts an order note
        /// </summary>
        /// <param name="orderId">The order identifier</param>
        /// <param name="note">The note</param>
        /// <param name="createdOn">The date and time of order note creation</param>
        /// <returns>Order note</returns>
        public static OrderNote InsertOrderNote(int orderId, string note, DateTime createdOn)
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
        public static OrderNote InsertOrderNote(int orderId, string note, 
            bool displayToCustomer, DateTime createdOn)
        {
            note = CommonHelper.EnsureMaximumLength(note, 4000);

            var context = ObjectContextHelper.CurrentObjectContext;

            var orderNote = context.OrderNotes.CreateObject();
            orderNote.OrderId = orderId;
            orderNote.Note = note;
            orderNote.DisplayToCustomer = displayToCustomer;
            orderNote.CreatedOn = createdOn;

            context.OrderNotes.AddObject(orderNote);
            context.SaveChanges();
            return orderNote;
        }

        /// <summary>
        /// Updates the order note
        /// </summary>
        /// <param name="orderNoteId">The order note identifier</param>
        /// <param name="orderId">The order identifier</param>
        /// <param name="note">The note</param>
        /// <param name="displayToCustomer">A value indicating whether the customer can see a note</param>
        /// <param name="createdOn">The date and time of order note creation</param>
        /// <returns>Order note</returns>
        public static OrderNote UpdateOrderNote(int orderNoteId, int orderId, 
            string note, bool displayToCustomer, DateTime createdOn)
        {
            note = CommonHelper.EnsureMaximumLength(note, 4000);

            var orderNote = GetOrderNoteById(orderNoteId);
            if (orderNote == null)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(orderNote))
                context.OrderNotes.Attach(orderNote);

            orderNote.OrderId = orderId;
            orderNote.Note = note;
            orderNote.DisplayToCustomer = displayToCustomer;
            orderNote.CreatedOn = createdOn;
            context.SaveChanges();
            return orderNote;
        }

        #endregion

        #region Order statuses

        /// <summary>
        /// Gets an order status full name
        /// </summary>
        /// <param name="orderStatusId">Order status identifier</param>
        /// <returns>Order status name</returns>
        public static string GetOrderStatusName(int orderStatusId)
        {
            var orderStatus = GetOrderStatusById(orderStatusId);
            if (orderStatus != null)
            {
                string name = string.Empty;
                if (NopContext.Current != null)
                {
                    name = LocalizationManager.GetLocaleResourceString(string.Format("OrderStatus.{0}", (OrderStatusEnum)orderStatus.OrderStatusId), NopContext.Current.WorkingLanguage.LanguageId, true, orderStatus.Name);
                }
                else
                {
                    name = orderStatus.Name;
                }
                return name;
            }
            else
            {
                return ((OrderStatusEnum)orderStatusId).ToString();
            }
        }

        /// <summary>
        /// Gets an order status by Id
        /// </summary>
        /// <param name="orderStatusId">Order status identifier</param>
        /// <returns>Order status</returns>
        public static OrderStatus GetOrderStatusById(int orderStatusId)
        {
            if (orderStatusId == 0)
                return null;

            string key = string.Format(ORDERSTATUSES_BY_ID_KEY, orderStatusId);
            object obj2 = NopRequestCache.Get(key);
            if (OrderManager.CacheEnabled && (obj2 != null))
            {
                return (OrderStatus)obj2;
            }

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from os in context.OrderStatuses
                        where os.OrderStatusId == orderStatusId
                        select os;
            var orderStatus = query.SingleOrDefault();

            if (OrderManager.CacheEnabled)
            {
                NopRequestCache.Add(key, orderStatus);
            }
            return orderStatus;
        }

        /// <summary>
        /// Gets all order statuses
        /// </summary>
        /// <returns>Order status collection</returns>
        public static List<OrderStatus> GetAllOrderStatuses()
        {
            string key = string.Format(ORDERSTATUSES_ALL_KEY);
            object obj2 = NopRequestCache.Get(key);
            if (OrderManager.CacheEnabled && (obj2 != null))
            {
                return (List<OrderStatus>)obj2;
            }

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from os in context.OrderStatuses
                        orderby os.OrderStatusId
                        select os;
            var orderStatuses = query.ToList();

            if (OrderManager.CacheEnabled)
            {
                NopRequestCache.Add(key, orderStatuses);
            }
            return orderStatuses;
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
        public static List<OrderProductVariantReportLine> OrderProductVariantReport(DateTime? startTime,
            DateTime? endTime, OrderStatusEnum? os, PaymentStatusEnum? ps,
            int? billingCountryId)
        {
            int? orderStatusId = null;
            if (os.HasValue)
                orderStatusId = (int)os.Value;

            int? paymentStatusId = null;
            if (ps.HasValue)
                paymentStatusId = (int)ps.Value;

            var context = ObjectContextHelper.CurrentObjectContext;
            var report = context.Sp_OrderProductVariantReport(startTime,
                endTime, orderStatusId, paymentStatusId, billingCountryId);
            return report;
        }

        /// <summary>
        /// Get the bests sellers report
        /// </summary>
        /// <param name="lastDays">Last number of days</param>
        /// <param name="recordsToReturn">Number of products to return</param>
        /// <param name="orderBy">1 - order by total count, 2 - Order by total amount</param>
        /// <returns>Result</returns>
        public static List<BestSellersReportLine> BestSellersReport(int lastDays, 
            int recordsToReturn, int orderBy)
        {
            var context = ObjectContextHelper.CurrentObjectContext;
            var report = context.Sp_SalesBestSellersReport(lastDays,
                recordsToReturn, orderBy);
            return report;
        }

        /// <summary>
        /// Get order average report
        /// </summary>
        /// <param name="os">Order status;</param>
        /// <param name="startTime">Start date</param>
        /// <param name="endTime">End date</param>
        /// <returns>Result</returns>
        public static OrderAverageReportLine GetOrderAverageReportLine(OrderStatusEnum os, 
            DateTime? startTime, DateTime? endTime)
        {
            int orderStatusId = (int)os;

            var context = ObjectContextHelper.CurrentObjectContext;
            var item = context.Sp_OrderAverageReport(startTime, endTime, orderStatusId);
            return item;
        }
        
        /// <summary>
        /// Get order average report
        /// </summary>
        /// <param name="os">Order status</param>
        /// <returns>Result</returns>
        public static OrderAverageReportLineSummary OrderAverageReport(OrderStatusEnum os)
        {
            int orderStatusId = (int)os;

            DateTime nowDT = DateTimeHelper.ConvertToUserTime(DateTime.Now);

            //today
            DateTime? startTime = DateTimeHelper.ConvertToUtcTime(new DateTime(nowDT.Year, nowDT.Month, nowDT.Day), DateTimeHelper.CurrentTimeZone);
            DateTime? endTime = null;
            var todayResult = GetOrderAverageReportLine(os, startTime, endTime);
            //week
            DayOfWeek fdow = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
            DateTime today = new DateTime(nowDT.Year, nowDT.Month, nowDT.Day);
            startTime = DateTimeHelper.ConvertToUtcTime(today.AddDays(-(today.DayOfWeek - fdow)), DateTimeHelper.CurrentTimeZone);
            endTime = null;
            var weekResult = GetOrderAverageReportLine(os, startTime, endTime);
            //month
            startTime = DateTimeHelper.ConvertToUtcTime(new DateTime(nowDT.Year, nowDT.Month, 1), DateTimeHelper.CurrentTimeZone);
            endTime = null;
            var monthResult = GetOrderAverageReportLine(os, startTime, endTime);
            //year
            startTime = DateTimeHelper.ConvertToUtcTime(new DateTime(nowDT.Year, 1, 1), DateTimeHelper.CurrentTimeZone);
            endTime = null;
            var yearResult = GetOrderAverageReportLine(os, startTime, endTime);
            //all time
            startTime = null;
            endTime = null;
            var allTimeResult = GetOrderAverageReportLine(os, startTime, endTime);

            var item = new OrderAverageReportLineSummary();
            item.SumTodayOrders = todayResult.SumOrders;
            item.CountTodayOrders = todayResult.CountOrders;
            item.SumThisWeekOrders = weekResult.SumOrders;
            item.CountThisWeekOrders = weekResult.CountOrders;
            item.SumThisMonthOrders = monthResult.SumOrders;
            item.CountThisMonthOrders = monthResult.CountOrders;
            item.SumThisYearOrders = yearResult.SumOrders;
            item.CountThisYearOrders = yearResult.CountOrders;
            item.SumAllTimeOrders = allTimeResult.SumOrders;
            item.CountAllTimeOrders = allTimeResult.CountOrders;
            item.OrderStatus = os;

            return item;
        }

        /// <summary>
        /// Gets an order report
        /// </summary>
        /// <param name="os">Order status; null to load all orders</param>
        /// <param name="ps">Order payment status; null to load all orders</param>
        /// <param name="ss">Order shippment status; null to load all orders</param>
        /// <returns>IdataReader</returns>
        public static OrderIncompleteReportLine GetOrderReport(OrderStatusEnum? os, 
            PaymentStatusEnum? ps, ShippingStatusEnum? ss)
        {
            int? orderStatusId = null;
            if (os.HasValue)
                orderStatusId = (int)os.Value;

            int? paymentStatusId = null;
            if (ps.HasValue)
                paymentStatusId = (int)ps.Value;

            int? shippmentStatusId = null;
            if (ss.HasValue)
                shippmentStatusId = (int)ss.Value;

            var context = ObjectContextHelper.CurrentObjectContext;
            var item = context.Sp_OrderIncompleteReport(orderStatusId, paymentStatusId, shippmentStatusId);
            return item;
        }
       
        #endregion

        #region Recurring payments

        /// <summary>
        /// Gets a recurring payment
        /// </summary>
        /// <param name="recurringPaymentId">The recurring payment identifier</param>
        /// <returns>Recurring payment</returns>
        public static RecurringPayment GetRecurringPaymentById(int recurringPaymentId)
        {
            if (recurringPaymentId == 0)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from rp in context.RecurringPayments
                        where rp.RecurringPaymentId == recurringPaymentId
                        select rp;
            var recurringPayment = query.SingleOrDefault();
            return recurringPayment;
        }

        /// <summary>
        /// Deletes a recurring payment
        /// </summary>
        /// <param name="recurringPaymentId">Recurring payment identifier</param>
        public static void DeleteRecurringPayment(int recurringPaymentId)
        {
            var recurringPayment = GetRecurringPaymentById(recurringPaymentId);
            if (recurringPayment != null)
            {
                UpdateRecurringPayment(recurringPayment.RecurringPaymentId, recurringPayment.InitialOrderId,
                    recurringPayment.CycleLength, recurringPayment.CyclePeriod,
                    recurringPayment.TotalCycles, recurringPayment.StartDate,
                    recurringPayment.IsActive, true, recurringPayment.CreatedOn);
            }
        }

        /// <summary>
        /// Inserts a recurring payment
        /// </summary>
        /// <param name="initialOrderId">The initial order identifier</param>
        /// <param name="cycleLength">The cycle length</param>
        /// <param name="cyclePeriod">The cycle period</param>
        /// <param name="totalCycles">The total cycles</param>
        /// <param name="startDate">The start date</param>
        /// <param name="isActive">The value indicating whether the payment is active</param>
        /// <param name="deleted">The value indicating whether the entity has been deleted</param>
        /// <param name="createdOn">The date and time of payment creation</param>
        /// <returns>Recurring payment</returns>
        public static RecurringPayment InsertRecurringPayment(int initialOrderId,
            int cycleLength, int cyclePeriod, int totalCycles,
            DateTime startDate, bool isActive, bool deleted, DateTime createdOn)
        {
            var context = ObjectContextHelper.CurrentObjectContext;

            var recurringPayment = context.RecurringPayments.CreateObject();
            recurringPayment.InitialOrderId = initialOrderId;
            recurringPayment.CycleLength = cycleLength;
            recurringPayment.CyclePeriod = cyclePeriod;
            recurringPayment.TotalCycles = totalCycles;
            recurringPayment.StartDate = startDate;
            recurringPayment.IsActive = isActive;
            recurringPayment.Deleted = deleted;
            recurringPayment.CreatedOn = createdOn;

            context.RecurringPayments.AddObject(recurringPayment);
            context.SaveChanges();

            return recurringPayment;
        }

        /// <summary>
        /// Updates the recurring payment
        /// </summary>
        /// <param name="recurringPaymentId">The recurring payment identifier</param>
        /// <param name="initialOrderId">The initial order identifier</param>
        /// <param name="cycleLength">The cycle length</param>
        /// <param name="cyclePeriod">The cycle period</param>
        /// <param name="totalCycles">The total cycles</param>
        /// <param name="startDate">The start date</param>
        /// <param name="isActive">The value indicating whether the payment is active</param>
        /// <param name="deleted">The value indicating whether the entity has been deleted</param>
        /// <param name="createdOn">The date and time of payment creation</param>
        /// <returns>Recurring payment</returns>
        public static RecurringPayment UpdateRecurringPayment(int recurringPaymentId,
            int initialOrderId, int cycleLength, int cyclePeriod, int totalCycles,
            DateTime startDate, bool isActive, bool deleted, DateTime createdOn)
        {
            var recurringPayment = GetRecurringPaymentById(recurringPaymentId);
            if (recurringPayment == null)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(recurringPayment))
                context.RecurringPayments.Attach(recurringPayment);

            recurringPayment.InitialOrderId = initialOrderId;
            recurringPayment.CycleLength = cycleLength;
            recurringPayment.CyclePeriod = cyclePeriod;
            recurringPayment.TotalCycles = totalCycles;
            recurringPayment.StartDate = startDate;
            recurringPayment.IsActive = isActive;
            recurringPayment.Deleted = deleted;
            recurringPayment.CreatedOn = createdOn;
            context.SaveChanges();

            return recurringPayment;
        }

        /// <summary>
        /// Search recurring payments
        /// </summary>
        /// <param name="customerId">The customer identifier; 0 to load all records</param>
        /// <param name="initialOrderId">The initial order identifier; 0 to load all records</param>
        /// <param name="initialOrderStatus">Initial order status identifier; null to load all records</param>
        /// <returns>Recurring payment collection</returns>
        public static List<RecurringPayment> SearchRecurringPayments(int customerId, 
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
        public static List<RecurringPayment> SearchRecurringPayments(bool showHidden,
            int customerId, int initialOrderId, OrderStatusEnum? initialOrderStatus)
        {
            int? initialOrderStatusId = null;
            if (initialOrderStatus.HasValue)
                initialOrderStatusId = (int)initialOrderStatus.Value;
            
            var context = ObjectContextHelper.CurrentObjectContext;

            var recurringPayments = context.Sp_RecurringPaymentLoadAll(showHidden,
                customerId, initialOrderId, initialOrderStatusId);
            return recurringPayments;
        }

        /// <summary>
        /// Deletes a recurring payment history
        /// </summary>
        /// <param name="recurringPaymentHistoryId">Recurring payment history identifier</param>
        public static void DeleteRecurringPaymentHistory(int recurringPaymentHistoryId)
        {
            var recurringPaymentHistory = GetRecurringPaymentHistoryById(recurringPaymentHistoryId);
            if (recurringPaymentHistory == null)
                return;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(recurringPaymentHistory))
                context.RecurringPaymentHistory.Attach(recurringPaymentHistory);
            context.DeleteObject(recurringPaymentHistory);
            context.SaveChanges();
        }

        /// <summary>
        /// Gets a recurring payment history
        /// </summary>
        /// <param name="recurringPaymentHistoryId">The recurring payment history identifier</param>
        /// <returns>Recurring payment history</returns>
        public static RecurringPaymentHistory GetRecurringPaymentHistoryById(int recurringPaymentHistoryId)
        {
            if (recurringPaymentHistoryId == 0)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from rph in context.RecurringPaymentHistory
                        where rph.RecurringPaymentHistoryId == recurringPaymentHistoryId
                        select rph;
            var recurringPaymentHistory = query.SingleOrDefault();
            return recurringPaymentHistory;
        }

        /// <summary>
        /// Inserts a recurring payment history
        /// </summary>
        /// <param name="recurringPaymentId">The recurring payment identifier</param>
        /// <param name="orderId">The order identifier</param>
        /// <param name="createdOn">The date and time of payment creation</param>
        /// <returns>Recurring payment history</returns>
        public static RecurringPaymentHistory InsertRecurringPaymentHistory(int recurringPaymentId,
            int orderId, DateTime createdOn)
        {
            var context = ObjectContextHelper.CurrentObjectContext;

            var recurringPaymentHistory = context.RecurringPaymentHistory.CreateObject();
            recurringPaymentHistory.RecurringPaymentId = recurringPaymentId;
            recurringPaymentHistory.OrderId = orderId;
            recurringPaymentHistory.CreatedOn = createdOn;

            context.RecurringPaymentHistory.AddObject(recurringPaymentHistory);
            context.SaveChanges();
            return recurringPaymentHistory;
        }

        /// <summary>
        /// Updates the recurring payment history
        /// </summary>
        /// <param name="recurringPaymentHistoryId">The recurring payment history identifier</param>
        /// <param name="recurringPaymentId">The recurring payment identifier</param>
        /// <param name="orderId">The order identifier</param>
        /// <param name="createdOn">The date and time of payment creation</param>
        /// <returns>Recurring payment history</returns>
        public static RecurringPaymentHistory UpdateRecurringPaymentHistory(int recurringPaymentHistoryId,
            int recurringPaymentId, int orderId, DateTime createdOn)
        {
            var recurringPaymentHistory = GetRecurringPaymentHistoryById(recurringPaymentHistoryId);
            if (recurringPaymentHistory == null)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(recurringPaymentHistory))
                context.RecurringPaymentHistory.Attach(recurringPaymentHistory);

            recurringPaymentHistory.RecurringPaymentId = recurringPaymentId;
            recurringPaymentHistory.OrderId = orderId;
            recurringPaymentHistory.CreatedOn = createdOn;
            context.SaveChanges();

            return recurringPaymentHistory;
        }

        /// <summary>
        /// Search recurring payment history
        /// </summary>
        /// <param name="recurringPaymentId">The recurring payment identifier; 0 to load all records</param>
        /// <param name="orderId">The order identifier; 0 to load all records</param>
        /// <returns>Recurring payment history collection</returns>
        public static List<RecurringPaymentHistory> SearchRecurringPaymentHistory(int recurringPaymentId, 
            int orderId)
        {
            var context = ObjectContextHelper.CurrentObjectContext;
            var recurringPaymentHistory = context.Sp_RecurringPaymentHistoryLoadAll(recurringPaymentId, orderId);
            return recurringPaymentHistory;
        }

        #endregion

        #region Gift Cards

        /// <summary>
        /// Deletes a gift card
        /// </summary>
        /// <param name="giftCardId">Gift card identifier</param>
        public static void DeleteGiftCard(int giftCardId)
        {
            var giftCard = GetGiftCardById(giftCardId);
            if (giftCard == null)
                return;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(giftCard))
                context.GiftCards.Attach(giftCard);
            context.DeleteObject(giftCard);
            context.SaveChanges();
        }

        /// <summary>
        /// Gets a gift card
        /// </summary>
        /// <param name="giftCardId">Gift card identifier</param>
        /// <returns>Gift card entry</returns>
        public static GiftCard GetGiftCardById(int giftCardId)
        {
            if (giftCardId == 0)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from gc in context.GiftCards
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
        public static List<GiftCard> GetAllGiftCards(int? orderId,
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

            var context = ObjectContextHelper.CurrentObjectContext;
            var giftCards = context.Sp_GiftCardLoadAll(orderId,
                customerId, startTime, endTime, orderStatusId, paymentStatusId, shippingStatusId,
                isGiftCardActivated, giftCardCouponCode);
            return giftCards;
        }

        /// <summary>
        /// Inserts a gift card
        /// </summary>
        /// <param name="purchasedOrderProductVariantId">Purchased order product variant identifier</param>
        /// <param name="amount">Amount</param>
        /// <param name="isGiftCardActivated">Value indicating whether gift card is activated</param>
        /// <param name="giftCardCouponCode">Gift card coupon code</param>
        /// <param name="recipientName">Recipient name</param>
        /// <param name="recipientEmail">Recipient email</param>
        /// <param name="senderName">Sender name</param>
        /// <param name="senderEmail">Sender email</param>
        /// <param name="message">Message</param>
        /// <param name="isRecipientNotified">Value indicating whether recipient is notified</param>
        /// <param name="createdOn">A date and time of instance creation</param>
        /// <returns>Gift card</returns>
        public static GiftCard InsertGiftCard(int purchasedOrderProductVariantId,
            decimal amount, bool isGiftCardActivated, string giftCardCouponCode,
            string recipientName, string recipientEmail,
            string senderName, string senderEmail, string message,
            bool isRecipientNotified, DateTime createdOn)
        {
            giftCardCouponCode = CommonHelper.EnsureMaximumLength(giftCardCouponCode, 100);
            recipientName = CommonHelper.EnsureMaximumLength(recipientName, 100);
            recipientEmail = CommonHelper.EnsureMaximumLength(recipientEmail, 100);
            senderName = CommonHelper.EnsureMaximumLength(senderName, 100);
            senderEmail = CommonHelper.EnsureMaximumLength(senderEmail, 100);
            message = CommonHelper.EnsureMaximumLength(message, 4000);

            var context = ObjectContextHelper.CurrentObjectContext;

            var giftCard = context.GiftCards.CreateObject();
            giftCard.PurchasedOrderProductVariantId = purchasedOrderProductVariantId;
            giftCard.Amount = amount;
            giftCard.IsGiftCardActivated = isGiftCardActivated;
            giftCard.GiftCardCouponCode = giftCardCouponCode;
            giftCard.RecipientName = recipientName;
            giftCard.RecipientEmail = recipientEmail;
            giftCard.SenderName = senderName;
            giftCard.SenderEmail = senderEmail;
            giftCard.Message = message;
            giftCard.IsRecipientNotified = isRecipientNotified;
            giftCard.CreatedOn = createdOn;

            context.GiftCards.AddObject(giftCard);
            context.SaveChanges();

            return giftCard;
        }

        /// <summary>
        /// Updates the gift card
        /// </summary>
        /// <param name="giftCardId">Gift card identifier</param>
        /// <param name="purchasedOrderProductVariantId">Purchased order product variant identifier</param>
        /// <param name="amount">Amount</param>
        /// <param name="isGiftCardActivated">Value indicating whether gift card is activated</param>
        /// <param name="giftCardCouponCode">Gift card coupon code</param>
        /// <param name="recipientName">Recipient name</param>
        /// <param name="recipientEmail">Recipient email</param>
        /// <param name="senderName">Sender name</param>
        /// <param name="senderEmail">Sender email</param>
        /// <param name="message">Message</param>
        /// <param name="isRecipientNotified">Value indicating whether recipient is notified</param>
        /// <param name="createdOn">A date and time of instance creation</param>
        /// <returns>Gift card</returns>
        public static GiftCard UpdateGiftCard(int giftCardId,
            int purchasedOrderProductVariantId,
            decimal amount, bool isGiftCardActivated, string giftCardCouponCode,
            string recipientName, string recipientEmail,
            string senderName, string senderEmail, string message,
            bool isRecipientNotified, DateTime createdOn)
        {
            giftCardCouponCode = CommonHelper.EnsureMaximumLength(giftCardCouponCode, 100);
            recipientName = CommonHelper.EnsureMaximumLength(recipientName, 100);
            recipientEmail = CommonHelper.EnsureMaximumLength(recipientEmail, 100);
            senderName = CommonHelper.EnsureMaximumLength(senderName, 100);
            senderEmail = CommonHelper.EnsureMaximumLength(senderEmail, 100);
            message = CommonHelper.EnsureMaximumLength(message, 4000);

            var giftCard = GetGiftCardById(giftCardId);
            if (giftCard == null)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(giftCard))
                context.GiftCards.Attach(giftCard);

            giftCard.PurchasedOrderProductVariantId = purchasedOrderProductVariantId;
            giftCard.Amount = amount;
            giftCard.IsGiftCardActivated = isGiftCardActivated;
            giftCard.GiftCardCouponCode = giftCardCouponCode;
            giftCard.RecipientName = recipientName;
            giftCard.RecipientEmail = recipientEmail;
            giftCard.SenderName = senderName;
            giftCard.SenderEmail = senderEmail;
            giftCard.Message = message;
            giftCard.IsRecipientNotified = isRecipientNotified;
            giftCard.CreatedOn = createdOn;
            context.SaveChanges();
            return giftCard;
        }

        /// <summary>
        /// Deletes a gift card usage history entry
        /// </summary>
        /// <param name="giftCardUsageHistoryId">Gift card usage history entry identifier</param>
        public static void DeleteGiftCardUsageHistory(int giftCardUsageHistoryId)
        {
            var giftCardUsageHistory = GetGiftCardUsageHistoryById(giftCardUsageHistoryId);
            if (giftCardUsageHistory == null)
                return;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(giftCardUsageHistory))
                context.GiftCardUsageHistory.Attach(giftCardUsageHistory);
            context.DeleteObject(giftCardUsageHistory);
            context.SaveChanges();
        }

        /// <summary>
        /// Gets a gift card usage history entry
        /// </summary>
        /// <param name="giftCardUsageHistoryId">Gift card usage history entry identifier</param>
        /// <returns>Gift card usage history entry</returns>
        public static GiftCardUsageHistory GetGiftCardUsageHistoryById(int giftCardUsageHistoryId)
        {
            if (giftCardUsageHistoryId == 0)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from gcuh in context.GiftCardUsageHistory
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
        public static List<GiftCardUsageHistory> GetAllGiftCardUsageHistoryEntries(int? giftCardId,
            int? customerId, int? orderId)
        {
            var context = ObjectContextHelper.CurrentObjectContext;
            var giftCardUsageHistoryEntries = context.Sp_GiftCardUsageHistoryLoadAll(giftCardId,
                customerId, orderId);
            return giftCardUsageHistoryEntries;
        }

        /// <summary>
        /// Inserts a gift card usage history entry
        /// </summary>
        /// <param name="giftCardId">Gift card identifier</param>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="orderId">Order identifier</param>
        /// <param name="usedValue">Used value</param>
        /// <param name="usedValueInCustomerCurrency">Used value (customer currency)</param>
        /// <param name="createdOn">A date and time of instance creation</param>
        /// <returns>Gift card usage history entry</returns>
        public static GiftCardUsageHistory InsertGiftCardUsageHistory(int giftCardId,
            int customerId, int orderId, decimal usedValue, 
            decimal usedValueInCustomerCurrency, DateTime createdOn)
        {
            var context = ObjectContextHelper.CurrentObjectContext;

            var giftCardUsageHistory = context.GiftCardUsageHistory.CreateObject();
            giftCardUsageHistory.GiftCardId = giftCardId;
            giftCardUsageHistory.CustomerId = customerId;
            giftCardUsageHistory.OrderId = orderId;
            giftCardUsageHistory.UsedValue = usedValue;
            giftCardUsageHistory.UsedValueInCustomerCurrency = usedValueInCustomerCurrency;
            giftCardUsageHistory.CreatedOn = createdOn;

            context.GiftCardUsageHistory.AddObject(giftCardUsageHistory);
            context.SaveChanges();

            return giftCardUsageHistory;
        }

        /// <summary>
        /// Updates the gift card usage history entry
        /// </summary>
        /// <param name="giftCardUsageHistoryId">Gift card usage history entry identifier</param>
        /// <param name="giftCardId">Gift card identifier</param>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="orderId">Order identifier</param>
        /// <param name="usedValue">Used value</param>
        /// <param name="usedValueInCustomerCurrency">Used value (customer currency)</param>
        /// <param name="createdOn">A date and time of instance creation</param>
        /// <returns>Gift card usage history entry</returns>
        public static GiftCardUsageHistory UpdateGiftCardUsageHistory(int giftCardUsageHistoryId,
            int giftCardId, int customerId, int orderId, decimal usedValue,
            decimal usedValueInCustomerCurrency, DateTime createdOn)
        {
            var giftCardUsageHistory = GetGiftCardUsageHistoryById(giftCardUsageHistoryId);
            if (giftCardUsageHistory == null)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(giftCardUsageHistory))
                context.GiftCardUsageHistory.Attach(giftCardUsageHistory);

            giftCardUsageHistory.GiftCardId = giftCardId;
            giftCardUsageHistory.CustomerId = customerId;
            giftCardUsageHistory.OrderId = orderId;
            giftCardUsageHistory.UsedValue = usedValue;
            giftCardUsageHistory.UsedValueInCustomerCurrency = usedValueInCustomerCurrency;
            giftCardUsageHistory.CreatedOn = createdOn;
            context.SaveChanges();
            return giftCardUsageHistory;
        }

        #endregion

        #region Reward points

        /// <summary>
        /// Deletes a reward point history entry
        /// </summary>
        /// <param name="rewardPointsHistoryId">Reward point history entry identifier</param>
        public static void DeleteRewardPointsHistory(int rewardPointsHistoryId)
        {
            var rewardPointsHistory = GetRewardPointsHistoryById(rewardPointsHistoryId);
            if (rewardPointsHistory == null)
                return;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(rewardPointsHistory))
                context.RewardPointsHistory.Attach(rewardPointsHistory);
            context.DeleteObject(rewardPointsHistory);
            context.SaveChanges();
        }

        /// <summary>
        /// Gets a reward point history entry
        /// </summary>
        /// <param name="rewardPointsHistoryId">Reward point history entry identifier</param>
        /// <returns>Reward point history entry</returns>
        public static RewardPointsHistory GetRewardPointsHistoryById(int rewardPointsHistoryId)
        {
            if (rewardPointsHistoryId == 0)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from rph in context.RewardPointsHistory
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
        /// <param name="pageSize">Page size</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="totalRecords">Total records</param>
        /// <returns>Reward point history entries</returns>
        public static List<RewardPointsHistory> GetAllRewardPointsHistoryEntries(int? customerId,
            int? orderId, int pageSize, int pageIndex, out int totalRecords)
        {
            if (pageSize <= 0)
                pageSize = 10;
            if (pageSize == int.MaxValue)
                pageSize = int.MaxValue - 1;

            if (pageIndex < 0)
                pageIndex = 0;
            if (pageIndex == int.MaxValue)
                pageIndex = int.MaxValue - 1;
            
            var context = ObjectContextHelper.CurrentObjectContext;

            var rewardPointsHistoryEntries = context.Sp_RewardPointsHistoryLoadAll(customerId,
                orderId, pageSize, pageIndex, out totalRecords);
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
        public static RewardPointsHistory InsertRewardPointsHistory(int customerId,
            int orderId, int points, decimal usedAmount,
            decimal usedAmountInCustomerCurrency, string customerCurrencyCode,
            string message, DateTime createdOn)
        {
            customerCurrencyCode = CommonHelper.EnsureMaximumLength(customerCurrencyCode, 5);
            message = CommonHelper.EnsureMaximumLength(message, 1000);

            Customer customer = CustomerManager.GetCustomerById(customerId);
            if (customer == null)
                throw new NopException("Customer not found. ID=" + customerId);

            int newPointsBalance = customer.RewardPointsBalance + points;

            var context = ObjectContextHelper.CurrentObjectContext;

            var rewardPointsHistory = context.RewardPointsHistory.CreateObject();
            rewardPointsHistory.CustomerId = customerId;
            rewardPointsHistory.OrderId = orderId;
            rewardPointsHistory.Points = points;
            rewardPointsHistory.PointsBalance = newPointsBalance;
            rewardPointsHistory.UsedAmount = usedAmount;
            rewardPointsHistory.UsedAmountInCustomerCurrency = usedAmountInCustomerCurrency;
            rewardPointsHistory.CustomerCurrencyCode = customerCurrencyCode;
            rewardPointsHistory.Message = message;
            rewardPointsHistory.CreatedOn = createdOn;

            context.RewardPointsHistory.AddObject(rewardPointsHistory);
            context.SaveChanges();

            customer.ResetCachedValues();

            return rewardPointsHistory;
        }

        /// <summary>
        /// Updates a reward point history entry
        /// </summary>
        /// <param name="rewardPointsHistoryId">Reward point history entry identifier</param>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="orderId">Order identifier</param>
        /// <param name="points">Points redeemed/added</param>
        /// <param name="pointsBalance">Points balance</param>
        /// <param name="usedAmount">Used amount</param>
        /// <param name="usedAmountInCustomerCurrency">Used amount (customer currency)</param>
        /// <param name="customerCurrencyCode">Customer currency code</param>
        /// <param name="message">Customer currency code</param>
        /// <param name="createdOn">A date and time of instance creation</param>
        /// <returns>Reward point history entry</returns>
        public static RewardPointsHistory UpdateRewardPointsHistory(int rewardPointsHistoryId,
            int customerId, int orderId, int points, int pointsBalance, decimal usedAmount,
            decimal usedAmountInCustomerCurrency, string customerCurrencyCode,
            string message, DateTime createdOn)
        {
            customerCurrencyCode = CommonHelper.EnsureMaximumLength(customerCurrencyCode, 5);
            message = CommonHelper.EnsureMaximumLength(message, 1000);

            var rewardPointsHistory = GetRewardPointsHistoryById(rewardPointsHistoryId);
            if (rewardPointsHistory == null)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(rewardPointsHistory))
                context.RewardPointsHistory.Attach(rewardPointsHistory);

            rewardPointsHistory.CustomerId = customerId;
            rewardPointsHistory.OrderId = orderId;
            rewardPointsHistory.Points = points;
            rewardPointsHistory.PointsBalance = pointsBalance;
            rewardPointsHistory.UsedAmount = usedAmount;
            rewardPointsHistory.UsedAmountInCustomerCurrency = usedAmountInCustomerCurrency;
            rewardPointsHistory.CustomerCurrencyCode = customerCurrencyCode;
            rewardPointsHistory.Message = message;
            rewardPointsHistory.CreatedOn = createdOn;
            context.SaveChanges();
            return rewardPointsHistory;
        }

        #endregion

        #region Return requests (RMA)

        /// <summary>
        /// Gets a return request status name
        /// </summary>
        /// <param name="rs">Return status</param>
        /// <returns>Return request status name</returns>
        public static string GetReturnRequestStatusName(ReturnStatusEnum rs)
        {
            string name = string.Empty;
            switch (rs)
            {
                case ReturnStatusEnum.Pending:
                    name = LocalizationManager.GetLocaleResourceString("ReturnStatus.Pending");
                    break;
                case ReturnStatusEnum.Received:
                    name = LocalizationManager.GetLocaleResourceString("ReturnStatus.Received");
                    break;
                case ReturnStatusEnum.ReturnAuthorized:
                    name = LocalizationManager.GetLocaleResourceString("ReturnStatus.ReturnAuthorized");
                    break;
                case ReturnStatusEnum.ItemsRepaired:
                    name = LocalizationManager.GetLocaleResourceString("ReturnStatus.ItemsRepaired");
                    break;
                case ReturnStatusEnum.ItemsRefunded:
                    name = LocalizationManager.GetLocaleResourceString("ReturnStatus.ItemsRefunded");
                    break;
                case ReturnStatusEnum.RequestRejected:
                    name = LocalizationManager.GetLocaleResourceString("ReturnStatus.RequestRejected");
                    break;
                case ReturnStatusEnum.Cancelled:
                    name = LocalizationManager.GetLocaleResourceString("ReturnStatus.Cancelled");
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
        public static bool IsReturnRequestAllowed(Order order)
        {
            if (!SettingManager.GetSettingValueBoolean("ReturnRequests.Enable"))
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
        public static ReturnRequest GetReturnRequestById(int returnRequestId)
        {
            if (returnRequestId == 0)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from rr in context.ReturnRequests
                        where rr.ReturnRequestId == returnRequestId
                        select rr;
            var returnRequest = query.SingleOrDefault();
            return returnRequest;
        }
        
        /// <summary>
        /// Deletes a return request
        /// </summary>
        /// <param name="returnRequestId">Return request identifier</param>
        public static void DeleteReturnRequest(int returnRequestId)
        {
            var returnRequest = GetReturnRequestById(returnRequestId);
            if (returnRequest == null)
                return;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(returnRequest))
                context.ReturnRequests.Attach(returnRequest);
            context.DeleteObject(returnRequest);
            context.SaveChanges();
        }

        /// <summary>
        /// Search return requests
        /// </summary>
        /// <param name="customerId">Customer identifier; null to load all entries</param>
        /// <param name="orderProductVariantId">Order product variant identifier; null to load all entries</param>
        /// <returns>Return requests</returns>
        public static List<ReturnRequest> SearchReturnRequests(int customerId, 
            int orderProductVariantId, ReturnStatusEnum? rs)
        {
            int? returnStatusId = null;
            if (rs.HasValue)
                returnStatusId = (int)rs.Value;

            var context = ObjectContextHelper.CurrentObjectContext;

            var query = from rr in context.ReturnRequests
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
        /// <param name="orderProductVariantId">Order product variant identifier</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="reasonForReturn">Reason to return</param>
        /// <param name="requestedAction">Requested action</param>
        /// <param name="customerComments">Customer comments</param>
        /// <param name="staffNotes">Staff notes</param>
        /// <param name="returnStatus">Return status</param>
        /// <param name="createdOn">The date and time of entity creation</param>
        /// <param name="updatedOn">The date and time of entity update</param>
        /// <returns>Return request</returns>
        public static ReturnRequest InsertReturnRequest(int orderProductVariantId,
            int quantity, int customerId, string reasonForReturn, 
            string requestedAction, string customerComments, 
            string staffNotes, ReturnStatusEnum returnStatus,
            DateTime createdOn, DateTime updatedOn)
        {
            reasonForReturn = CommonHelper.EnsureMaximumLength(reasonForReturn, 400);
            requestedAction = CommonHelper.EnsureMaximumLength(requestedAction, 400);

            var context = ObjectContextHelper.CurrentObjectContext;

            var returnRequest = context.ReturnRequests.CreateObject();
            returnRequest.OrderProductVariantId = orderProductVariantId;
            returnRequest.Quantity = quantity;
            returnRequest.CustomerId = customerId;
            returnRequest.ReasonForReturn = reasonForReturn;
            returnRequest.RequestedAction = requestedAction;
            returnRequest.CustomerComments = customerComments;
            returnRequest.StaffNotes = staffNotes;
            returnRequest.ReturnStatusId = (int)returnStatus;
            returnRequest.CreatedOn = createdOn;
            returnRequest.UpdatedOn = updatedOn;

            context.ReturnRequests.AddObject(returnRequest);
            context.SaveChanges();
            return returnRequest;
        }


        /// <summary>
        /// Updates the return request
        /// </summary>
        /// <param name="returnRequestId">Return request identifier</param>
        /// <param name="orderProductVariantId">Order product variant identifier</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="reasonForReturn">Reason to return</param>
        /// <param name="requestedAction">Requested action</param>
        /// <param name="customerComments">Customer comments</param>
        /// <param name="staffNotes">Staff notes</param>
        /// <param name="returnStatus">Return status</param>
        /// <param name="createdOn">The date and time of entity creation</param>
        /// <param name="updatedOn">The date and time of entity update</param>
        /// <returns>Return request</returns>
        public static ReturnRequest UpdateReturnRequest(int returnRequestId, int orderProductVariantId,
            int quantity, int customerId, string reasonForReturn,
            string requestedAction, string customerComments,
            string staffNotes,  ReturnStatusEnum returnStatus,
            DateTime createdOn, DateTime updatedOn)
        {
            reasonForReturn = CommonHelper.EnsureMaximumLength(reasonForReturn, 400);
            requestedAction = CommonHelper.EnsureMaximumLength(requestedAction, 400);

            var returnRequest = GetReturnRequestById(returnRequestId);
            if (returnRequest == null)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(returnRequest))
                context.ReturnRequests.Attach(returnRequest);

            returnRequest.OrderProductVariantId = orderProductVariantId;
            returnRequest.Quantity = quantity;
            returnRequest.CustomerId = customerId;
            returnRequest.ReasonForReturn = reasonForReturn;
            returnRequest.RequestedAction = requestedAction;
            returnRequest.CustomerComments = customerComments;
            returnRequest.StaffNotes = staffNotes;
            returnRequest.ReturnStatusId = (int)returnStatus;
            returnRequest.CreatedOn = createdOn;
            returnRequest.UpdatedOn = updatedOn;
            context.SaveChanges();
            return returnRequest;
        }

        /// <summary>
        /// Formats the comments text of a return request
        /// </summary>
        /// <param name="text">Text</param>
        /// <returns>Formatted text</returns>
        public static string FormatReturnRequestCommentsText(string text)
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
        public static bool IsDownloadAllowed(OrderProductVariant orderProductVariant)
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
        public static bool IsLicenseDownloadAllowed(OrderProductVariant orderProductVariant)
        {
            if (orderProductVariant == null)
                return false;

            return IsDownloadAllowed(orderProductVariant) && orderProductVariant.LicenseDownloadId > 0;
        }

        /// <summary>
        /// Formats the order note text
        /// </summary>
        /// <param name="text">Text</param>
        /// <returns>Formatted text</returns>
        public static string FormatOrderNoteText(string text)
        {
            if (String.IsNullOrEmpty(text))
                return string.Empty;

            text = HtmlHelper.FormatText(text, false, true, false, false, false, false);
            return text;
        }

        /// <summary>
        /// Places an order
        /// </summary>
        /// <param name="paymentInfo">Payment info</param>
        /// <param name="customer">Customer</param>
        /// <param name="orderId">Order identifier</param>
        /// <returns>The error status, or String.Empty if no errors</returns>
        public static string PlaceOrder(PaymentInfo paymentInfo, Customer customer, 
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
        public static string PlaceOrder(PaymentInfo paymentInfo, Customer customer, 
            Guid orderGuid, out int orderId)
        {
            orderId = 0;
            var processPaymentResult = new ProcessPaymentResult();
            try
            {
                if (customer == null)
                    throw new ArgumentNullException("customer");

                if (customer.IsGuest && !CustomerManager.AnonymousCheckoutAllowed)
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

                var paymentMethod = PaymentMethodManager.GetPaymentMethodById(paymentInfo.PaymentMethodId);
                if (paymentMethod == null)
                    throw new NopException("Payment method couldn't be loaded");

                if (!paymentMethod.IsActive)
                    throw new NopException("Payment method is not active");

                if (paymentInfo.CreditCardCvv2 == null)
                    paymentInfo.CreditCardCvv2 = string.Empty;

                if (paymentInfo.CreditCardName == null)
                    paymentInfo.CreditCardName = string.Empty;

                if (paymentInfo.CreditCardNumber == null)
                    paymentInfo.CreditCardNumber = string.Empty;

                if (paymentInfo.CreditCardType == null)
                    paymentInfo.CreditCardType = string.Empty;

                if (paymentInfo.PurchaseOrderNumber == null)
                    paymentInfo.PurchaseOrderNumber = string.Empty;

                ShoppingCart cart = null;
                if (!paymentInfo.IsRecurringPayment)
                {
                    cart = ShoppingCartManager.GetCustomerShoppingCart(customer.CustomerId, ShoppingCartTypeEnum.ShoppingCart);

                    //validate cart
                    var warnings = ShoppingCartManager.GetShoppingCartWarnings(cart, customer.CheckoutAttributes, true);
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
                        var sciWarnings = ShoppingCartManager.GetShoppingCartItemWarnings(sci.ShoppingCartType,
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
                    if (TaxManager.AllowCustomersToSelectTaxDisplayType)
                        customerTaxDisplayType = customer.TaxDisplayType;
                    else
                        customerTaxDisplayType = TaxManager.TaxDisplayType;
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
                if (!paymentInfo.IsRecurringPayment)
                {
                    string subTotalError1 = ShoppingCartManager.GetShoppingCartSubTotal(cart, customer,
                        true, out orderSubTotalInclTax);
                    
                    string subTotalError2 = ShoppingCartManager.GetShoppingCartSubTotal(cart, customer,
                        false, out orderSubTotalExclTax);
                    
                    if (!String.IsNullOrEmpty(subTotalError1) || !String.IsNullOrEmpty(subTotalError2))
                        throw new NopException("Sub total couldn't be calculated");
                    
                    //in customer currency
                    orderSubtotalInclTaxInCustomerCurrency = CurrencyManager.ConvertCurrency(orderSubTotalInclTax, CurrencyManager.PrimaryStoreCurrency, paymentInfo.CustomerCurrency);
                    orderSubtotalExclTaxInCustomerCurrency = CurrencyManager.ConvertCurrency(orderSubTotalExclTax, CurrencyManager.PrimaryStoreCurrency, paymentInfo.CustomerCurrency);
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
                    orderWeight = ShippingManager.GetShoppingCartTotalWeight(cart, customer);
                    shoppingCartRequiresShipping = ShippingManager.ShoppingCartRequiresShipping(cart);
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
                    string shippingTotalError1 = string.Empty;
                    string shippingTotalError2 = string.Empty;
                    Discount shippingTotalDiscount = null;
                    orderShippingTotalInclTax = ShippingManager.GetShoppingCartShippingTotal(cart, customer, true, out shippingTotalDiscount, ref shippingTotalError1);
                    orderShippingTotalExclTax = ShippingManager.GetShoppingCartShippingTotal(cart, customer, false, ref shippingTotalError2);
                    if (!orderShippingTotalInclTax.HasValue || !orderShippingTotalExclTax.HasValue)
                        throw new NopException("Shipping total couldn't be calculated");
                    if (shippingTotalDiscount != null && !appliedDiscounts.ContainsDiscount(shippingTotalDiscount.Name))
                        appliedDiscounts.Add(shippingTotalDiscount);

                    //in customer currency
                    orderShippingInclTaxInCustomerCurrency = CurrencyManager.ConvertCurrency(orderShippingTotalInclTax.Value, CurrencyManager.PrimaryStoreCurrency, paymentInfo.CustomerCurrency);
                    orderShippingExclTaxInCustomerCurrency = CurrencyManager.ConvertCurrency(orderShippingTotalExclTax.Value, CurrencyManager.PrimaryStoreCurrency, paymentInfo.CustomerCurrency);

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
                    decimal paymentAdditionalFee = PaymentManager.GetAdditionalHandlingFee(paymentInfo.PaymentMethodId);
                    paymentAdditionalFeeInclTax = TaxManager.GetPaymentMethodAdditionalFee(paymentAdditionalFee, true, customer, ref paymentAdditionalFeeError1);
                    paymentAdditionalFeeExclTax = TaxManager.GetPaymentMethodAdditionalFee(paymentAdditionalFee, false, customer, ref paymentAdditionalFeeError2);
                    if (!String.IsNullOrEmpty(paymentAdditionalFeeError1))
                        throw new NopException("Payment method fee couldn't be calculated");
                    if (!String.IsNullOrEmpty(paymentAdditionalFeeError2))
                        throw new NopException("Payment method fee couldn't be calculated");

                    //in customer currency
                    paymentAdditionalFeeInclTaxInCustomerCurrency = CurrencyManager.ConvertCurrency(paymentAdditionalFeeInclTax, CurrencyManager.PrimaryStoreCurrency, paymentInfo.CustomerCurrency);
                    paymentAdditionalFeeExclTaxInCustomerCurrency = CurrencyManager.ConvertCurrency(paymentAdditionalFeeExclTax, CurrencyManager.PrimaryStoreCurrency, paymentInfo.CustomerCurrency);
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
                if (!paymentInfo.IsRecurringPayment)
                {
                    //tax amount
                    string taxError = string.Empty;
                    orderTaxTotal = TaxManager.GetTaxTotal(cart, 
                        paymentInfo.PaymentMethodId, customer, ref taxError);
                    if (!String.IsNullOrEmpty(taxError))
                        throw new NopException("Tax total couldn't be calculated");

                    //in customer currency
                    orderTaxInCustomerCurrency = CurrencyManager.ConvertCurrency(orderTaxTotal, CurrencyManager.PrimaryStoreCurrency, paymentInfo.CustomerCurrency);

                    //VAT number
                    if (TaxManager.EUVatEnabled && customer.VatNumberStatus == VatNumberStatusEnum.Valid)
                    {
                        vatNumber = customer.VatNumber;
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

                    orderTotal = ShoppingCartManager.GetShoppingCartTotal(cart,
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
                    orderDiscountInCustomerCurrency = CurrencyManager.ConvertCurrency(orderDiscountAmount,
                        CurrencyManager.PrimaryStoreCurrency, paymentInfo.CustomerCurrency);
                    orderTotalInCustomerCurrency = CurrencyManager.ConvertCurrency(orderTotal.Value, 
                        CurrencyManager.PrimaryStoreCurrency, paymentInfo.CustomerCurrency);
                }
                else
                {
                    orderDiscountAmount = initialOrder.OrderDiscount;
                    orderTotal = initialOrder.OrderTotal;

                    orderDiscountInCustomerCurrency = initialOrder.OrderDiscountInCustomerCurrency;
                    orderTotalInCustomerCurrency = initialOrder.OrderTotalInCustomerCurrency;
                }
                paymentInfo.OrderTotal = orderTotal.Value;

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
                        string recurringCyclesError = ShoppingCartManager.GetReccuringCycleInfo(cart, out recurringCycleLength, out recurringCyclePeriod, out recurringTotalCycles);
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
                
                //process payment
                if (!paymentInfo.IsRecurringPayment)
                {
                    if (isRecurringShoppingCart)
                    {
                        //recurring cart
                        var recurringPaymentType = PaymentManager.SupportRecurringPayments(paymentMethod.PaymentMethodId);
                        switch (recurringPaymentType)
                        {
                            case RecurringPaymentTypeEnum.NotSupported:
                                throw new NopException("Recurring payments are not supported by selected payment method");
                                break;
                            case RecurringPaymentTypeEnum.Manual:
                            case RecurringPaymentTypeEnum.Automatic:
                                PaymentManager.ProcessRecurringPayment(paymentInfo, customer, orderGuid, ref processPaymentResult);
                                break;
                            default:
                                throw new NopException("Not supported recurring payment type");
                                break;
                        }
                    }
                    else
                    {
                        //standard cart
                        PaymentManager.ProcessPayment(paymentInfo, customer, orderGuid, ref processPaymentResult);
                    }
                }
                else
                {
                    if (isRecurringShoppingCart)
                    {
                        var recurringPaymentType = PaymentManager.SupportRecurringPayments(paymentMethod.PaymentMethodId);
                        switch (recurringPaymentType)
                        {
                            case RecurringPaymentTypeEnum.NotSupported:
                                throw new NopException("Recurring payments are not supported by selected payment method");
                                break;
                            case RecurringPaymentTypeEnum.Manual:
                                PaymentManager.ProcessRecurringPayment(paymentInfo, customer, orderGuid, ref processPaymentResult);
                                break;
                            case RecurringPaymentTypeEnum.Automatic:
                                //payment is processed on payment gateway site
                                break;
                            default:
                                throw new NopException("Not supported recurring payment type");
                                break;
                        }
                    }
                    else
                    {
                        throw new NopException("No recurring products");
                    }
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

                        var order = InsertOrder(orderGuid,
                             customer.CustomerId,
                             customerLanguageId,
                             customerTaxDisplayType,
                             NopContext.Current.UserHostAddress,
                             orderSubTotalInclTax,
                             orderSubTotalExclTax,
                             orderShippingTotalInclTax.Value,
                             orderShippingTotalExclTax.Value,
                             paymentAdditionalFeeInclTax,
                             paymentAdditionalFeeExclTax,
                             orderTaxTotal,
                             orderTotal.Value,
                             orderDiscountAmount,
                             orderSubtotalInclTaxInCustomerCurrency,
                             orderSubtotalExclTaxInCustomerCurrency,
                             orderShippingInclTaxInCustomerCurrency,
                             orderShippingExclTaxInCustomerCurrency,
                             paymentAdditionalFeeInclTaxInCustomerCurrency,
                             paymentAdditionalFeeExclTaxInCustomerCurrency,
                             orderTaxInCustomerCurrency,
                             orderTotalInCustomerCurrency,
                             orderDiscountInCustomerCurrency,
                             checkoutAttributeDescription,
                             checkoutAttributesXml,
                             customerCurrencyCode,
                             orderWeight,
                             customer.AffiliateId,
                             OrderStatusEnum.Pending,
                             processPaymentResult.AllowStoringCreditCardNumber,
                             processPaymentResult.AllowStoringCreditCardNumber ? SecurityHelper.Encrypt(paymentInfo.CreditCardType) : string.Empty,
                             processPaymentResult.AllowStoringCreditCardNumber ? SecurityHelper.Encrypt(paymentInfo.CreditCardName) : string.Empty,
                             processPaymentResult.AllowStoringCreditCardNumber ? SecurityHelper.Encrypt(paymentInfo.CreditCardNumber) : string.Empty,
                             SecurityHelper.Encrypt(PaymentManager.GetMaskedCreditCardNumber(paymentInfo.CreditCardNumber)),
                             processPaymentResult.AllowStoringCreditCardNumber ? SecurityHelper.Encrypt(paymentInfo.CreditCardCvv2) : string.Empty,
                             processPaymentResult.AllowStoringCreditCardNumber ? SecurityHelper.Encrypt(paymentInfo.CreditCardExpireMonth.ToString()) : string.Empty,
                             processPaymentResult.AllowStoringCreditCardNumber ? SecurityHelper.Encrypt(paymentInfo.CreditCardExpireYear.ToString()) : string.Empty,
                             paymentMethod.PaymentMethodId,
                             paymentMethod.Name,
                             processPaymentResult.AuthorizationTransactionId,
                             processPaymentResult.AuthorizationTransactionCode,
                             processPaymentResult.AuthorizationTransactionResult,
                             processPaymentResult.CaptureTransactionId,
                             processPaymentResult.CaptureTransactionResult,
                             processPaymentResult.SubscriptionTransactionId,
                             paymentInfo.PurchaseOrderNumber,
                             processPaymentResult.PaymentStatus,
                             null,
                             billingFirstName,
                             billingLastName,
                             billingPhoneNumber,
                             billingEmail,
                             billingFaxNumber,
                             billingCompany,
                             billingAddress1,
                             billingAddress2,
                             billingCity,
                             billingStateProvince,
                             billingStateProvinceId,
                             billingZipPostalCode,
                             billingCountry,
                             billingCountryId,
                             shippingStatusEnum,
                             shippingFirstName,
                             shippingLastName,
                             shippingPhoneNumber,
                             shippingEmail,
                             shippingFaxNumber,
                             shippingCompany,
                             shippingAddress1,
                             shippingAddress2,
                             shippingCity,
                             shippingStateProvince,
                             shippingStateProvinceId,
                             shippingZipPostalCode,
                             shippingCountry,
                             shippingCountryId,
                             shippingMethodName,
                             shippingRateComputationMethodId,
                             null,
                             null,
                             string.Empty,
                             vatNumber,
                             false,
                             DateTime.UtcNow);

                        orderId = order.OrderId;

                        if (!paymentInfo.IsRecurringPayment)
                        {
                            //move shopping cart items to order product variants
                            foreach (var sc in cart)
                            {
                                //prices
                                decimal scUnitPrice = PriceHelper.GetUnitPrice(sc, customer, true);
                                decimal scSubTotal = PriceHelper.GetSubTotal(sc, customer, true);
                                decimal scUnitPriceInclTax = TaxManager.GetPrice(sc.ProductVariant, scUnitPrice, true, customer);
                                decimal scUnitPriceExclTax = TaxManager.GetPrice(sc.ProductVariant, scUnitPrice, false, customer);
                                decimal scSubTotalInclTax = TaxManager.GetPrice(sc.ProductVariant, scSubTotal, true, customer);
                                decimal scSubTotalExclTax = TaxManager.GetPrice(sc.ProductVariant, scSubTotal, false, customer);
                                decimal scUnitPriceInclTaxInCustomerCurrency = CurrencyManager.ConvertCurrency(scUnitPriceInclTax, CurrencyManager.PrimaryStoreCurrency, paymentInfo.CustomerCurrency);
                                decimal scUnitPriceExclTaxInCustomerCurrency = CurrencyManager.ConvertCurrency(scUnitPriceExclTax, CurrencyManager.PrimaryStoreCurrency, paymentInfo.CustomerCurrency);
                                decimal scSubTotalInclTaxInCustomerCurrency = CurrencyManager.ConvertCurrency(scSubTotalInclTax, CurrencyManager.PrimaryStoreCurrency, paymentInfo.CustomerCurrency);
                                decimal scSubTotalExclTaxInCustomerCurrency = CurrencyManager.ConvertCurrency(scSubTotalExclTax, CurrencyManager.PrimaryStoreCurrency, paymentInfo.CustomerCurrency);

                                //discounts
                                Discount scDiscount = null;
                                decimal discountAmount = PriceHelper.GetDiscountAmount(sc, customer, out scDiscount);
                                decimal discountAmountInclTax = TaxManager.GetPrice(sc.ProductVariant, discountAmount, true, customer);
                                decimal discountAmountExclTax = TaxManager.GetPrice(sc.ProductVariant, discountAmount, false, customer);
                                if (scDiscount != null && !appliedDiscounts.ContainsDiscount(scDiscount.Name))
                                    appliedDiscounts.Add(scDiscount);

                                //attributes
                                string attributeDescription = ProductAttributeHelper.FormatAttributes(sc.ProductVariant, sc.AttributesXml, customer, "<br />");

                                //save item
                                var opv = InsertOrderProductVariant(Guid.NewGuid(), order.OrderId,
                                    sc.ProductVariantId, scUnitPriceInclTax, scUnitPriceExclTax, scSubTotalInclTax, scSubTotalExclTax,
                                    scUnitPriceInclTaxInCustomerCurrency, scUnitPriceExclTaxInCustomerCurrency,
                                    scSubTotalInclTaxInCustomerCurrency, scSubTotalExclTaxInCustomerCurrency,
                                    attributeDescription, sc.AttributesXml, sc.Quantity, discountAmountInclTax,
                                    discountAmountExclTax, 0, false, 0);

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
                                        var gc = InsertGiftCard(opv.OrderProductVariantId, scUnitPriceExclTax,
                                            false, GiftCardHelper.GenerateGiftCardCode(),
                                            giftCardRecipientName, giftCardRecipientEmail,
                                            giftCardSenderName, giftCardSenderEmail,
                                            giftCardMessage, false, DateTime.UtcNow);
                                    }
                                }

                                //inventory
                                ProductManager.AdjustInventory(sc.ProductVariantId, true, sc.Quantity, sc.AttributesXml);
                            }

                            //clear shopping cart
                            foreach (var sc in cart)
                            {
                                ShoppingCartManager.DeleteShoppingCartItem(sc.ShoppingCartItemId, false);
                            }
                        }
                        else
                        {
                            var initialOrderProductVariants = initialOrder.OrderProductVariants;
                            foreach (var opv in initialOrderProductVariants)
                            {
                                //save item
                               var newOpv = InsertOrderProductVariant(Guid.NewGuid(), order.OrderId,
                                    opv.ProductVariantId, opv.UnitPriceInclTax, opv.UnitPriceExclTax,
                                    opv.PriceInclTax, opv.PriceExclTax,
                                    opv.UnitPriceInclTaxInCustomerCurrency, opv.UnitPriceExclTaxInCustomerCurrency,
                                    opv.PriceInclTaxInCustomerCurrency, opv.PriceExclTaxInCustomerCurrency,
                                    opv.AttributeDescription, opv.AttributesXml, opv.Quantity, opv.DiscountAmountInclTax,
                                    opv.DiscountAmountExclTax, 0, false, 0);

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
                                        GiftCard gc = InsertGiftCard(newOpv.OrderProductVariantId, opv.UnitPriceExclTax,
                                            false, GiftCardHelper.GenerateGiftCardCode(),
                                            giftCardRecipientName, giftCardRecipientEmail,
                                            giftCardSenderName, giftCardSenderEmail,
                                            giftCardMessage, false, DateTime.UtcNow);
                                    }
                                }

                                //inventory
                                ProductManager.AdjustInventory(opv.ProductVariantId, true, opv.Quantity, opv.AttributesXml);
                            }
                        }

                        //discount usage history
                        if (!paymentInfo.IsRecurringPayment)
                        {
                            foreach (var discount in appliedDiscounts)
                            {
                                var duh = DiscountManager.InsertDiscountUsageHistory(discount.DiscountId,
                                    customer.CustomerId, order.OrderId, DateTime.UtcNow);
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
                                    decimal amountUsedInCustomerCurrency = CurrencyManager.ConvertCurrency(amountUsed, CurrencyManager.PrimaryStoreCurrency, paymentInfo.CustomerCurrency);
                                    var gcuh = InsertGiftCardUsageHistory(agc.GiftCardId,
                                        customer.CustomerId, order.OrderId,
                                        amountUsed, amountUsedInCustomerCurrency, DateTime.UtcNow);
                                }
                            }
                        }

                        //reward points history
                        if (redeemedRewardPointsAmount > decimal.Zero)
                        {
                            decimal redeemedRewardPointsAmountInCustomerCurrency = CurrencyManager.ConvertCurrency(redeemedRewardPointsAmount, CurrencyManager.PrimaryStoreCurrency, paymentInfo.CustomerCurrency);
                            string message = string.Format(LocalizationManager.GetLocaleResourceString("RewardPoints.Message.RedeemedForOrder", order.CustomerLanguageId), order.OrderId);

                            RewardPointsHistory rph = OrderManager.InsertRewardPointsHistory(customer.CustomerId,
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
                                var rp = InsertRecurringPayment(order.OrderId,
                                    paymentInfo.RecurringCycleLength, paymentInfo.RecurringCyclePeriod,
                                    paymentInfo.RecurringTotalCycles, DateTime.UtcNow,
                                    true, false, DateTime.UtcNow);


                                var recurringPaymentType = PaymentManager.SupportRecurringPayments(paymentMethod.PaymentMethodId);
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
                                            RecurringPaymentHistory rph = InsertRecurringPaymentHistory(rp.RecurringPaymentId,
                                                order.OrderId, DateTime.UtcNow);
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

                        int orderPlacedStoreOwnerNotificationQueuedEmailId = MessageManager.SendOrderPlacedStoreOwnerNotification(order, LocalizationManager.DefaultAdminLanguage.LanguageId);
                        if (orderPlacedStoreOwnerNotificationQueuedEmailId > 0)
                        {
                            InsertOrderNote(order.OrderId, string.Format("\"Order placed\" email (to store owner) has been queued. Queued email identifier: {0}.", orderPlacedStoreOwnerNotificationQueuedEmailId), false, DateTime.UtcNow);
                        }

                        int orderPlacedCustomerNotificationQueuedEmailId = MessageManager.SendOrderPlacedCustomerNotification(order, order.CustomerLanguageId);
                        if (orderPlacedCustomerNotificationQueuedEmailId > 0)
                        {
                            InsertOrderNote(order.OrderId, string.Format("\"Order placed\" email (to customer) has been queued. Queued email identifier: {0}.", orderPlacedCustomerNotificationQueuedEmailId), false, DateTime.UtcNow);
                        }

                        SMSManager.SendOrderPlacedNotification(order);

                        //order status
                        order = CheckOrderStatus(order.OrderId);

                        //reset checkout data
                        if (!paymentInfo.IsRecurringPayment)
                        {
                            CustomerManager.ResetCheckoutData(customer.CustomerId, true);
                        }

                        //log
                        if (!paymentInfo.IsRecurringPayment)
                        {
                            CustomerActivityManager.InsertActivity(
                                "PlaceOrder",
                                LocalizationManager.GetLocaleResourceString("ActivityLog.PlaceOrder"),
                                order.OrderId);
                        }

                        //uncomment this line to support transactions
                        //scope.Complete();


                        //raise event             
                        EventContext.Current.OnOrderPlaced(null,
                            new OrderEventArgs() { Order = order });
            
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
                LogManager.InsertLog(LogTypeEnum.OrderError, string.Format("Error while placing order. {0}", processPaymentResult.Error), processPaymentResult.FullError);
            }
            return processPaymentResult.Error;
        }

        /// <summary>
        /// Place order items in current user shopping cart.
        /// </summary>
        /// <param name="orderId">The order identifier</param>
        public static void ReOrder(int orderId)
        {
            var order = GetOrderById(orderId);
            if(order != null)
            {
                foreach (var orderProductVariant in order.OrderProductVariants)
                {
                    ShoppingCartManager.AddToCart(ShoppingCartTypeEnum.ShoppingCart, 
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
        public static void ProcessNextRecurringPayment(int recurringPaymentId)
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
                string result = OrderManager.PlaceOrder(paymentInfo, customer,
                    Guid.NewGuid(), out newOrderId);
                if (!String.IsNullOrEmpty(result))
                {
                    throw new NopException(result);
                }
                else
                {
                    InsertRecurringPaymentHistory(rp.RecurringPaymentId, newOrderId, DateTime.UtcNow);
                }
            }
            catch (Exception exc)
            {
                LogManager.InsertLog(LogTypeEnum.OrderError, string.Format("Error while processing recurring order. {0}", exc.Message), exc);
                throw;
            }
        }

        /// <summary>
        /// Cancels a recurring payment
        /// </summary>
        /// <param name="recurringPaymentId">Recurring payment identifier</param>
        public static RecurringPayment CancelRecurringPayment(int recurringPaymentId)
        {
            return CancelRecurringPayment(recurringPaymentId, true);
        }

        /// <summary>
        /// Cancels a recurring payment
        /// </summary>
        /// <param name="recurringPaymentId">Recurring payment identifier</param>
        /// <param name="throwException">A value indicating whether to throw the exception after an error has occupied.</param>
        public static RecurringPayment CancelRecurringPayment(int recurringPaymentId, 
            bool throwException)
        {
            var recurringPayment = GetRecurringPaymentById(recurringPaymentId);
            try
            {
                if (recurringPayment != null)
                {
                    //update recurring payment
                    UpdateRecurringPayment(recurringPayment.RecurringPaymentId, recurringPayment.InitialOrderId,
                        recurringPayment.CycleLength, recurringPayment.CyclePeriod,
                        recurringPayment.TotalCycles, recurringPayment.StartDate,
                        false, recurringPayment.Deleted, recurringPayment.CreatedOn);

                    var initialOrder = recurringPayment.InitialOrder;
                    if (initialOrder == null)
                        return recurringPayment;

                    //old info from placing order
                    var cancelPaymentResult = new CancelPaymentResult();                    
                    cancelPaymentResult.AuthorizationTransactionId = initialOrder.AuthorizationTransactionId;
                    cancelPaymentResult.CaptureTransactionId = initialOrder.CaptureTransactionId;
                    cancelPaymentResult.SubscriptionTransactionId = initialOrder.SubscriptionTransactionId;
                    cancelPaymentResult.Amount = initialOrder.OrderTotal;
                    PaymentManager.CancelRecurringPayment(initialOrder, ref cancelPaymentResult);
                    if (String.IsNullOrEmpty(cancelPaymentResult.Error))
                    {
                        InsertOrderNote(initialOrder.OrderId, string.Format("Recurring payment has been cancelled"), false, DateTime.UtcNow);
                    }
                    else
                    {
                        InsertOrderNote(initialOrder.OrderId, string.Format("Error cancelling recurring payment. Error: {0}", cancelPaymentResult.Error), false, DateTime.UtcNow);
                    }
                }
            }
            catch (Exception exc)
            {
                LogManager.InsertLog(LogTypeEnum.OrderError, "Error cancelling recurring payment", exc);
                if (throwException)
                    throw;
            }
            return recurringPayment;
        }

        /// <summary>
        /// Gets a value indicating whether a customer can cancel recurring payment
        /// </summary>
        /// <param name="customerToValidate">Customer</param>
        /// <param name="recurringPayment">Recurring Payment</param>
        /// <returns>value indicating whether a customer can cancel recurring payment</returns>
        public static bool CanCancelRecurringPayment(Customer customerToValidate, RecurringPayment recurringPayment)
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
        public static bool CanShip(Order order)
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
        public static Order Ship(int orderId, bool notifyCustomer)
        {
            var order = GetOrderById(orderId);
            if (order == null)
                return order;

            if (!CanShip(order))
                throw new NopException("Can not do shipment for order.");

            var shippedDate = DateTime.UtcNow;
            order = UpdateOrder(order.OrderId, order.OrderGuid, order.CustomerId, order.CustomerLanguageId,
                order.CustomerTaxDisplayType, order.CustomerIP, order.OrderSubtotalInclTax, order.OrderSubtotalExclTax, order.OrderShippingInclTax,
                order.OrderShippingExclTax, order.PaymentMethodAdditionalFeeInclTax, order.PaymentMethodAdditionalFeeExclTax,
                order.OrderTax, order.OrderTotal, order.OrderDiscount,
                order.OrderSubtotalInclTaxInCustomerCurrency, order.OrderSubtotalExclTaxInCustomerCurrency,
                order.OrderShippingInclTaxInCustomerCurrency, order.OrderShippingExclTaxInCustomerCurrency,
                order.PaymentMethodAdditionalFeeInclTaxInCustomerCurrency, order.PaymentMethodAdditionalFeeExclTaxInCustomerCurrency,
                order.OrderTaxInCustomerCurrency, order.OrderTotalInCustomerCurrency,
                order.OrderDiscountInCustomerCurrency,
                order.CheckoutAttributeDescription, order.CheckoutAttributesXml, 
                order.CustomerCurrencyCode, order.OrderWeight,
                order.AffiliateId, order.OrderStatus, order.AllowStoringCreditCardNumber, order.CardType,
                order.CardName, order.CardNumber, order.MaskedCreditCardNumber,
                order.CardCvv2, order.CardExpirationMonth, order.CardExpirationYear,
                order.PaymentMethodId, order.PaymentMethodName,
                order.AuthorizationTransactionId,
                order.AuthorizationTransactionCode, order.AuthorizationTransactionResult,
                order.CaptureTransactionId, order.CaptureTransactionResult,
                order.SubscriptionTransactionId, order.PurchaseOrderNumber, order.PaymentStatus, order.PaidDate,
                order.BillingFirstName, order.BillingLastName, order.BillingPhoneNumber,
                order.BillingEmail, order.BillingFaxNumber, order.BillingCompany, order.BillingAddress1,
                order.BillingAddress2, order.BillingCity,
                order.BillingStateProvince, order.BillingStateProvinceId, order.BillingZipPostalCode,
                order.BillingCountry, order.BillingCountryId, ShippingStatusEnum.Shipped,
                order.ShippingFirstName, order.ShippingLastName, order.ShippingPhoneNumber,
                order.ShippingEmail, order.ShippingFaxNumber, order.ShippingCompany,
                order.ShippingAddress1, order.ShippingAddress2, order.ShippingCity,
                order.ShippingStateProvince, order.ShippingStateProvinceId, order.ShippingZipPostalCode,
                order.ShippingCountry, order.ShippingCountryId,
                order.ShippingMethod, order.ShippingRateComputationMethodId, shippedDate,
                order.DeliveryDate, order.TrackingNumber, order.VatNumber, 
                order.Deleted, order.CreatedOn);

            InsertOrderNote(order.OrderId, string.Format("Order has been shipped"), false, DateTime.UtcNow);

            if (notifyCustomer)
            {
                int orderShippedCustomerNotificationQueuedEmailId = MessageManager.SendOrderShippedCustomerNotification(order, order.CustomerLanguageId);
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
        public static bool CanDeliver(Order order)
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
        public static Order Deliver(int orderId, bool notifyCustomer)
        {
            var order = GetOrderById(orderId);
            if (order == null)
                return order;

            if (!CanDeliver(order))
                throw new NopException("Can not do delivery for order.");

            var deliveryDate = DateTime.UtcNow;
            order = UpdateOrder(order.OrderId, order.OrderGuid, order.CustomerId, order.CustomerLanguageId,
                order.CustomerTaxDisplayType, order.CustomerIP, order.OrderSubtotalInclTax, order.OrderSubtotalExclTax, order.OrderShippingInclTax,
                order.OrderShippingExclTax, order.PaymentMethodAdditionalFeeInclTax, order.PaymentMethodAdditionalFeeExclTax,
                order.OrderTax, order.OrderTotal, order.OrderDiscount,
                order.OrderSubtotalInclTaxInCustomerCurrency, order.OrderSubtotalExclTaxInCustomerCurrency,
                order.OrderShippingInclTaxInCustomerCurrency, order.OrderShippingExclTaxInCustomerCurrency,
                order.PaymentMethodAdditionalFeeInclTaxInCustomerCurrency, order.PaymentMethodAdditionalFeeExclTaxInCustomerCurrency,
                order.OrderTaxInCustomerCurrency, order.OrderTotalInCustomerCurrency,
                order.OrderDiscountInCustomerCurrency,
                order.CheckoutAttributeDescription, order.CheckoutAttributesXml,
                order.CustomerCurrencyCode, order.OrderWeight,
                order.AffiliateId, order.OrderStatus, order.AllowStoringCreditCardNumber, order.CardType,
                order.CardName, order.CardNumber, order.MaskedCreditCardNumber,
                order.CardCvv2, order.CardExpirationMonth, order.CardExpirationYear,
                order.PaymentMethodId, order.PaymentMethodName,
                order.AuthorizationTransactionId,
                order.AuthorizationTransactionCode, order.AuthorizationTransactionResult,
                order.CaptureTransactionId, order.CaptureTransactionResult,
                order.SubscriptionTransactionId, order.PurchaseOrderNumber, order.PaymentStatus, order.PaidDate,
                order.BillingFirstName, order.BillingLastName, order.BillingPhoneNumber,
                order.BillingEmail, order.BillingFaxNumber, order.BillingCompany, order.BillingAddress1,
                order.BillingAddress2, order.BillingCity,
                order.BillingStateProvince, order.BillingStateProvinceId, order.BillingZipPostalCode,
                order.BillingCountry, order.BillingCountryId, ShippingStatusEnum.Delivered,
                order.ShippingFirstName, order.ShippingLastName, order.ShippingPhoneNumber,
                order.ShippingEmail, order.ShippingFaxNumber, order.ShippingCompany,
                order.ShippingAddress1, order.ShippingAddress2, order.ShippingCity,
                order.ShippingStateProvince, order.ShippingStateProvinceId, order.ShippingZipPostalCode,
                order.ShippingCountry, order.ShippingCountryId,
                order.ShippingMethod, order.ShippingRateComputationMethodId, order.ShippedDate,
                deliveryDate, order.TrackingNumber, order.VatNumber, 
                order.Deleted, order.CreatedOn);

            InsertOrderNote(order.OrderId, string.Format("Order has been delivered"), false, DateTime.UtcNow);

            if (notifyCustomer)
            {
                int orderDeliveredCustomerNotificationQueuedEmailId = MessageManager.SendOrderDeliveredCustomerNotification(order, order.CustomerLanguageId);
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
        public static bool CanCancelOrder(Order order)
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
        public static Order CancelOrder(int orderId, bool notifyCustomer)
        {
            var order = GetOrderById(orderId);
            if (order == null)
                return order;

            if (!CanCancelOrder(order))
                throw new NopException("Can not do cancel for order.");
            
            //Cancel order
            order = SetOrderStatus(order.OrderId, OrderStatusEnum.Cancelled, notifyCustomer);

            InsertOrderNote(order.OrderId, string.Format("Order has been cancelled"), false, DateTime.UtcNow);
            
            //cancel recurring payments
            var recurringPayments = SearchRecurringPayments(0, order.OrderId, null);
            foreach (var rp in recurringPayments)
            {
                CancelRecurringPayment(rp.RecurringPaymentId, false);
            }
                
            //Adjust inventory
            foreach (var opv in order.OrderProductVariants)
                ProductManager.AdjustInventory(opv.ProductVariantId, false, opv.Quantity, opv.AttributesXml);

            return order;
        }

        /// <summary>
        /// Gets a value indicating whether order can be marked as authorized
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A value indicating whether order can be marked as authorized</returns>
        public static bool CanMarkOrderAsAuthorized(Order order)
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
        public static Order MarkAsAuthorized(int orderId)
        {
            var order = GetOrderById(orderId);
            if (order == null)
                return order;

            order = UpdateOrder(order.OrderId, order.OrderGuid, order.CustomerId, order.CustomerLanguageId,
                order.CustomerTaxDisplayType, order.CustomerIP, order.OrderSubtotalInclTax, order.OrderSubtotalExclTax, order.OrderShippingInclTax,
                   order.OrderShippingExclTax, order.PaymentMethodAdditionalFeeInclTax, order.PaymentMethodAdditionalFeeExclTax,
                   order.OrderTax, order.OrderTotal, order.OrderDiscount,
                   order.OrderSubtotalInclTaxInCustomerCurrency, order.OrderSubtotalExclTaxInCustomerCurrency,
                   order.OrderShippingInclTaxInCustomerCurrency, order.OrderShippingExclTaxInCustomerCurrency,
                   order.PaymentMethodAdditionalFeeInclTaxInCustomerCurrency, order.PaymentMethodAdditionalFeeExclTaxInCustomerCurrency,
                   order.OrderTaxInCustomerCurrency, order.OrderTotalInCustomerCurrency,
                   order.OrderDiscountInCustomerCurrency,
                   order.CheckoutAttributeDescription, order.CheckoutAttributesXml,
                   order.CustomerCurrencyCode, order.OrderWeight,
                   order.AffiliateId, order.OrderStatus, order.AllowStoringCreditCardNumber,
                   order.CardType, order.CardName, order.CardNumber, order.MaskedCreditCardNumber,
                   order.CardCvv2, order.CardExpirationMonth, order.CardExpirationYear,
                   order.PaymentMethodId, order.PaymentMethodName,
                   order.AuthorizationTransactionId,
                   order.AuthorizationTransactionCode, order.AuthorizationTransactionResult,
                   order.CaptureTransactionId, order.CaptureTransactionResult,
                   order.SubscriptionTransactionId, order.PurchaseOrderNumber, 
                   PaymentStatusEnum.Authorized, order.PaidDate,
                   order.BillingFirstName, order.BillingLastName, order.BillingPhoneNumber,
                   order.BillingEmail, order.BillingFaxNumber, order.BillingCompany, order.BillingAddress1,
                   order.BillingAddress2, order.BillingCity,
                   order.BillingStateProvince, order.BillingStateProvinceId, order.BillingZipPostalCode,
                   order.BillingCountry, order.BillingCountryId, order.ShippingStatus,
                   order.ShippingFirstName, order.ShippingLastName, order.ShippingPhoneNumber,
                   order.ShippingEmail, order.ShippingFaxNumber, order.ShippingCompany,
                   order.ShippingAddress1, order.ShippingAddress2, order.ShippingCity,
                   order.ShippingStateProvince, order.ShippingStateProvinceId, order.ShippingZipPostalCode,
                   order.ShippingCountry, order.ShippingCountryId,
                   order.ShippingMethod, order.ShippingRateComputationMethodId,
                   order.ShippedDate, order.DeliveryDate,
                   order.TrackingNumber, order.VatNumber, order.Deleted, order.CreatedOn);

            InsertOrderNote(order.OrderId, string.Format("Order has been marked as authorized"), false, DateTime.UtcNow);

            order = CheckOrderStatus(order.OrderId);

            return order;
        }

        /// <summary>
        /// Gets a value indicating whether capture from admin panel is allowed
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A value indicating whether capture from admin panel is allowed</returns>
        public static bool CanCapture(Order order)
        {
            if (order == null)
                return false;

            if (order.OrderStatus == OrderStatusEnum.Cancelled ||
                order.OrderStatus == OrderStatusEnum.Pending)
                return false;

            if (order.PaymentStatus == PaymentStatusEnum.Authorized &&
                PaymentManager.CanCapture(order.PaymentMethodId))
                return true;

            return false;
        }

        /// <summary>
        /// Captures order (from admin panel)
        /// </summary>
        /// <param name="orderId">Order identifier</param>
        /// <param name="error">Error</param>
        /// <returns>Captured order</returns>
        public static Order Capture(int orderId, ref string error)
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

                PaymentManager.Capture(order, ref processPaymentResult);

                if (String.IsNullOrEmpty(processPaymentResult.Error))
                {
                    var paidDate = order.PaidDate;
                    var paymentStatus = processPaymentResult.PaymentStatus;
                    if (paymentStatus == PaymentStatusEnum.Paid)
                        paidDate = DateTime.UtcNow;
                    order = UpdateOrder(order.OrderId, order.OrderGuid, order.CustomerId, order.CustomerLanguageId,
                        order.CustomerTaxDisplayType, order.CustomerIP, order.OrderSubtotalInclTax, order.OrderSubtotalExclTax, order.OrderShippingInclTax,
                        order.OrderShippingExclTax, order.PaymentMethodAdditionalFeeInclTax, order.PaymentMethodAdditionalFeeExclTax,
                        order.OrderTax, order.OrderTotal, order.OrderDiscount,
                        order.OrderSubtotalInclTaxInCustomerCurrency, order.OrderSubtotalExclTaxInCustomerCurrency,
                        order.OrderShippingInclTaxInCustomerCurrency, order.OrderShippingExclTaxInCustomerCurrency,
                        order.PaymentMethodAdditionalFeeInclTaxInCustomerCurrency, order.PaymentMethodAdditionalFeeExclTaxInCustomerCurrency,
                        order.OrderTaxInCustomerCurrency, order.OrderTotalInCustomerCurrency,
                        order.OrderDiscountInCustomerCurrency,
                        order.CheckoutAttributeDescription, order.CheckoutAttributesXml, 
                        order.CustomerCurrencyCode, order.OrderWeight,
                        order.AffiliateId, order.OrderStatus, order.AllowStoringCreditCardNumber,
                        order.CardType, order.CardName, order.CardNumber, order.MaskedCreditCardNumber,
                        order.CardCvv2, order.CardExpirationMonth, order.CardExpirationYear,
                        order.PaymentMethodId, order.PaymentMethodName,
                        processPaymentResult.AuthorizationTransactionId,
                        processPaymentResult.AuthorizationTransactionCode,
                        processPaymentResult.AuthorizationTransactionResult,
                        processPaymentResult.CaptureTransactionId,
                        processPaymentResult.CaptureTransactionResult,
                        processPaymentResult.SubscriptionTransactionId,
                        order.PurchaseOrderNumber, paymentStatus, paidDate,
                        order.BillingFirstName, order.BillingLastName, order.BillingPhoneNumber,
                        order.BillingEmail, order.BillingFaxNumber, order.BillingCompany, order.BillingAddress1,
                        order.BillingAddress2, order.BillingCity,
                        order.BillingStateProvince, order.BillingStateProvinceId, order.BillingZipPostalCode,
                        order.BillingCountry, order.BillingCountryId, order.ShippingStatus,
                        order.ShippingFirstName, order.ShippingLastName, order.ShippingPhoneNumber,
                        order.ShippingEmail, order.ShippingFaxNumber, order.ShippingCompany,
                        order.ShippingAddress1, order.ShippingAddress2, order.ShippingCity,
                        order.ShippingStateProvince, order.ShippingStateProvinceId, order.ShippingZipPostalCode,
                        order.ShippingCountry, order.ShippingCountryId,
                        order.ShippingMethod, order.ShippingRateComputationMethodId,
                        order.ShippedDate, order.DeliveryDate,
                        order.TrackingNumber, order.VatNumber, order.Deleted, order.CreatedOn);

                    InsertOrderNote(order.OrderId, string.Format("Order has been captured"), false, DateTime.UtcNow);

                }
                else
                {
                    InsertOrderNote(order.OrderId, string.Format("Unable to capture order. Error: {0}", processPaymentResult.Error), false, DateTime.UtcNow);

                }
                order = CheckOrderStatus(order.OrderId);
            }
            catch (Exception exc)
            {
                processPaymentResult.Error = exc.Message;
                processPaymentResult.FullError = exc.ToString();
            }

            if (!String.IsNullOrEmpty(processPaymentResult.Error))
            {
                error = processPaymentResult.Error;
                LogManager.InsertLog(LogTypeEnum.OrderError, string.Format("Error capturing order. {0}", processPaymentResult.Error), processPaymentResult.FullError);
            }
            return order;
        }

        /// <summary>
        /// Gets a value indicating whether order can be marked as paid
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A value indicating whether order can be marked as paid</returns>
        public static bool CanMarkOrderAsPaid(Order order)
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
        public static Order MarkOrderAsPaid(int orderId)
        {
            var order = GetOrderById(orderId);
            if (order == null)
                return order;

            if (!CanMarkOrderAsPaid(order))
                throw new NopException("You can't mark this order as paid");

            order = UpdateOrder(order.OrderId, order.OrderGuid, order.CustomerId, order.CustomerLanguageId,
                    order.CustomerTaxDisplayType, order.CustomerIP, order.OrderSubtotalInclTax, order.OrderSubtotalExclTax, order.OrderShippingInclTax,
                    order.OrderShippingExclTax, order.PaymentMethodAdditionalFeeInclTax, order.PaymentMethodAdditionalFeeExclTax,
                    order.OrderTax, order.OrderTotal, order.OrderDiscount,
                    order.OrderSubtotalInclTaxInCustomerCurrency, order.OrderSubtotalExclTaxInCustomerCurrency,
                    order.OrderShippingInclTaxInCustomerCurrency, order.OrderShippingExclTaxInCustomerCurrency,
                    order.PaymentMethodAdditionalFeeInclTaxInCustomerCurrency, order.PaymentMethodAdditionalFeeExclTaxInCustomerCurrency,
                    order.OrderTaxInCustomerCurrency, order.OrderTotalInCustomerCurrency,
                    order.OrderDiscountInCustomerCurrency,
                    order.CheckoutAttributeDescription, order.CheckoutAttributesXml, 
                    order.CustomerCurrencyCode, order.OrderWeight,
                    order.AffiliateId, order.OrderStatus, order.AllowStoringCreditCardNumber, order.CardType,
                    order.CardName, order.CardNumber, order.MaskedCreditCardNumber,
                    order.CardCvv2, order.CardExpirationMonth, order.CardExpirationYear,
                    order.PaymentMethodId, order.PaymentMethodName,
                    order.AuthorizationTransactionId,
                    order.AuthorizationTransactionCode, order.AuthorizationTransactionResult,
                    order.CaptureTransactionId, order.CaptureTransactionResult,
                    order.SubscriptionTransactionId, order.PurchaseOrderNumber, PaymentStatusEnum.Paid, DateTime.UtcNow,
                    order.BillingFirstName, order.BillingLastName, order.BillingPhoneNumber,
                    order.BillingEmail, order.BillingFaxNumber, order.BillingCompany, order.BillingAddress1,
                    order.BillingAddress2, order.BillingCity,
                    order.BillingStateProvince, order.BillingStateProvinceId, order.BillingZipPostalCode,
                    order.BillingCountry, order.BillingCountryId, order.ShippingStatus,
                    order.ShippingFirstName, order.ShippingLastName, order.ShippingPhoneNumber,
                    order.ShippingEmail, order.ShippingFaxNumber, order.ShippingCompany,
                    order.ShippingAddress1, order.ShippingAddress2, order.ShippingCity,
                    order.ShippingStateProvince, order.ShippingStateProvinceId, order.ShippingZipPostalCode,
                    order.ShippingCountry, order.ShippingCountryId,
                    order.ShippingMethod, order.ShippingRateComputationMethodId,
                    order.ShippedDate, order.DeliveryDate,
                    order.TrackingNumber, order.VatNumber, order.Deleted, order.CreatedOn);

            InsertOrderNote(order.OrderId, string.Format("Order has been marked as paid"), false, DateTime.UtcNow);

            order = CheckOrderStatus(order.OrderId);

            return order;
        }

        /// <summary>
        /// Gets a value indicating whether refund from admin panel is allowed
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A value indicating whether refund from admin panel is allowed</returns>
        public static bool CanRefund(Order order)
        {
            if (order == null)
                return false;

            if (order.OrderStatus == OrderStatusEnum.Cancelled)
                return false;

            if (order.PaymentStatus == PaymentStatusEnum.Paid &&
                PaymentManager.CanRefund(order.PaymentMethodId))
                return true;

            return false;
        }

        /// <summary>
        /// Refunds order (from admin panel)
        /// </summary>
        /// <param name="orderId">Order identifier</param>
        /// <param name="error">Error</param>
        /// <returns>Refunded order</returns>
        public static Order Refund(int orderId, ref string error)
        {
            var order = GetOrderById(orderId);
            if (order == null)
                return order;

            if (!CanRefund(order))
                throw new NopException("Can not do refund for order.");

            var cancelPaymentResult = new CancelPaymentResult();
            try
            {
                //old info from placing order
                cancelPaymentResult.AuthorizationTransactionId = order.AuthorizationTransactionId;
                cancelPaymentResult.CaptureTransactionId = order.CaptureTransactionId;
                cancelPaymentResult.SubscriptionTransactionId = order.SubscriptionTransactionId;
                cancelPaymentResult.Amount = order.OrderTotal;
                cancelPaymentResult.PaymentStatus = order.PaymentStatus;

                PaymentManager.Refund(order, ref cancelPaymentResult);

                if (String.IsNullOrEmpty(cancelPaymentResult.Error))
                {
                    order = UpdateOrder(order.OrderId, order.OrderGuid, order.CustomerId, order.CustomerLanguageId,
                        order.CustomerTaxDisplayType, order.CustomerIP, order.OrderSubtotalInclTax, order.OrderSubtotalExclTax, order.OrderShippingInclTax,
                        order.OrderShippingExclTax, order.PaymentMethodAdditionalFeeInclTax, order.PaymentMethodAdditionalFeeExclTax,
                        order.OrderTax, order.OrderTotal, order.OrderDiscount,
                        order.OrderSubtotalInclTaxInCustomerCurrency, order.OrderSubtotalExclTaxInCustomerCurrency,
                        order.OrderShippingInclTaxInCustomerCurrency, order.OrderShippingExclTaxInCustomerCurrency,
                        order.PaymentMethodAdditionalFeeInclTaxInCustomerCurrency, order.PaymentMethodAdditionalFeeExclTaxInCustomerCurrency,
                        order.OrderTaxInCustomerCurrency, order.OrderTotalInCustomerCurrency,
                        order.OrderDiscountInCustomerCurrency,
                        order.CheckoutAttributeDescription, order.CheckoutAttributesXml,
                        order.CustomerCurrencyCode, order.OrderWeight,
                        order.AffiliateId, order.OrderStatus, order.AllowStoringCreditCardNumber,
                        order.CardType, order.CardName, order.CardNumber, order.MaskedCreditCardNumber,
                        order.CardCvv2, order.CardExpirationMonth, order.CardExpirationYear,
                        order.PaymentMethodId, order.PaymentMethodName,
                        cancelPaymentResult.AuthorizationTransactionId,
                        order.AuthorizationTransactionCode,
                        order.AuthorizationTransactionResult,
                        cancelPaymentResult.CaptureTransactionId,
                        order.CaptureTransactionResult,
                        order.SubscriptionTransactionId, order.PurchaseOrderNumber, cancelPaymentResult.PaymentStatus, order.PaidDate,
                        order.BillingFirstName, order.BillingLastName, order.BillingPhoneNumber,
                        order.BillingEmail, order.BillingFaxNumber, order.BillingCompany, order.BillingAddress1,
                        order.BillingAddress2, order.BillingCity,
                        order.BillingStateProvince, order.BillingStateProvinceId, order.BillingZipPostalCode,
                        order.BillingCountry, order.BillingCountryId, order.ShippingStatus,
                        order.ShippingFirstName, order.ShippingLastName, order.ShippingPhoneNumber,
                        order.ShippingEmail, order.ShippingFaxNumber, order.ShippingCompany,
                        order.ShippingAddress1, order.ShippingAddress2, order.ShippingCity,
                        order.ShippingStateProvince, order.ShippingStateProvinceId, order.ShippingZipPostalCode,
                        order.ShippingCountry, order.ShippingCountryId,
                        order.ShippingMethod, order.ShippingRateComputationMethodId,
                        order.ShippedDate, order.DeliveryDate,
                        order.TrackingNumber, order.VatNumber, order.Deleted, order.CreatedOn);

                    InsertOrderNote(order.OrderId, string.Format("Order has been refunded"), false, DateTime.UtcNow);

                }
                else
                {
                    InsertOrderNote(order.OrderId, string.Format("Unable to refund order. Error: {0}", cancelPaymentResult.Error), false, DateTime.UtcNow);

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
                LogManager.InsertLog(LogTypeEnum.OrderError, string.Format("Error refunding order. {0}", cancelPaymentResult.Error), cancelPaymentResult.FullError);
            }
            return order;
        }

        /// <summary>
        /// Gets a value indicating whether order can be marked as refunded
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A value indicating whether order can be marked as refunded</returns>
        public static bool CanRefundOffline(Order order)
        {
            if (order == null)
                return false;

            if (order.OrderStatus == OrderStatusEnum.Cancelled)
                return false;

            if (order.PaymentStatus == PaymentStatusEnum.Paid)
                return true;

            return false;
        }

        /// <summary>
        /// Refunds order (offline)
        /// </summary>
        /// <param name="orderId">Order identifier</param>
        /// <returns>Updated order</returns>
        public static Order RefundOffline(int orderId)
        {
            var order = GetOrderById(orderId);
            if (order == null)
                return order;

            if (!CanRefundOffline(order))
                throw new NopException("You can't refund this order");

            order = UpdateOrder(order.OrderId, order.OrderGuid, order.CustomerId, order.CustomerLanguageId,
                order.CustomerTaxDisplayType, order.CustomerIP, order.OrderSubtotalInclTax, order.OrderSubtotalExclTax, order.OrderShippingInclTax,
                   order.OrderShippingExclTax, order.PaymentMethodAdditionalFeeInclTax, order.PaymentMethodAdditionalFeeExclTax,
                   order.OrderTax, order.OrderTotal, order.OrderDiscount,
                   order.OrderSubtotalInclTaxInCustomerCurrency, order.OrderSubtotalExclTaxInCustomerCurrency,
                   order.OrderShippingInclTaxInCustomerCurrency, order.OrderShippingExclTaxInCustomerCurrency,
                   order.PaymentMethodAdditionalFeeInclTaxInCustomerCurrency, order.PaymentMethodAdditionalFeeExclTaxInCustomerCurrency,
                   order.OrderTaxInCustomerCurrency, order.OrderTotalInCustomerCurrency,
                   order.OrderDiscountInCustomerCurrency,
                   order.CheckoutAttributeDescription, order.CheckoutAttributesXml, 
                   order.CustomerCurrencyCode, order.OrderWeight,
                   order.AffiliateId, order.OrderStatus, order.AllowStoringCreditCardNumber, order.CardType,
                   order.CardName, order.CardNumber, order.MaskedCreditCardNumber,
                   order.CardCvv2, order.CardExpirationMonth, order.CardExpirationYear,
                   order.PaymentMethodId, order.PaymentMethodName,
                   order.AuthorizationTransactionId,
                   order.AuthorizationTransactionCode, order.AuthorizationTransactionResult,
                   order.CaptureTransactionId, order.CaptureTransactionResult,
                   order.SubscriptionTransactionId, order.PurchaseOrderNumber, PaymentStatusEnum.Refunded, order.PaidDate,
                   order.BillingFirstName, order.BillingLastName, order.BillingPhoneNumber,
                   order.BillingEmail, order.BillingFaxNumber, order.BillingCompany, order.BillingAddress1,
                   order.BillingAddress2, order.BillingCity,
                   order.BillingStateProvince, order.BillingStateProvinceId, order.BillingZipPostalCode,
                   order.BillingCountry, order.BillingCountryId, order.ShippingStatus,
                   order.ShippingFirstName, order.ShippingLastName, order.ShippingPhoneNumber,
                   order.ShippingEmail, order.ShippingFaxNumber, order.ShippingCompany,
                   order.ShippingAddress1, order.ShippingAddress2, order.ShippingCity,
                   order.ShippingStateProvince, order.ShippingStateProvinceId, order.ShippingZipPostalCode,
                   order.ShippingCountry, order.ShippingCountryId,
                   order.ShippingMethod, order.ShippingRateComputationMethodId,
                   order.ShippedDate, order.DeliveryDate,
                   order.TrackingNumber, order.VatNumber, order.Deleted, order.CreatedOn);

            InsertOrderNote(order.OrderId, string.Format("Order has been marked as refunded"), false, DateTime.UtcNow);

            order = CheckOrderStatus(order.OrderId);

            return order;
        }

        /// <summary>
        /// Gets a value indicating whether void from admin panel is allowed
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A value indicating whether void from admin panel is allowed</returns>
        public static bool CanVoid(Order order)
        {
            if (order == null)
                return false;

            if (order.OrderStatus == OrderStatusEnum.Cancelled)
                return false;

            if (order.PaymentStatus == PaymentStatusEnum.Authorized &&
                PaymentManager.CanVoid(order.PaymentMethodId))
                return true;

            return false;
        }

        /// <summary>
        /// Voids order (from admin panel)
        /// </summary>
        /// <param name="orderId">Order identifier</param>
        /// <param name="error">Error</param>
        /// <returns>Voided order</returns>
        public static Order Void(int orderId, ref string error)
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

                PaymentManager.Void(order, ref cancelPaymentResult);

                if (String.IsNullOrEmpty(cancelPaymentResult.Error))
                {
                    order = UpdateOrder(order.OrderId, order.OrderGuid, order.CustomerId, order.CustomerLanguageId,
                        order.CustomerTaxDisplayType, order.CustomerIP, order.OrderSubtotalInclTax, order.OrderSubtotalExclTax, order.OrderShippingInclTax,
                        order.OrderShippingExclTax, order.PaymentMethodAdditionalFeeInclTax, order.PaymentMethodAdditionalFeeExclTax,
                        order.OrderTax, order.OrderTotal, order.OrderDiscount,
                        order.OrderSubtotalInclTaxInCustomerCurrency, order.OrderSubtotalExclTaxInCustomerCurrency,
                        order.OrderShippingInclTaxInCustomerCurrency, order.OrderShippingExclTaxInCustomerCurrency,
                        order.PaymentMethodAdditionalFeeInclTaxInCustomerCurrency, order.PaymentMethodAdditionalFeeExclTaxInCustomerCurrency,
                        order.OrderTaxInCustomerCurrency, order.OrderTotalInCustomerCurrency,
                        order.OrderDiscountInCustomerCurrency,
                        order.CheckoutAttributeDescription, order.CheckoutAttributesXml, 
                        order.CustomerCurrencyCode, order.OrderWeight,
                        order.AffiliateId, order.OrderStatus, order.AllowStoringCreditCardNumber,
                        order.CardType, order.CardName, order.CardNumber, order.MaskedCreditCardNumber,
                        order.CardCvv2, order.CardExpirationMonth, order.CardExpirationYear,
                        order.PaymentMethodId, order.PaymentMethodName,
                        cancelPaymentResult.AuthorizationTransactionId,
                        order.AuthorizationTransactionCode,
                        order.AuthorizationTransactionResult,
                        cancelPaymentResult.CaptureTransactionId,
                        order.CaptureTransactionResult,
                        order.SubscriptionTransactionId, order.PurchaseOrderNumber,
                        cancelPaymentResult.PaymentStatus, order.PaidDate,
                        order.BillingFirstName, order.BillingLastName, order.BillingPhoneNumber,
                        order.BillingEmail, order.BillingFaxNumber, order.BillingCompany, order.BillingAddress1,
                        order.BillingAddress2, order.BillingCity,
                        order.BillingStateProvince, order.BillingStateProvinceId, order.BillingZipPostalCode,
                        order.BillingCountry, order.BillingCountryId, order.ShippingStatus,
                        order.ShippingFirstName, order.ShippingLastName, order.ShippingPhoneNumber,
                        order.ShippingEmail, order.ShippingFaxNumber, order.ShippingCompany,
                        order.ShippingAddress1, order.ShippingAddress2, order.ShippingCity,
                        order.ShippingStateProvince, order.ShippingStateProvinceId, order.ShippingZipPostalCode,
                        order.ShippingCountry, order.ShippingCountryId,
                        order.ShippingMethod, order.ShippingRateComputationMethodId,
                        order.ShippedDate, order.DeliveryDate,
                        order.TrackingNumber, order.VatNumber, order.Deleted, order.CreatedOn);

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
                LogManager.InsertLog(LogTypeEnum.OrderError, string.Format("Error voiding order. {0}", cancelPaymentResult.Error), cancelPaymentResult.FullError);
            }
            return order;
        }

        /// <summary>
        /// Gets a value indicating whether order can be marked as voided
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A value indicating whether order can be marked as voided</returns>
        public static bool CanVoidOffline(Order order)
        {
            if (order == null)
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
        public static Order VoidOffline(int orderId)
        {
            var order = GetOrderById(orderId);
            if (order == null)
                return order;

            if (!CanVoidOffline(order))
                throw new NopException("You can't void this order");

            order = UpdateOrder(order.OrderId, order.OrderGuid, order.CustomerId, order.CustomerLanguageId,
                order.CustomerTaxDisplayType, order.CustomerIP, order.OrderSubtotalInclTax, order.OrderSubtotalExclTax, order.OrderShippingInclTax,
                   order.OrderShippingExclTax, order.PaymentMethodAdditionalFeeInclTax, order.PaymentMethodAdditionalFeeExclTax,
                   order.OrderTax, order.OrderTotal, order.OrderDiscount,
                   order.OrderSubtotalInclTaxInCustomerCurrency, order.OrderSubtotalExclTaxInCustomerCurrency,
                   order.OrderShippingInclTaxInCustomerCurrency, order.OrderShippingExclTaxInCustomerCurrency,
                   order.PaymentMethodAdditionalFeeInclTaxInCustomerCurrency, order.PaymentMethodAdditionalFeeExclTaxInCustomerCurrency,
                   order.OrderTaxInCustomerCurrency, order.OrderTotalInCustomerCurrency,
                   order.OrderDiscountInCustomerCurrency,
                   order.CheckoutAttributeDescription, order.CheckoutAttributesXml, 
                   order.CustomerCurrencyCode, order.OrderWeight,
                   order.AffiliateId, order.OrderStatus, order.AllowStoringCreditCardNumber, 
                   order.CardType, order.CardName, order.CardNumber, 
                   order.MaskedCreditCardNumber, order.CardCvv2, 
                   order.CardExpirationMonth, order.CardExpirationYear,
                   order.PaymentMethodId, order.PaymentMethodName,
                   order.AuthorizationTransactionId,
                   order.AuthorizationTransactionCode, order.AuthorizationTransactionResult,
                   order.CaptureTransactionId, order.CaptureTransactionResult,
                   order.SubscriptionTransactionId, order.PurchaseOrderNumber, PaymentStatusEnum.Voided, order.PaidDate,
                   order.BillingFirstName, order.BillingLastName, order.BillingPhoneNumber,
                   order.BillingEmail, order.BillingFaxNumber, order.BillingCompany, order.BillingAddress1,
                   order.BillingAddress2, order.BillingCity,
                   order.BillingStateProvince, order.BillingStateProvinceId, order.BillingZipPostalCode,
                   order.BillingCountry, order.BillingCountryId, order.ShippingStatus,
                   order.ShippingFirstName, order.ShippingLastName, order.ShippingPhoneNumber,
                   order.ShippingEmail, order.ShippingFaxNumber, order.ShippingCompany,
                   order.ShippingAddress1, order.ShippingAddress2, order.ShippingCity,
                   order.ShippingStateProvince, order.ShippingStateProvinceId, order.ShippingZipPostalCode,
                   order.ShippingCountry, order.ShippingCountryId,
                   order.ShippingMethod, order.ShippingRateComputationMethodId,
                   order.ShippedDate, order.DeliveryDate,
                   order.TrackingNumber, order.VatNumber, order.Deleted, order.CreatedOn);

            InsertOrderNote(order.OrderId, string.Format("Order has been marked as voided"), false, DateTime.UtcNow);

            order = CheckOrderStatus(order.OrderId);

            return order;
        }

        /// <summary>
        /// Converts reward points to amount primary store currency
        /// </summary>
        /// <param name="rewardPoints">Reward points</param>
        /// <returns>Converted value</returns>
        public static decimal ConvertRewardPointsToAmount(int rewardPoints)
        {
            decimal result = decimal.Zero;
            if (rewardPoints <= 0)
                return decimal.Zero;

            result = rewardPoints * OrderManager.RewardPointsExchangeRate;            
            result = Math.Round(result, 2);
            return result;
        }

        /// <summary>
        /// Converts an amount in primary store currency to reward points
        /// </summary>
        /// <param name="rewardPoints">Reward points</param>
        /// <returns>Converted value</returns>
        public static int ConvertAmountToRewardPoints(decimal amount)
        {
            int result = 0;
            if (amount <= 0)
                return 0;

            if (OrderManager.RewardPointsExchangeRate > 0)
            {
                result = (int)Math.Ceiling(amount / OrderManager.RewardPointsExchangeRate);
            }
            return result;
        }

        #endregion

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether cache is enabled
        /// </summary>
        public static bool CacheEnabled
        {
            get
            {
                return SettingManager.GetSettingValueBoolean("Cache.OrderManager.CacheEnabled");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether customer can make re-order
        /// </summary>
        public static bool IsReOrderAllowed
        {
            get
            {
                return SettingManager.GetSettingValueBoolean("Order.IsReOrderAllowed", true);
            }
            set
            {
                SettingManager.SetParam("Order.IsReOrderAllowed", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Reward Points Program is enabled
        /// </summary>
        public static bool RewardPointsEnabled
        {
            get
            {
                return SettingManager.GetSettingValueBoolean("RewardPoints.Enabled", false);
            }
            set
            {
                SettingManager.SetParam("RewardPoints.Enabled", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Reward Points exchange rate
        /// </summary>
        public static decimal RewardPointsExchangeRate
        {
            get
            {
                return SettingManager.GetSettingValueDecimalNative("RewardPoints.Rate", 1.00M);
            }
            set
            {
                SettingManager.SetParamNative("RewardPoints.Rate", value);
            }
        }

        /// <summary>
        /// Gets or sets a number of points awarded for registration
        /// </summary>
        public static int RewardPointsForRegistration
        {
            get
            {
                return SettingManager.GetSettingValueInteger("RewardPoints.Earning.ForRegistration", 0);
            }
            set
            {
                SettingManager.SetParam("RewardPoints.Earning.ForRegistration", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a number of points awarded for purchases (amount in primary store currency)
        /// </summary>
        public static decimal RewardPointsForPurchases_Amount
        {
            get
            {
                return SettingManager.GetSettingValueDecimalNative("RewardPoints.Earning.RewardPointsForPurchases.Amount", 10.00M);
            }
            set
            {
                SettingManager.SetParamNative("RewardPoints.Earning.RewardPointsForPurchases.Amount", value);
            }
        }

        /// <summary>
        /// Gets or sets a number of points awarded for purchases
        /// </summary>
        public static int RewardPointsForPurchases_Points
        {
            get
            {
                return SettingManager.GetSettingValueInteger("RewardPoints.Earning.RewardPointsForPurchases.Points", 1);
            }
            set
            {
                SettingManager.SetParam("RewardPoints.Earning.RewardPointsForPurchases.Points", value.ToString());
            }
        }

        /// <summary>
        /// Points are awarded when the order status is
        /// </summary>
        public static OrderStatusEnum RewardPointsForPurchases_Awarded
        {
            get
            {
                return (OrderStatusEnum)SettingManager.GetSettingValueInteger("RewardPoints.Earning.RewardPointsForPurchases.AwardedOS", (int)OrderStatusEnum.Complete);
            }
            set
            {
                SettingManager.SetParam("RewardPoints.Earning.RewardPointsForPurchases.AwardedOS", ((int)value).ToString());
            }
        }

        /// <summary>
        /// Points are canceled when the order is
        /// </summary>
        public static OrderStatusEnum RewardPointsForPurchases_Canceled
        {
            get
            {
                return (OrderStatusEnum)SettingManager.GetSettingValueInteger("RewardPoints.Earning.RewardPointsForPurchases.CanceledOS", (int)OrderStatusEnum.Cancelled);
            }
            set
            {
                SettingManager.SetParam("RewardPoints.Earning.RewardPointsForPurchases.CanceledOS", ((int)value).ToString());
            }
        }

        /// <summary>
        /// Gift cards are activated when the order status is
        /// </summary>
        public static OrderStatusEnum? GiftCards_Activated
        {
            get
            {
                int os = SettingManager.GetSettingValueInteger("GiftCards.Activation.ActivatedOS");
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
                    SettingManager.SetParam("GiftCards.Activation.ActivatedOS", ((int)value).ToString());
                }
                else
                {
                    SettingManager.SetParam("GiftCards.Activation.ActivatedOS", "0");
                }
            }
        }

        /// <summary>
        /// Gift cards are deactivated when the order status is
        /// </summary>
        public static OrderStatusEnum? GiftCards_Deactivated
        {
            get
            {
                int os = SettingManager.GetSettingValueInteger("GiftCards.Activation.DeactivatedOS");
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
                    SettingManager.SetParam("GiftCards.Activation.DeactivatedOS", ((int)value).ToString());
                }
                else
                {
                    SettingManager.SetParam("GiftCards.Activation.DeactivatedOS", "0");
                }
            }
        }
        
        #endregion
    }
}
