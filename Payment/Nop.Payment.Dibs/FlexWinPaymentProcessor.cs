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
using System.Globalization;
using System.Web;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Payment;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

namespace NopSolutions.NopCommerce.Payment.Methods.Dibs
{
    /// <summary>
    /// FlexWin payment processor
    /// </summary>
    public class FlexWinPaymentProcessor : IPaymentMethod
    {
        #region Methods
        public void ProcessPayment(PaymentInfo paymentInfo, Customer customer, Guid orderGuid, ref ProcessPaymentResult processPaymentResult)
        {
            processPaymentResult.PaymentStatus = PaymentStatusEnum.Pending;
        }

        /// <summary>
        /// Sends POST request to the DIBS server with order info
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>String.Empty</returns>
        public string PostProcessPayment(Order order)
        {
            RemotePost post = new RemotePost();

            post.FormName = "FlexWin";
            post.Url = FlexWinSettings.GatewayUrl;
            if(FlexWinSettings.UseSandbox)
            {
                post.Add("test", "yes");
            }
            post.Add("uniqueoid", "yes");

            Language lang = IoC.Resolve<ILanguageService>().GetLanguageById(order.CustomerLanguageId);
            int currency = DibsHelper.GetCurrencyNumberByCode(IoC.Resolve<ICurrencyService>().PrimaryStoreCurrency.CurrencyCode);
            int amount = (int)((double)order.OrderTotal * 100);
            int merhcantID = FlexWinSettings.MerchantId;

            post.Add("lang", DibsHelper.GetLanguageCodeByLanguageCulture(lang.LanguageCulture));
            post.Add("currency", currency.ToString());
            post.Add("color", FlexWinSettings.ColorTheme);
            post.Add("decorator", FlexWinSettings.Decorator);
            post.Add("merchant", merhcantID.ToString());
            post.Add("orderid", order.OrderId.ToString());
            post.Add("amount", amount.ToString());
            post.Add("md5key", FlexWinHelper.CalcMD5Key(merhcantID, order.OrderId, currency, amount));
            post.Add("accepturl", String.Format("{0}DibsFlexWinReturn.aspx?x={1}", CommonHelper.GetStoreHost(false), order.OrderId));
            post.Add("cancelurl", String.Format("{0}shoppingcart.aspx", CommonHelper.GetStoreHost(false)));
            post.Add("delivery1.Name", order.ShippingFullName);
            post.Add("delivery2.Address", order.ShippingAddress1);

            post.Add("ordline0-1", "SKU");
            post.Add("ordline0-2", "Description");
            post.Add("ordline0-3", "Quantity");
            post.Add("ordline0-4", "Price");

            var products = order.OrderProductVariants;
            for (int i = 0; i < products.Count; i++)
            {
                string lineName = String.Format("ordline{0}", (i + 1));
                ProductVariant pv = products[i].ProductVariant;

                post.Add(lineName + "-1", HttpUtility.HtmlEncode(pv.SKU));
                post.Add(lineName + "-2", HttpUtility.HtmlEncode(pv.LocalizedName));
                post.Add(lineName + "-3", HttpUtility.HtmlEncode(products[i].Quantity.ToString()));
                post.Add(lineName + "-4", pv.Price.ToString(NumberFormatInfo.CurrentInfo));
            }

            post.Post();

            return String.Empty;
        }

        public decimal GetAdditionalHandlingFee()
        {
            return FlexWinSettings.AdditionalFee;
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
