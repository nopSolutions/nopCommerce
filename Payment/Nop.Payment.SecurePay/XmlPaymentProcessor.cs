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
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Payment;

namespace NopSolutions.NopCommerce.Payment.Methods.SecurePay
{
    public class XmlPaymentProcessor : IPaymentMethod
	{
		#region Fields
		private const string ID = "nopCommerce / SecureXML 1.0.0";
		#endregion

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
            XmlTransaction sxml = new XmlTransaction(XmlPaymentSettings.TestMode ? XmlTransaction.MODE_TEST : XmlTransaction.MODE_LIVE, SecurePaySettings.MerchantId, SecurePaySettings.MerchantPassword, ID);
			bool success = false;
			string code = "";

            if(XmlPaymentSettings.AuthorizeOnly)
			{
				success = sxml.processPreauth(paymentInfo.OrderTotal, orderGuid.ToString(), paymentInfo.CreditCardNumber, paymentInfo.CreditCardExpireMonth.ToString("D2"), paymentInfo.CreditCardExpireYear.ToString().Substring(2, 2), paymentInfo.CreditCardCvv2.ToString());
			}
			else
			{
				success = sxml.processCredit(paymentInfo.OrderTotal, orderGuid.ToString(), paymentInfo.CreditCardNumber, paymentInfo.CreditCardExpireMonth.ToString("D2"), paymentInfo.CreditCardExpireYear.ToString().Substring(2, 2), paymentInfo.CreditCardCvv2.ToString());
			}

			code = sxml["response_code"];

			if (!success)
			{
				processPaymentResult.Error = String.Format("Declined ({0})", code);
				processPaymentResult.FullError = sxml.Error;
			}
			else
			{
                if(XmlPaymentSettings.AuthorizeOnly)
                {
                    processPaymentResult.AuthorizationTransactionCode = (XmlPaymentSettings.AuthorizeOnly ? sxml["preauth_id"] : "");
                    processPaymentResult.AuthorizationTransactionId = sxml["transaction_id"];
                    processPaymentResult.AuthorizationTransactionResult = String.Format("Approved ({0})", code);
                    processPaymentResult.PaymentStatus = PaymentStatusEnum.Authorized;
                }
                else
                {
                    processPaymentResult.CaptureTransactionId = sxml["transaction_id"];
                    processPaymentResult.CaptureTransactionResult = String.Format("Approved ({0})", code);
                    processPaymentResult.PaymentStatus = PaymentStatusEnum.Paid;
                }
			}
		}

		/// <summary>
		/// Post process payment (payment gateways that require redirecting)
		/// </summary>
		/// <param name="order">Order</param>
		/// <returns>The error status, or String.Empty if no errors</returns>
		public string PostProcessPayment(Order order)
		{
			return string.Empty;
		}
		
		/// <summary>
		/// Gets additional handling fee
		/// </summary>
		/// <returns>Additional handling fee</returns>
		public decimal GetAdditionalHandlingFee()
		{
            return SecurePaySettings.AdditionalFee;
		}

		/// <summary>
		/// Captures payment (from admin panel)
		/// </summary>
		/// <param name="order">Order</param>
		/// <param name="processPaymentResult">Process payment result</param>
		public void Capture(Order order, ref ProcessPaymentResult processPaymentResult)
		{
            XmlTransaction sxml = new XmlTransaction(XmlPaymentSettings.TestMode ? XmlTransaction.MODE_TEST : XmlTransaction.MODE_LIVE, SecurePaySettings.MerchantId, SecurePaySettings.MerchantPassword, ID);
			bool success = false;
			string code = "";

			success = sxml.processAdvice(order.OrderTotal,order.OrderGuid.ToString(),processPaymentResult.AuthorizationTransactionCode);
			
			code = sxml["response_code"];

			if (!success)
			{
				processPaymentResult.Error = String.Format("Declined ({0})", code);
				processPaymentResult.FullError = sxml.Error;
			}
			else
			{
				processPaymentResult.CaptureTransactionId = sxml["transaction_id"];
                processPaymentResult.CaptureTransactionResult = String.Format("Approved ({0})", code);

				processPaymentResult.PaymentStatus = PaymentStatusEnum.Paid;
			}
		}

        public void Refund(Order order, ref CancelPaymentResult cancelPaymentResult)
        {
            XmlTransaction sxml = new XmlTransaction(XmlPaymentSettings.TestMode ? XmlTransaction.MODE_TEST : XmlTransaction.MODE_LIVE, SecurePaySettings.MerchantId, SecurePaySettings.MerchantPassword, ID);
            bool success = false;
            string code = "";

            success = sxml.processRefund(cancelPaymentResult.Amount, order.OrderGuid.ToString(), cancelPaymentResult.CaptureTransactionId);

            code = sxml["response_code"];

            if(!success)
            {
                cancelPaymentResult.Error = String.Format("Declined ({0})", code);
                cancelPaymentResult.FullError = sxml.Error;
            }
            else
            {
                cancelPaymentResult.PaymentStatus = PaymentStatusEnum.Refunded;
            }
        }

        public void Void(Order order, ref CancelPaymentResult cancelPaymentResult)
        {
            throw new NotImplementedException();
        }

        public void ProcessRecurringPayment(PaymentInfo paymentInfo, Customer customer, Guid orderGuid, ref ProcessPaymentResult processPaymentResult)
        {
            throw new NotImplementedException();
        }

        public void CancelRecurringPayment(Order order, ref CancelPaymentResult cancelPaymentResult)
        {
            throw new NotImplementedException();
        }
		#endregion
		
		#region Properties
		/// <summary>
		/// Gets a value indicating whether capture is allowed from admin panel
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
                return false;
            }
        }

        public bool CanRefund
        {
            get
            {
                return true;
            }
        }

        public bool CanVoid
        {
            get
            {
                return false;
            }
        }

        public RecurringPaymentTypeEnum SupportRecurringPayments
        {
            get
            {
                return RecurringPaymentTypeEnum.NotSupported;
            }
        }

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
