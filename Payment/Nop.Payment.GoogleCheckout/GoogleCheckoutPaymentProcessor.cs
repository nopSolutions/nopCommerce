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
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Xml;
using System.Xml.Serialization;
using GCheckout.AutoGen;
using GCheckout.Checkout;
using GCheckout.Util;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Audit;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Localization;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Payment;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Products.Attributes;
using NopSolutions.NopCommerce.BusinessLogic.Promo.Discounts;
using NopSolutions.NopCommerce.BusinessLogic.Shipping;
using NopSolutions.NopCommerce.BusinessLogic.Tax;
using NopSolutions.NopCommerce.BusinessLogic.Utils;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.IoC;

namespace NopSolutions.NopCommerce.Payment.Methods.GoogleCheckout
{
    /// <summary>
    /// Google Checkout payment processor
    /// </summary>
    public class GoogleCheckoutPaymentProcessor : IPaymentMethod
    {
        #region Utilities

        private void logMessage(string message)
        {
            try
            {
                if (!IoCFactory.Resolve<ISettingManager>().GetSettingValueBoolean("PaymentMethod.GoogleCheckout.DebugModeEnabled"))
                    return;
                message = string.Format("{0}*******{1}{2}", DateTime.Now, Environment.NewLine, message);
                string logPath = HttpContext.Current.Server.MapPath("google/google_log.txt");
                using (FileStream fs = new FileStream(logPath, FileMode.Append, FileAccess.Write, FileShare.Read))
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine(message);
                }
            }
            catch (Exception exc)
            {
                IoCFactory.Resolve<ILogManager>().InsertLog(LogTypeEnum.CommonError, exc.Message, exc);
            }
        }

        private void processNewOrderNotification(string xmlData)
        {
            try
            {
                NewOrderNotification newOrderNotification = (NewOrderNotification)EncodeHelper.Deserialize(xmlData, typeof(NewOrderNotification));
                string googleOrderNumber = newOrderNotification.googleordernumber;

                XmlNode CustomerInfo = newOrderNotification.shoppingcart.merchantprivatedata.Any[0];
                int CustomerID = Convert.ToInt32(CustomerInfo.Attributes["CustomerID"].Value);
                int CustomerLanguageID = Convert.ToInt32(CustomerInfo.Attributes["CustomerLanguageID"].Value);
                int CustomerCurrencyID = Convert.ToInt32(CustomerInfo.Attributes["CustomerCurrencyID"].Value);
                Customer customer = IoCFactory.Resolve<ICustomerManager>().GetCustomerById(CustomerID);

                NopSolutions.NopCommerce.BusinessLogic.Orders.ShoppingCart Cart = IoCFactory.Resolve<IShoppingCartManager>().GetCustomerShoppingCart(customer.CustomerId, ShoppingCartTypeEnum.ShoppingCart);

                if (customer == null)
                {
                    logMessage("Could not load a customer");
                    return;
                }

                NopContext.Current.User = customer;

                if (Cart.Count == 0)
                {
                    logMessage("Cart is empty");
                    return;
                }

                //validate cart
                foreach (NopSolutions.NopCommerce.BusinessLogic.Orders.ShoppingCartItem sci in Cart)
                {
                    bool ok = false;
                    foreach (Item item in newOrderNotification.shoppingcart.items)
                    {
                        if (!String.IsNullOrEmpty(item.merchantitemid))
                        {
                            if ((Convert.ToInt32(item.merchantitemid) == sci.ShoppingCartItemId) && (item.quantity == sci.Quantity))
                            {
                                ok = true;
                                break;
                            }
                        }
                    }

                    if (!ok)
                    {
                        logMessage(string.Format("Shopping Cart item has been changed. {0}. {1}", sci.ShoppingCartItemId, sci.Quantity));
                        return;
                    }
                }


                string[] billingFullname = newOrderNotification.buyerbillingaddress.contactname.Trim().Split(new char[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
                string billingFirstName = billingFullname[0];
                string billingLastName = string.Empty;
                if (billingFullname.Length > 1)
                    billingLastName = billingFullname[1];
                string billingEmail = newOrderNotification.buyerbillingaddress.email.Trim();
                string billingAddress1 = newOrderNotification.buyerbillingaddress.address1.Trim();
                string billingAddress2 = newOrderNotification.buyerbillingaddress.address2.Trim();
                string billingPhoneNumber = newOrderNotification.buyerbillingaddress.phone.Trim();
                string billingCity = newOrderNotification.buyerbillingaddress.city.Trim();
                int billingStateProvinceID = 0;
                StateProvince billingStateProvince = IoCFactory.Resolve<IStateProvinceManager>().GetStateProvinceByAbbreviation(newOrderNotification.buyerbillingaddress.region.Trim());
                if (billingStateProvince != null)
                    billingStateProvinceID = billingStateProvince.StateProvinceId;
                string billingZipPostalCode = newOrderNotification.buyerbillingaddress.postalcode.Trim();
                int billingCountryID = 0;
                Country billingCountry = IoCFactory.Resolve<ICountryManager>().GetCountryByTwoLetterIsoCode(newOrderNotification.buyerbillingaddress.countrycode.Trim());
                if (billingCountry != null)
                    billingCountryID = billingCountry.CountryId;

                NopSolutions.NopCommerce.BusinessLogic.CustomerManagement.Address billingAddress = customer.BillingAddresses.FindAddress(
                    billingFirstName, billingLastName, billingPhoneNumber,
                    billingEmail, string.Empty, string.Empty, billingAddress1, billingAddress2, billingCity,
                    billingStateProvinceID, billingZipPostalCode, billingCountryID);

                if (billingAddress == null)
                {
                    billingAddress = new BusinessLogic.CustomerManagement.Address()
                    {
                        CustomerId = CustomerID,
                        IsBillingAddress = true,
                        FirstName = billingFirstName,
                        LastName = billingLastName,
                        PhoneNumber = billingPhoneNumber,
                        Email = billingEmail,
                        Address1 = billingAddress1,
                        Address2 = billingAddress2,
                        City = billingCity,
                        StateProvinceId = billingStateProvinceID,
                        ZipPostalCode = billingZipPostalCode,
                        CountryId = billingCountryID,
                        CreatedOn = DateTime.UtcNow,
                        UpdatedOn = DateTime.UtcNow
                    };
                    IoCFactory.Resolve<ICustomerManager>().InsertAddress(billingAddress);
                }
                //set default billing address
                customer.BillingAddressId = billingAddress.AddressId;
                IoCFactory.Resolve<ICustomerManager>().UpdateCustomer(customer);

                NopSolutions.NopCommerce.BusinessLogic.CustomerManagement.Address shippingAddress = null;
                customer.LastShippingOption = null;
                bool shoppingCartRequiresShipping = IoCFactory.Resolve<IShippingManager>().ShoppingCartRequiresShipping(Cart);
                if (shoppingCartRequiresShipping)
                {
                    string[] shippingFullname = newOrderNotification.buyershippingaddress.contactname.Trim().Split(new char[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
                    string shippingFirstName = shippingFullname[0];
                    string shippingLastName = string.Empty;
                    if (shippingFullname.Length > 1)
                        shippingLastName = shippingFullname[1];
                    string shippingEmail = newOrderNotification.buyershippingaddress.email.Trim();
                    string shippingAddress1 = newOrderNotification.buyershippingaddress.address1.Trim();
                    string shippingAddress2 = newOrderNotification.buyershippingaddress.address2.Trim();
                    string shippingPhoneNumber = newOrderNotification.buyershippingaddress.phone.Trim();
                    string shippingCity = newOrderNotification.buyershippingaddress.city.Trim();
                    int shippingStateProvinceID = 0;
                    StateProvince shippingStateProvince = IoCFactory.Resolve<IStateProvinceManager>().GetStateProvinceByAbbreviation(newOrderNotification.buyershippingaddress.region.Trim());
                    if (shippingStateProvince != null)
                        shippingStateProvinceID = shippingStateProvince.StateProvinceId;
                    int shippingCountryID = 0;
                    string shippingZipPostalCode = newOrderNotification.buyershippingaddress.postalcode.Trim();
                    Country shippingCountry = IoCFactory.Resolve<ICountryManager>().GetCountryByTwoLetterIsoCode(newOrderNotification.buyershippingaddress.countrycode.Trim());
                    if (shippingCountry != null)
                        shippingCountryID = shippingCountry.CountryId;

                    shippingAddress = customer.ShippingAddresses.FindAddress(
                        shippingFirstName, shippingLastName, shippingPhoneNumber,
                        shippingEmail, string.Empty, string.Empty, 
                        shippingAddress1, shippingAddress2, shippingCity,
                        shippingStateProvinceID, shippingZipPostalCode, shippingCountryID);
                    if (shippingAddress == null)
                    {
                        shippingAddress = new BusinessLogic.CustomerManagement.Address()
                        {
                            CustomerId = CustomerID,
                            IsBillingAddress = false,
                            FirstName = shippingFirstName,
                            LastName = shippingLastName,
                            PhoneNumber = shippingPhoneNumber,
                            Email = shippingEmail,
                            Address1 = shippingAddress1,
                            Address2 = shippingAddress2,
                            City = shippingCity,
                            StateProvinceId = shippingStateProvinceID,
                            ZipPostalCode = shippingZipPostalCode,
                            CountryId = shippingCountryID,
                            CreatedOn = DateTime.UtcNow,
                            UpdatedOn = DateTime.UtcNow
                        };
                        IoCFactory.Resolve<ICustomerManager>().InsertAddress(shippingAddress);
                    }
                    //set default shipping address
                    customer.ShippingAddressId =  shippingAddress.AddressId;
                    IoCFactory.Resolve<ICustomerManager>().UpdateCustomer(customer);

                    string shippingMethod = string.Empty;
                    decimal shippingCost = decimal.Zero;
                    if (newOrderNotification.orderadjustment != null &&
                        newOrderNotification.orderadjustment.shipping != null &&
                        newOrderNotification.orderadjustment.shipping.Item != null)
                    {
                        FlatRateShippingAdjustment ShippingMethod = (FlatRateShippingAdjustment)newOrderNotification.orderadjustment.shipping.Item;
                        shippingMethod = ShippingMethod.shippingname;
                        shippingCost = ShippingMethod.shippingcost.Value;


                        ShippingOption shippingOption = new ShippingOption();
                        shippingOption.Name = shippingMethod;
                        shippingOption.Rate = shippingCost;
                        customer.LastShippingOption = shippingOption;
                    }
                }

                //customer.LastCalculatedTax = decimal.Zero;

                PaymentMethod googleCheckoutPaymentMethod = IoCFactory.Resolve<IPaymentManager>().GetPaymentMethodBySystemKeyword("GoogleCheckout");

                PaymentInfo paymentInfo = new PaymentInfo();
                paymentInfo.PaymentMethodId = googleCheckoutPaymentMethod.PaymentMethodId;
                paymentInfo.BillingAddress = billingAddress;
                paymentInfo.ShippingAddress = shippingAddress;
                paymentInfo.CustomerLanguage = IoCFactory.Resolve<ILanguageManager>().GetLanguageById(CustomerLanguageID);
                paymentInfo.CustomerCurrency = IoCFactory.Resolve<ICurrencyManager>().GetCurrencyById(CustomerCurrencyID);
                paymentInfo.GoogleOrderNumber = googleOrderNumber;
                int orderID = 0;
                string result = IoCFactory.Resolve<IOrderManager>().PlaceOrder(paymentInfo, customer, out orderID);
                if (!String.IsNullOrEmpty(result))
                {
                    logMessage("new-order-notification received. CreateOrder() error: Order Number " + orderID + ". " + result);
                    return;
                }

                Order order = IoCFactory.Resolve<IOrderManager>().GetOrderById(orderID);
                logMessage("new-order-notification received and saved: Order Number " + orderID);

            }
            catch (Exception exc)
            {
                logMessage("processNewOrderNotification Exception: " + exc.Message + ": " + exc.StackTrace);
            }
        }
        private void processOrderStateChangeNotification(string xmlData)
        {
            try
            {
                OrderStateChangeNotification changeOrder = (OrderStateChangeNotification)EncodeHelper.Deserialize(xmlData, typeof(OrderStateChangeNotification));

                FinancialOrderState orderState = changeOrder.newfinancialorderstate;
                Order order = getMerchantOrderByGoogleOrderID(changeOrder.googleordernumber);
                if (order != null)
                {
                    string message = string.Format("Order status {0} from Google: Order Number {1}", orderState, changeOrder.googleordernumber);
                    logMessage(message);
                    IoCFactory.Resolve<IOrderManager>().InsertOrderNote(order.OrderId, message, false, DateTime.UtcNow);

                    if (orderState == FinancialOrderState.CHARGING ||
                        orderState == FinancialOrderState.REVIEWING)
                    {
                    }

                    if (orderState == FinancialOrderState.CHARGEABLE)
                    {
                        order = IoCFactory.Resolve<IOrderManager>().MarkAsAuthorized(order.OrderId);
                    }
                    if (orderState == FinancialOrderState.CHARGED)
                    {
                        order = IoCFactory.Resolve<IOrderManager>().MarkOrderAsPaid(order.OrderId);
                    }
                    if (orderState == FinancialOrderState.CANCELLED || orderState == FinancialOrderState.CANCELLED_BY_GOOGLE)
                    {
                        order = IoCFactory.Resolve<IOrderManager>().CancelOrder(order.OrderId, true);
                    }
                }
            }
            catch (Exception exc)
            {
                logMessage("processOrderStateChangeNotification Exception: " + exc.Message + ": " + exc.StackTrace);
            }
        }
        private void processErrorNotification(string xmlData)
        {
            try
            {
                ErrorResponse errorResponse = (ErrorResponse)EncodeHelper.Deserialize(xmlData, typeof(ErrorResponse));

                StringBuilder errorSB = new StringBuilder();
                errorSB.Append(string.Format("Error response message received: {0}", errorResponse.errormessage));
                foreach (string Warning in errorResponse.warningmessages)
                    errorSB.Append("Warning: " + Warning);
                string message = errorSB.ToString();
                logMessage(message);
            }
            catch (Exception exc)
            {
                logMessage("processErrorNotification Exception: " + exc.Message + ": " + exc.StackTrace);
            }
        }
        private void processRiskInformationNotification(string xmlData)
        {
            RiskInformationNotification riskInformationNotification = (RiskInformationNotification)EncodeHelper.Deserialize(xmlData, typeof(RiskInformationNotification));
             
            StringBuilder riskSB = new StringBuilder();
            riskSB.Append("Risk Information: ");
            riskSB.Append("googleordernumber: ");
            riskSB.Append(riskInformationNotification.googleordernumber);
            riskSB.Append(", avsresponse: ");
            riskSB.Append(riskInformationNotification.riskinformation.avsresponse);
            riskSB.Append(", ipaddress: ");
            riskSB.Append(riskInformationNotification.riskinformation.ipaddress);
            riskSB.Append(", partialccnumber: ");
            riskSB.Append(riskInformationNotification.riskinformation.partialccnumber);
            string message = riskSB.ToString();
            logMessage(message);

            Order order = getMerchantOrderByGoogleOrderID(riskInformationNotification.googleordernumber);
            if (order != null)
            {
                IoCFactory.Resolve<IOrderManager>().InsertOrderNote(order.OrderId, message, false, DateTime.UtcNow);
            }
        }
        private Order getMerchantOrderByGoogleOrderID(string GoogleOrderID)
        {
            if (String.IsNullOrEmpty(GoogleOrderID))
                return null;

            PaymentMethod googleCheckoutPaymentMethod = IoCFactory.Resolve<IPaymentManager>().GetPaymentMethodBySystemKeyword("GoogleCheckout");
            if (googleCheckoutPaymentMethod == null)
                return null;

            return IoCFactory.Resolve<IOrderManager>().GetOrderByAuthorizationTransactionIdAndPaymentMethodId(GoogleOrderID, googleCheckoutPaymentMethod.PaymentMethodId);
        }

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
            processPaymentResult.PaymentStatus = PaymentStatusEnum.Pending;
            processPaymentResult.AuthorizationTransactionId = paymentInfo.GoogleOrderNumber;

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
            //override if payment method requires additional handling fee
            return decimal.Zero;
        }

        /// <summary>
        /// Process google callback request
        /// </summary>
        /// <param name="xmlData">xml data</param>
        public void ProcessCallBackRequest(string xmlData)
        {
            if (String.IsNullOrEmpty(xmlData))
                return;

            try
            {
                string commandName = EncodeHelper.GetTopElement(xmlData);
                logMessage(string.Format("Google callback command: {0}", commandName));
                logMessage(string.Format("Raw xml request: {0}", xmlData));

                switch (commandName)
                {
                    case "new-order-notification":
                        processNewOrderNotification(xmlData);
                        break;
                    case "order-state-change-notification":
                        processOrderStateChangeNotification(xmlData);
                        break;
                    case "risk-information-notification":
                        processRiskInformationNotification(xmlData);
                        break;
                    case "error":
                        processErrorNotification(xmlData);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception exc)
            {
                logMessage(string.Format("An error occurred: {0}", exc));
            }
        }

        /// <summary>
        /// Captures payment
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="processPaymentResult">Process payment result</param>
        public void Capture(Order order, ref ProcessPaymentResult processPaymentResult)
        {
            string googleOrderNumber = processPaymentResult.AuthorizationTransactionId;
            GCheckout.OrderProcessing.ChargeOrderRequest chargeOrderRequest = new GCheckout.OrderProcessing.ChargeOrderRequest(googleOrderNumber);
            GCheckoutResponse chargeOrderResponse = chargeOrderRequest.Send();
            if (chargeOrderResponse.IsGood)
            {
                processPaymentResult.PaymentStatus = PaymentStatusEnum.Paid;
                processPaymentResult.CaptureTransactionResult = chargeOrderResponse.ResponseXml;
            }
            else
            {
                processPaymentResult.Error = chargeOrderResponse.ErrorMessage;
            }
        }

        /// <summary>
        /// Post cart to google
        /// </summary>
        /// <param name="req">Pre-generated request</param>
        /// <param name="cart">Shopping cart</param>
        /// <returns>Response</returns>
        public GCheckoutResponse PostCartToGoogle(CheckoutShoppingCartRequest req,
            NopSolutions.NopCommerce.BusinessLogic.Orders.ShoppingCart cart)
        {
            //items
            foreach (NopSolutions.NopCommerce.BusinessLogic.Orders.ShoppingCartItem sci in cart)
            {
                ProductVariant productVariant = sci.ProductVariant;
                if (productVariant != null)
                {
                    decimal taxRate = decimal.Zero;
                    string description = ProductAttributeHelper.FormatAttributes(productVariant, sci.AttributesXml, NopContext.Current.User, ", ", false);
                    string fullName = productVariant.LocalizedFullProductName;
                    decimal unitPrice = IoCFactory.Resolve<ITaxManager>().GetPrice(sci.ProductVariant, PriceHelper.GetUnitPrice(sci, NopContext.Current.User, true), out taxRate);
                    req.AddItem(fullName, description, sci.ShoppingCartItemId.ToString(), unitPrice, sci.Quantity);
                }
            }


            bool shoppingCartRequiresShipping = IoCFactory.Resolve<IShippingManager>().ShoppingCartRequiresShipping(cart);
            if (shoppingCartRequiresShipping)
            {
                string shippingError = string.Empty;
                //AddMerchantCalculatedShippingMethod
                //AddCarrierCalculatedShippingOption
                List<ShippingOption> shippingOptions = IoCFactory.Resolve<IShippingManager>().GetShippingOptions(cart, NopContext.Current.User, null, ref shippingError);
                foreach (ShippingOption shippingOption in shippingOptions)
                    req.AddFlatRateShippingMethod(shippingOption.Name, IoCFactory.Resolve<ITaxManager>().GetShippingPrice(shippingOption.Rate, NopContext.Current.User));
            }

            //add only US, GB states
            //CountryCollection countries = IoCFactory.Resolve<ICountryManager>().GetAllCountries();                
            //foreach (Country country in countries)
            //{
            //    foreach (StateProvince state in country.StateProvinces)
            //    {
            //        TaxByStateProvinceCollection taxByStateProvinceCollection = TaxByIoCFactory.Resolve<IStateProvinceManager>().GetAllByStateProvinceID(state.StateProvinceID);
            //        foreach (TaxByStateProvince taxByStateProvince in taxByStateProvinceCollection)
            //        {
            //            if (!String.IsNullOrEmpty(state.Abbreviation))
            //            {
            //                Req.AddStateTaxRule(state.Abbreviation, (double)taxByStateProvince.Percentage, false);
            //            }
            //        }
            //    }
            //}

            //if (subTotalDiscountBase > decimal.Zero)
            //{
            //    req.AddItem("Discount", string.Empty, string.Empty, (decimal)(-1.0) * subTotalDiscountBase, 1);
            //}

            //foreach (AppliedGiftCard agc in appliedGiftCards)
            //{
            //    req.AddItem(string.Format("Gift Card - {0}", agc.GiftCard.GiftCardCouponCode), string.Empty, string.Empty, (decimal)(-1.0) * agc.AmountCanBeUsed, 1);
            //}

            XmlDocument customerInfoDoc = new XmlDocument();
            XmlElement customerInfo = customerInfoDoc.CreateElement("CustomerInfo");
            customerInfo.SetAttribute("CustomerID", NopContext.Current.User.CustomerId.ToString());
            customerInfo.SetAttribute("CustomerLanguageID", NopContext.Current.WorkingLanguage.LanguageId.ToString());
            customerInfo.SetAttribute("CustomerCurrencyID", NopContext.Current.WorkingCurrency.CurrencyId.ToString());
            req.AddMerchantPrivateDataNode(customerInfo);

            req.ContinueShoppingUrl = CommonHelper.GetStoreLocation(false);
            req.EditCartUrl = CommonHelper.GetStoreLocation(false) + "ShoppingCart.aspx";

            GCheckoutResponse resp = req.Send();

            return resp;
        }

        /// <summary>
        /// Get notification acknowledgment text
        /// </summary>
        /// <returns>Notification ack</returns>
        public string GetNotificationAcknowledgmentText()
        {
            NotificationAcknowledgment gNotificationAcknowledgment = new NotificationAcknowledgment();

            StringBuilder sb = new StringBuilder();
            using (StringWriter sw = new StringWriter(sb))
            {
                XmlSerializer serResponse = new XmlSerializer(gNotificationAcknowledgment.GetType(), "http://checkout.google.com/schema/2");
                serResponse.Serialize(sw, gNotificationAcknowledgment);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Verifies message authentication
        /// </summary>
        /// <param name="authStr">Authenticatio string</param>
        /// <returns>Result</returns>
        public bool VerifyMessageAuthentication(string authStr)
        {
            bool result = false;
            System.Configuration.Configuration config = WebConfigurationManager.OpenWebConfiguration("~");

            if (!Convert.ToBoolean(config.AppSettings.Settings["GoogleAuthenticateCallback"].Value))
            {
                result = true;
            }
            else if (String.IsNullOrEmpty(authStr) || authStr.IndexOf("Basic", 0) != 0)
            {
                result = false;
            }
            else
            {
                byte[] decodedBytes = Convert.FromBase64String(authStr.Trim().Substring(6));
                string decodedAuthString = Encoding.ASCII.GetString(decodedBytes);

                string username = decodedAuthString.Split(':')[0];
                string password = decodedAuthString.Split(':')[1];

                string merchantID = config.AppSettings.Settings["GoogleMerchantID"].Value;
                string merchantKey = config.AppSettings.Settings["GoogleMerchantKey"].Value;

                result = (username == merchantID && password == merchantKey);
            }

            logMessage(string.Format("callback authorization result = {0}", result));

            return result;
        }

        /// <summary>
        /// Refunds payment
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="cancelPaymentResult">Cancel payment result</param>        
        public void Refund(Order order, ref CancelPaymentResult cancelPaymentResult)
        {
            throw new NopException("Refund method not supported");
        }

        /// <summary>
        /// Voids payment
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="cancelPaymentResult">Cancel payment result</param>        
        public void Void(Order order, ref CancelPaymentResult cancelPaymentResult)
        {
            throw new NopException("Void method not supported");
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

        #region Properies

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
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether refund is supported
        /// </summary>
        public bool CanRefund
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether void is supported
        /// </summary>
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
                return PaymentMethodTypeEnum.Button;
            }
        }
        #endregion
    }
}
