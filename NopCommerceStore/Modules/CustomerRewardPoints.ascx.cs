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
using System.ComponentModel;
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
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Payment;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class CustomerRewardPointsControl : BaseNopUserControl
    {
        protected override void OnInit(EventArgs e)
        {
            if (NopContext.Current.User == null)
            {
                string loginURL = SEOHelper.GetLoginPageUrl(true);
                Response.Redirect(loginURL);
            }

            BindData();

            base.OnInit(e);
        }

        protected void gvRewardPoints_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.gvRewardPoints.PageIndex = e.NewPageIndex;
            BindData();
        }

        private void BindData()
        {
            if (!OrderManager.RewardPointsEnabled)
            {
                this.Visible = false;
                return;
            }
            int rewardPointsBalance = NopContext.Current.User.RewardPointsBalance;
            decimal rewardPointsAmountBase = OrderManager.ConvertRewardPointsToAmount(rewardPointsBalance);
            decimal rewardPointsAmount = CurrencyManager.ConvertCurrency(rewardPointsAmountBase, CurrencyManager.PrimaryStoreCurrency, NopContext.Current.WorkingCurrency);
            lblBalance.Text = GetLocaleResourceString("Customer.RewardPoints.CurrentBalance", rewardPointsBalance, PriceHelper.FormatPrice(rewardPointsAmount, true, false));
            //lblRate.Text = GetLocaleResourceString("Customer.RewardPoints.CurrentRate", PriceHelper.FormatPrice(CurrencyManager.ConvertCurrency(OrderManager.RewardPointsForPurchases_Amount, CurrencyManager.PrimaryStoreCurrency, NopContext.Current.WorkingCurrency), true, false), OrderManager.RewardPointsForPurchases_Points);
            lblRate.Visible = false;

            var rphc = NopContext.Current.User.RewardPointsHistory;
            if (rphc.Count > 0)
            {
                gvRewardPoints.Visible = true;
                lblHistoryMessage.Visible = false;
                gvRewardPoints.DataSource = rphc;
                gvRewardPoints.DataBind();
            }
            else
            {
                gvRewardPoints.Visible = false;
                lblHistoryMessage.Visible = true;
                lblHistoryMessage.Text = GetLocaleResourceString("Customer.RewardPoints.NoHistory");
            }
        }
    }
}