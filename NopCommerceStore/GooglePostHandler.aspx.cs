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
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Content.Blog;
using NopSolutions.NopCommerce.BusinessLogic.Localization;
using NopSolutions.NopCommerce.BusinessLogic.Payment;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Payment.Methods.GoogleCheckout;

namespace NopSolutions.NopCommerce.Web
{
    public partial class GooglePostHandlerPage : BaseNopPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string xmlData = string.Empty;
            Stream RequestStream = Request.InputStream;
            using (StreamReader RequestStreamReader = new StreamReader(RequestStream))
                xmlData = RequestStreamReader.ReadToEnd();

            GoogleCheckoutPaymentProcessor googleCheckoutPaymentProcessor = new GoogleCheckoutPaymentProcessor();

            //authorize google callback request
            if (!googleCheckoutPaymentProcessor.VerifyMessageAuthentication(Request.Headers["Authorization"]))
            {
                Response.StatusCode = 401;
                Response.StatusDescription = "Access Denied";
                Response.End();
            }

            if (SettingManager.GetSettingValueBoolean("PaymentMethod.GoogleCheckout.DebugModeEnabled"))
            {
                if (CommonHelper.QueryStringInt("nopCommerceTestNewOrder1") > 0)
                {
                    xmlData = File.ReadAllText(HttpContext.Current.Server.MapPath("google/sample-neworder.txt"));
                }
                if (CommonHelper.QueryStringInt("nopCommerceTestNewOrder2") > 0)
                {
                    xmlData = File.ReadAllText(HttpContext.Current.Server.MapPath("google/sample-neworder-noShipment.txt"));
                }
                else if (CommonHelper.QueryStringInt("nopCommerceTestOrderChange") > 0)
                {
                    xmlData = File.ReadAllText(HttpContext.Current.Server.MapPath("google/sample-orderchangestate.txt"));
                }
                else if (CommonHelper.QueryStringInt("nopCommerceTestRisk") > 0)
                {
                    xmlData = File.ReadAllText(HttpContext.Current.Server.MapPath("google/sample-risk.txt"));
                }
            }
            googleCheckoutPaymentProcessor.ProcessCallBackRequest(xmlData);

            //ack
            string NotificationAcknowledgment = googleCheckoutPaymentProcessor.GetNotificationAcknowledgmentText();
            Response.Clear();
            Response.Write(NotificationAcknowledgment);
            Response.Flush();
            Response.End();
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