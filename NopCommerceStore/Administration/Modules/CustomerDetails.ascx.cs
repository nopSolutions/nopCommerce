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
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic.Audit;
using NopSolutions.NopCommerce.BusinessLogic.Content.Forums;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.IoC;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.BusinessLogic.Promo.Affiliates;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class CustomerDetailsControl : BaseNopAdministrationUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Customer customer = IoCFactory.Resolve<ICustomerService>().GetCustomerById(this.CustomerId);
                if (customer != null)
                {
                    lblTitle.Text = Server.HtmlEncode(customer.Email);
                }

                this.SelectTab(this.CustomerTabs, this.TabId);

                pnlCustomerAvatar.Visible = IoCFactory.Resolve<ICustomerService>().AllowCustomersToUploadAvatars;
                pnlCustomerForumSubscriptions.Visible = IoCFactory.Resolve<IForumService>().AllowCustomersToManageSubscriptions;
            }
        }

        protected Customer Save()
        {
            Customer customer = IoCFactory.Resolve<ICustomerService>().GetCustomerById(this.CustomerId);

            customer = ctrlCustomerInfo.SaveInfo();
            ctrlCustomerBillingAddresses.SaveInfo();
            ctrlCustomerShippingAddresses.SaveInfo();
            ctrlCustomerOrders.SaveInfo();
            ctrlCustomerRoleMappings.SaveInfo();
            ctrlCurrentShoppingCart.SaveInfo();
            ctrlCurrentWishlist.SaveInfo();
            ctrlCustomerRewardPoints.SaveInfo();
            ctrlCustomerForumSubscriptions.SaveInfo();

            IoCFactory.Resolve<ICustomerActivityService>().InsertActivity(
                "EditCustomer",
                GetLocaleResourceString("ActivityLog.EditCustomer"),
                customer.CustomerId);

            return customer;
        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    Customer customer = Save();
                    Response.Redirect("Customers.aspx");
                }
                catch (Exception exc)
                {
                    ProcessException(exc);
                }
            }
        }

        protected void SaveAndStayButton_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    Customer customer = Save();
                    Response.Redirect(string.Format("CustomerDetails.aspx?CustomerID={0}&TabID={1}", customer.CustomerId, this.GetActiveTabId(this.CustomerTabs)));
                }
                catch (Exception exc)
                {
                    ProcessException(exc);
                }
            }
        }
        
        protected void DeleteButton_Click(object sender, EventArgs e)
        {
            try
            {
                IoCFactory.Resolve<ICustomerService>().MarkCustomerAsDeleted(this.CustomerId);

                IoCFactory.Resolve<ICustomerActivityService>().InsertActivity(
                    "DeleteCustomer",
                    GetLocaleResourceString("ActivityLog.DeleteCustomer"),
                    this.CustomerId);

                Response.Redirect("Customers.aspx");
            }
            catch (Exception exc)
            {
                ProcessException(exc);
            }
        }

        public int CustomerId
        {
            get
            {
                return CommonHelper.QueryStringInt("CustomerId");
            }
        }

        protected string TabId
        {
            get
            {
                return CommonHelper.QueryString("TabId");
            }
        }
    }
}