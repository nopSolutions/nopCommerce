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
// Contributor(s): 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NopSolutions.NopCommerce.BusinessLogic.Payment;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using System.Globalization;
using NopSolutions.NopCommerce.Payment.Methods.USAePay.services;
using System.Web;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.BusinessLogic.IoC;

namespace NopSolutions.NopCommerce.Payment.Methods.USAePay
{
    /// <summary>
    /// Represents an USA ePay ePayment Form payment gateway
    /// </summary>
    public class EPaymentFormPaymentProcessor : IPaymentMethod
    {
        #region Methods
        /// <summary>
        /// Process payment
        /// </summary>
        /// <param name="paymentInfo">Payment info required for an order processing</param>
        /// <param name="customer">Customer</param>
        /// <param name="orderGuid">Unique order identifier</param>
        /// <param name="processPaymentResult">Process payment result</param>
        public void ProcessPayment(PaymentInfo paymentInfo, Customer customer, Guid orderGuid, ref ProcessPaymentResult processPaymentResult)
        {
            processPaymentResult.PaymentStatus = PaymentStatusEnum.Pending;
        }

        /// <summary>
        /// Post process payment (payment gateways that require redirecting)
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>The error status, or String.Empty if no errors</returns>
        public string PostProcessPayment(Order order)
        {
            Uri gatewayUrl = new Uri(EPaymentFormSettings.GatewayUrl);

            RemotePost post = new RemotePost();

            post.FormName = "ePaymentForm";
            post.Url = gatewayUrl.ToString();
            post.Method = "POST";

            post.Add("UMkey", EPaymentFormSettings.SourceKey);
            post.Add("UMcommand", (EPaymentFormSettings.AuthorizeOnly ? "authonly" : "sale"));
            post.Add("UMamount", String.Format(CultureInfo.InvariantCulture, "{0:0.00}", order.OrderTotal));
            post.Add("UMcurrency", IoCFactory.Resolve<ICurrencyManager>().PrimaryStoreCurrency.CurrencyCode);
            post.Add("UMinvoice", String.Format("{0:0000000000}", order.OrderId));
            post.Add("UMredirApproved", String.Format("{0}USAePayEPaymentFormReturn.aspx", CommonHelper.GetStoreLocation(false)));
            post.Add("UMemail", order.BillingEmail);

            post.Add("UMbillfname", order.BillingFirstName);
            post.Add("UMbilllname", order.BillingLastName);
            post.Add("UMbillcompany", order.BillingCompany);
            post.Add("UMbillstreet", order.BillingAddress1);
            post.Add("UMbillstreet2", order.BillingAddress2);
            post.Add("UMbillcity", order.BillingCity);
            post.Add("UMbillstate", order.BillingStateProvince);
            post.Add("UMbillzip", order.BillingZipPostalCode);
            post.Add("UMbillcountry", order.BillingCountry);
            post.Add("UMbillphone", order.BillingPhoneNumber);

            post.Add("UMshipfname", order.ShippingFirstName);
            post.Add("UMshiplname", order.ShippingLastName);
            post.Add("UMshipcompany", order.ShippingCompany);
            post.Add("UMshipstreet", order.ShippingAddress1);
            post.Add("UMshipstreet2", order.ShippingAddress2);
            post.Add("UMshipcity", order.ShippingCity);
            post.Add("UMshipstate", order.ShippingStateProvince);
            post.Add("UMshipzip", order.ShippingZipPostalCode);
            post.Add("UMshipcountry", order.ShippingCountry);
            post.Add("UMshipphone", order.ShippingPhoneNumber);

            if(EPaymentFormSettings.UsePIN)
            {
                post.Add("UMhash", EPaymentFormHelper.CalcRequestSign(post.Params));
            }
            
            post.Post();

            return String.Empty;
        }

        /// <summary>
        /// Gets additional handling fee
        /// </summary>
        /// <returns>Additional handling fee</returns>
        public decimal GetAdditionalHandlingFee()
        {
            return EPaymentFormSettings.AdditionalFee;
        }

        /// <summary>
        /// Captures payment
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="processPaymentResult">Process payment result</param>
        public void Capture(Order order, ref ProcessPaymentResult processPaymentResult)
        {
            try
            {
                using(usaepayService svc = new usaepayService())
                {
                    svc.Url = EPaymentFormSettings.ServiceUrl;
                    TransactionResponse rsp = svc.captureTransaction(EPaymentFormHelper.ServiceSecurityToken, processPaymentResult.AuthorizationTransactionId, (double)order.OrderTotal);
                    switch(rsp.ResultCode)
                    {
                        case "A":
                            processPaymentResult.CaptureTransactionId = rsp.RefNum;
                            processPaymentResult.AVSResult = rsp.AvsResult;
                            processPaymentResult.PaymentStatus = PaymentStatusEnum.Paid;
                            break;
                        case "D":
                        case "E":
                        default:
                            processPaymentResult.Error = rsp.ErrorCode;
                            processPaymentResult.FullError = rsp.Error;
                            break;
                    }
                }
            }
            catch(Exception ex)
            {
                processPaymentResult.Error = ex.Message;
                processPaymentResult.FullError = ex.Message;
            }
        }

        /// <summary>
        /// Refunds payment
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="cancelPaymentResult">Cancel payment result</param>
        public void Refund(Order order, ref CancelPaymentResult cancelPaymentResult)
        {
            try
            {
                using(usaepayService svc = new usaepayService())
                {
                    svc.Url = EPaymentFormSettings.ServiceUrl;
                    TransactionResponse rsp = svc.refundTransaction(EPaymentFormHelper.ServiceSecurityToken, cancelPaymentResult.CaptureTransactionId, (double)cancelPaymentResult.Amount);
                    switch(rsp.ResultCode)
                    {
                        case "A":
                            {
                                if (cancelPaymentResult.IsPartialRefund)
                                    cancelPaymentResult.PaymentStatus = PaymentStatusEnum.PartiallyRefunded;
                                else
                                    cancelPaymentResult.PaymentStatus = PaymentStatusEnum.Refunded;
                            }
                            break;
                        case "D":
                        case "E":
                        default:
                            cancelPaymentResult.Error = rsp.ErrorCode;
                            cancelPaymentResult.FullError = rsp.Error;
                            break;
                    }
                }
            }
            catch(Exception ex)
            {
                cancelPaymentResult.Error = ex.Message;
                cancelPaymentResult.FullError = ex.Message;
            }
        }

        /// <summary>
        /// Voids paymen
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="cancelPaymentResult">Cancel payment result</param>
        public void Void(Order order, ref CancelPaymentResult cancelPaymentResult)
        {
            try
            {
                using(usaepayService svc = new usaepayService())
                {
                    svc.Url = EPaymentFormSettings.ServiceUrl;
                    if(svc.voidTransaction(EPaymentFormHelper.ServiceSecurityToken, cancelPaymentResult.AuthorizationTransactionId))
                    {
                        cancelPaymentResult.PaymentStatus = PaymentStatusEnum.Voided;
                    }
                    else
                    {
                        cancelPaymentResult.Error = "Failed";
                        cancelPaymentResult.FullError = "Failed";
                    }
                }
            }
            catch(Exception ex)
            {
                cancelPaymentResult.Error = ex.Message;
                cancelPaymentResult.FullError = ex.Message;
            }
        }

        /// <summary>
        /// Process recurring payment
        /// </summary>
        /// <param name="paymentInfo">Payment info required for an order processing</param>
        /// <param name="customer">Customer</param>
        /// <param name="orderGuid">Unique order identifier</param>
        /// <param name="processPaymentResult">Process payment result</param>
        public void ProcessRecurringPayment(PaymentInfo paymentInfo, Customer customer, Guid orderGuid, ref ProcessPaymentResult processPaymentResult)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Cancels recurring payment
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="cancelPaymentResult">Cancel payment result</param>
        public void CancelRecurringPayment(Order order, ref CancelPaymentResult cancelPaymentResult)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether capture is supported
        /// </summary>
        public bool CanCapture
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether partial refund is supported
        /// </summary>
        public bool CanPartiallyRefund
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether refund is supported
        /// </summary>
        public bool CanRefund
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether void is supported
        /// </summary>
        public bool CanVoid
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a recurring payment type of payment method
        /// </summary>
        /// <returns>A recurring payment type of payment method</returns>
        public RecurringPaymentTypeEnum SupportRecurringPayments
        {
            get
            {
                return RecurringPaymentTypeEnum.NotSupported;
            }
        }

        /// <summary>
        /// Gets a payment method type
        /// </summary>
        /// <returns>A payment method type</returns>
        public PaymentMethodTypeEnum PaymentMethodType
        {
            get
            {
                return PaymentMethodTypeEnum.Standard;
            }
        }
        #endregion
    }
}
