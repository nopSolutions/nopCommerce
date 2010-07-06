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
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic.Security;
using NopSolutions.NopCommerce.BusinessLogic.Tax;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Web.Templates.Tax;
using NopSolutions.NopCommerce.BusinessLogic.Audit;

namespace NopSolutions.NopCommerce.Web.Administration
{
    public partial class Administration_TaxProviderDetails : BaseNopAdministrationPage
    {
        protected override bool ValidatePageSecurity()
        {
            return ACLManager.IsActionAllowed("ManageTaxSettings");
        }

        private void BindData()
        {
            TaxProvider taxProvider = TaxProviderManager.GetTaxProviderById(this.TaxProviderId);
            if (taxProvider != null)
            {
                this.txtName.Text = taxProvider.Name;
                this.txtDescription.Text = taxProvider.Description;
                this.txtConfigureTemplatePath.Text = taxProvider.ConfigureTemplatePath;
                this.txtClassName.Text = taxProvider.ClassName;
                this.txtDisplayOrder.Value = taxProvider.DisplayOrder;
            }
            else
                Response.Redirect("TaxProviders.aspx");
        }
        
        private void CreateChildControlsTree()
        {
            TaxProvider taxProvider = TaxProviderManager.GetTaxProviderById(this.TaxProviderId);
            if (taxProvider != null)
            {
                Control child = null;
                try
                {
                    child = base.LoadControl(taxProvider.ConfigureTemplatePath);
                    this.ConfigurePlaceHolder.Controls.Add(child);
                }
                catch (Exception)
                {
                }
            }
        }

        private IConfigureTaxModule GetConfigureModule()
        {
            foreach (Control ctrl in this.ConfigurePlaceHolder.Controls)
                if (ctrl is IConfigureTaxModule)
                    return (IConfigureTaxModule)ctrl;
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
                this.SelectTab(this.TaxTabs, this.TabId);
            }
        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    var taxProvider = TaxProviderManager.GetTaxProviderById(this.TaxProviderId);

                    if (taxProvider != null)
                    {
                        taxProvider = TaxProviderManager.UpdateTaxProvider(taxProvider.TaxProviderId, txtName.Text,
                            txtDescription.Text, txtConfigureTemplatePath.Text, txtClassName.Text,
                          txtDisplayOrder.Value);

                        var configureModule = GetConfigureModule();
                        if (configureModule != null)
                            configureModule.Save();

                        CustomerActivityManager.InsertActivity(
                            "EditTaxProvider",
                            GetLocaleResourceString("ActivityLog.EditTaxProvider"),
                            taxProvider.Name);

                        Response.Redirect(string.Format("TaxProviderDetails.aspx?TaxProviderID={0}&TabID={1}", taxProvider.TaxProviderId, this.GetActiveTabId(this.TaxTabs)));
                    }
                    else
                        Response.Redirect("TaxProviders.aspx");
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
                TaxProviderManager.DeleteTaxProvider(this.TaxProviderId);
                Response.Redirect("TaxProviders.aspx");
            }
            catch (Exception exc)
            {
                ProcessException(exc);
            }
        }

        public int TaxProviderId
        {
            get
            {
                return CommonHelper.QueryStringInt("TaxProviderId");
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
