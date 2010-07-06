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
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Payment.Methods.PayPal;
using NopSolutions.NopCommerce.BusinessLogic.Payment;
using NopSolutions.NopCommerce.BusinessLogic.Audit;
namespace NopSolutions.NopCommerce.Web
{
    public partial class PaypalPDTHandlerPage : BaseNopPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            CommonHelper.SetResponseNoCache(Response);

            if (!Page.IsPostBack)
            {
                string tx = CommonHelper.QueryString("tx");
                Dictionary<string, string> values;
                string response;

                PayPalStandardPaymentProcessor processor = new PayPalStandardPaymentProcessor();
                if (processor.GetPDTDetails(tx, out values, out response))
                {
                    string orderNumber = string.Empty;
                    values.TryGetValue("custom", out orderNumber);
                    Guid orderNumberGuid = Guid.Empty;
                    try
                    {
                        orderNumberGuid = new Guid(orderNumber);
                    }
                    catch { }
                    Order order = OrderManager.GetOrderByGuid(orderNumberGuid);
                    if (order != null)
                    {
                        decimal total = decimal.Zero;
                        try
                        {
                            total = decimal.Parse(values["mc_gross"], new CultureInfo("en-US"));
                        }
                        catch (Exception exc)
                        {
                            LogManager.InsertLog(LogTypeEnum.OrderError, "PayPal IPN. Error getting orderGUID", exc);
                        }

                        string payer_status = string.Empty;
                        values.TryGetValue("payer_status", out payer_status);
                        string payment_status = string.Empty;
                        values.TryGetValue("payment_status", out payment_status);
                        string pending_reason = string.Empty;
                        values.TryGetValue("pending_reason", out pending_reason);
                        string mc_currency = string.Empty;
                        values.TryGetValue("mc_currency", out mc_currency);
                        string txn_id = string.Empty;
                        values.TryGetValue("txn_id", out txn_id);
                        string payment_type = string.Empty;
                        values.TryGetValue("payment_type", out payment_type);
                        string payer_id = string.Empty;
                        values.TryGetValue("payer_id", out payer_id);
                        string receiver_id = string.Empty;
                        values.TryGetValue("receiver_id", out receiver_id);
                        string invoice = string.Empty;
                        values.TryGetValue("invoice", out invoice);
                        string payment_fee = string.Empty;
                        values.TryGetValue("payment_fee", out payment_fee);

                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("Paypal PDT:");
                        sb.AppendLine("total: " + total);
                        sb.AppendLine("Payer status: " + payer_status);
                        sb.AppendLine("Payment status: " + payment_status);
                        sb.AppendLine("Pending reason: " + pending_reason);
                        sb.AppendLine("mc_currency: " + mc_currency);
                        sb.AppendLine("txn_id: " + txn_id);
                        sb.AppendLine("payment_type: " + payment_type);
                        sb.AppendLine("payer_id: " + payer_id);
                        sb.AppendLine("receiver_id: " + receiver_id);
                        sb.AppendLine("invoice: " + invoice);
                        sb.AppendLine("payment_fee: " + payment_fee);

                        OrderManager.InsertOrderNote(order.OrderId, sb.ToString(), false, DateTime.UtcNow);
                        if (OrderManager.CanMarkOrderAsPaid(order))
                        {
                            OrderManager.MarkOrderAsPaid(order.OrderId);
                        }
                    
                    }
                    Response.Redirect("~/checkoutcompleted.aspx");
                }
                else
                {
                    string orderNumber = string.Empty;
                    values.TryGetValue("custom", out orderNumber);
                    Guid orderNumberGuid = Guid.Empty;
                    try
                    {
                        orderNumberGuid = new Guid(orderNumber);
                    }
                    catch { }
                    Order order = OrderManager.GetOrderByGuid(orderNumberGuid);
                    if (order != null)
                    {
                        OrderManager.InsertOrderNote(order.OrderId, "PayPal PDT failed. " + response, false, DateTime.UtcNow);
                    }
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
    }
}