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
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Tax;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Tax;
using NopSolutions.NopCommerce.Web.Administration.Modules;
using NopSolutions.NopCommerce.Web.Templates.Tax;

namespace NopSolutions.NopCommerce.Web.Administration.Tax.FixedRate
{
    public partial class ConfigureTax : BaseNopAdministrationUserControl, IConfigureTaxModule
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
                BindData();
        }

        private void BindData()
        {
            var taxCategories = TaxCategoryManager.GetAllTaxCategories();
            gvTaxCategories.DataSource = taxCategories;
            gvTaxCategories.DataBind();
        }

        protected void gvTaxCategories_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                TaxCategory taxCategory = (TaxCategory)e.Row.DataItem;

                DecimalTextBox txtRate = e.Row.FindControl("txtRate") as DecimalTextBox;
                if (txtRate != null)
                {
                   txtRate.Value = SettingManager.GetSettingValueDecimalNative(string.Format("Tax.TaxProvider.FixedRate.TaxCategoryId{0}", taxCategory.TaxCategoryId));
                }
            }
        }


        public void Save()
        {
            foreach (GridViewRow row in gvTaxCategories.Rows)
            {
                DecimalTextBox txtRate = row.FindControl("txtRate") as DecimalTextBox;
                HiddenField hfTaxCategoryId = row.FindControl("hfTaxCategoryId") as HiddenField;

                int taxCategoryId = int.Parse(hfTaxCategoryId.Value);
                decimal rate = txtRate.Value;

                SettingManager.SetParamNative(string.Format("Tax.TaxProvider.FixedRate.TaxCategoryId{0}", taxCategoryId), rate);
            }
        }
    }
}
