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
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic.Audit;
using NopSolutions.NopCommerce.BusinessLogic.Payment;
using NopSolutions.NopCommerce.BusinessLogic.Security;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Web.Templates.Payment;

namespace NopSolutions.NopCommerce.Web.Administration
{
    public partial class Administration_PaymentMethodDetails : BaseNopAdministrationPage
    {
        protected override bool ValidatePageSecurity()
        {
            return ACLManager.IsActionAllowed("ManagePaymentSettings");
        }

        private void BindData()
        {
            PaymentMethod paymentMethod = PaymentMethodManager.GetPaymentMethodById(this.PaymentMethodId);
            if (paymentMethod != null)
            {
                this.txtName.Text = paymentMethod.Name;
                this.txtVisibleName.Text = paymentMethod.VisibleName;
                this.txtDescription.Text = paymentMethod.Description;
                this.txtConfigureTemplatePath.Text = paymentMethod.ConfigureTemplatePath;
                this.txtUserTemplatePath.Text = paymentMethod.UserTemplatePath;
                this.txtClassName.Text = paymentMethod.ClassName;
                this.txtSystemKeyword.Text = paymentMethod.SystemKeyword;
                this.cbHidePaymentInfoForZeroOrders.Checked = paymentMethod.HidePaymentInfoForZeroOrders;
                this.cbActive.Checked = paymentMethod.IsActive;
                this.txtDisplayOrder.Value = paymentMethod.DisplayOrder;
                try
                {
                    lblCanCapture.Text = PaymentManager.CanCapture(this.PaymentMethodId) ? GetLocaleResourceString("Admin.Common.Yes") : GetLocaleResourceString("Admin.Common.No");
                    lblCanRefund.Text = PaymentManager.CanRefund(this.PaymentMethodId) ? GetLocaleResourceString("Admin.Common.Yes") : GetLocaleResourceString("Admin.Common.No");
                    lblCanPartiallyRefund.Text = PaymentManager.CanPartiallyRefund(this.PaymentMethodId) ? GetLocaleResourceString("Admin.Common.Yes") : GetLocaleResourceString("Admin.Common.No");
                    lblCanVoid.Text = PaymentManager.CanVoid(this.PaymentMethodId) ? GetLocaleResourceString("Admin.Common.Yes") : GetLocaleResourceString("Admin.Common.No");
                    lblSupportRecurringPayments.Text = CommonHelper.ConvertEnum(PaymentManager.SupportRecurringPayments(this.PaymentMethodId).ToString());
                }
                catch (Exception exc)
                {
                    Debug.WriteLine(exc.ToString());
                    lblCanCapture.Text = "Unknown";
                    lblCanRefund.Text = "Unknown";
                    lblCanPartiallyRefund.Text = "Unknown";
                    lblCanVoid.Text = "Unknown";
                    lblSupportRecurringPayments.Text = "Unknown";
                }
            }
            else
                Response.Redirect("PaymentMethods.aspx");
        }
        
        private void CreateChildControlsTree()
        {
            PaymentMethod paymentMethod = PaymentMethodManager.GetPaymentMethodById(this.PaymentMethodId);
            if (paymentMethod != null)
            {
                Control child = null;
                try
                {
                    child = base.LoadControl(paymentMethod.ConfigureTemplatePath);
                    this.ConfigureMethodHolder.Controls.Add(child);
                }
                catch (Exception)
                {
                }
            }
        }

        private IConfigurePaymentMethodModule GetConfigureModule()
        {
            foreach (Control ctrl in this.ConfigureMethodHolder.Controls)
                if (ctrl is IConfigurePaymentMethodModule)
                    return (IConfigurePaymentMethodModule)ctrl;
            return null;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.CreateChildControlsTree();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.BindData();
                this.SelectTab(this.PaymentTabs, this.TabId);
            }
        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    var paymentMethod = PaymentMethodManager.GetPaymentMethodById(this.PaymentMethodId);

                    if (paymentMethod != null)
                    {
                        paymentMethod = PaymentMethodManager.UpdatePaymentMethod(paymentMethod.PaymentMethodId,
                            txtName.Text, txtVisibleName.Text, txtDescription.Text, 
                            txtConfigureTemplatePath.Text, txtUserTemplatePath.Text, txtClassName.Text,
                            txtSystemKeyword.Text, cbHidePaymentInfoForZeroOrders.Checked,
                            cbActive.Checked, txtDisplayOrder.Value);

                        var configureModule = GetConfigureModule();
                        if (configureModule != null)
                            configureModule.Save();

                        CustomerActivityManager.InsertActivity(
                            "EditPaymentMethod",
                            GetLocaleResourceString("ActivityLog.EditPaymentMethod"),
                            paymentMethod.Name);

                        Response.Redirect(string.Format("PaymentMethodDetails.aspx?PaymentMethodID={0}&TabID={1}", paymentMethod.PaymentMethodId, this.GetActiveTabId(this.PaymentTabs)));
                    }
                    else
                        Response.Redirect("PaymentMethods.aspx");
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
                PaymentMethodManager.DeletePaymentMethod(this.PaymentMethodId);
                Response.Redirect("PaymentMethods.aspx");
            }
            catch (Exception exc)
            {
                ProcessException(exc);
            }
        }

        public int PaymentMethodId
        {
            get
            {
                return CommonHelper.QueryStringInt("PaymentMethodId");
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
