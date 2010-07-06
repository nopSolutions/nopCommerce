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
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class CheckoutCompletedControl : BaseNopUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if ((NopContext.Current.User == null) || (NopContext.Current.User.IsGuest && !CustomerManager.AnonymousCheckoutAllowed))
            {
                string loginURL = SEOHelper.GetLoginPageUrl(true);
                Response.Redirect(loginURL);
            }

            if (!Page.IsPostBack)
            {
                var orderCollection = NopContext.Current.User.Orders;
                if (orderCollection.Count == 0)
                {
                    Response.Redirect(CommonHelper.GetStoreLocation());
                }
                else
                {
                    var lastOrder = orderCollection[0];
                    lblOrderNumber.Text = lastOrder.OrderId.ToString();
                    hlOrderDetails.NavigateUrl = string.Format("{0}OrderDetails.aspx?OrderID={1}", CommonHelper.GetStoreLocation(), lastOrder.OrderId).ToLowerInvariant();
                }
            }
        }

        protected void btnContinue_Click(object sender, EventArgs e)
        {
            Response.Redirect(CommonHelper.GetStoreLocation());
        }
    }
}