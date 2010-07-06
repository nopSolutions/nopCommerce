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
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.BusinessLogic.Promo.Affiliates;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class CustomerRewardPointsControl : BaseNopAdministrationUserControl
    {
        private void BindData()
        {
            if (OrderManager.RewardPointsEnabled)
            {
                Customer customer = CustomerManager.GetCustomerById(this.CustomerId);
                if (customer != null)
                {
                    gvRewardPointsHistory.DataSource = customer.RewardPointsHistory;
                    gvRewardPointsHistory.DataBind();
                }
                else
                {
                    this.Visible = false;
                }
            }
            else
            {
                pnlData.Visible = false;
                pnlMessage.Visible = true;
                lMessage.Text = GetLocaleResourceString("Admin.CustomerRewardPoints.Disabled");
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.BindData();
            }
        }

        protected void gvRewardPointsHistory_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.gvRewardPointsHistory.PageIndex = e.NewPageIndex;
            BindData();
        }

        public void SaveInfo()
        {
        }

        protected void btnAddPoints_Click(object sender, EventArgs e)
        {
            try
            {
                Customer customer = CustomerManager.GetCustomerById(this.CustomerId);
                if (customer != null)
                {
                    int points = txtNewPoints.Value;
                    string message = txtNewMessage.Text;
                    RewardPointsHistory rph = OrderManager.InsertRewardPointsHistory(
                        this.CustomerId, 0, points, decimal.Zero, decimal.Zero,
                        string.Empty, message, DateTime.UtcNow);

                    BindData();
                }
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
    }
}