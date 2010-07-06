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
using NopSolutions.NopCommerce.BusinessLogic.Directory;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class StateProvincesControl : BaseNopAdministrationUserControl
    {
        private void FillDropDowns()
        {
            this.ddlCountry.Items.Clear();
            var countryCollection = CountryManager.GetAllCountries();
            foreach (Country country in countryCollection)
            {
                ListItem ddlCountryItem2 = new ListItem(country.Name, country.CountryId.ToString());
                this.ddlCountry.Items.Add(ddlCountryItem2);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                FillDropDowns();
                BindGrid();
            }
        }

        void BindGrid()
        {
            int countryId = int.Parse(this.ddlCountry.SelectedItem.Value);
            var stateProvinceCollection = StateProvinceManager.GetStateProvincesByCountryId(countryId);
            gvStateProvinces.DataSource = stateProvinceCollection;
            gvStateProvinces.DataBind();
        }

        protected void AddButton_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                int countryId = int.Parse(this.ddlCountry.SelectedItem.Value);
                Response.Redirect("StateProvinceAdd.aspx?CountryID=" + countryId.ToString());
            }
        }

        protected void ddlCountry_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindGrid();
        }
    }
}