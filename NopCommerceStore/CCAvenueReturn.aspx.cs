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
// Contributor(s): praneeth kumar.p_______. 
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
using NopSolutions.NopCommerce.Payment.Methods.CCAvenue;
using NopSolutions.NopCommerce.Payment.Methods.PayPal;
namespace NopSolutions.NopCommerce.Web
{
    public partial class CCAvenueReturnPage : BaseNopPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Response.CacheControl = "private";
            //Response.Expires = 0;
            //Response.AddHeader("pragma", "no-cache");

            //comment this line to process return
           // Response.Redirect(CommonHelper.GetStoreLocation());

            if (!Page.IsPostBack)
            {
                Populate();
            }
        }

        void Populate()
        {
            CCAvenueHelper myUtility = new CCAvenueHelper();
            string WorkingKey, Order_Id, Merchant_Id, Amount, AuthDesc, checksum;

            //Assign following values to send it to verifychecksum function.
            WorkingKey = SettingManager.GetSettingValue("PaymentMethod.CCAvenue.Key");   // put in the 32 bit working key in the quotes provided here
            if (String.IsNullOrWhiteSpace(WorkingKey))
                throw new NopException("CCAvenue key is not set");

            Merchant_Id = Request.Form["Merchant_Id"];
            Order_Id = Request.Form["Order_Id"];
            Amount = Request.Form["Amount"];
            AuthDesc = Request.Form["AuthDesc"];
            checksum = Request.Form["Checksum"];

            checksum = myUtility.verifychecksum(Merchant_Id, Order_Id, Amount, AuthDesc, WorkingKey, checksum);

            if ((checksum == "true") && (AuthDesc == "Y"))
            {

                /* 
                    Here you need to put in the routines for a successful 
                     transaction such as sending an email to customer,
                     setting database status, informing logistics etc etc
                */

                Order order = OrderManager.GetOrderById(Convert.ToInt32(Order_Id));
                if (OrderManager.CanMarkOrderAsPaid(order))
                {
                    OrderManager.MarkOrderAsPaid(order.OrderId);
                }
                lInfo.Text = "<br>Thank you for shopping with us. Your credit card has been charged and your transaction is successful.";

            }
            else if ((checksum == "true") && (AuthDesc == "N"))
            {
                /*
                    Here you need to put in the routines for a failed
                    transaction such as sending an email to customer
                    setting database status etc etc
                */

                string message = "<br>Thank you for shopping with us. However, the transaction has been declined.";
                lInfo.Text = message;

            }
            else if ((checksum == "true") && (AuthDesc == "B"))
            {
                /*
                    Here you need to put in the routines/e-mail for a  "Batch Processing" order
                    This is only if payment for this transaction has been made by an American Express Card
                    since American Express authorisation status is available only after 5-6 hours by mail from ccavenue and at the "View Pending Orders"
             */

                string message = "<br>Thank you for shopping with us. We will keep you posted regarding the status of your order through e-mail";
                lInfo.Text = message;

            }
            else
            {
                /*
                    Here you need to simply ignore this and dont need
                    to perform any operation in this condition
                */

                string message = "<br>Security Error. Illegal access detected";
                lInfo.Text = message;
            }
        }
    }
}