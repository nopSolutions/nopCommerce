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
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Audit;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Payment;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Payment.Methods.TwoCheckout;
using NopSolutions.NopCommerce.BusinessLogic.IoC;
namespace NopSolutions.NopCommerce.Web
{
    public partial class TwoCheckoutINSHandlerPage : BaseNopPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            CommonHelper.SetResponseNoCache(Response);
            
            if (!Page.IsPostBack)
            {
                //item_id_1 or vendor_order_id
                string nopOrderIdStr = HttpContext.Current.Request.Form["item_id_1"];
                int nopOrderId = 0;
                int.TryParse(nopOrderIdStr, out nopOrderId);
                Order order = IoCFactory.Resolve<IOrderManager>().GetOrderById(nopOrderId);
                if (order != null)
                {
                    //debug info
                    StringBuilder sbDebug = new StringBuilder();
                    sbDebug.AppendLine("2Checkout IPN:");
                    foreach (string key in HttpContext.Current.Request.Form.AllKeys)
                    {
                        string value = HttpContext.Current.Request.Form[key];
                        sbDebug.AppendLine(key + ": " + value);
                    }
                    IoCFactory.Resolve<IOrderManager>().InsertOrderNote(order.OrderId, sbDebug.ToString(), false, DateTime.UtcNow);


                    bool useSandbox = IoCFactory.Resolve<ISettingManager>().GetSettingValueBoolean("PaymentMethod.TwoCheckout.UseSandbox");

                    //sale id
                    string sale_id = string.Empty;
                    if (useSandbox)
                        sale_id = "1";
                    else
                        sale_id = HttpContext.Current.Request.Form["sale_id"];
                    if (sale_id == null)
                        sale_id = string.Empty;

                    //invoice id
                    string invoice_id = HttpContext.Current.Request.Form["invoice_id"];
                    if (invoice_id == null)
                        invoice_id = string.Empty;

                    if (IoCFactory.Resolve<ISettingManager>().GetSettingValueBoolean("PaymentMethod.TwoCheckout.UseMD5Hashing"))
                    {
                        string vendorId = IoCFactory.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.TwoCheckout.VendorId");
                        string secretWord = IoCFactory.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.TwoCheckout.SecretWord");

                        string compareHash1 = TwoCheckoutPaymentProcessor.CalculateMD5hash(sale_id + vendorId + invoice_id + secretWord);
                        if (String.IsNullOrEmpty(compareHash1))
                            throw new NopException("2Checkout empty hash string");
                        string compareHash2 = HttpContext.Current.Request.Form["md5_hash"];
                        if (compareHash2 == null)
                            compareHash2 = string.Empty;

                        if (compareHash1.ToUpperInvariant() != compareHash2.ToUpperInvariant())
                        {
                            IoCFactory.Resolve<IOrderManager>().InsertOrderNote(order.OrderId, "Hash validation failed", false, DateTime.UtcNow);
                            Response.Redirect(CommonHelper.GetStoreLocation());
                        }
                    }

                    string message_type = HttpContext.Current.Request.Form["message_type"];
                    if (message_type == null)
                        message_type = string.Empty;
                    string invoice_status = HttpContext.Current.Request.Form["invoice_status"];
                    if (invoice_status == null)
                        invoice_status = string.Empty;
                    string fraud_status = HttpContext.Current.Request.Form["fraud_status"];
                    if (fraud_status == null)
                        fraud_status = string.Empty;
                    string payment_type = HttpContext.Current.Request.Form["payment_type"];
                    if (payment_type == null)
                        payment_type = string.Empty;

                    PaymentStatusEnum newPaymentStatus = TwoCheckoutPaymentProcessor.GetPaymentStatus(message_type, invoice_status, fraud_status, payment_type);

                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("2Checkout IPN:");
                    sb.AppendLine("sale_id: " + sale_id);
                    sb.AppendLine("invoice_id: " + invoice_id);
                    sb.AppendLine("message_type: " + message_type);
                    sb.AppendLine("invoice_status: " + invoice_status);
                    sb.AppendLine("fraud_status: " + fraud_status);
                    sb.AppendLine("payment_type: " + payment_type);
                    sb.AppendLine("New payment status: " + IoCFactory.Resolve<IPaymentManager>().GetPaymentStatusName((int)newPaymentStatus));
                    IoCFactory.Resolve<IOrderManager>().InsertOrderNote(order.OrderId, sb.ToString(), false, DateTime.UtcNow);

                    //new payment status
                    switch (newPaymentStatus)
                    {
                        case PaymentStatusEnum.Pending:
                            {
                            }
                            break;
                        case PaymentStatusEnum.Authorized:
                            {
                                if (IoCFactory.Resolve<IOrderManager>().CanMarkOrderAsAuthorized(order))
                                {
                                    IoCFactory.Resolve<IOrderManager>().MarkAsAuthorized(order.OrderId);
                                }
                            }
                            break;
                        case PaymentStatusEnum.Paid:
                            {
                                if (IoCFactory.Resolve<IOrderManager>().CanMarkOrderAsPaid(order))
                                {
                                    IoCFactory.Resolve<IOrderManager>().MarkOrderAsPaid(order.OrderId);
                                }
                            }
                            break;
                        case PaymentStatusEnum.Refunded:
                            {
                                if (IoCFactory.Resolve<IOrderManager>().CanRefundOffline(order))
                                {
                                    IoCFactory.Resolve<IOrderManager>().RefundOffline(order.OrderId);
                                }
                            }
                            break;
                        case PaymentStatusEnum.Voided:
                            {
                                if (IoCFactory.Resolve<IOrderManager>().CanVoidOffline(order))
                                {
                                    IoCFactory.Resolve<IOrderManager>().VoidOffline(order.OrderId);
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    Response.Redirect(CommonHelper.GetStoreLocation());
                }
            }
        }

        public override bool AllowGuestNavigation
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this page is tracked by 'Online Customers' module
        /// </summary>
        public override bool TrackedByOnlineCustomersModule
        {
            get
            {
                return false;
            }
        }
    }
}