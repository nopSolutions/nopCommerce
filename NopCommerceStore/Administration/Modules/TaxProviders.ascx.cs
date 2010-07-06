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
using System.IO;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Tax;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class TaxProvidersControl : BaseNopAdministrationUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindGrid();
            }
        }

        private void BindGrid()
        {
            var taxProviders = TaxProviderManager.GetAllTaxProviders();
            gvTaxProviders.DataSource = taxProviders;
            gvTaxProviders.DataBind();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (GridViewRow row in gvTaxProviders.Rows)
                {
                    RadioButton rdbIsDefault = (RadioButton)row.FindControl("rdbIsDefault");
                    HiddenField hfTaxProviderId = (HiddenField)row.FindControl("hfTaxProviderId");
                    int taxProviderId = int.Parse(hfTaxProviderId.Value);
                    if (rdbIsDefault.Checked)
                        TaxManager.ActiveTaxProvider = TaxProviderManager.GetTaxProviderById(taxProviderId);
                }
                BindGrid();
            }
            catch (Exception exc)
            {
                ProcessException(exc);
            }
        }
    }
}