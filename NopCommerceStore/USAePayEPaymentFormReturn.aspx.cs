using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Payment.Methods.USAePay;

namespace NopSolutions.NopCommerce.Web
{
    public partial class USAePayEPaymentFormReturn : BaseNopPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(NopContext.Current.User == null)
            {
                string loginURL = SEOHelper.GetLoginPageUrl(true);
                Response.Redirect(loginURL);
            }

            CommonHelper.SetResponseNoCache(Response);

            if(!Page.IsPostBack)
            {
                if(EPaymentFormSettings.UsePIN && !EPaymentFormHelper.ValidateResponseSign(Request.Form))
                {
                    Response.Redirect(CommonHelper.GetStoreLocation());
                }
                if(!Request.Form["UMstatus"].Equals("Approved"))
                {
                    Response.Redirect(CommonHelper.GetStoreLocation());
                }

                int orderId = 0;
                if(!Int32.TryParse(Request.Form["UMinvoice"], out orderId))
                {
                    Response.Redirect(CommonHelper.GetStoreLocation());
                }
                Order order = OrderManager.GetOrderById(orderId);
                if(order == null || NopContext.Current.User.CustomerId != order.CustomerId)
                {
                    Response.Redirect(CommonHelper.GetStoreLocation());
                }

                string transactionId = Request.Form["UMrefNum"];

                if(EPaymentFormSettings.AuthorizeOnly)
                {
                    //set AuthorizationTransactionID
                    order = OrderManager.UpdateOrder(order.OrderId, order.OrderGuid, order.CustomerId, order.CustomerLanguageId,
                       order.CustomerTaxDisplayType, order.CustomerIP, order.OrderSubtotalInclTax, order.OrderSubtotalExclTax, order.OrderShippingInclTax,
                       order.OrderShippingExclTax, order.PaymentMethodAdditionalFeeInclTax, order.PaymentMethodAdditionalFeeExclTax,
                       order.TaxRates, order.OrderTax, order.OrderTotal,
                       order.RefundedAmount, order.OrderDiscount,
                       order.OrderSubtotalInclTaxInCustomerCurrency, order.OrderSubtotalExclTaxInCustomerCurrency,
                       order.OrderShippingInclTaxInCustomerCurrency, order.OrderShippingExclTaxInCustomerCurrency,
                       order.PaymentMethodAdditionalFeeInclTaxInCustomerCurrency, order.PaymentMethodAdditionalFeeExclTaxInCustomerCurrency,
                       order.TaxRatesInCustomerCurrency, order.OrderTaxInCustomerCurrency,
                       order.OrderTotalInCustomerCurrency,
                       order.OrderDiscountInCustomerCurrency,
                       order.CheckoutAttributeDescription, order.CheckoutAttributesXml,
                       order.CustomerCurrencyCode, order.OrderWeight,
                       order.AffiliateId, order.OrderStatus, order.AllowStoringCreditCardNumber,
                       order.CardType, order.CardName, order.CardNumber, order.MaskedCreditCardNumber,
                       order.CardCvv2, order.CardExpirationMonth, order.CardExpirationYear,
                       order.PaymentMethodId, order.PaymentMethodName,
                       transactionId,
                       order.AuthorizationTransactionCode, order.AuthorizationTransactionResult,
                       order.CaptureTransactionId, order.CaptureTransactionResult,
                       order.SubscriptionTransactionId, order.PurchaseOrderNumber,
                       order.PaymentStatus, order.PaidDate,
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

                    if(OrderManager.CanMarkOrderAsAuthorized(order))
                    {
                        OrderManager.MarkAsAuthorized(order.OrderId);
                    }
                }
                else
                {
                    //set CaptureTransactionID
                    order = OrderManager.UpdateOrder(order.OrderId, order.OrderGuid, order.CustomerId, order.CustomerLanguageId,
                       order.CustomerTaxDisplayType, order.CustomerIP, order.OrderSubtotalInclTax, order.OrderSubtotalExclTax, order.OrderShippingInclTax,
                       order.OrderShippingExclTax, order.PaymentMethodAdditionalFeeInclTax, order.PaymentMethodAdditionalFeeExclTax,
                       order.TaxRates, order.OrderTax, order.OrderTotal,
                       order.RefundedAmount, order.OrderDiscount,
                       order.OrderSubtotalInclTaxInCustomerCurrency, order.OrderSubtotalExclTaxInCustomerCurrency,
                       order.OrderShippingInclTaxInCustomerCurrency, order.OrderShippingExclTaxInCustomerCurrency,
                       order.PaymentMethodAdditionalFeeInclTaxInCustomerCurrency, order.PaymentMethodAdditionalFeeExclTaxInCustomerCurrency,
                       order.TaxRatesInCustomerCurrency, order.OrderTaxInCustomerCurrency,
                       order.OrderTotalInCustomerCurrency,
                       order.OrderDiscountInCustomerCurrency,
                       order.CheckoutAttributeDescription, order.CheckoutAttributesXml, 
                       order.CustomerCurrencyCode, order.OrderWeight,
                       order.AffiliateId, order.OrderStatus, order.AllowStoringCreditCardNumber,
                       order.CardType, order.CardName, order.CardNumber, order.MaskedCreditCardNumber,
                       order.CardCvv2, order.CardExpirationMonth, order.CardExpirationYear,
                       order.PaymentMethodId, order.PaymentMethodName,
                       order.AuthorizationTransactionId,
                       order.AuthorizationTransactionCode, order.AuthorizationTransactionResult,
                       transactionId, order.CaptureTransactionResult,
                       order.SubscriptionTransactionId, order.PurchaseOrderNumber,
                       order.PaymentStatus, order.PaidDate,
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

                    if(OrderManager.CanMarkOrderAsPaid(order))
                    {
                        OrderManager.MarkOrderAsPaid(order.OrderId);
                    }
                }

                Response.Redirect("~/checkoutcompleted.aspx");
            }
        }

        public override bool AllowGuestNavigation
        {
            get
            {
                return true;
            }
        }
    }
}
