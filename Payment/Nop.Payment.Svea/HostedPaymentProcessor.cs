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
using System.Text;
using System.Web;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Payment;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Tax;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.Payment.Methods.Svea
{
    /// <summary>
    /// Svea hosted payment processor
    /// </summary>
    public class HostedPaymentProcessor : IPaymentMethod
    {
        #region Methods
        public void ProcessPayment(PaymentInfo paymentInfo, Customer customer, Guid orderGuid, ref ProcessPaymentResult processPaymentResult)
        {
            processPaymentResult.PaymentStatus = PaymentStatusEnum.Pending;
        }

        /// <summary>
        /// Builds the query string and redirect user to Svea payment gateway
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public string PostProcessPayment(Order order)
        {
            StringBuilder sb = new StringBuilder(HostedPaymentSettings.GatewayUrl);
            var opvCollection = order.OrderProductVariants;
            Customer customer = order.Customer;

            sb.AppendFormat("?Username={0}&", HostedPaymentSettings.Username);

            int rowNumber = 1;

            //items
            for(int i = 0; i < opvCollection.Count; i++, rowNumber++)
            {
                OrderProductVariant opv = opvCollection[i];
                ProductVariant pv = opv.ProductVariant;

                sb.AppendFormat("Row{0}AmountExVAT={1}&", rowNumber, opv.UnitPriceExclTax);
                sb.AppendFormat("Row{0}Description={1}&", rowNumber, HttpUtility.UrlEncode(pv.LocalizedFullProductName, Encoding.UTF8));
                sb.AppendFormat("Row{0}Quantity={1}&", rowNumber, opv.Quantity);

                string errStr = String.Empty;
                sb.AppendFormat("Row{0}VATPercentage={1}&", rowNumber, (int)TaxManager.GetTaxRate(pv, customer, ref errStr));
            }

            //discount
            if (order.OrderDiscount != Decimal.Zero)
            {
                sb.AppendFormat("Row{0}AmountExVAT={1}&", rowNumber, -order.OrderDiscount);
                sb.AppendFormat("Row{0}Description=Discount&", rowNumber);
                sb.AppendFormat("Row{0}Quantity=1&", rowNumber);
                sb.AppendFormat("Row{0}VATPercentage=0&", rowNumber);

                rowNumber++;
            }

            //shipping
            if(order.OrderShippingExclTax != decimal.Zero)
            {
                sb.AppendFormat("Row{0}AmountExVAT={1}&", rowNumber, order.OrderShippingExclTax);
                sb.AppendFormat("Row{0}Description=Shipping&", rowNumber);
                sb.AppendFormat("Row{0}Quantity=1&", rowNumber);
                sb.AppendFormat("Row{0}VATPercentage={1}&", rowNumber, (int)(((order.OrderShippingInclTax - order.OrderShippingExclTax) / order.OrderShippingExclTax) * 100));

                rowNumber++;
            }

            //payment method fee
            if(order.PaymentMethodAdditionalFeeInclTax != Decimal.Zero)
            {
                sb.AppendFormat("Row{0}AmountExVAT={1}&", rowNumber, order.PaymentMethodAdditionalFeeExclTax);
                sb.AppendFormat("Row{0}Description=Additional&", rowNumber);
                sb.AppendFormat("Row{0}Quantity=1&", rowNumber);
                sb.AppendFormat("Row{0}VATPercentage={1}&", rowNumber, (int)(((order.PaymentMethodAdditionalFeeInclTax - order.PaymentMethodAdditionalFeeExclTax) / order.PaymentMethodAdditionalFeeExclTax) * 100));

                rowNumber++;
            }

            //gift cards
            var gcuhC = OrderManager.GetAllGiftCardUsageHistoryEntries(null, null, order.OrderId);
            foreach (var gcuh in gcuhC)
            {
                sb.AppendFormat("Row{0}AmountExVAT={1}&", rowNumber, -gcuh.UsedValue);
                sb.AppendFormat("Row{0}Description=GiftCard-{1}&", rowNumber, gcuh.GiftCard.GiftCardCouponCode);
                sb.AppendFormat("Row{0}Quantity=1&", rowNumber);
                sb.AppendFormat("Row{0}VATPercentage={1}&", rowNumber, 0);

                rowNumber++;
            }

            //reward points
            if (order.RedeemedRewardPoints != null)
            {
                sb.AppendFormat("Row{0}AmountExVAT={1}&", rowNumber, -order.RedeemedRewardPoints.UsedAmount);
                sb.AppendFormat("Row{0}Description={1}-RewardPoints&", rowNumber, -order.RedeemedRewardPoints.Points);
                sb.AppendFormat("Row{0}Quantity=1&", rowNumber);
                sb.AppendFormat("Row{0}VATPercentage={1}&", rowNumber, 0);

                rowNumber++;
            }

            sb.AppendFormat("{0}={1}&", "OrderId", order.OrderId);
            sb.AppendFormat("{0}={1}&", "PaymentMethod", HostedPaymentSettings.PaymentMethod);
            sb.AppendFormat("{0}={1}&", "ResponseURL", HttpUtility.UrlEncode(String.Format("{0}SveaHostedPaymentReturn.aspx", CommonHelper.GetStoreHost(false)), Encoding.UTF8));
            sb.AppendFormat("{0}={1}&", "CancelURL", HttpUtility.UrlEncode(String.Format("{0}Checkout.aspx", CommonHelper.GetStoreHost(false)), Encoding.UTF8));
            sb.AppendFormat("{0}={1}&", "TestMode", HostedPaymentSettings.UseSandbox);
            sb.AppendFormat("{0}={1}&", "Language", "SV");
            sb.AppendFormat("{0}={1}&", "Country", "SE");
            sb.AppendFormat("{0}={1}", "Currency", CurrencyManager.PrimaryStoreCurrency.CurrencyCode);

            sb.AppendFormat("&{0}={1}", "MD5", HostedPaymentHelper.CalcMd5Hash(sb.ToString() + HostedPaymentSettings.Password));

            HttpContext.Current.Response.Redirect(sb.ToString());

            return String.Empty;
        }

        public decimal GetAdditionalHandlingFee()
        {
            return HostedPaymentSettings.AdditionalFee;
        }

        public void Capture(Order order, ref ProcessPaymentResult processPaymentResult)
        {
            throw new NotImplementedException();
        }

        public void Refund(Order order, ref CancelPaymentResult cancelPaymentResult)
        {
            throw new NotImplementedException();
        }

        public void Void(Order order, ref CancelPaymentResult cancelPaymentResult)
        {
            throw new NotImplementedException();
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
            throw new NopException("Recurring payments not supported");
        }

        /// <summary>
        /// Cancels recurring payment
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="cancelPaymentResult">Cancel payment result</param>        
        public void CancelRecurringPayment(Order order, ref CancelPaymentResult cancelPaymentResult)
        {
        }

        #endregion

        #region Properties

        public bool CanCapture
        {
            get
            {
                return false;
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
                return false;
            }
        }

        public bool CanVoid
        {
            get
            {
                return false;
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
