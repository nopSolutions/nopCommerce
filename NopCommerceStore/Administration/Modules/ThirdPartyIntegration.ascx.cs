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
using System.Web.UI;
using NopSolutions.NopCommerce.BusinessLogic.Audit;
using NopSolutions.NopCommerce.BusinessLogic.QuickBooks;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.Orders;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class ThirdPartyIntegrationControl : BaseNopAdministrationUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                SelectTab(ThirdPartyIntegrationTabs, TabId);
                FillDropDowns();
                BindData();
            }
        }

        private void BindData()
        {
            cbQuickBooksEnabled.Checked = QBManager.QBIsEnabled;
            txtQuickBooksUsername.Text = QBManager.QBUsername;
            txtQuickBooksPassword.Text = QBManager.QBPassword;
            txtQuickBooksItemRef.Text = QBManager.QBItemRef;
            txtQuickBooksDiscountAccountRef.Text = QBManager.QBDiscountAccountRef;
            txtQuickBooksShippingAccountRef.Text = QBManager.QBShippingAccountRef;
            txtQuickBooksSalesTaxAccountRef.Text = QBManager.QBSalesTaxAccountRef;
        }

        private void FillDropDowns()
        {
        }

        protected override void OnPreRender(EventArgs e)
        {
            BindJQuery();

            cbQuickBooksEnabled.Attributes.Add("onclick", "toggleQuickBooks();");

            base.OnPreRender(e);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    QBManager.QBIsEnabled = cbQuickBooksEnabled.Checked;
                    QBManager.QBUsername = txtQuickBooksUsername.Text;
                    QBManager.QBPassword = txtQuickBooksPassword.Text;
                    QBManager.QBItemRef = txtQuickBooksItemRef.Text;
                    QBManager.QBDiscountAccountRef = txtQuickBooksDiscountAccountRef.Text;
                    QBManager.QBShippingAccountRef = txtQuickBooksShippingAccountRef.Text;
                    QBManager.QBSalesTaxAccountRef = txtQuickBooksSalesTaxAccountRef.Text;
                    
                    CustomerActivityManager.InsertActivity("EditThirdPartyIntegration", GetLocaleResourceString("ActivityLog.EditThirdPartyIntegration"));

                    Response.Redirect(string.Format("ThirdPartyIntegration.aspx?TabID={0}", GetActiveTabId(ThirdPartyIntegrationTabs)));
                }
                catch (Exception exc)
                {
                    ProcessException(exc);
                }
            }
        }

        protected void btnQuickBooksSyn_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (Order order in OrderManager.LoadAllOrders())
                {
                    QBManager.RequestSynchronization(order);
                }
                ShowMessage(GetLocaleResourceString("Admin.ThirdPartyIntegration.QuickBooks.SynchronizationSuccess"));
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
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
